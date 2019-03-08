import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Modal, Alert, Button } from 'react-bootstrap';
import { IModalContainerState } from './IModalContainerState';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { ICrudComponent } from './ICrudComponent';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import OrderComponent from './transaction/OrderComponent';
import MaestroCustomer from '../classes/dbEntities/IMaestroCustomer';
import OrderMaster from '../classes/dbEntities/IOrderMaster';

export default class ModalContainer extends React.Component<string, IModalContainerState> {

    tranComponent: ICrudComponent;

    constructor(props) {
        super(props);
        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";

        let data = this.createContent(props.tranCode, null);

        this.state = {
            showError: false, errorInfo: errorInfo,
            showSuccess: false, successMessage: "",
            showModal: false, modalContent: null, modalCaption: "",
            responseMessage: new ResponseMessage()
        };



    }

    createContent(tranCode: string, item:DbEntityBase) {
        if (tranCode == "CUSTOMER")
            return <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} {...item as MaestroCustomer} />
        else if (tranCode == "ORDER")
            return <OrderComponent ref={(comp) => this.tranComponent = comp} {...item as OrderMaster} />
    }

    handleClose() {
        this.setState({ showModal: false });
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ showSuccess: true, successMessage: response.ResultMessage });
            this.handleClose();
        }
        catch (error) {
            this.setState({ errorInfo: error, showError: true });
        }

    }

    render() {
        return (
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
                        <hr />
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
        );
    }


}