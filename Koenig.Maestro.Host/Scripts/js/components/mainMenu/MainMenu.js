var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MenuItem from './MenuItem';
import GridDisplay from '../GridDisplay';
import TranRequest from '../../classes/ListRequest';
import TransactionDisplay from '../TransactionDisplay';
import ErrorInfo from '../../classes/ErrorInfo';
import ResponseMessage from '../../classes/ResponseMessage';
import EntityAgent from '../../classes/EntityAgent';
import ModalContainer from '../ModalConatiner';
import { Alert, Modal, Row, Col, Button } from 'react-bootstrap';
export default class MainMenu extends React.Component {
    constructor() {
        super(null);
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
        this.handleModalClose = () => {
            this.setState({ ShowModal: false });
            //$('#mainMenu').show();
            //$('#wait').hide();
            $("body").removeClass("loading");
        };
        this.handleClick = (id) => __awaiter(this, void 0, void 0, function* () {
            //this.setState({ loading: true })
            let req = new TranRequest();
            req.ListTitle = id.props.caption;
            req.Action = id.props.action;
            req.TranCode = id.props.tranCode;
            req.MsgExtension = id.props.msgExtension;
            req.ButtonList = id.props.buttonList;
            req.ListSelect = id.props.listSelect;
            if (id.props.action == "New") {
                let tranCode = id.props.tranCode;
                let entity = EntityAgent.FactoryCreate(tranCode);
                yield this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
            }
            if (id.props.action == "List") {
                $('#mainMenu').hide();
                $("body").addClass("loading");
                ReactDOM.render(React.createElement(GridDisplay, Object.assign({}, req)), document.getElementById('content'));
                return;
            }
            if (id.props.action == "ImportQb") {
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
                yield this.setState({ TranCode: id.props.tranCode, Action: id.props.action, ConfirmShow: true });
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
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), Action: "", TranCode: "",
            ShowSuccess: false, SuccessMessage: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "", selected: [],
            ResponseMessage: new ResponseMessage(), ConfirmText: "", ConfirmShow: false, ButtonAction: "",
            MsgDataExtension: {}
        };
        this.saveFct = this.saveFct.bind(this);
    }
    render() {
        let dt = new Date();
        return (React.createElement("div", { className: "container", style: { width: "800px", paddingTop: "50px" } },
            React.createElement("div", { className: "row" },
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Orders",
                        React.createElement(MenuItem, { imgName: "order_new.png", action: "New", tranCode: "ORDER", eventHandler: this.handleClick, caption: "New order", itemType: "button", msgExtension: {}, buttonList: [], listSelect: false, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "icon-order.png", action: "List", tranCode: "ORDER", eventHandler: this.handleClick, caption: "Orders", itemType: "button", msgExtension: { ['PERIOD']: 'Month' }, height: "70px", width: "100%", buttonList: ["New", "Return", "Today", "Week", "Month", "Year", "All"], listSelect: false })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Imports",
                        React.createElement(MenuItem, { imgName: "qb_customers.png", action: "ImportQb", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Import Customers", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, buttonList: ["Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "qb_products.png", action: "ImportQb", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Import Products", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, buttonList: ["Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "qb_invoices.png", action: "ImportQb", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Import Invoices", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, buttonList: ["Return"], height: "70px", width: "100%", listSelect: false })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Integration",
                        React.createElement(MenuItem, { imgName: "invoice_export.png", action: "List", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Export Orders to Quickbooks", itemType: "button", msgExtension: { ["NOT_INTEGRATED"]: 'True' }, buttonList: ["Return", "Today", "Week", "Month", "Year", "All", "QB"], height: "70px", width: "100%", listSelect: true }))),
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Definitions",
                        React.createElement(MenuItem, { imgName: "cake.png", action: "List", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Products", itemType: "button", msgExtension: {}, buttonList: ["Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "clients.png", action: "List", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Customers", itemType: "button", msgExtension: {}, buttonList: ["Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "measure.png", action: "List", tranCode: "UNIT", eventHandler: this.handleClick, caption: "Units", itemType: "button", msgExtension: {}, buttonList: ["New", "Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "units.png", action: "List", tranCode: "UNIT_TYPE", eventHandler: this.handleClick, caption: "Unit Types", itemType: "button", msgExtension: {}, buttonList: ["New", "Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "map.png", action: "List", tranCode: "REGION", eventHandler: this.handleClick, caption: "Regions", itemType: "button", msgExtension: {}, buttonList: ["New", "Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "CUSTOMER_PRODUCT_UNIT", eventHandler: this.handleClick, caption: "Customer Product Units", itemType: "button", msgExtension: {}, buttonList: ["New", "Return"], height: "70px", width: "100%", listSelect: false }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Invoices Logs", itemType: "button", buttonList: ["Return"], listSelect: false, msgExtension: {
                                ['BEGIN_DATE']: new Date(dt.getFullYear(), dt.getMonth(), 1).toUTCString(),
                                ['END_DATE']: dt.toUTCString()
                            }, height: "70px", width: "100%" })))),
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
            }))));
    }
}
//# sourceMappingURL=MainMenu.js.map