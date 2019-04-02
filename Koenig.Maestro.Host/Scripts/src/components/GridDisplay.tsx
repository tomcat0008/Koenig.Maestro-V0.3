import * as React from 'react';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import { Button, Alert, Row, Col } from 'react-bootstrap';
import AxiosAgent from '../classes/AxiosAgent';
import { ITranRequest } from '../classes/ITranRequest';
import { ICrudComponent } from './ICrudComponent';
import EntityAgent from '../classes/EntityAgent';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { IModalContainerState } from './IModalContainerState';
import ModalContainer from './ModalConatiner';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';
import filterFactory, { textFilter, dateFilter, numberFilter } from 'react-bootstrap-table2-filter';
import ToolkitProvider, { CSVExport } from 'react-bootstrap-table2-toolkit';



export default class GridDisplay extends React.Component<ITranRequest, IModalContainerState> {

    tranComponent: ICrudComponent;
    state = {
    ShowError: false, ErrorInfo: new ErrorInfo(), Entity: null,
    ShowSuccess: false, SuccessMessage: "",
        ResponseMessage: new ResponseMessage(), TranCode: "", ConfirmText:"", ConfirmShow:false,
        Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
        MsgDataExtension: {}
    
    };

    constructor(props: ITranRequest) {
        super(props);
        this.renderList = this.renderList.bind(this);
        this.onDoubleClick = this.onDoubleClick.bind(this);
        this.loadGridData = this.loadGridData.bind(this);
        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            ShowError: false, ErrorInfo: errorInfo, Entity:null,
            ShowSuccess:false, SuccessMessage:"",
            ResponseMessage: new ResponseMessage(), TranCode: props.TranCode, ConfirmText:"", ConfirmShow:false,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
            MsgDataExtension:props.MsgExtension
        };
        //this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }

    handleModalClose = async () => {
        this.setState({ ShowModal: false });
        await this.loadGridData();
    }



    async loadGridData() {
        try {
            let response: IResponseMessage = await new AxiosAgent().getList(this.props.TranCode, this.state.MsgDataExtension);
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
        }
        //console.debug(dummyCol);
        $("body").removeClass("loading");
        //$('#wait').hide();
    }
    
    async componentDidMount() {
        await this.loadGridData();
    }


    handleNew() {
        this.setState({ ModalContent: null, ShowModal: true, ModalCaption: "New " + this.props.TranCode.toLowerCase(), Entity: EntityAgent.FactoryCreate(this.props.TranCode), Action: "New" });
    }

    onDoubleClick(e, itemObject) {
        this.setState({ ModalContent: null, ShowModal: true, ModalCaption: "Editing " + this.props.TranCode.toLowerCase() + " " + itemObject.Id, Entity: itemObject, Action: "Update" });
    }

    async handleDateSelect(period: string) {
        let dataExt: { [key: string]: string } = { ['PERIOD']: period };
        await this.setState({ MsgDataExtension: dataExt });
        await this.loadGridData();
    }

