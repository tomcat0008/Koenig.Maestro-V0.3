import * as React from 'react';
import { Modal, Alert, Button, Row, Col } from 'react-bootstrap';
import { IModalContainerState } from './IModalContainerState';
import ResponseMessage, { IResponseMessage } from '../classes/ResponseMessage';
import ErrorInfo, { IErrorInfo } from '../classes/ErrorInfo';
import { ICrudComponent } from './ICrudComponent';
import MaestroCustomerComponent from './transaction/MaestroCustomerComponent';
import OrderComponent from './transaction/OrderComponent';
import { IModalContent } from './IModalContent';
import { DbEntityBase } from '../classes/dbEntities/DbEntityBase';
import { ITranComponentProp } from '../classes/ITranComponentProp';
import CustomerProductUnitsComponent from './transaction/CustomerProductUnitsComponent';
import UnitComponent from './transaction/UnitComponent';
import UnitTypeComponent from './transaction/UnitTypeComponent';
import RegionComponent from './transaction/RegionComponent';
import QbInvoiceLogComponent from './transaction/QbInvoiceLogComponent';
import ReportFilterComponent from './transaction/ReportFilterComponent';
import { IReportComponent } from './IReportComponent';
import InvoiceMerge from './transaction/InvoiceMerge';

export default class ModalContainer extends React.Component<IModalContent, IModalContainerState> {

    tranComponent: ICrudComponent;
    reportComponent: IReportComponent;

