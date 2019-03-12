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

export default class ModalContainer extends React.Component<IModalContent, IModalContainerState> {

    tranComponent: ICrudComponent;

    constructor(props: IModalContent) {
        super(props);


        let contentComponent = this.createContent(props.TranCode, props.Entity);

        this.setState ({
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode:props.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action:props.Action, Init:true, Entity:props.Entity,
            ShowModal: props.Show, ModalContent: contentComponent, ModalCaption: "",
            ResponseMessage: new ResponseMessage()
        });

    }

    componentWillReceiveProps(nextProps: IModalContent) {

        let contentComponent = this.createContent(nextProps.TranCode, nextProps.Entity);
        this.setState({
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: nextProps.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: nextProps.Action, Init: true, Entity: nextProps.Entity,
            ShowModal: nextProps.Show, ModalContent: contentComponent, ModalCaption: nextProps.Caption,
            ResponseMessage: new ResponseMessage()
        });

    }


    createContent(tranCode: string, item:DbEntityBase) {
        if (tranCode == "CUSTOMER")
            return <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} {...item as MaestroCustomer} />
        else if (tranCode == "ORDER")
            return <OrderComponent ref={(comp) => this.tranComponent = comp} {...item as OrderMaster} />
        else
            return <div/>
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = true;
        }
        catch (error) {
            this.setState({ ErrorInfo: error, ShowError: true });
        }

    }

    render() {
        return (
            <Modal show={this.props.Show} dialogClassName="modal-90w" onHide={this.props.Close} centered size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>{this.state == null ? "" : this.state.ModalCaption}</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Alert id="modalAlertId" dismissible show={this.state == null ? false : this.state.ShowError} variant="danger" data-dismiss="alert" >
                        <Alert.Heading id="modalAlertHeadingId">Exception occured</Alert.Heading>
                        <p id="modalAlertUserFriendlyId">
                            {this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage}
                        </p>
                        <hr />
                        <p id="modalAlertStackTraceId">
                            {this.state == null ? "" :this.state.ErrorInfo.StackTrace}
                        </p>
                    </Alert>
                    <Alert variant="success" dismissible show={this.state == null ? false : this.state.ShowSuccess} data-dismiss="alert">
                        <p id="">{this.state == null ? "" : this.state.SuccessMessage}</p>
                    </Alert>


                    <div id="modalRender">{this.state == null ? "" : this.state.ModalContent}</div>
                </Modal.Body>

                <Modal.Footer>
                    <Button variant="secondary" onClick={this.props.Close } >Close</Button>
                    <Button variant="primary" id="btnSave" onClick={() => this.saveFct()} >Save changes</Button>
                </Modal.Footer>
            </Modal>
        );
    }


}