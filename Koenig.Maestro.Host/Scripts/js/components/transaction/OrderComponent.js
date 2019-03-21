var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import OrderMaster from '../../classes/dbEntities/IOrderMaster';
import AxiosAgent from '../../classes/AxiosAgent';
import EntityAgent from '../../classes/EntityAgent';
import { Form, Col, Tabs, Tab, Container } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import ErrorInfo from '../../classes/ErrorInfo';
import DatePicker from "react-datepicker";
import OrderProductItem from './OrderProductItem';
import { OrderItem } from '../../classes/dbEntities/IOrderItem';
import Draggable from 'react-draggable';
export default class OrderComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            Entity: new OrderMaster(0), Customers: [], Products: [], ProductMaps: [],
            OrderDate: new Date(), DeliveryDate: new Date(), ProductGroups: [], Units: [],
            CustomerProductUnits: [], Init: false, ErrorInfo: new ErrorInfo(), SummaryDisplay: { display: "block" }
        };
        this.DisableEnable = (disable) => {
            document.getElementById("orderCustomerId").disabled = disable;
            document.getElementById("orderDateId").disabled = disable;
            document.getElementById("deliveryDateId").disabled = disable;
            document.getElementById("notesId").disabled = disable;
            let maps = this.state.ProductMaps;
            for (let map of maps) {
                let bt = document.getElementById("mapName_" + map.Id);
                if (bt != undefined && bt != null) {
                    bt.disabled = disable;
                    document.getElementById("btnIncrease_" + map.Id).disabled = disable;
                    document.getElementById("btnDecrease_" + map.Id).disabled = disable;
                    document.getElementById("btnDisable" + map.Id).disabled = disable;
                    document.getElementById("quantity_" + map.Id).disabled = disable;
                    document.getElementById("unitNr_" + map.Id).disabled = disable;
                }
            }
        };
        this.OnCustomerChange = (evt) => {
            let order = this.state.Entity;
            order.CustomerId = parseInt(evt.target.value);
            this.setState({ Entity: order });
        };
        this.UpdateSummary = (order) => {
            let summaryDiv = document.getElementById("orderSummary");
            summaryDiv.innerHTML = "";
            if (order.OrderItems != undefined)
                if (order.OrderItems.length > 0) {
                    let tbl = document.createElement("table");
                    tbl.className = "summaryTable";
                    let row = tbl.insertRow(0);
                    row.className = "summaryTableHeaderRow";
                    row.insertCell(0).innerHTML = "Product";
                    row.insertCell(1).innerHTML = "Quantity";
                    row.insertCell(2).innerHTML = "Unit";
                    row.insertCell(3).innerHTML = "Price";
                    for (let item of order.OrderItems) {
                        let row = tbl.insertRow(tbl.rows.length);
                        let unit = this.state.Units.find(u => u.Id == item.UnitId);
                        let map = this.state.ProductMaps.find(m => m.Id == item.MapId);
                        row.insertCell(0).innerHTML = map.QuickBooksDescription;
                        row.insertCell(1).innerHTML = item.Quantity.toString();
                        row.insertCell(2).innerHTML = unit == undefined ? "" : unit.Name;
                        row.insertCell(3).innerHTML = map.Price.toString();
                    }
                    summaryDiv.appendChild(tbl);
                }
        };
        this.UpdateOrder = (mapId, unitId, quantity) => {
            let order = this.state.Entity;
            if (order.OrderItems == undefined)
                order.OrderItems = new Array();
            let map = this.state.ProductMaps.find(m => m.Id == mapId);
            let item = order.OrderItems.find(i => i.MapId == mapId);
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
        };
        this.state = {
            Customers: [], Products: [], ProductMaps: [], CustomerProductUnits: [],
            OrderDate: new Date(), DeliveryDate: new Date(), ProductGroups: [], Units: [],
            Entity: props.Entity, Init: true, ErrorInfo: new ErrorInfo(),
            SummaryDisplay: { display: "block" }
        };
        this.renderTabs = this.renderTabs.bind(this);
        this.renderMaps = this.renderMaps.bind(this);
    }
    Save() {
        return __awaiter(this, void 0, void 0, function* () {
            let order = this.state.Entity;
            const getError = (msg) => {
                let result = new ErrorInfo();
                result.UserFriendlyMessage = msg;
                result.DisableAction = false;
                return result;
            };
            if (order.CustomerId == undefined || order.CustomerId <= 0) {
                //alert("Please select customer");
                throw getError("Please select customer");
            }
            else if (order.OrderItems == undefined || order.OrderItems.length == 0) {
                //alert("Please select at least 1 product");
                throw getError("Please select at least 1 product");
            }
            else if (order.OrderItems.find(i => i.UnitId <= 0) != undefined) {
                let msg = "Please select unit for `" + this.state.ProductMaps.find(m => m.Id == order.OrderItems.find(i => i.UnitId <= 0).MapId).QuickBooksDescription + "`";
                //alert(msg);
                throw getError(msg);
                //return;
            }
            //order.CustomerId = parseInt((document.getElementById("orderCustomerId") as HTMLSelectElement).value);
            order.DeliveryDate = this.state.DeliveryDate;
            order.OrderDate = this.state.OrderDate;
            order.Notes = document.getElementById("notesId").value;
            order.PaymentType = "";
            order.CreateInvoiceOnQb = document.getElementById("chkInvoiceCreate").checked;
            let ea = new EntityAgent();
            this.DisableEnable(true);
            let result = yield ea.SaveOrder(order);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
            }
            else {
                return result;
            }
        });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            //this.Save = this.Save.bind(this);
            let response;
            let order;
            if (this.props.Entity.Id == 0) {
                response = yield new AxiosAgent().getNewOrderId();
                if (response.TransactionStatus == "ERROR")
                    throw (response.ErrorInfo);
                order = new OrderMaster(response.TransactionResult);
                order.IsNew = true;
            }
            else {
                order = this.props.Entity;
                order.IsNew = false;
            }
            let ea = new EntityAgent();
            let cd = yield ea.GetOrderDisplay();
            const setOrderState = () => {
                cd.Entity = order;
                cd.Init = false;
                cd.DeliveryDate = order.DeliveryDate;
                cd.OrderDate = order.OrderDate;
                this.setState(cd);
            };
            let exOccured = false;
            if (cd.ErrorInfo != null) {
                if (cd.ErrorInfo.UserFriendlyMessage != "") {
                    exOccured = true;
                    this.props.ExceptionMethod(cd.ErrorInfo);
                }
            }
            if (!exOccured)
                setOrderState();
            $('#wait').hide();
        });
    }
    renderMaps(mapId, productId, productLabel, unitTypeLabel, units) {
        let cpuList = this.state.CustomerProductUnits;
        let customerId = this.state.Entity.CustomerId;
        let cpu = customerId > 0 ? cpuList.find(c => c.CustomerId == customerId && c.ProductId == productId) : undefined;
        let item = this.state.Entity.OrderItems == undefined ? undefined : this.state.Entity.OrderItems.find(i => i.MapId == mapId);
        let quantity = item == undefined ? 0 : item.Quantity;
        return (React.createElement(OrderProductItem, Object.assign({}, {
            UpdateOrderMethod: this.UpdateOrder, Cpu: cpu,
            ProductLabel: productLabel, Quantity: quantity, UnitTypeLabel: unitTypeLabel, Units: units, MapId: mapId
        })));
    }
    renderTabs() {
        let products = this.state.Products;
        let productGroups = this.state.ProductGroups;
        let maps = this.state.ProductMaps;
        let units = this.state.Units;
        return (React.createElement("div", { style: { padding: "8px", textAlign: "left", width: "900px" } },
            React.createElement(Tabs, { defaultActiveKey: "1", id: "uncontrolled-tab-example" }, productGroups.filter(pg => pg.Name != "UNKNOWN").map(pg => React.createElement(Tab, { id: pg.Name + "_tabId", eventKey: pg.Id, title: pg.Name },
                React.createElement(Container, { className: "tabRegion" }, maps.filter(m => m.UnitTypeCanHaveUnits && m.ProductGroupId == pg.Id && m.QuickBooksParentCode == "")
                    .concat(maps.filter(m => !m.UnitTypeCanHaveUnits && pg.Id == m.ProductGroupId && m.QuickBooksParentCode != "")).sort((a, b) => a.QuickBooksDescription.localeCompare(b.QuickBooksDescription))
                    .map(m => this.renderMaps(m.Id, m.ProductId, m.QuickBooksDescription, m.UnitTypeName, m.UnitTypeCanHaveUnits ? units.filter(u => maps.filter(map => map.QuickBooksParentCode == m.QuickBooksCode).find(map => map.UnitId == u.Id)) : null))))))));
    }
    render() {
        if (!this.state.Init) {
            let customers = this.state.Customers;
            if (customers.find(c => c.Id == -1) == undefined)
                customers.unshift(EntityAgent.GetFirstSelecItem("CUSTOMER"));
            customers.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            let order = this.state.Entity;
            return (React.createElement("div", { className: "container" },
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Order Id"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { plaintext: true, readOnly: true, defaultValue: order.Id.toString() })),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 },
                        React.createElement("img", { src: "/img/orderSummary.png", className: this.state.SummaryDisplay.display == "none" ? "" : "disabled", onClick: () => { this.setState({ SummaryDisplay: { display: "block" } }); }, style: { float: "right", cursor: this.state.SummaryDisplay.display == "none" ? "pointer" : "default" } }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Integration status"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 6 },
                        React.createElement(Form.Control, { plaintext: true, readOnly: true, defaultValue: order.IntegrationStatus }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Customer"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 6 },
                        React.createElement(Form.Control, { as: "select", id: "orderCustomerId", onChange: this.OnCustomerChange }, customers.map(customer => React.createElement("option", { selected: customer.Id == order.CustomerId, value: customer.Id },
                            customer.Name + (customer.Name.startsWith("--") ? "" : " (" + customer.QuickBoosCompany + ")"),
                            " "))))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Order Date"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 },
                        React.createElement(Form.Control, { as: DatePicker, id: "orderDateId", selected: this.state.OrderDate, onChange: (dt) => { this.setState({ OrderDate: dt }); } })),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Delivery Date"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 },
                        React.createElement(Form.Control, { as: DatePicker, id: "deliveryDateId", selected: this.state.DeliveryDate, onChange: (dt) => { this.setState({ DeliveryDate: dt }); } }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Notes"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 6 },
                        React.createElement(Form.Control, { as: "textarea", id: "notesId", rows: "2", value: order.Notes }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Create Invoice"),
                    React.createElement(Col, null,
                        React.createElement(Form.Check, { "aria-label": "chkInvoiceCreate", id: "chkInvoiceCreate" }))),
                React.createElement(Row, null,
                    React.createElement(Col, { sm: 12 }, this.renderTabs())),
                React.createElement(Draggable, { axis: "both", handle: ".orderSummaryHeader", defaultPosition: { x: 0, y: 0 }, position: null, grid: [10, 10], scale: 1, offsetParent: document.getElementById("content") },
                    React.createElement("div", null,
                        React.createElement("div", { style: this.state.SummaryDisplay, className: "orderSummaryHeader" },
                            "Order Summary",
                            React.createElement("img", { src: "/img/closeWindow.png", onClick: () => {
                                    this.setState({ SummaryDisplay: { display: "none" } });
                                }, style: { float: "right", cursor: "pointer" } })),
                        React.createElement("div", { style: this.state.SummaryDisplay, className: "orderSummary", id: "orderSummary" })))));
        }
        else
            return (React.createElement("div", null));
    }
}
//# sourceMappingURL=OrderComponent.js.map