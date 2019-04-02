import * as React from 'react';

import OrderMaster, { IOrderMaster } from '../../classes/dbEntities/IOrderMaster';
import { IResponseMessage } from '../../classes/ResponseMessage';
import AxiosAgent from '../../classes/AxiosAgent';
import { ICrudComponent } from '../ICrudComponent';
import EntityAgent, { IOrderDisplay } from '../../classes/EntityAgent';
import { Form, Col, Tabs, Tab, Container } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import ErrorInfo, { IErrorInfo } from '../../classes/ErrorInfo';
import { ITranComponentProp } from '../../classes/ITranComponentProp';
import { IMaestroCustomer } from '../../classes/dbEntities/IMaestroCustomer';
import DatePicker from "react-datepicker";
import { IMaestroProduct } from '../../classes/dbEntities/IMaestroProduct';
import { IMaestroProductGroup } from '../../classes/dbEntities/IProductGroup';

import { IQbProductMap } from '../../classes/dbEntities/IQbProductMap';
import OrderProductItem from './OrderProductItem';
import { IMaestroUnit } from '../../classes/dbEntities/IMaestroUnit';
import { IOrderItem, OrderItem } from '../../classes/dbEntities/IOrderItem';
import { ICustomerProductUnit } from '../../classes/dbEntities/ICustomerProductUnit';


import Draggable from 'react-draggable';


export default class OrderComponent extends React.Component<ITranComponentProp, IOrderDisplay> implements ICrudComponent {

    state = {
        Entity: new OrderMaster(0), Customers: [], Products: [], ProductMaps: [],
        OrderDate: new Date(), DeliveryDate: new Date(), ProductGroups:[], Units:[],
        CustomerProductUnits: [], Init: false, ErrorInfo: new ErrorInfo(), SummaryDisplay: { display:"block"}
    };

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = {
            Customers: [], Products: [], ProductMaps: [], CustomerProductUnits: [],
            OrderDate: new Date(), DeliveryDate: new Date(), ProductGroups:[], Units:[],
            Entity: props.Entity as IOrderMaster, Init: true, ErrorInfo: new ErrorInfo(),
            SummaryDisplay: { display: "block" }
        };

