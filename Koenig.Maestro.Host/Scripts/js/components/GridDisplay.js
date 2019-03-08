var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import ResponseMessage from '../classes/ResponseMessage';
import { Modal, Button, Alert } from 'react-bootstrap';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import paginationFactory from 'react-bootstrap-table2-paginator';
import BootstrapTable from 'react-bootstrap-table-next';
import AxiosAgent from '../classes/AxiosAgent';
import OrderComponent from './transaction/OrderComponent';
import EntityAgent from '../classes/EntityAgent';
import ErrorInfo from '../classes/ErrorInfo';
export default class GridDisplay extends React.Component {
    constructor(props) {
        super(props);
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield this.tranComponent.Save();
                this.setState({ showSuccess: true, successMessage: response.ResultMessage });
                this.loadGridData();
                this.handleClose();
            }
            catch (error) {
                this.setState({ errorInfo: error, showError: true });
            }
        });
        this.renderList = this.renderList.bind(this);
        this.handleClose = this.handleClose.bind(this);
        this.onDoubleClick = this.onDoubleClick.bind(this);
        this.loadGridData = this.loadGridData.bind(this);
        let errorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            showError: false, errorInfo: errorInfo,
            showSuccess: false, successMessage: "",
            responseMessage: new ResponseMessage(),
            init: true, showModal: false, modalContent: null, modalCaption: "", action: ""
        };
        this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }
    loadGridData() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield new AxiosAgent().getList(this.props.tranCode, this.props.msgExtension);
                this.setState({ responseMessage: response, init: false });
                console.log(response);
            }
            catch (err) {
                this.setState({ errorInfo: err, showError: true });
            }
            $('#wait').hide();
        });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            this.loadGridData();
        });
    }
    handleClose() {
        this.setState({ showModal: false });
    }
    displayModal(caption, item, itemAction) {
        let data;
        if (this.props.tranCode == "CUSTOMER")
            data = React.createElement(MaestroCustomerComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        else if (this.props.tranCode == "ORDER")
            data = React.createElement(OrderComponent, Object.assign({ ref: (comp) => this.tranComponent = comp }, item));
        this.setState({ modalContent: data, showModal: true, modalCaption: caption, action: itemAction });
    }
    handleNew() {
        let entity = EntityAgent.FactoryCreate(this.props.tranCode);
        this.displayModal("New " + this.props.tranCode.toLowerCase(), entity, "New");
    }
    onDoubleClick(e, itemObject) {
        this.displayModal("Editing " + this.props.tranCode.toLowerCase(), itemObject, "Update");
    }
    renderList() {
        const actions = [
            React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px" }, href: "/MainPage/Index" }, "Return"),
            React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px" }, onClick: this.handleNew }, "New")
        ];
        const selectRow = {
            mode: 'checkbox',
            clickToSelect: true,
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        };
        const options = { onDoubleClick: this.onDoubleClick };
        return (React.createElement(BootstrapTable, { keyField: 'Id', bootstrap4: "true", rowEvents: options, headerClasses: "grid-header-style", selectRow: selectRow, caption: actions, pagination: paginationFactory(), data: this.state.responseMessage.TransactionResult, columns: this.state.responseMessage.GridDisplayMembers }));
    }
    render() {
        if (!this.state.init) {
            return (React.createElement("div", null,
                React.createElement(Alert, { id: "gridAlertId", dismissible: true, show: this.state.showSuccess, variant: "success", "data-dismiss": "alert" },
                    React.createElement("p", { id: "gridAlertMessage" }, this.state.successMessage)),
                React.createElement("div", null, this.renderList()),
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
        }
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=GridDisplay.js.map