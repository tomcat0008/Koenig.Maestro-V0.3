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
import filterFactory, { textFilter, dateFilter, numberFilter } from 'react-bootstrap-table2-filter';
import ToolkitProvider, { CSVExport } from 'react-bootstrap-table2-toolkit';
export default class GridDisplay extends React.Component {
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
            yield this.loadGridData();
        });
        this.handleGridSelect = (row, isSelect) => __awaiter(this, void 0, void 0, function* () {
            let selection = this.state.selected;
            if (isSelect) {
                selection.push(row.Id);
            }
            else {
                selection = selection.filter(s => s != row.Id);
            }
            yield this.setState({ selected: selection });
            let btn = document.getElementById("cmdQb");
            btn.disabled = selection.length == 0;
            if (selection.length > 0) {
                btn.onclick = this.startIntegration;
            }
            return isSelect;
        });
        this.renderList = this.renderList.bind(this);
        this.onDoubleClick = this.onDoubleClick.bind(this);
        this.loadGridData = this.loadGridData.bind(this);
        this.startIntegration = this.startIntegration.bind(this);
        let errorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            ShowError: false, ErrorInfo: errorInfo, Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: props.TranCode, ConfirmText: "", ConfirmShow: false,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
            MsgDataExtension: props.MsgExtension, selected: []
        };
        //this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }
    loadGridData() {
        return __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield new AxiosAgent().getList(this.props.TranCode, this.state.MsgDataExtension);
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
            const dummyCol = {
                filter: textFilter()
            };
            //console.debug(dummyCol);
            $("body").removeClass("loading");
            //$('#wait').hide();
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
    handleDateSelect(period) {
        return __awaiter(this, void 0, void 0, function* () {
            let dataExt = { ['PERIOD']: period };
            yield this.setState({ MsgDataExtension: dataExt });
            yield this.loadGridData();
        });
    }
    startIntegration() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            try {
                $("body").addClass("loading");
                let result = yield ea.CreateInvoices(this.state.selected);
                if (result.TransactionStatus == "ERROR") {
                    throw result.ErrorInfo;
                }
                $("body").removeClass("loading");
                this.setState({ ShowSuccess: true, SuccessMessage: result.ResultMessage });
                yield this.loadGridData();
            }
            catch (error) {
                $("body").removeClass("loading");
                this.setState({ ErrorInfo: error, ShowError: true });
            }
        });
    }
    renderList() {
        const selectRow = {
            mode: 'checkbox',
            clickToSelect: true,
            selected: this.state.selected,
            onSelect: this.handleGridSelect,
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        };
        const hideSelect = {
            mode: 'checkbox',
            clickToSelect: false,
            hideSelectColumn: true
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
        const pageOpts = {
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
        let displayMembers = this.state.ResponseMessage.GridDisplayMembers;
        if (displayMembers != null && displayMembers != undefined) {
            for (let col of displayMembers) {
                if (col["columnWidth"] != null && col["columnWidth"] != undefined && col["columnWidth"] != "") {
                    col["headerStyle"] = (column, colIndex) => {
                        return { width: col["columnWidth"] };
                    };
                }
                if (col["filterType"] == "DateTime")
                    col["filter"] = dateFilter();
                else if (col["filterType"] == "String")
                    col["filter"] = textFilter();
                else if (col["filterType"] == "Int" || col["filterType"] == "Long")
                    col["filter"] = numberFilter();
            }
        }
        let data = this.state.ResponseMessage.TransactionResult == null ? [] : (this.state.ResponseMessage.TransactionResult.length == 0 ? [] : this.state.ResponseMessage.TransactionResult);
        const { ExportCSVButton } = CSVExport;
        const MyExportCSV = (props) => {
            const handleClick = () => {
                props.onExport();
            };
            return (React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px" }, onClick: handleClick }, "Export CSV"));
        };
        const basePropsObj = {
            keyField: 'Id',
            data: { data },
            columns: { displayMembers },
            filter: filterFactory()
        };
        let nr = Date.now();
        return (React.createElement("div", null,
            React.createElement(ToolkitProvider, { keyField: "Id", data: data, columns: displayMembers, baseProps: basePropsObj, exportAll: true, exportCSV: {
                    fileName: this.state.TranCode + "_" + nr + ".csv"
                } }, props => (React.createElement("div", null,
                React.createElement("div", { style: { textAlign: "left" } },
                    React.createElement(Button, { key: "add", variant: "outline-secondary", style: { width: "120px", display: this.props.ButtonList.indexOf("Return") > -1 ? "" : "none" }, href: "/MainPage/Index" }, "Return"),
                    React.createElement("span", null, "  "),
                    React.createElement(Button, { key: "add", variant: "primary", style: { width: "120px", display: this.props.ButtonList.indexOf("New") > -1 ? "" : "none" }, onClick: this.handleNew }, "New"),
                    React.createElement("span", null, "  "),
                    React.createElement(Button, { key: "add", variant: "outline-primary", style: { width: "120px", display: this.props.ButtonList.indexOf("Today") > -1 ? "" : "none" }, onClick: () => { this.handleDateSelect("Today"); } }, "Today"),
                    React.createElement(Button, { key: "add", variant: "outline-primary", style: { width: "120px", display: this.props.ButtonList.indexOf("Week") > -1 ? "" : "none" }, onClick: () => { this.handleDateSelect("Week"); } }, "Week"),
                    React.createElement(Button, { key: "add", variant: "outline-primary", style: { width: "120px", display: this.props.ButtonList.indexOf("Month") > -1 ? "" : "none" }, onClick: () => { this.handleDateSelect("Month"); } }, "Month"),
                    React.createElement(Button, { key: "add", variant: "outline-primary", style: { width: "120px", display: this.props.ButtonList.indexOf("Year") > -1 ? "" : "none" }, onClick: () => { this.handleDateSelect("Year"); } }, "Year"),
                    React.createElement(Button, { key: "add", variant: "outline-primary", style: { width: "120px", display: this.props.ButtonList.indexOf("Year") > -1 ? "" : "none" }, onClick: () => { this.handleDateSelect("All"); } }, "All"),
                    React.createElement("span", null, "  "),
                    React.createElement(Button, { key: "add", id: "cmdQb", disabled: true, variant: "primary", style: { width: "120px", display: this.props.ButtonList.indexOf("QB") > -1 ? "" : "none" }, onClick: () => { this.startIntegration(); } }, "Send to QB"),
                    React.createElement("span", null, "  "),
                    React.createElement(MyExportCSV, Object.assign({}, props.csvProps), "Export CSV!!")),
                React.createElement(BootstrapTable, Object.assign({}, props.baseProps, { pagination: paginationFactory(pageOpts), condensed: true, hover: true, bootstrap4: true, filter: filterFactory(), rowEvents: { onDoubleClick: this.onDoubleClick }, headerClasses: "grid-header-style", selectRow: this.props.ListSelect ? selectRow : hideSelect, noDataIndication: "No data found" })))))));
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
                        React.createElement("div", null, this.state.ResponseMessage == null ? "" :
                            (this.state.ResponseMessage.TransactionStatus == "ERROR" ? "" : this.renderList())))),
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