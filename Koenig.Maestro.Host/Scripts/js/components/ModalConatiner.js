var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import { Modal, Alert, Button } from 'react-bootstrap';
import ResponseMessage from '../classes/ResponseMessage';
import ErrorInfo from '../classes/ErrorInfo';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import OrderComponent from './transaction/OrderComponent';
import CustomerProductUnitsComponent from './transaction/CustomerProductUnitsComponent';
export default class ModalContainer extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: "",
            ShowSuccess: false, SuccessMessage: "", Action: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "",
            ResponseMessage: new ResponseMessage()
        };
        this.showException = (error) => {
            this.setState({ ErrorInfo: error, ShowError: true, Init: false });
        };
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                document.getElementById('btnSave').disabled = true;
                document.getElementById('btnCancel').disabled = true;
                let response = yield this.tranComponent.Save();
                this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: response.ResultMessage });
            }
            catch (error) {
                console.error(error);
                if (error.DisableAction == false)
                    document.getElementById('btnSave').disabled = false;
                this.setState({ ErrorInfo: error, ShowError: true, Init: false });
            }
        });
        this.cancelFct = () => __awaiter(this, void 0, void 0, function* () {
        });
    }
    componentDidMount() {
        let contentComponent = this.createContent(this.props.TranCode, this.props.Entity);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: this.props.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: this.props.Action, Init: true, Entity: this.props.Entity,
            ShowModal: this.props.Show, ModalContent: contentComponent, ModalCaption: "",
            ResponseMessage: new ResponseMessage()
        };
    }
    componentWillReceiveProps(nextProps) {
        try {
            let contentComponent = this.createContent(nextProps.TranCode, nextProps.Entity);
            this.setState({
                ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: nextProps.TranCode,
                ShowSuccess: false, SuccessMessage: "", Action: nextProps.Action, Init: true, Entity: nextProps.Entity,
                ShowModal: nextProps.Show, ModalContent: contentComponent, ModalCaption: nextProps.Caption,
                ResponseMessage: new ResponseMessage()
            });
        }
        catch (error) {
            this.setState({ ShowError: true, ErrorInfo: error });
        }
    }
    createContent(tranCode, item) {
        let componentProp = { Entity: item, ExceptionMethod: this.showException };
        if (tranCode == "CUSTOMER")
            return React.createElement(MaestroCustomerComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "ORDER")
            return React.createElement(OrderComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else if (tranCode == "CUSTOMER_PRODUCT_UNIT")
            return React.createElement(CustomerProductUnitsComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, componentProp));
        else
            return React.createElement("div", null);
    }
    render() {
        return (React.createElement(Modal, { show: this.props.Show, "aria-labelledby": "modalWidth", dialogClassName: "modal-90w", onHide: this.props.Close },
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
                React.createElement(Button, { style: { display: this.state.TranCode == "ORDER" ? "block" : "none" }, variant: "primary", id: "btnCancel", onClick: () => this.cancelFct() }, "Cancel Order"),
                React.createElement(Button, { variant: "primary", id: "btnSave", onClick: () => this.saveFct() }, "Save changes"))));
    }
}
//# sourceMappingURL=ModalConatiner.js.map