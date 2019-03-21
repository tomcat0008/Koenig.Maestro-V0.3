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
import { Button, Alert, Row, Col } from 'react-bootstrap';
import AxiosAgent from '../classes/AxiosAgent';
import EntityAgent from '../classes/EntityAgent';
import ErrorInfo from '../classes/ErrorInfo';
import ModalContainer from './ModalConatiner';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
export default class GridDisplay extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: "",
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: ""
        };
        /*
        saveFct = async () => {
            try {
                let response: IResponseMessage = await this.tranComponent.Save();
                this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
                await this.loadGridData();
                this.handleModalClose();
            }
            catch(error)
            {
                this.setState({ ErrorInfo: error, ShowError: true });
                console.debug(error);
            }
    
        }
        */
        this.handleModalClose = () => __awaiter(this, void 0, void 0, function* () {
            this.setState({ ShowModal: false });
            yield this.loadGridData();
        });
        this.renderList = this.renderList.bind(this);
        this.onDoubleClick = this.onDoubleClick.bind(this);
        this.loadGridData = this.loadGridData.bind(this);
        let errorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            ShowError: false, ErrorInfo: errorInfo, Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: props.TranCode,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: ""
        };
        //this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }
    loadGridData() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield new AxiosAgent().getList(this.props.TranCode, this.props.MsgExtension);
                this.setState({ ResponseMessage: response, Init: false });
                if (response.TransactionStatus == "ERROR")
                    throw (response.ErrorInfo);
            }
            catch (error) {
                this.setState({ ErrorInfo: error, ShowError: true });
                console.error(error);
            }
            $('#wait').hide();
        });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            yield this.loadGridData();
        });
    }
    handleNew() {
        this.setState({ ModalContent: null, ShowModal: true, ModalCaption: "New " + this.props.TranCode.toLowerCase(), Entity: EntityAgent.FactoryCreate(this.props.TranCode), Action: "New" });
    }
    onDoubleClick(e, itemObject) {
        this.setState({ ModalContent: null, ShowModal: true, ModalCaption: "Editing " + this.props.TranCode.toLowerCase() + " " + itemObject.Id, Entity: itemObject, Action: "Update" });
    }
    renderList() {
        const selectRow = {
            mode: 'checkbox',
            clickToSelect: true,
            onSelect: (row, isSelect, rowIndex, e) => { },
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        };
        const customTotal = (from, to, size) => (React.createElement("span", { className: "react-bootstrap-table-pagination-total" },
            " ",
            "Showing ",
            from,
            " to ",
            to,
            " of ",
            size,
            " Results"));
        const options = {
            noDataText: 'This is custom text for empty data',
            paginationSize: 4,
            pageStartIndex: 0,
            firstPageText: 'First',
            prePageText: 'Back',
            nextPageText: 'Next',
            lastPageText: 'Last',
            nextPageTitle: 'First page',
            prePageTitle: 'Pre page',
            firstPageTitle: 'Next page',
            lastPageTitle: 'Last page',
            showTotal: true,
            paginationTotalRenderer: customTotal,
            sizePerPageList: [{
                    text: '10', value: 10
                },
                {
                    text: '30', value: 30
                },
                {
                    text: 'All', value: (this.state.ResponseMessage.TransactionResult == null ? 0 : this.state.ResponseMessage.TransactionResult.length)
                }
            ]
        };
        return (React.createElement("div", null,
            React.createElement("div", { style: { textAlign: "left" } },
                React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px" }, href: "/MainPage/Index" }, "Return"),
                React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px" }, onClick: this.handleNew }, "New")),
            React.createElement(BootstrapTable, { keyField: 'Id', bootstrap4: "true", condensed: true, hover: true, rowEvents: { onDoubleClick: this.onDoubleClick }, headerClasses: "grid-header-style", selectRow: selectRow, data: this.state.ResponseMessage.TransactionResult == null ? [] : this.state.ResponseMessage.TransactionResult, columns: this.state.ResponseMessage.GridDisplayMembers == null ? [{ "text": "" }] : this.state.ResponseMessage.GridDisplayMembers, pagination: paginationFactory(options) })));
    }
    render() {
        if (!this.state.Init) {
            return (React.createElement("div", null,
                React.createElement(Row, null,
                    React.createElement(Col, null,
                        React.createElement(Alert, { id: "gridErrorAlertId", dismissible: true, show: this.state == null ? false : this.state.ShowError, variant: "danger", "data-dismiss": "alert" },
                            React.createElement(Alert.Heading, { id: "gridAlertHeadingId" }, "Exception occured"),
                            React.createElement("p", { id: "gridAlertUserFriendlyId" }, this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage),
                            React.createElement("hr", null),
                            React.createElement("div", { className: "errorStackTrace" },
                                React.createElement("p", { id: "gridAlertStackTraceId" }, this.state == null ? "" : this.state.ErrorInfo.StackTrace))))),
                React.createElement(Row, null,
                    React.createElement(Col, null,
                        React.createElement(Alert, { id: "gridSuccess", dismissible: true, show: this.state.ShowSuccess, variant: "success", "data-dismiss": "alert" },
                            React.createElement("p", { id: "gridSuccessMessage" }, this.state.SuccessMessage)))),
                React.createElement(Row, null,
                    React.createElement(Col, null,
                        React.createElement("div", null, this.renderList()))),
                React.createElement(ModalContainer, Object.assign({}, {
                    TranCode: this.props.TranCode,
                    Action: this.state.Action,
                    Entity: this.state.Entity,
                    Show: this.state.ShowModal,
                    Close: this.handleModalClose,
                    Caption: this.state.ModalCaption
                }))));
        }
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=GridDisplay.js.map