import * as React from 'react';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import { Modal, Button, Image, Alert } from 'react-bootstrap';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import paginationFactory  from 'react-bootstrap-table2-paginator';
import BootstrapTable  from 'react-bootstrap-table-next';
import AxiosAgent from '../classes/AxiosAgent';
import { ITranRequest } from '../classes/ITranRequest';
import OrderComponent from './transaction/OrderComponent';
import MaestroCustomer, { IMaestroCustomer } from '../classes/dbEntities/IMaestroCustomer';
import OrderMaster, { IOrderMaster } from '../classes/dbEntities/IOrderMaster';
import { ICrudComponent } from './ICrudComponent';
import EntityAgent from '../classes/EntityAgent';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { IModalContainerState } from './IModalContainerState';

interface IGridState extends IModalContainerState {
    
    init: boolean,
    action: string

}

export default class GridDisplay extends React.Component<ITranRequest, IGridState> {

    tranComponent: ICrudComponent;

    constructor(props) {
        super(props);
        this.renderList = this.renderList.bind(this);
        this.handleClose = this.handleClose.bind(this);
        this.onDoubleClick = this.onDoubleClick.bind(this);
        this.loadGridData = this.loadGridData.bind(this);
        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            showError: false, errorInfo: errorInfo,
            showSuccess:false, successMessage:"",
            responseMessage: new ResponseMessage(),
            init: true, showModal: false, modalContent: null, modalCaption: "", action: ""
        };
        this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ showSuccess: true, successMessage: response.ResultMessage });
            this.loadGridData();
            this.handleClose();
        }
        catch(error)
        {
            this.setState({ errorInfo: error, showError: true });
        }

    }

    async loadGridData() {
        try {
            let response: IResponseMessage = await new AxiosAgent().getList(this.props.tranCode, this.props.msgExtension);
            this.setState({ responseMessage: response, init: false });
            console.log(response);

        }
        catch (err) {
            this.setState({ errorInfo: err, showError: true });
        }

        $('#wait').hide();
    }
    
    async componentDidMount() {
        this.loadGridData();
    }

    handleClose() {
        this.setState({ showModal: false });
    }

    private displayModal(caption:string, item:DbEntityBase, itemAction:string) {


        let data;
        if (this.props.tranCode == "CUSTOMER")
            data = <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} {...item as MaestroCustomer} />
        else if(this.props.tranCode == "ORDER")
            data = <OrderComponent ref={(comp) => this.tranComponent = comp} {...item as OrderMaster} />


        this.setState({ modalContent:data, showModal: true, modalCaption: caption, action: itemAction });
    }

    handleNew() {

        let entity: DbEntityBase = EntityAgent.FactoryCreate(this.props.tranCode);
        this.displayModal("New " + this.props.tranCode.toLowerCase(), entity, "New");
    }

    onDoubleClick(e, itemObject) {

        this.displayModal("Editing " + this.props.tranCode.toLowerCase(), itemObject, "Update");

    }

    renderList() {
        const actions = [
            <Button key="add" variant="outline-secondary" style={{ width: "120px" }} href="/MainPage/Index" >Return</Button>,
            <Button key="add" variant="outline-secondary" style={{ width: "120px" }} onClick={  this.handleNew } >New</Button>
        ];

        const selectRow = {
            mode: 'checkbox',
            clickToSelect: true,
            style: (row, rowIndex) => {
                const backgroundColor = '#dce4ed';
                return { backgroundColor };
            }
        }
        
        const options = { onDoubleClick: this.onDoubleClick }; 
        return (

            <BootstrapTable keyField='Id' bootstrap4="true"
                rowEvents={options}
                headerClasses="grid-header-style"
                selectRow={selectRow}
                caption={actions} 
                pagination={paginationFactory()}
                data={this.state.responseMessage.TransactionResult}
                columns={this.state.responseMessage.GridDisplayMembers} />
        );
    }

    render() {
        if (!this.state.init) {
            return (
                <div>
                    <Alert id="gridAlertId" dismissible show={this.state.showSuccess} variant="success" data-dismiss="alert" >
                        <p id="gridAlertMessage">
                            {this.state.successMessage}
                        </p>
                    </Alert>
                    <div>{this.renderList()}</div>

                    <Modal show={this.state.showModal} dialogClassName="modal-90w" onHide={this.handleClose} centered size="lg">
                        <Modal.Header closeButton>
                            <Modal.Title>{this.state.modalCaption}</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <Alert id="modalAlertId" dismissible show={this.state.showError} variant="danger" data-dismiss="alert" >
                                <Alert.Heading id="modalAlertHeadingId">Exception occured</Alert.Heading>
                                <p id="modalAlertUserFriendlyId">
                                    {this.state.errorInfo.UserFriendlyMessage}
                                </p>
                                <hr/>
                                <p id="modalAlertStackTraceId">
                                    {this.state.errorInfo.StackTrace}
                                </p>
                            </Alert>
                            <div id="modalRender">{this.state.modalContent}</div>
                        </Modal.Body>

                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => this.setState({ showModal: false })} >Close</Button>
                            <Button variant="primary" onClick={() => this.saveFct()} >Save changes</Button>
                        </Modal.Footer>
                    </Modal>
                </div>

            );
        }
        else {
            return (<div></div>);

        }



    }
}