    state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: "",selected:[],
            ShowSuccess: false, SuccessMessage: "", Action: "", Init: true, Entity: null,
        ShowModal: false, ModalContent: null, ModalCaption: "", ConfirmText: "", ConfirmShow: false,
        MsgDataExtension: {}, ButtonAction:"",ResponseMessage: new ResponseMessage()};

    constructor(props: IModalContent) {
        super(props);
    }

    componentDidMount() {
        let contentComponent = this.createContent(this.props.TranCode, this.props.Entity, this.props.Action);

        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: this.props.TranCode,
            ShowSuccess: false, SuccessMessage: "", Action: this.props.Action, Init: true, Entity: this.props.Entity,
            ShowModal: this.props.Show, ModalContent: contentComponent, ModalCaption: "",
            ConfirmText: "", ConfirmShow: false, ButtonAction: "", MsgDataExtension: {}, selected: [],
            ResponseMessage: new ResponseMessage()
        };
    }

    componentWillReceiveProps(nextProps: IModalContent) {
        try {
            let contentComponent = this.createContent(nextProps.TranCode, nextProps.Entity, nextProps.Action);
            this.setState({
                ShowError: false, ErrorInfo: new ErrorInfo(), TranCode: nextProps.TranCode,
                ShowSuccess: false, SuccessMessage: "", Action: nextProps.Action, Init: true, 
                ShowModal: nextProps.Show, ModalContent: contentComponent, ModalCaption: nextProps.Caption, MsgDataExtension: {},
                ResponseMessage: new ResponseMessage()
            });
        }
        catch (error) {
            this.setState({ ShowError: true, ErrorInfo: error });
        }
    }

    createContent(tranCode: string, item: DbEntityBase, action:string) {
        let componentProp: ITranComponentProp = {
            Entity: item, ExceptionMethod: this.showException,
            ButtonSetMethod: this.buttonSetFct

        };


        if (tranCode == "REPORT") {
            return <ReportFilterComponent ref={(comp) => this.reportComponent = comp} {...componentProp} />
        }
        else if (tranCode == "CUSTOMER")
            return <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "ORDER") {
            return <OrderComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        }
        else if (tranCode == "CUSTOMER_PRODUCT_UNIT")
            return <CustomerProductUnitsComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "UNIT")
            return <UnitComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "UNIT_TYPE")
            return <UnitTypeComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "REGION")
            return <RegionComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
        else if (tranCode == "QUICKBOOKS_INVOICE") {
            if (action == "Merge") {
                return <InvoiceMerge ref={(comp) => this.tranComponent = comp} {...componentProp} />
            }
            else {
                return <QbInvoiceLogComponent ref={(comp) => this.tranComponent = comp} {...componentProp} />
            }
        }
        else
            return <div />
    }

    showException = (error: IErrorInfo) => {
        
        this.setState({ ErrorInfo: error, ShowError: true, Init:false });
    }

    runReportFct = async () => {
        try {
            this.reportComponent.Run();
        }
        catch (error) {
            if (error.DisableAction == false)
                (document.getElementById('btnRun') as HTMLButtonElement).disabled = false;

            this.setState({ ErrorInfo: error, ShowError: true, Init: false });
        }
    }

    saveFct = async () => {
        try {
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = true;
            /*(document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;*/         
            let response: IResponseMessage = await this.tranComponent.Save();
            if (response.TransactionStatus == "ERROR")
                throw response.ErrorInfo;
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = false;
            this.setState({ ShowSuccess: true, ShowError:false, SuccessMessage: response.ResultMessage });
        }
        catch (error) {
            //console.error(error);
            if (error.DisableAction == false)
                (document.getElementById('btnSave') as HTMLButtonElement).disabled = false;

            this.setState({ ErrorInfo: error, ShowError: true, Init:false });
        }

    }

    cancelFct = async () => {
        (document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;
        try {
            let response: IResponseMessage = await this.tranComponent.Cancel();
            if (response.TransactionStatus == "ERROR")
                throw response.ErrorInfo;
            this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: response.ResultMessage });
        }
        catch(error)
        {
            if (error.DisableAction == false)
                (document.getElementById('btnCancel') as HTMLButtonElement).disabled = false;

            this.setState({ ErrorInfo: error, ShowError: true, Init: false });
        }

    }

    createInvoice = async () => {
        try {
            (document.getElementById('btnIntegrate') as HTMLButtonElement).disabled = true;
            (document.getElementById('btnCancel') as HTMLButtonElement).disabled = true;
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = true;
            (document.getElementById('btnRun') as HTMLButtonElement).disabled = true;
            let response: IResponseMessage = await this.tranComponent.Integrate();
            let successMsg: string = response.ResultMessage;
            if (response.Warnings != null) {
                if (response.Warnings.length > 0) {
                    successMsg = successMsg.concat("\r\n Warnings:")
                    response.Warnings.forEach(w => successMsg = successMsg.concat("\r\n" + w));
                }
            }

            (document.getElementById('btnCancel') as HTMLButtonElement).disabled = false;

            this.setState({ ShowSuccess: true, ShowError: false, SuccessMessage: successMsg });
        }
        catch (error) {
            //console.error(error);
            if (error.DisableAction == false)
                (document.getElementById('btnIntegrate') as HTMLButtonElement).disabled = false;
            (document.getElementById('btnCancel') as HTMLButtonElement).disabled = false;
            (document.getElementById('btnSave') as HTMLButtonElement).disabled = false;
            (document.getElementById('btnRun') as HTMLButtonElement).disabled = false;
            this.setState({ ErrorInfo: error, ShowError: true, Init: false });
        }
        
        
    }

    buttonSetFct = (actions: string[]) => {
        if (actions != undefined && actions != null) {
            (document.getElementById("btnCancel") as HTMLButtonElement).style.display = actions.indexOf("Cancel") > -1 ? "" : "none";
            (document.getElementById("btnSave") as HTMLButtonElement).style.display = actions.indexOf("Save") > -1 ? "" : "none";
            (document.getElementById("btnIntegrate") as HTMLButtonElement).style.display = actions.indexOf("Integrate") > -1 ? "" : "none";
            (document.getElementById("btnIntegrate") as HTMLButtonElement).style.display = actions.indexOf("Integrate") > -1 ? "" : "none";
            (document.getElementById("btnRun") as HTMLButtonElement).style.display = actions.indexOf("Run") > -1 ? "" : "none";
        }
    }

    toogleModal(text: string, show: boolean, buttonAction:string) {
        this.setState({ ConfirmShow: show, ConfirmText: text, ButtonAction:buttonAction });
    }

    onYes = () => {
        this.setState({ ConfirmShow: false });

        switch (this.state.ButtonAction)
        {
            case "SAVE":
                this.saveFct()
                break;
            case "CANCEL":
                this.cancelFct();
                break;
            case "INTEGRATE":
                this.createInvoice();
                break;
        }

    }
    onNo = () => {

        this.setState({ ConfirmShow: false, ButtonAction:"", ConfirmText:"" });
    }

    render() {

        return (
            <div>
            <Modal
                size="sm"
                centered
                show={this.state.ConfirmShow}
                aria-labelledby="example-modal-sizes-title-sm"
                dialogClassName="modal-300p"
            >
                <Modal.Header><Modal.Title ></Modal.Title></Modal.Header>
                <Modal.Body>
                        <Row><Col>{this.state.ConfirmText}</Col></Row>
                        
                        <Row style={{ marginTop:"20px" }}>
                            <Col><Button variant="primary" id="btnYes" onClick={this.onYes } >Yes</Button></Col>
                            <Col><Button style={{ float: "right" }} variant="primary" id="btnNo" onClick={this.onNo }>No</Button></Col>
                        </Row>
                </Modal.Body>
            </Modal>

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
                        <Button variant="primary" id="btnCancel" onClick={() => this.toogleModal("Do you want to cancel the order ?",true,"CANCEL" ) } >Cancel Order</Button>
                        <Button variant="primary" id="btnSave" onClick={this.saveFct} >Save changes</Button>
                        <Button variant="primary" id="btnIntegrate" onClick={() => this.toogleModal("Do you want to create the invoice ?", true, "INTEGRATE")} >Create Qb Invoice</Button>
                        <Button variant="primary" id="btnRun" onClick={this.runReportFct} >Run report</Button>
                </Modal.Footer>
                </Modal>
            </div>
        );
    }


}