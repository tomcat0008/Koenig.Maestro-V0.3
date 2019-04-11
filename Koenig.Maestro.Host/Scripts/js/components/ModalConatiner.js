var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import { Modal, Alert, Button, Row, Col } from 'react-bootstrap';
import ResponseMessage from '../classes/ResponseMessage';
import ErrorInfo from '../classes/ErrorInfo';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import OrderComponent from './transaction/OrderComponent';
import CustomerProductUnitsComponent from './transaction/CustomerProductUnitsComponent';
import UnitComponent from './transaction/UnitComponent';
import UnitTypeComponent from './transaction/UnitTypeComponent';
import RegionComponent from './transaction/RegionComponent';
import QbInvoiceLogComponent from './transaction/QbInvoiceLogComponent';
export default class ModalContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: "", selected: [],
            ShowSuccess: false, SuccessMessage: "", Action: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "", ConfirmText: "", ConfirmShow: false,
            MsgDataExtension: {}, ButtonAction: "", ResponseMessage: new ResponseMessage()
        };
        this.showException = (error) => {
            this.setState({ ErrorInfo: error, ShowError: true, Init: false });
        };
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                document.getElementById('btnSave').disabled = true;
                /*(document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;*/
                let response = yield this.tranComponent.Save();
                if (response.TransactionStatus == "ERROR")
                    throw response.ErrorInfo;
                document.getElementById('btnSave').disabled = false;
                this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: response.ResultMessage });
            }
            catch (error) {
                //console.error(error);
                if (error.DisableAction == false)
                    document.getElementById('btnSave').disabled = false;
                this.setState({ ErrorInfo: error, ShowError: true, Init: false });
            }
        });
        this.cancelFct = () => __awaiter(this, void 0, void 0, function* () {
            document.getElementById('btnCancel').disabled = true;
            try {
                let response = yield this.tranComponent.Cancel();
                if (response.TransactionStatus == "ERROR")
                    throw response.ErrorInfo;
                this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: response.ResultMessage });
            }
            catch (error) {
                if (error.DisableAction == false)
                    document.getElementById('btnCancel').disabled = false;
                this.setState({ ErrorInfo: error, ShowError: true, Init: false });
            }
        });
        this.createInvoice = () => __awaiter(this, void 0, void 0, function* () {
            try {
                document.getElementById('btnIntegrate').disabled = true;
                document.getElementById('btnCancel').disabled = true;
                document.getElementById('btnSave').disabled = true;
                /*(document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;*/
                let response = yield this.tranComponent.Integrate();
                let successMsg = response.ResultMessage;
                if (response.Warnings != null) {
                    if (response.Warnings.length > 0) {
                        successMsg = successMsg.concat("\r\n Warnings:");
                        response.Warnings.forEach(w => successMsg = successMsg.concat("\r\n" + w));
                    }
                }
                document.getElementById('btnCancel').disabled = false;
                this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: successMsg });
            }
            catch (error) {
                //console.error(error);
                if (error.DisableAction == false)
                    document.getElementById('btnIntegrate').disabled = false;
                document.getElementById('btnCancel').disabled = false;
                document.getElementById('btnSave').disabled = false;
                this.setState({ ErrorInfo: error, ShowError: true, Init: false });
            }
        });
        this.buttonSetFct = (actions) => {
            if (actions != undefined && actions != null) {
                document.getElementById("btnCancel").style.display = actions.indexOf("Cancel") > -1 ? "" : "none";
                document.getElementById("btnSave").style.display = actions.indexOf("Save") > -1 ? "" : "none";
                document.getElementById("btnIntegrate").style.display = actions.indexOf("Integrate") > -1 ? "" : "none";
            }
        };
        this.onYes = () => {
            this.setState({ ConfirmShow: false });
            switch (this.state.ButtonAction) {
                case "SAVE":
                    this.saveFct();
                    break;
                case "CANCEL":
                    this.cancelFct();
                    break;
                case "INTEGRATE":
                    this.createInvoice();
                    break;
            }
        };
        this.onNo = () => {
            this.setState({ ConfirmShow: false, ButtonAction: "", ConfirmText: "" });
        };
    }
    componentDidMount() {
        let contentComponent = this.createContent(this.props.TranCode, this.props.Entity);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: this.props.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: this.props.Action, Init: true, Entity: this.props.Entity,
            ShowModal: this.props.Show, ModalContent: contentComponent, ModalCaption: "",
            ConfirmText: "", ConfirmShow: false, ButtonAction: "", MsgDataExtension: {}, selected: [],
            ResponseMessage: new ResponseMessage()
        };
    }
    componentWillReceiveProps(nextProps) {
        try {
            let contentComponent = this.createContent(nextProps.TranCode, nextProps.Entity);
            this.setState({
                ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: nextProps.TranCode,
                ShowSuccess: false, SuccessMessage: "", Action: nextProps.Action, Init: true,
                ShowModal: nextProps.Show, ModalContent: contentComponent, ModalCaption: nextProps.Caption, MsgDataExtension: {},
                ResponseMessage: new ResponseMessage()
            });
        }
        catch (error) {
            this.setState({ ShowError: true, ErrorInfo: error });
        }
    }
    createContent(tranCode, item) {
        let componentProp = {
            Entity: item, ExceptionMethod: this.showException,
            ButtonSetMethod: this.buttonSetFct
        };
        if (tranCode == "CUSTOMER")
            return React.createElement(MaestroCustomerComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "ORDER")
            return React.createElement(OrderComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "CUSTOMER_PRODUCT_UNIT")
            return React.createElement(CustomerProductUnitsComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "UNIT")
            return React.createElement(UnitComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "UNIT_TYPE")
            return React.createElement(UnitTypeComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "REGION")
            return React.createElement(RegionComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "QUICKBOOKS_INVOICE")
            return React.createElement(QbInvoiceLogComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else
            return React.createElement("div", null);
    }
    toogleModal(text, show, buttonAction) {
        this.setState({ ConfirmShow: show, ConfirmText: text, ButtonAction: buttonAction });
    }
    render() {
        return (React.createElement("div", null,
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
            React.createElement(Modal, { show: this.props.Show, "aria-labelledby": "modalWidth", dialogClassName: "modal-90w", onHide: this.props.Close },
                React.createElement(Modal.Header, { closeButton: true },
                    React.createElement(Modal.Title, null, this.state == null ? "" : this.state.ModalCaption)),
                React.createElement(Modal.Body, null,
                    React.createElement(Alert, { id: "modalAlertId", show: this.state.ShowError, variant: "danger" },
                        React.createElement(Alert.Heading, { id: "modalAlertHeadingId" },
                            "Exception occured",
                            React.createElement("div", { style: {
                                    float: "right", fontFamily: "Segoe UI", cursor: "pointer", fontSize: "16px"
                                }, onClick: () => { this.setState({ ShowError: false }); } }, "X")),
                        React.createElement("div", { className: "errorHeader" },
                            React.createElement("p", { id: "modalAlertUserFriendlyId" }, this.state == null ? "" : (this.state.Init ? "" : this.state.ErrorInfo.UserFriendlyMessage))),
                        React.createElement("hr", null),
                        React.createElement("div", { className: "errorStackTrace" },
                            React.createElement("p", { id: "modalAlertStackTraceId" }, this.state == null ? "" : (this.state.Init ? "" : this.state.ErrorInfo.StackTrace)))),
                    React.createElement(Alert, { variant: "success", dismissible: true, show: this.state == null ? false : this.state.ShowSuccess, "data-dismiss": "alert" },
                        React.createElement("p", { id: "" }, this.state == null ? "" : this.state.SuccessMessage)),
                    React.createElement("div", { id: "modalRender" }, this.state == null ? "" : this.state.ModalContent)),
                React.createElement(Modal.Footer, null,
                    React.createElement(Button, { variant: "secondary", onClick: this.props.Close }, "Close"),
                    React.createElement(Button, { variant: "primary", id: "btnCancel", onClick: () => this.toogleModal("Do you want to cancel the order ?", true, "CANCEL") }, "Cancel Order"),
                    React.createElement(Button, { variant: "primary", id: "btnSave", onClick: this.saveFct }, "Save changes"),
                    React.createElement(Button, { variant: "primary", id: "btnIntegrate", onClick: () => this.toogleModal("Do you want to create the invoice ?", true, "INTEGRATE") }, "Create Qb Invoice")))));
    }
}
//# sourceMappingURL=ModalConatiner.js.map