        this.renderTabs = this.renderTabs.bind(this);
        this.renderMaps = this.renderMaps.bind(this);
        this.Integrate = this.Integrate.bind(this);
    }


    DisableEnable = (disable:boolean)=> {
        (document.getElementById("orderCustomerId") as HTMLSelectElement).disabled = disable;
        (document.getElementById("orderDateId") as HTMLInputElement).disabled = disable;
        (document.getElementById("deliveryDateId") as HTMLInputElement).disabled = disable;
        (document.getElementById("notesId") as HTMLTextAreaElement).disabled = disable;

        let maps: IQbProductMap[] = this.state.ProductMaps as IQbProductMap[];
        for (let map of maps) {
            let bt: HTMLButtonElement = document.getElementById("mapName_" + map.Id) as HTMLButtonElement;
            if (bt != undefined && bt != null) {
                bt.disabled = disable;
                (document.getElementById("btnIncrease_" + map.Id) as HTMLButtonElement).disabled = disable;
                (document.getElementById("btnDecrease_" + map.Id) as HTMLButtonElement).disabled = disable;
                (document.getElementById("btnDisable" + map.Id) as HTMLButtonElement).disabled = disable;

                (document.getElementById("quantity_" + map.Id) as HTMLInputElement).disabled = disable;
                (document.getElementById("unitNr_" + map.Id) as HTMLSelectElement).disabled = disable;
            }

        }
    }

    OnCustomerChange = (evt) => {
        let order: IOrderMaster = this.state.Entity;
        order.CustomerId = parseInt(evt.target.value);
        this.setState({Entity : order});
    }

    UpdateSummary = (order: IOrderMaster) => {
        let summaryDiv: HTMLElement = (document.getElementById("orderSummary") as HTMLElement);
        summaryDiv.innerHTML = "";
        if (order.OrderItems != undefined)
            if (order.OrderItems.length > 0) {
                let tbl: HTMLTableElement = document.createElement("table");
                tbl.className = "summaryTable"
                let row: HTMLTableRowElement = tbl.insertRow(0);
                row.className = "summaryTableHeaderRow"
                row.insertCell(0).innerHTML = "Product";
                row.insertCell(1).innerHTML = "Quantity";
                row.insertCell(2).innerHTML = "Unit";
                row.insertCell(3).innerHTML = "Price"

                for (let item of order.OrderItems) {
                    let row: HTMLTableRowElement = tbl.insertRow(tbl.rows.length);
                    let unit: IMaestroUnit = (this.state.Units as IMaestroUnit[]).find(u => u.Id == item.UnitId);
                    let map: IQbProductMap = (this.state.ProductMaps as IQbProductMap[]).find(m => m.Id == item.MapId);
                    row.insertCell(0).innerHTML = map.QuickBooksDescription;
                    row.insertCell(1).innerHTML = item.Quantity.toString();
                    row.insertCell(2).innerHTML = unit == undefined ? "" : unit.Name;
                    row.insertCell(3).innerHTML = map.Price.toString();
                }
                summaryDiv.appendChild(tbl);
            }
    }

    UpdateOrder = (mapId: number, unitId: number, quantity: number) => {
        $("body").addClass("loading");
        let order: IOrderMaster = this.state.Entity;
        if (order.OrderItems == undefined)
            order.OrderItems = new Array<IOrderItem>();

        let map:IQbProductMap =(this.state.ProductMaps as IQbProductMap[]).find(m => m.Id == mapId);

        let item: IOrderItem = order.OrderItems.find(i => i.MapId == mapId);
        if (item == undefined) {
            if (quantity > 0) {
                item = new OrderItem(order.Id, 0);
                item.MapId = mapId;
                item.Quantity = quantity;
                item.UnitId = unitId;
                item.ProductId = map.ProductId;
                item.ItemPrice = map.Price;
                order.OrderItems.push(item);
            }
        }
        else {
            if (quantity > 0) {
                item.Quantity = quantity;
                item.UnitId = unitId;
                item.ItemPrice = map.Price;
            }
            else
                order.OrderItems.splice(order.OrderItems.indexOf(item), 1);
        }

        this.setState({ Entity: order });
        this.UpdateSummary(order);
        $("body").removeClass("loading");
    }

    async Cancel(): Promise<IResponseMessage> {
        return null;
    }

    async Integrate(): Promise<IResponseMessage> {

        $("body").addClass("loading");
        let order: OrderMaster = this.state.Entity;
        let ea: EntityAgent = new EntityAgent();
        this.DisableEnable(true);
        let result: IResponseMessage = await ea.ExportOrder(order);
        if (result.ErrorInfo != null) {
            $("body").removeClass("loading");
            throw result.ErrorInfo;
        }
        order = result.TransactionResult as IOrderMaster;
        this.setState({ Entity: order });
        (document.getElementById("intergationStatusId") as HTMLInputElement).value = order.IntegrationStatus;
        $("body").removeClass("loading");
        return result;
    }


    async Save(): Promise<IResponseMessage> {

        $("body").addClass("loading");
        let order: OrderMaster = this.state.Entity;
        const getError = (msg: string): IErrorInfo => {
            let result: ErrorInfo = new ErrorInfo();
            result.UserFriendlyMessage = msg;
            result.DisableAction = false;
            return result;
        };

        if (order.CustomerId == undefined || order.CustomerId <= 0) {
            //alert("Please select customer");
            $("body").removeClass("loading");
            throw getError("Please select customer");
        } else if (order.OrderItems == undefined || order.OrderItems.length == 0) {
            //alert("Please select at least 1 product");
            $("body").removeClass("loading");
            throw getError("Please select at least 1 product");
        } else if (order.OrderItems.find(i => i.UnitId <= 0) != undefined) {
            $("body").removeClass("loading");
            let msg: string = "Please select unit for `" + (this.state.ProductMaps as IQbProductMap[]).find(m => m.Id == order.OrderItems.find(i => i.UnitId <= 0).MapId).QuickBooksDescription + "`";
            //alert(msg);
            throw getError(msg);
            //return;
        }

        //order.CustomerId = parseInt((document.getElementById("orderCustomerId") as HTMLSelectElement).value);
        order.DeliveryDate = this.state.DeliveryDate;
        order.OrderDate = this.state.OrderDate;
        order.Notes = (document.getElementById("notesId") as HTMLInputElement).value;
        order.PaymentType = "";
        order.CreateInvoiceOnQb = (document.getElementById("chkInvoiceCreate") as HTMLInputElement).checked;
        let ea: EntityAgent = new EntityAgent();
        this.DisableEnable(true);
        let result: IResponseMessage = await ea.SaveOrder(order);
        order.Actions = new Array<string>();
        if (result.ErrorInfo != null) {
            this.DisableEnable(false);
            order.Actions.push("Save");
            $("body").removeClass("loading");
            this.setState({ Entity: order });
            
            throw result.ErrorInfo;
        }
        else {
            $("body").removeClass("loading");
            order.Actions.push("Cancel");
            if (!order.CreateInvoiceOnQb)
                order.Actions.push("Integrate");
            this.setState({ Entity: order });
            
            
            return result;

        }
    }

    async componentDidMount() {


        //this.Save = this.Save.bind(this);

        let response: IResponseMessage;
        let order: IOrderMaster;

        $("body").addClass("loading");
        
        if (this.props.Entity.Id == 0) {
            response = await new AxiosAgent().getNewOrderId();

            if (response.TransactionStatus == "ERROR")
                throw (response.ErrorInfo);
            order = new OrderMaster(response.TransactionResult);
            order.IsNew = true;
            order.Actions = new Array<string>();
            order.Actions.push("Save");
        }
        else {
            order = this.props.Entity as IOrderMaster;
            order.IsNew = false;
            order.Actions = new Array<string>();
            
            order.Actions.push("Cancel");
            if (order.OrderStatus == "QB" || order.OrderStatus == "CC") {
                order.Actions.push("Cancel");    
            }
            else {
                order.Actions.push("Integrate");
                order.Actions.push("Save");
            }

        }

        let ea: EntityAgent = new EntityAgent();

        let cd: IOrderDisplay = await ea.GetOrderDisplay();

        const setOrderState =  () => {
            cd.Entity = order;
            cd.Init = false;
            cd.DeliveryDate = order.DeliveryDate;
            cd.OrderDate = order.OrderDate;
            this.setState(cd);
            this.UpdateSummary(order);
            if (order.OrderStatus == "QB" || order.OrderStatus == "CC") { //cancelled or integrated
                this.DisableEnable(true);
                //this.props.ButtonSetMethod(order.Actions);
            }
        };

        let exOccured:boolean = false
        if (cd.ErrorInfo != null) {
            if (cd.ErrorInfo.UserFriendlyMessage != "") {
                exOccured = true;
                this.props.ExceptionMethod(cd.ErrorInfo);
            }
        }

        if (!exOccured)
            setOrderState();

        $("body").removeClass("loading");
        //$('#wait').hide();


    }




    renderMaps(mapId: number, productId:number, productLabel: string, unitTypeLabel: string, units: IMaestroUnit[]) {

        let cpuList: ICustomerProductUnit[] = this.state.CustomerProductUnits as ICustomerProductUnit[];
        let customerId: number = this.state.Entity.CustomerId;
        let cpu: ICustomerProductUnit = customerId > 0 ? cpuList.find(c => c.CustomerId == customerId && c.ProductId == productId) : undefined;

        let item: IOrderItem = this.state.Entity.OrderItems == undefined ? undefined : this.state.Entity.OrderItems.find(i => i.MapId == mapId);
        
        let quantity: number =  item == undefined ? 0 : item.Quantity;
        return (
            <OrderProductItem {...{
                UpdateOrderMethod: this.UpdateOrder, Cpu:cpu,
                ProductLabel: productLabel, Quantity: quantity, UnitTypeLabel: unitTypeLabel, Units: units, MapId: mapId
            }} />
        );
    }




    renderTabs() {

        let products: IMaestroProduct[] = this.state.Products;
        let productGroups: IMaestroProductGroup[] = this.state.ProductGroups;
        let maps: IQbProductMap[] = this.state.ProductMaps as IQbProductMap[];
        let units: IMaestroUnit[] = this.state.Units as IMaestroUnit[];
        return (
            <div style={{ padding: "8px", textAlign: "left", width:"900px"}}>
                <Tabs  defaultActiveKey="1" id="uncontrolled-tab-example">
                    {
                        productGroups.filter(pg => pg.Name != "UNKNOWN").map(pg =>
                            <Tab id={pg.Name+"_tabId"} eventKey={pg.Id} title={pg.Name}>
                                <Container className="tabRegion">
                                    {

                                        maps.filter(m=>m.UnitTypeCanHaveUnits && m.ProductGroupId == pg.Id && m.QuickBooksParentCode=="")
                                            .concat(
                                                maps.filter(m => !m.UnitTypeCanHaveUnits && pg.Id == m.ProductGroupId && m.QuickBooksParentCode != "")
                                        ).sort((a, b) => a.QuickBooksDescription.localeCompare(b.QuickBooksDescription))
                                            .map(m => this.renderMaps(
                                                m.Id,
                                                m.ProductId,
                                                m.QuickBooksDescription,
                                                m.UnitTypeName,
                                                m.UnitTypeCanHaveUnits ? units.filter(u => maps.filter(map => map.QuickBooksParentCode == m.QuickBooksCode).find(map => map.UnitId == u.Id)) : null
                                                ))

                                    }
                                </Container>    
                            </Tab>)
                    }
                </Tabs>
            </div>
        );
    }

    render() {
        if (!this.state.Init) {
            
            let customers: IMaestroCustomer[] = this.state.Customers;
            if (customers.find(c => c.Id == -1) == undefined)
                customers.unshift(EntityAgent.GetFirstSelecItem("CUSTOMER") as IMaestroCustomer);

            customers.sort((a, b) => { return a.Name.localeCompare(b.Name); });

            let order: IOrderMaster = this.state.Entity;





            this.props.ButtonSetMethod(order.Actions);
            return (
                <div className="container">

                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Order Id</Col>
                        <Col style={{ paddingTop: "5px" }}>
                            <Form.Control plaintext readOnly defaultValue={order.Id.toString()} />
                        </Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                            <img src="/img/orderSummary.png" className={ this.state.SummaryDisplay.display == "none" ? "" : "disabled"  }
                                onClick={() => { this.setState({ SummaryDisplay: { display: "block" } }) }}
                                style={{ float: "right", cursor: this.state.SummaryDisplay.display == "none" ? "pointer" : "default" }} />
                        </Col>
                    </Row>
                    <Row >
                        <Col style={{ paddingTop: "5px" }} sm={2}>Integration status</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                            <Form.Control id="intergationStatusId" plaintext readOnly defaultValue={order.IntegrationStatus} />
                        </Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Order status</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                            <Form.Control id="orderStatusId" plaintext readOnly defaultValue={order.OrderStatus} />
                        </Col>


                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Customer</Col>
                        <Col style={{ paddingTop: "5px" }} sm={6}>
                            <Form.Control as="select" id="orderCustomerId" onChange={this.OnCustomerChange} >
                                {
                                    customers.map(customer => <option selected={customer.Id == order.CustomerId} value={customer.Id}>{customer.Name + (customer.Name.startsWith("--") ? "" : " (" + customer.QuickBoosCompany + ")")} </option>)
                                }

                            </Form.Control>
                        </Col>
                    </Row>

                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Order Date</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                            <Form.Control as={DatePicker} id="orderDateId"
                                selected={this.state.OrderDate}
                                onChange={(dt) => { this.setState({ OrderDate: dt }) }} />
                        </Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Delivery Date</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                            <Form.Control as={DatePicker} id="deliveryDateId"
                                selected={this.state.DeliveryDate}
                                onChange={(dt) => { this.setState({ DeliveryDate: dt }) }} />

                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Notes</Col>
                        <Col style={{ paddingTop: "5px" }} sm={6}>
                            <Form.Control as="textarea" id="notesId" rows="2" value={order.Notes} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Create Invoice</Col>
                        <Col>
                            <Form.Check aria-label="chkInvoiceCreate" id="chkInvoiceCreate" />
                        </Col>
                    </Row>
                    <Row>
                        <Col sm={12}>
                            {this.renderTabs()}
                        </Col>
                    </Row>



                    <Draggable
                        axis="both"
                        handle=".orderSummaryHeader"
                        defaultPosition={{ x: 0, y: 0 }}
                        position={null}
                        grid={[10, 10]}
                        scale={1}
                        offsetParent={document.getElementById("content") as HTMLElement}
                    >
                        <div>
                            <div style={this.state.SummaryDisplay} className="orderSummaryHeader">Order Summary
                                <img src="/img/closeWindow.png"
                                    onClick={() => {
                                        this.setState({ SummaryDisplay: { display: "none" } })
                                    }}
                                    style={{ float: "right", cursor:"pointer" }} />

                            </div>
                            <div style={this.state.SummaryDisplay} className="orderSummary" id="orderSummary" ></div>
                        </div>
                    </Draggable>
                    </div>
            );
        }
        else
            return(<div></div>)
    }

}