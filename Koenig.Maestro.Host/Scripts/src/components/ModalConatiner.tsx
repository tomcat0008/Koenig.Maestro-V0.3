import * as React from 'react';
import { Modal, Alert, Button } from 'react-bootstrap';
import { IModalContainerState } from './IModalContainerState';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { ICrudComponent } from './ICrudComponent';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import OrderComponent from './transaction/OrderComponent';
import MaestroCustomer from '../classes/dbEntities/IMaestroCustomer';
import OrderMaster from '../classes/dbEntities/IOrderMaster';
import { IModalContent } from './IModalContent';
import { DbEntityBase } from '../classes/dbEntities/DbEntityBase';
import { ITranComponentProp } from '../classes/ITranComponentProp';
import CustomerProductUnitsComponent from './transaction/CustomerProductUnitsComponent';

export default class ModalContainer extends React.Component<IModalContent, IModalContainerState> {

    tranComponent: ICrudComponent;

    state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: "",
            ShowSuccess: false, SuccessMessage: "", Action: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "",
            ResponseMessage: new ResponseMessage()};

    constructor(props: IModalContent) {
        super(props);
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

    componentWillReceiveProps(nextProps: IModalContent) {
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

    createContent(tranCode: string, item: DbEntityBase) {
        let componentProp: ITranComponentProp = { Entity: item, ExceptionMethod: this.showException };
        if (tranCode == "CUSTOMER") 
            return <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} { ...componentProp } />
        else if (tranCode == "ORDER")
            return <OrderComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "CUSTOMER_PRODUCT_UNIT")
            return <CustomerProductUnitsComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else
            return <div />
    }

    showException = (error: IErrorInfo) => {
        
        this.setState({ ErrorInfo: error, ShowError: true, Init:false });
    }

    saveFct = async () => {
        try {
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = true;
            (document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;            
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ ShowSuccess: true, ShowError:false, SuccessMessage: response.ResultMessage });
        }
        catch (error) {
            console.error(error);
            if (error.DisableAction == false)
                (document.getElementById('btnSave') as HTMLButtonElement).disabled = false;

            this.setState({ ErrorInfo: error, ShowError: true, Init:false });
        }

    }

    cancelFct = async () => {

    }

    render() {
        return (
            <Modal show={this.props.Show} aria-labelledby="modalWidth" dialogClassName="modal-90w" onHide={this.props.Close}>
                <Modal.Header closeButton>
                    <Modal.Title>{this.state == null ? "" : this.state.ModalCaption}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Alert id="modalAlertId" show={this.state.ShowError} variant="danger" >
                        <Alert.Heading id="modalAlertHeadingId">Exception occured
                            <div style={{
                                float: "right", fontFamily: "Segoe UI", cursor: "pointer", fontSize: "16px" }}
                                onClick={() => { this.setState({ ShowError: false }) }}>
                                X
                            </div>

                            
                        </Alert.Heading>
                        <div className="errorHeader">
                            <p id="modalAlertUserFriendlyId">
                                {this.state == null ? "" : (this.state.Init  ? "" : this.state.ErrorInfo.UserFriendlyMessage)}
                            </p>
                        </div>

                        <hr />
                        <div className="errorStackTrace">
                            <p id="modalAlertStackTraceId">
                                {this.state == null ? "" : (this.state.Init ? "" : this.state.ErrorInfo.StackTrace)}
                            </p>
                        </div>
                    </Alert>
                    <Alert variant="success" dismissible show={this.state == null ? false : this.state.ShowSuccess} data-dismiss="alert">
                        <p id="">{this.state == null ? "" : this.state.SuccessMessage}</p>
                    </Alert>

                    <div id="modalRender">{this.state == null ? "" : this.state.ModalContent}</div>
                </Modal.Body>

                <Modal.Footer>
                    <Button variant="secondary" onClick={this.props.Close } >Close</Button>
                    <Button style={{ display: this.state.TranCode == "ORDER" ? "block" : "none" }} variant="primary" id="btnCancel" onClick={() => this.cancelFct()} >Cancel Order</Button>
                    <Button variant="primary" id="btnSave" onClick={() => this.saveFct()} >Save changes</Button>
                    
                </Modal.Footer>
            </Modal>
        );
    }


}