    renderList() {


        const selectRow = {
            mode: 'checkbox',
            clickToSelect: true,
            onSelect: (row, isSelect, rowIndex, e) => {  },
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        }


        const hideSelect = {
            mode: 'checkbox',
            clickToSelect: false,
            hideSelectColumn:true
        }

        

        const customTotal = (from, to, size) => (
            <span className="react-bootstrap-table-pagination-total">
                {" " }Showing {from} to {to} of {size} Results
            </span>
        );

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
                text: 'All', value: (this.state.ResponseMessage.TransactionResult==null ? 0 : this.state.ResponseMessage.TransactionResult.length)
            }
            ]
        };



        let displayMembers = this.state.ResponseMessage.GridDisplayMembers;
        if (displayMembers != null && displayMembers != undefined) {
            for (let col of displayMembers) {
                
                if (col["columnWidth"] != null && col["columnWidth"] != undefined && col["columnWidth"] != "") {
                    col["headerStyle"] = (column, colIndex) => {
                        return { width: col["columnWidth"] }
                    }
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
            return (
                    <Button key="add" variant="outline-secondary" style={{ width: "120px" }} onClick={handleClick} >Export CSV</Button>
            );
        };

        const basePropsObj = {
            keyField: 'Id',
            data: { data },
            columns: { displayMembers },
            filter: filterFactory()
        }
        let nr: number = Date.now();
        return (
            <div>
                
                <ToolkitProvider
                    keyField="Id"
                    data={data}
                    columns={displayMembers}
                    baseProps={basePropsObj}
                    exportAll
                    exportCSV={{
                        fileName: this.state.TranCode+"_"+nr+".csv"
                    }}

                >
                    {
                        props => (
                            <div>
                                <div style={{ textAlign: "left" }}>
                                    <Button key="add" variant="outline-secondary" style={{ width: "120px", display: this.props.ButtonList.indexOf("Return")>-1 ? "" : "none"  }} href="/MainPage/Index" >Return</Button>
                                    <Button key="add" variant="outline-secondary" style={{ width: "120px", display: this.props.ButtonList.indexOf("New") > -1 ? "" : "none" }} onClick={this.handleNew} >New</Button>
                                    <Button key="add" variant="outline-primary" style={{ width: "120px", display: this.props.ButtonList.indexOf("Week") > -1 ? "" : "none" }} onClick={() => { this.handleDateSelect("Today"); }} >Today</Button>
                                    <Button key="add" variant="outline-primary" style={{ width: "120px", display: this.props.ButtonList.indexOf("Week") > -1 ? "" : "none" }} onClick={() => { this.handleDateSelect("Week"); }} >Week</Button>
                                    <Button key="add" variant="outline-primary" style={{ width: "120px", display: this.props.ButtonList.indexOf("Month") > -1 ? "" : "none" }} onClick={() => { this.handleDateSelect("Month"); }} >Month</Button>
                                    <Button key="add" variant="outline-primary" style={{ width: "120px", display: this.props.ButtonList.indexOf("Year") > -1 ? "" : "none" }} onClick={() => { this.handleDateSelect("Year"); }} >Year</Button>
                                    <MyExportCSV {...props.csvProps}>Export CSV!!</MyExportCSV>
                                </div>
                                <BootstrapTable {...props.baseProps}
                                    pagination={paginationFactory(pageOpts)}
                                    condensed
                                    hover
                                    bootstrap4
                                    filter={filterFactory()}
                                    rowEvents={{ onDoubleClick: this.onDoubleClick }}
                                    headerClasses="grid-header-style"
                                    selectRow={this.props.ListSelect ? selectRow : hideSelect}  
                                    noDataIndication="No data found"
                                />
                            </div>
                        )

                    }
                </ToolkitProvider>

                
            </div>
        );
    }

    render() {
        if (!this.state.Init) {
            return (
                    <div>
                    <Row><Col >
                    <Alert id="gridErrorAlertId" dismissible show={this.state == null ? false : this.state.ShowError} variant="danger" data-dismiss="alert" >
                        <Alert.Heading id="gridAlertHeadingId">Exception occured</Alert.Heading>
                        <p id="gridAlertUserFriendlyId">
                            {this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage}
                        </p>
                        <hr />
                        <div className="errorStackTrace">
                            <p id="gridAlertStackTraceId" >
                            {this.state == null ? "" : this.state.ErrorInfo.StackTrace}
                        </p>
                        </div>
                        </Alert>
                    </Col></Row>
                    <Row><Col>
                    <Alert id="gridSuccess" dismissible show={this.state.ShowSuccess} variant="success" data-dismiss="alert" >
                        <p id="gridSuccessMessage">
                            {this.state.SuccessMessage}
                        </p>
                        </Alert>
                    </Col></Row>
                    <Row><Col>
                        <div >{
                            this.state.ResponseMessage == null ? "" :
                            (this.state.ResponseMessage.TransactionStatus == "ERROR" ? "" : this.renderList())
                        }</div>
                    </Col></Row>
                    <ModalContainer { 
                        ...{
                            TranCode: this.props.TranCode,
                            Action: this.state.Action,
                            Entity: this.state.Entity,
                            Show: this.state.ShowModal,
                            Close: this.handleModalClose,
                            Caption:this.state.ModalCaption
                            
                        }} />

                </div>

            );
        }
        else {
            return (<div></div>);

        }



    }
}