var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from "react";
import { ListGroup, Image, Card, Alert, Modal, Row, Col, Button, CardGroup } from "react-bootstrap";
import Chart from 'react-google-charts';
import ResponseMessage from "../../classes/ResponseMessage";
import AxiosAgent from "../../classes/AxiosAgent";
import ErrorInfo from "../../classes/ErrorInfo";
import ModalContainer from "../ModalConatiner";
import TranRequest from "../../classes/ListRequest";
import TransactionDisplay from "../TransactionDisplay";
import * as ReactDOM from "react-dom";
import GridDisplay from "../GridDisplay";
import EntityAgent from "../../classes/EntityAgent";
export default class Dashboard extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: "", ConfirmText: "", ConfirmShow: false,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
            MsgDataExtension: {}, selected: []
        };
        this.handleModalClose = () => __awaiter(this, void 0, void 0, function* () {
            this.setState({ ShowModal: false });
            yield this.loadDashboardData();
        });
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield this.tranComponent.Save();
                this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
                this.handleModalClose;
            }
            catch (error) {
                this.setState({ ErrorInfo: error, ShowError: true });
            }
        });
        this.handleClick = (id) => __awaiter(this, void 0, void 0, function* () {
            //this.setState({ loading: true })
            let req = new TranRequest();
            req.ListTitle = id.caption;
            req.Action = id.action;
            req.TranCode = id.tranCode;
            req.MsgExtension = id.msgExtension;
            req.ButtonList = id.buttonList;
            req.ListSelect = id.listSelect;
            let tranCode = id.tranCode;
            if (id.action == "New") {
                let entity = EntityAgent.FactoryCreate(tranCode);
                yield this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
            }
            if (id.action == "List") {
                $('#mainMenu').hide();
                $("body").addClass("loading");
                ReactDOM.render(React.createElement(GridDisplay, Object.assign({}, req)), document.getElementById('content'));
                return;
            }
            if (id.action == "Report") {
                yield this.setState({ TranCode: id.tranCode, Action: id.action, ConfirmShow: false, ShowModal: true });
                return;
            }
            if (id.action == "Merge") {
                if (tranCode == "QUICKBOOKS_INVOICE") {
                    let entity = EntityAgent.FactoryCreate(tranCode);
                    yield this.setState({ ShowModal: true, ModalCaption: "Merge " + tranCode.toLowerCase(), Action: "Merge", TranCode: tranCode, Entity: entity });
                }
                return;
            }
            if (id.action == "ImportQb") {
                switch (this.state.TranCode) {
                    case "PRODUCT":
                        yield this.setState({
                            ConfirmText: "Do you want to import products from QB ?"
                        });
                        break;
                    case "CUSTOMER":
                        yield this.setState({
                            ConfirmText: "Do you want to import customers from QB ?"
                        });
                        break;
                    case "QUICKBOOKS_INVOICE":
                        yield this.setState({
                            ConfirmText: "Do you want to import invoices from QB ?"
                        });
                        break;
                }
                yield this.setState({ TranCode: id.tranCode, Action: id.action, ConfirmShow: true });
                return;
            }
        });
        this.onYes = () => {
            this.setState({ ConfirmShow: false });
            let req = new TranRequest();
            req.ListTitle = "";
            req.Action = this.state.Action;
            req.TranCode = this.state.TranCode;
            req.MsgExtension = { ["IMPORT_TYPE"]: '' };
            req.ButtonList = [];
            req.ListSelect = false;
            $('#wait').show();
            ReactDOM.render(React.createElement(TransactionDisplay, Object.assign({}, req)), document.getElementById('content'));
        };
        this.onNo = () => {
            this.setState({ ConfirmShow: false, ButtonAction: "", ConfirmText: "" });
        };
        this.prepareTimeLineData = () => {
            let orderHistory = this.state.ResponseMessage.TransactionResult["YTD_ORDER_TOTAL"];
            const rows = [];
            const columns = [
                { type: 'date', label: 'Date' },
                { type: 'number', label: 'Revenue' },
            ];
            for (let i = 0; i < orderHistory.length; i++) {
                let dtString = orderHistory[i]["ORDER_DATE"];
                let dt = new Date(parseInt(dtString.substr(6, 4)), parseInt(dtString.substr(3, 2)) - 1, parseInt(dtString.substr(0, 2)));
                let total = parseFloat(orderHistory[i]["TOTAL"]);
                rows.push([dt, total]);
            }
            let chartData = [columns, ...rows];
            return chartData;
        };
        this.prepareMonthlySummary = () => {
            let data = this.state.ResponseMessage.TransactionResult["YEAR_MONTH_TOTAL"];
            const rows = [];
            const columns = [
                { type: 'string', label: 'Month' },
                { type: 'number', label: new Date().getFullYear() - 1 },
                { type: 'number', label: new Date().getFullYear() }
            ];
            for (let i = 0; i < data.length; i++) {
                rows.push([
                    data[i]["ORDER_MONTH"],
                    parseFloat(data[i][new Date().getFullYear() - 1]),
                    parseFloat(data[i][new Date().getFullYear()])
                ]);
            }
            let chartData = [columns, ...rows];
            //console.log(chartData);
            return chartData;
        };
        let errorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            ShowError: false, ErrorInfo: errorInfo, Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: "", ConfirmText: "", ConfirmShow: false,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
            MsgDataExtension: {}, selected: []
        };
    }
    loadDashboardData() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                //$('#content').hide();
                $("body").addClass("loading");
                let msgExt = { ["LIST_CODE"]: 'DashboardSummary' };
                let response = yield new AxiosAgent().getList("ORDER", msgExt);
                this.setState({ ResponseMessage: response, Init: false });
                if (response.TransactionStatus == "ERROR") {
                    $("body").removeClass("loading");
                    throw (response.ErrorInfo);
                }
            }
            catch (error) {
                this.setState({ ErrorInfo: error, ShowError: true });
                //console.error(error);
            }
            $("body").removeClass("loading");
        });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.loadDashboardData();
        });
    }
    renderNextDayOrders() {
        let data = this.state.ResponseMessage.TransactionResult["ORDERS_NEXT_DAY"];
        let dt = new Date();
        dt.setDate(dt.getDate() + 1);
        let tomorrow = "Orders for tomorrow <br/>" + dt.toLocaleDateString("en-GB") + " " + dt.toLocaleDateString("en-GB", { weekday: 'long' });
        let orderList = "<div class=\"listCap\">" + tomorrow + "</div>";
        let orderId = 0;
        let customerName = "";
        if (data.length > 0) {
            for (let i = 0; i < data.length; i++) {
                let id = parseInt(data[i]["ID"]);
                let quantity = parseInt(data[i]["QUANTITY"]);
                let product = data[i]["QB_DESCRIPTION"];
                if (orderId != id) {
                    orderId = id;
                    customerName = data[i]["QB_COMPANY"];
                    orderList += "<br/><p><b>" + customerName + " (" + id + ")</b></p>";
                    //orderList.push("<p><b>" + customerName + " (" + id + ")</b></p>")
                }
                orderList += "<li>" + quantity + " " + product + "</li>";
                //orderList.push("<li>" + quantity + " " + product + "</li>")
            }
        }
        else {
            orderList += "<br/><br/><div class=\"listCap\"><center><b>Oh no...no orders found !!!</b></center></div>";
        }
        return (React.createElement("div", { className: "dbInnerTable", dangerouslySetInnerHTML: { __html: orderList } }));
    }
    render() {
        if (!this.state.Init) {
            let dt = new Date();
            let c = this.state.ResponseMessage.TransactionResult["YTD_CUSTOMER_TOTAL"];
            let p = this.state.ResponseMessage.TransactionResult["YTD_PRODUCT_TOTAL"];
            let orderHistory = this.prepareTimeLineData();
            let prepareMonthlySummary = this.prepareMonthlySummary();
            return (React.createElement("div", { className: "dashboard" },
                React.createElement("div", { className: "dbrdMenuCol" },
                    React.createElement(ListGroup, null,
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "ORDER",
                                    action: "New",
                                    caption: "New order",
                                    msgExtension: {},
                                    buttonList: [],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/order_new.png" }),
                            "New Order"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "ORDER",
                                    action: "List",
                                    caption: "Orders",
                                    msgExtension: { ['PERIOD']: 'Month' },
                                    buttonList: ["New", "Return", "Today", "Week", "Month", "Year", "All"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/icon-order.png" }),
                            "Orders")),
                    React.createElement("br", null),
                    React.createElement(ListGroup, null,
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "REPORT",
                                    action: "Report",
                                    caption: "Reports",
                                    msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                    buttonList: ["Return"],
                                    listSelect: true
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/report.png" }),
                            "Reports"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "List",
                                    caption: "Export Orders to Quickbooks",
                                    msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                    buttonList: ["Return", "Today", "Week", "Month", "Year", "All", "QB"],
                                    listSelect: true
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/invoice_export.png" }),
                            "Export Invoices"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "Merge",
                                    caption: "Merge Invoices",
                                    msgExtension: { ["LIST_CODE"]: 'MERGE_INVOICE' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/invoice_export.png" }),
                            "Merge Invoices")),
                    React.createElement("br", null),
                    React.createElement(ListGroup, null,
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "PRODUCT",
                                    action: "List",
                                    caption: "Products",
                                    msgExtension: {},
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/cake.png" }),
                            "Products"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "CUSTOMER",
                                    action: "List",
                                    caption: "Customers",
                                    msgExtension: {},
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/clients.png" }),
                            "Customers"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "CUSTOMER_PRODUCT_UNIT",
                                    action: "List",
                                    caption: "Customer Product Units",
                                    msgExtension: {},
                                    buttonList: ["New", "Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/cup.png" }),
                            "Customer-Product-Unit"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "UNIT_TYPE",
                                    action: "List",
                                    caption: "Unit Types",
                                    msgExtension: {},
                                    buttonList: ["New", "Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/units.png" }),
                            "Unit Types"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "UNIT",
                                    action: "List",
                                    caption: "Units",
                                    msgExtension: {},
                                    buttonList: ["New", "Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/measure.png" }),
                            "Units"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "REGION",
                                    action: "List",
                                    caption: "Regions",
                                    msgExtension: {},
                                    buttonList: ["Region", "Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/map.png" }),
                            "Regions"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "List",
                                    caption: "Invoices",
                                    msgExtension: {
                                        ['BEGIN_DATE']: new Date(dt.getFullYear(), dt.getMonth(), 1).toUTCString(),
                                        ['END_DATE']: dt.toUTCString()
                                    },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/invoices.png" }),
                            "Invoices"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "QB_PRODUCT_MAP",
                                    action: "List",
                                    caption: "QB Product Maps",
                                    msgExtension: {},
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/cake2.png" }),
                            "Product Maps")),
                    React.createElement("br", null),
                    React.createElement(ListGroup, null,
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "CUSTOMER",
                                    action: "ImportQb",
                                    caption: "Import Customers",
                                    msgExtension: { ["IMPORT_TYPE"]: '' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/qb_customers.png" }),
                            "Import Customers"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "PRODUCT",
                                    action: "ImportQb",
                                    caption: "Import Products",
                                    msgExtension: { ["IMPORT_TYPE"]: '' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/qb_products.png" }),
                            "Import Products"),
                        React.createElement(ListGroup.Item, { action: true, onClick: () => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "ImportQb",
                                    caption: "Import Invoices",
                                    msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            } },
                            React.createElement(Image, { src: "/Maestro/img/qb_invoices.png" }),
                            "Import Invoices"))),
                React.createElement("div", { className: "dbrdColumn" },
                    React.createElement("table", { cellPadding: "6", cellSpacing: "6", style: { width: "100%", backgroundColor: "whitesmoke" } },
                        React.createElement("tr", null,
                            React.createElement("td", { style: { width: "75%" } },
                                React.createElement(CardGroup, null,
                                    React.createElement(Card, { bg: "success", text: "white" },
                                        React.createElement(Card.Title, null,
                                            React.createElement(Image, { src: "/Maestro/img/clients.png" }),
                                            "Top 5 Customers (Revenue YTD)"),
                                        React.createElement(Card.Body, null,
                                            React.createElement("div", { style: { textAlign: "left", marginLeft: "10px" } },
                                                "1." + c[0]["CUSTOMER_NAME"] + " - " + c[0]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "2." + c[1]["CUSTOMER_NAME"] + " - " + c[1]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "3." + c[2]["CUSTOMER_NAME"] + " - " + c[2]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "4." + c[3]["CUSTOMER_NAME"] + " - " + c[3]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "5." + c[4]["CUSTOMER_NAME"] + " - " + c[4]["TOTAL"] + " $"))),
                                    React.createElement(Card, { bg: "warning", text: "dark" },
                                        React.createElement(Card.Title, null,
                                            React.createElement(Image, { src: "/Maestro/img/cake.png" }),
                                            "Top 5 Products (Revenue YTD)"),
                                        React.createElement(Card.Body, null,
                                            React.createElement("div", { style: { textAlign: "left", marginLeft: "10px" } },
                                                "1." + p[0]["PRODUCT_NAME"] + " - " + p[0]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "2." + p[1]["PRODUCT_NAME"] + " - " + p[1]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "3." + p[2]["PRODUCT_NAME"] + " - " + p[2]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "4." + p[3]["PRODUCT_NAME"] + " - " + p[3]["TOTAL"] + " $",
                                                React.createElement("br", null),
                                                "5." + p[4]["PRODUCT_NAME"] + " - " + p[4]["TOTAL"] + " $"))))),
                            React.createElement("td", { style: { width: "25%" }, rowSpan: 3 },
                                this.renderNextDayOrders(),
                                " ")),
                        React.createElement("tr", null,
                            React.createElement("td", { align: "center" },
                                React.createElement("div", { className: "dbColDiv" },
                                    React.createElement(Chart, { width: "100%", height: 180, chartType: "Calendar", loader: React.createElement("div", null, "Loading Chart"), data: orderHistory, options: {
                                            title: 'YTD Daily Revenue',
                                            colorAxis: { colors: ['#ffffcc', '#33cc33'], maxValue: 10000 }
                                        } }))),
                            React.createElement("td", null)),
                        React.createElement("tr", null,
                            React.createElement("td", { align: "center" },
                                React.createElement("div", { className: "dbColDiv" },
                                    React.createElement(Chart, { width: "100%", height: '600px', chartType: "Line", loader: React.createElement("div", null, "Loading Chart"), data: prepareMonthlySummary, options: {
                                            chart: {
                                                title: 'Revenues in CAD'
                                            },
                                            colors: ["#ddf6b2", "#3cce39"],
                                        } }))),
                            React.createElement("td", null)))),
                React.createElement("div", null,
                    React.createElement(Alert, { id: "mmAlertId", dismissible: true, show: this.state == null ? false : this.state.ShowError, variant: "danger", "data-dismiss": "alert" },
                        React.createElement(Alert.Heading, { id: "mmAlertHeadingId" }, "Exception occured"),
                        React.createElement("div", { className: "errorStackTrace" },
                            React.createElement("p", { id: "mmAlertUserFriendlyId" }, this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage)),
                        React.createElement("hr", null),
                        React.createElement("div", { className: "errorStackTrace" },
                            React.createElement("p", { id: "mmAlertStackTraceId" }, this.state == null ? "" : this.state.ErrorInfo.StackTrace))),
                    React.createElement(Alert, { variant: "success", dismissible: true, show: this.state == null ? false : this.state.ShowSuccess, "data-dismiss": "alert" },
                        React.createElement("p", { id: "mmSuccess" }, this.state == null ? "" : this.state.SuccessMessage)),
                    React.createElement(Modal, { size: "sm", centered: true, show: this.state.ConfirmShow, "aria-labelledby": "example-modal-sizes-title-sm", dialogClassName: "modal-300p" },
                        React.createElement(Modal.Header, null,
                            React.createElement(Modal.Title, null)),
                        React.createElement(Modal.Body, null,
                            React.createElement(Row, null,
                                React.createElement(Col, null, this.state.ConfirmText)),
                            React.createElement(Row, { style: { marginTop: "20px" } },
                                React.createElement(Col, null,
                                    React.createElement(Button, { variant: "primary", id: "btnYes", onClick: this.onYes }, "Yes")),
                                React.createElement(Col, null,
                                    React.createElement(Button, { style: { float: "right" }, variant: "primary", id: "btnNo", onClick: this.onNo }, "No"))))),
                    React.createElement(ModalContainer, Object.assign({}, {
                        TranCode: (this.state == null ? "" : this.state.TranCode),
                        Action: (this.state == null ? "" : this.state.Action),
                        Entity: (this.state == null ? null : this.state.Entity),
                        Show: (this.state == null ? false : this.state.ShowModal),
                        Caption: (this.state == null ? "" : "New " + this.state.TranCode.toLowerCase()),
                        Close: this.handleModalClose
                    })))));
        }
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=Dashboard.js.map