import * as React from 'react';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import { Button, Alert } from 'react-bootstrap';
import AxiosAgent from '../classes/AxiosAgent';
import { ITranRequest } from '../classes/ITranRequest';
import { ICrudComponent } from './ICrudComponent';
import EntityAgent from '../classes/EntityAgent';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { IModalContainerState } from './IModalContainerState';
import ModalContainer from './ModalConatiner';
import BootstrapTable from 'react-bootstrap-table-next';
import paginationFactory from 'react-bootstrap-table2-paginator';

export default class GridDisplay extends React.Component<ITranRequest, IModalContainerState> {

    tranComponent: ICrudComponent;

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
            ResponseMessage: new ResponseMessage(),TranCode:props.TranCode,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: ""
        };
        this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
            this.loadGridData();
            this.handleModalClose();
        }
        catch(error)
        {
            this.setState({ ErrorInfo: error, ShowError: true });
        }

    }

    handleModalClose = () => {
        this.setState({ ShowModal: false });
    }

    async loadGridData() {
        try {
            let response: IResponseMessage = await new AxiosAgent().getList(this.props.TranCode, this.props.MsgExtension);
            this.setState({ ResponseMessage: response, Init: false });
            console.log(response);

        }
        catch (err) {
            this.setState({ ErrorInfo: err, ShowError: true });
        }

        $('#wait').hide();
    }
    
    async componentDidMount() {
        this.loadGridData();
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
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        }

        const customTotal = (from, to, size) => (
            <span className="react-bootstrap-table-pagination-total">
                {" " }Showing {from} to {to} of {size} Results
            </span>
        );

        const options = {
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
            }, {
                text: '30', value: 30
            }, {
                    text: 'All', value: this.state.ResponseMessage.TransactionResult.length
            }]
        };

        return (
            <div>
                <div style={{ textAlign:"left"}}>
                    <Button key="add" variant="outline-secondary" style={{ width: "120px" }} href="/MainPage/Index" >Return</Button>
                    <Button key="add" variant="outline-secondary" style={{ width: "120px" }} onClick={this.handleNew} >New</Button>
                </div>
            <BootstrapTable keyField='Id' bootstrap4="true"
                condensed hover 
                rowEvents={{ onDoubleClick: this.onDoubleClick }}
                headerClasses="grid-header-style"
                selectRow={selectRow} 
                data={this.state.ResponseMessage.TransactionResult}
                columns={this.state.ResponseMessage.GridDisplayMembers}
                pagination={paginationFactory(options)}
                />
            </div>
        );
    }

    render() {
        if (!this.state.Init) {
            return (
                <div>
                    <Alert id="gridAlertId" dismissible show={this.state.ShowSuccess} variant="success" data-dismiss="alert" >
                        <p id="gridAlertMessage">
                            {this.state.SuccessMessage}
                        </p>
                    </Alert>
                    <div>{this.renderList()}</div>
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