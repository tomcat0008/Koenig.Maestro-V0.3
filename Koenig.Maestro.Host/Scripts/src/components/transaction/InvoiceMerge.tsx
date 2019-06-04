import * as React from 'react';
import { ICrudComponent } from '../ICrudComponent';
import ResponseMessage, { IResponseMessage } from '../../classes/ResponseMessage';
import { ITranComponentProp } from '../../classes/ITranComponentProp';
import EntityAgent, { IInvoiceMergeDisplay } from '../../classes/EntityAgent';
import ErrorInfo from '../../classes/ErrorInfo';
import { Row, Col, Form, Button, Image } from 'react-bootstrap';
import { IMaestroCustomer } from '../../classes/dbEntities/IMaestroCustomer';
import AxiosAgent from '../../classes/AxiosAgent';
import { IOrderMaster } from '../../classes/dbEntities/IOrderMaster';
import { IOrderItem } from '../../classes/dbEntities/IOrderItem';
import MergeOrderComponent from '../MergeOrderComponent';
import { IQbInvoiceLog } from '../../classes/dbEntities/IQbInvoiceLog';

export default class InvoiceMerge extends React.Component<ITranComponentProp, IInvoiceMergeDisplay> implements ICrudComponent{

    state = { Init: true, ErrorInfo: new ErrorInfo(), Customers: null, Orders: null, Templates: [""], OrdersToMerge: [], Invoice:null };
    
    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { Invoice: null, Init: true, ErrorInfo: new ErrorInfo(), Customers: null, Orders: null, Templates: [""], OrdersToMerge:[] };
    }
    
    Save(): Promise<IResponseMessage> {
        throw new Error("Method not implemented.");
    }

    Cancel(): Promise<IResponseMessage> {
        throw new Error("Method not implemented.");
    }

    Integrate(): Promise<IResponseMessage> {
        let s: string = "";
        if (this.state.OrdersToMerge.length > 0) {
            
            [...this.state.OrdersToMerge].map(n => s += "<P>" + n + "</P>");
        }
        
            
        alert(s);
        throw new Error("Method not implemented.");
    }

    DisableEnable(disable: boolean): void {
        throw new Error("Method not implemented.");
    }

    
    async componentDidMount() {
        let ea: EntityAgent = new EntityAgent();
        let cd: IInvoiceMergeDisplay = await ea.GetInvoiceMergeDisplay();
        let invoice: IQbInvoiceLog = cd.Invoice;
        invoice.Actions = new Array<string>();
        invoice.Actions.push("Integrate");

        const setWindowState = () => {
            cd.Init = false;
            this.setState(cd);
            
            //if (order.OrderStatus == "QB" || order.OrderStatus == "CC") { //cancelled or integrated
                //this.DisableEnable(true);
            this.props.ButtonSetMethod(invoice.Actions);
            
        };
        let exOccured: boolean = false


        if (cd.ErrorInfo != null) {
            if (cd.ErrorInfo.UserFriendlyMessage != "") {
                exOccured = true;
                this.props.ExceptionMethod(cd.ErrorInfo);
            }
        }
        
        if (!exOccured)
            setWindowState();
        
    }

    OnInvoiceGroupChange = async (evt) => {
        let invGrp: string = evt.target.value;
        let templates: string[] = [""];
        let orders: IOrderMaster[] = null;
        if (invGrp != "") {

            let customers: IMaestroCustomer[] = this.state.Customers.filter(c => c.InvoiceGroup == invGrp);
            templates = [...new Set(customers.map(c => c.CustomerGroup))];

            if (templates.length > 1) {
                if (templates.find(t => t.startsWith("--")) == undefined)
                    templates.unshift("--Please select--");
            }

            let exOccured: boolean = false
            let msg: { [key: string]: string } = { ["INVOICE_GROUP"]: invGrp, ["LIST_CODE"]:"MERGE_INVOICE" };
            
            let response: IResponseMessage = await new AxiosAgent().getList("ORDER", msg);
            if (response.ErrorInfo != null) {
                if (response.ErrorInfo.UserFriendlyMessage != "") {
                    exOccured = true;
                    this.props.ExceptionMethod(response.ErrorInfo);
                }
            }

            if (!exOccured)
                orders = response.TransactionResult as IOrderMaster[];

        }
        this.setState({ Templates: templates, Orders: orders})

    }

    ChangeOrderStatus = async (id: number, location: string, show: boolean) => {


        let ordersToMerge: number[] = this.state.OrdersToMerge;

        if (location == "SOURCE") {
            if (show)
                ordersToMerge = ordersToMerge.filter(n => n != id);
            else {
                if (ordersToMerge.indexOf(id) == -1)
                    ordersToMerge.push(id);
            }
        }
        else {
            if (show) {
                if (ordersToMerge.indexOf(id) == -1)
                    ordersToMerge.push(id);
            }
            else
                ordersToMerge = ordersToMerge.filter(n => n != id);
        }

        await this.setState({ OrdersToMerge: ordersToMerge });

    }

    renderOrders(location:string) {

        let orders: IOrderMaster[] = this.state.Orders;

        return (
            <div className="mergeOrderHostDiv">
                {orders.map(o => <MergeOrderComponent ClickFct={this.ChangeOrderStatus} RenderOrderMaster={o} Location={location} Display={"SOURCE" == location ? this.state.OrdersToMerge.indexOf(o.Id) < 0 : this.state.OrdersToMerge.indexOf(o.Id) >= 0 } />)}
            </div>
        );
    }

    render() {

        if (!this.state.Init) {

            let templateList: string[] = this.state.Templates;
            let disableTemplates: boolean = templateList.length == 1 && templateList[0] == "";
            let customers: IMaestroCustomer[] = this.state.Customers.filter(c => c.Name != "UNKNOWN");

            let invoiceGroups: string[] = [...new Set(customers.map(c => c.InvoiceGroup))];
            invoiceGroups.sort((a, b) => a.localeCompare(b));

            if (invoiceGroups.find(c => c.startsWith("--")) == undefined)
                invoiceGroups.unshift("--Please select--");

            return (
                <div className="container">
                    <div className="invMergeTop" >
                        <Row>
                            <Col sm={2}>Customer</Col>
                            <Col sm={6}>
                                <Form.Control as="select" id="orderCustomerId" onChange={this.OnInvoiceGroupChange} >
                                    {
                                        //customers.map(customer => <option value={customer.Id}>{customer.Name + (customer.Name.startsWith("--") ? "" : " (" + customer.QuickBoosCompany + ")")} </option>)
                                        invoiceGroups.map(i => <option value={i.startsWith("--") ? "" : i }>{i}</option>)
                                    }

                                </Form.Control>
                            </Col>
                        </Row>
                        <Row style={{paddingTop:"20px"}}>
                            <Col sm={2}>Invoice Template</Col>
                            <Col sm={6}>
                                <Form.Control as="select" id="templateId" disabled={disableTemplates}>
                                    {
                                        templateList.map(t => <option value={t.startsWith("--") ? "" : t}>{t}</option>)
                                    }
                                </Form.Control>
                            </Col>
                        </Row>
                    </div>
                    <div className="invMergeSides">
                        { this.state.Orders == null ? "" : this.renderOrders("SOURCE") }
                    </div>
                    <div className="invMergeSides">
                        {this.state.Orders == null ? "" : this.renderOrders("TARGET")}
                    </div>
                </div>
                
            );
        }
        else {
            return (<div></div>);
        }
    }


}