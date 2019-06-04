var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import EntityAgent from '../../classes/EntityAgent';
import ErrorInfo from '../../classes/ErrorInfo';
import { Row, Col, Form } from 'react-bootstrap';
import AxiosAgent from '../../classes/AxiosAgent';
import MergeOrderComponent from '../MergeOrderComponent';
export default class InvoiceMerge extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Init: true, ErrorInfo: new ErrorInfo(), Customers: null, Orders: null, Templates: [""], OrdersToMerge: [], Invoice: null };
        this.OnInvoiceGroupChange = (evt) => __awaiter(this, void 0, void 0, function* () {
            let invGrp = evt.target.value;
            let templates = [""];
            let orders = null;
            if (invGrp != "") {
                let customers = this.state.Customers.filter(c => c.InvoiceGroup == invGrp);
                templates = [...new Set(customers.map(c => c.CustomerGroup))];
                if (templates.length > 1) {
                    if (templates.find(t => t.startsWith("--")) == undefined)
                        templates.unshift("--Please select--");
                }
                let exOccured = false;
                let msg = { ["INVOICE_GROUP"]: invGrp, ["LIST_CODE"]: "MERGE_INVOICE" };
                let response = yield new AxiosAgent().getList("ORDER", msg);
                if (response.ErrorInfo != null) {
                    if (response.ErrorInfo.UserFriendlyMessage != "") {
                        exOccured = true;
                        this.props.ExceptionMethod(response.ErrorInfo);
                    }
                }
                if (!exOccured)
                    orders = response.TransactionResult;
            }
            this.setState({ Templates: templates, Orders: orders });
        });
        this.ChangeOrderStatus = (id, location, show) => __awaiter(this, void 0, void 0, function* () {
            let ordersToMerge = this.state.OrdersToMerge;
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
            yield this.setState({ OrdersToMerge: ordersToMerge });
            if (this.state.OrdersToMerge.length > 0) {
                let s = "";
                [...this.state.OrdersToMerge].map(n => s += "<P>" + n + "</P>");
                document.getElementById("xxx").innerHTML = s;
            }
            else
                document.getElementById("xxx").innerHTML = "";
        });
        this.state = { Invoice: null, Init: true, ErrorInfo: new ErrorInfo(), Customers: null, Orders: null, Templates: [""], OrdersToMerge: [] };
    }
    Save() {
        throw new Error("Method not implemented.");
    }
    Cancel() {
        throw new Error("Method not implemented.");
    }
    Integrate() {
        throw new Error("Method not implemented.");
    }
    DisableEnable(disable) {
        throw new Error("Method not implemented.");
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cd = yield ea.GetInvoiceMergeDisplay();
            let invoice = cd.Invoice;
            invoice.Actions = new Array();
            invoice.Actions.push("Integrate");
            const setWindowState = () => {
                cd.Init = false;
                this.setState(cd);
                //if (order.OrderStatus == "QB" || order.OrderStatus == "CC") { //cancelled or integrated
                //this.DisableEnable(true);
                this.props.ButtonSetMethod(invoice.Actions);
            };
            let exOccured = false;
            if (cd.ErrorInfo != null) {
                if (cd.ErrorInfo.UserFriendlyMessage != "") {
                    exOccured = true;
                    this.props.ExceptionMethod(cd.ErrorInfo);
                }
            }
            if (!exOccured)
                setWindowState();
        });
    }
    renderOrders(location) {
        let orders = this.state.Orders;
        return (React.createElement("div", { className: "mergeOrderHostDiv" }, orders.map(o => React.createElement(MergeOrderComponent, { ClickFct: this.ChangeOrderStatus, RenderOrderMaster: o, Location: location, Display: "SOURCE" == location ? this.state.OrdersToMerge.indexOf(o.Id) < 0 : this.state.OrdersToMerge.indexOf(o.Id) >= 0 }))));
    }
    render() {
        if (!this.state.Init) {
            let templateList = this.state.Templates;
            let disableTemplates = templateList.length == 1 && templateList[0] == "";
            let customers = this.state.Customers.filter(c => c.Name != "UNKNOWN");
            let invoiceGroups = [...new Set(customers.map(c => c.InvoiceGroup))];
            invoiceGroups.sort((a, b) => a.localeCompare(b));
            if (invoiceGroups.find(c => c.startsWith("--")) == undefined)
                invoiceGroups.unshift("--Please select--");
            return (React.createElement("div", { className: "container" },
                React.createElement("div", { className: "invMergeTop" },
                    React.createElement(Row, null,
                        React.createElement(Col, { sm: 2 }, "Customer"),
                        React.createElement(Col, { sm: 6 },
                            React.createElement(Form.Control, { as: "select", id: "orderCustomerId", onChange: this.OnInvoiceGroupChange }, 
                            //customers.map(customer => <option value={customer.Id}>{customer.Name + (customer.Name.startsWith("--") ? "" : " (" + customer.QuickBoosCompany + ")")} </option>)
                            invoiceGroups.map(i => React.createElement("option", { value: i.startsWith("--") ? "" : i }, i))))),
                    React.createElement(Row, { style: { paddingTop: "20px" } },
                        React.createElement(Col, { sm: 2 }, "Invoice Template"),
                        React.createElement(Col, { sm: 6 },
                            React.createElement(Form.Control, { as: "select", id: "templateId", disabled: disableTemplates }, templateList.map(t => React.createElement("option", { value: t.startsWith("--") ? "" : t }, t)))))),
                React.createElement("div", { className: "invMergeSides" }, this.state.Orders == null ? "" : this.renderOrders("SOURCE")),
                React.createElement("div", { className: "invMergeSides" }, this.state.Orders == null ? "" : this.renderOrders("TARGET")),
                React.createElement("div", { id: "xxx" })));
        }
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=InvoiceMerge.js.map