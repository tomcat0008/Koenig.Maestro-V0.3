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
import NewItemDisplay from '../NewItemDisplay';
import { Modal, Button, Alert } from 'react-bootstrap';
import ErrorInfo from '../../classes/ErrorInfo';
import ResponseMessage from '../../classes/ResponseMessage';
import EntityAgent from '../../classes/EntityAgent';
import MaestroCustomerComponent from '../transaction/MaestroCustomerComponent';
import OrderComponent from '../transaction/OrderComponent';
export default class MainMenu extends React.Component {
    constructor() {
        super(null);
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield this.tranComponent.Save();
                this.setState({ showSuccess: true, successMessage: response.ResultMessage });
                this.handleClose();
            }
            catch (error) {
                this.setState({ errorInfo: error, showError: true });
            }
        });
        this.mainmenuRender = (React.createElement("div", { className: "container", style: { width: "800px", paddingTop: "50px" } },
            React.createElement("div", { className: "row" },
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Orders",
                        React.createElement(MenuItem, { imgName: "order_new.png", action: "New", tranCode: "ORDER", eventHandler: this.handleClick, caption: "New order", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "icon-order.png", action: "List", tranCode: "ORDER", eventHandler: this.handleClick, caption: "Orders", itemType: "button", msgExtension: { ['STATUS']: 'CR' }, height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Imports",
                        React.createElement(MenuItem, { imgName: "qb_customers.png", action: "ImportQb", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Import Customers", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_products.png", action: "ImportQb", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Import Products", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_invoices.png", action: "ImportQb", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Import Invoices", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Integration",
                        React.createElement(MenuItem, { imgName: "invoice_export.png", action: "Export", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Export Orders to Quickbooks", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }))),
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Definitions",
                        React.createElement(MenuItem, { imgName: "cake.png", action: "List", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Products", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "clients.png", action: "List", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Customers", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "measure.png", action: "List", tranCode: "UNIT", eventHandler: this.handleClick, caption: "Units", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "units.png", action: "List", tranCode: "UNIT_TYPE", eventHandler: this.handleClick, caption: "Unit Types", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "map.png", action: "List", tranCode: "REGION", eventHandler: this.handleClick, caption: "Regions", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "CUSTOMER_PRODUCT_UNIT", eventHandler: this.handleClick, caption: "Customer Product Units", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Invoices Logs", itemType: "button", msgExtension: {}, height: "70px", width: "100%" })))),
            React.createElement(Modal, { show: this.state.showModal, dialogClassName: "modal-90w", onHide: this.handleClose, centered: true, size: "lg" },
                React.createElement(Modal.Header, { closeButton: true },
                    React.createElement(Modal.Title, null, this.state.modalCaption)),
                React.createElement(Modal.Body, null,
                    React.createElement(Alert, { id: "modalAlertId", dismissible: true, show: this.state.showError, variant: "danger", "data-dismiss": "alert" },
                        React.createElement(Alert.Heading, { id: "modalAlertHeadingId" }, "Exception occured"),
                        React.createElement("p", { id: "modalAlertUserFriendlyId" }, this.state.errorInfo.UserFriendlyMessage),
                        React.createElement("hr", null),
                        React.createElement("p", { id: "modalAlertStackTraceId" }, this.state.errorInfo.StackTrace)),
                    React.createElement("div", { id: "modalRender" }, this.state.modalContent)),
                React.createElement(Modal.Footer, null,
                    React.createElement(Button, { variant: "secondary", onClick: () => this.setState({ showModal: false }) }, "Close"),
                    React.createElement(Button, { variant: "primary", onClick: () => this.saveFct() }, "Save changes")))));
        this.handleClose = this.handleClose.bind(this);
        let errorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            showError: false, errorInfo: errorInfo,
            showSuccess: false, successMessage: "",
            showModal: false, modalContent: null, modalCaption: "",
            responseMessage: new ResponseMessage()
        };
        this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
        //this.handleClick = this.handleClick.bind(this);
    }
    handleNew(tranCode) {
        let entity = EntityAgent.FactoryCreate(tranCode);
        this.displayModal("New " + tranCode.toLowerCase(), entity, "New");
    }
    displayModal(caption, item, itemAction) {
        let data;
        if (this.props.tranCode == "CUSTOMER")
            data = React.createElement(MaestroCustomerComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        else if (this.props.tranCode == "ORDER")
            data = React.createElement(OrderComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        this.setState({ modalContent: data, showModal: true, modalCaption: caption });
    }
    handleClose() {
        this.setState({ showModal: false });
    }
    handleClick(id) {
        //this.setState({ loading: true })
        let req = new TranRequest();
        req.listTitle = id.props.caption;
        req.action = id.props.action;
        req.tranCode = id.props.tranCode;
        req.msgExtension = id.props.msgExtension;
        $('#mainMenu').hide();
        if (id.props.action == "New") {
            ReactDOM.render(React.createElement(NewItemDisplay, Object.assign({}, req)), document.getElementById('content'));
        }
        if (id.props.action == "List") {
            $('#wait').show();
            ReactDOM.render(React.createElement(GridDisplay, Object.assign({}, req)), document.getElementById('content'));
            return;
        }
        if (id.props.action == "ImportQb") {
            $('#wait').show();
            ReactDOM.render(React.createElement(TransactionDisplay, Object.assign({}, req)), document.getElementById('content'));
            return;
        }
        //this.setState({ loading: false })
    }
    render() {
        return this.mainmenuRender;
    }
}
//# sourceMappingURL=MainMenu.js.map