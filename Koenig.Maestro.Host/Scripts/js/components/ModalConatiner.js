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
export default class ModalContainer extends React.Component {
    constructor(props) {
        super(props);
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield this.tranComponent.Save();
                this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
                document.getElementById('btnSave').disabled = true;
            }
            catch (error) {
                this.setState({ ErrorInfo: error, ShowError: true });
            }
        });
        let contentComponent = this.createContent(props.TranCode, props.Entity);
        this.setState({
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: props.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: props.Action, Init: true, Entity: props.Entity,
            ShowModal: props.Show, ModalContent: contentComponent, ModalCaption: "",
            ResponseMessage: new ResponseMessage()
        });
    }
    componentWillReceiveProps(nextProps) {
        let contentComponent = this.createContent(nextProps.TranCode, nextProps.Entity);
        this.setState({
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: nextProps.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: nextProps.Action, Init: true, Entity: nextProps.Entity,
            ShowModal: nextProps.Show, ModalContent: contentComponent, ModalCaption: nextProps.Caption,
            ResponseMessage: new ResponseMessage()
        });
    }
    createContent(tranCode, item) {
        if (tranCode == "CUSTOMER")
            return React.createElement(MaestroCustomerComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        else if (tranCode == "ORDER")
            return React.createElement(OrderComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        else
            return React.createElement("div", null);
    }
    render() {
        return (React.createElement(Modal, { show: this.props.Show, dialogClassName: "modal-90w", onHide: this.props.Close, centered: true, size: "lg" },
            React.createElement(Modal.Header, { closeButton: true },
                React.createElement(Modal.Title, null, this.state == null ? "" : this.state.ModalCaption)),
            React.createElement(Modal.Body, null,
                React.createElement(Alert, { id: "modalAlertId", dismissible: true, show: this.state == null ? false : this.state.ShowError, variant: "danger", "data-dismiss": "alert" },
                    React.createElement(Alert.Heading, { id: "modalAlertHeadingId" }, "Exception occured"),
                    React.createElement("p", { id: "modalAlertUserFriendlyId" }, this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage),
                    React.createElement("hr", null),
                    React.createElement("p", { id: "modalAlertStackTraceId" }, this.state == null ? "" : this.state.ErrorInfo.StackTrace)),
                React.createElement(Alert, { variant: "success", dismissible: true, show: this.state == null ? false : this.state.ShowSuccess, "data-dismiss": "alert" },
                    React.createElement("p", { id: "" }, this.state == null ? "" : this.state.SuccessMessage)),
                React.createElement("div", { id: "modalRender" }, this.state == null ? "" : this.state.ModalContent)),
            React.createElement(Modal.Footer, null,
                React.createElement(Button, { variant: "secondary", onClick: this.props.Close }, "Close"),
                React.createElement(Button, { variant: "primary", id: "btnSave", onClick: () => this.saveFct() }, "Save changes"))));
    }
}
//# sourceMappingURL=ModalConatiner.js.map