import * as React from "react";
import { ITranComponentProp } from "../../classes/ITranComponentProp";
import EntityAgent, { IRegionDisplay, IQbInvoiceLogDisplay } from "../../classes/EntityAgent";
import { ICrudComponent } from "../ICrudComponent";
import ErrorInfo from "../../classes/ErrorInfo";
import { IResponseMessage } from "../../classes/ResponseMessage";
import MaestroRegion, { IMaestroRegion } from "../../classes/dbEntities/IMaestroRegion";
import { Form, Row, Col } from "react-bootstrap";
import { IQbInvoiceLog } from "../../classes/dbEntities/IQbInvoiceLog";

export default class QbInvoiceLogComponent extends React.Component<ITranComponentProp, IQbInvoiceLogDisplay> implements ICrudComponent {

    state = { InvoiceLog: null, Init: true, ErrorInfo: new ErrorInfo() }

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { InvoiceLog: props.Entity as IQbInvoiceLog, Init: false, ErrorInfo: new ErrorInfo() }
    }

    async Save(): Promise<IResponseMessage> { return null; }

    DisableEnable(disable: boolean) { }

    async Cancel(): Promise<IResponseMessage> {
        return null;
    }

    async Integrate(): Promise<IResponseMessage> {
        return null;
    }

    render() {
        let log: IQbInvoiceLog = this.state.InvoiceLog;
        this.props.ButtonSetMethod(log.Actions);
        if (this.state.Init) {
            return (<p></p>);
        }
        else {

            return (
                <div className="container">
                    <Row>
                        <Col className="col-form-label" sm={2}>Log Id</Col>
                        <Col style={{ paddingTop: "0px" }} sm={4}>
                            <Form.Control id="logId" plaintext disabled defaultValue={ log.Id.toString()} />
                        </Col>
                    </Row>
                    <Row>
                        <Col className="col-form-label" sm={2}>Customer</Col>
                        <Col style={{ paddingTop: "0px" }} sm={8}>
                            <Form.Control id="customerId" plaintext disabled defaultValue={log.CustomerName +" (" + log.QuickBooksCustomerId + ")" } />
                        </Col>
                    </Row>
                    <Row>
                        <Col className="col-form-label" sm={2}>Integration Status</Col>
                        <Col style={{ paddingTop: "0px" }} sm={2}>
                            <Form.Control id="integrationStatusId" plaintext disabled defaultValue={log.IntegrationStatus} />
                        </Col>
                        <Col className="col-form-label" sm={2}>Integration Date</Col>
                        <Col style={{ paddingTop: "0px" }} sm={4}>
                            <Form.Control id="integrationDateId" plaintext disabled defaultValue={log.IntegrationDate.toString().replace("T"," ")} />
                        </Col>
                    </Row>
                    <Row>
                        <Col className="col-form-label" sm={2}>QB Invoice No</Col>
                        <Col style={{ paddingTop: "0px" }} sm={2}>
                            <Form.Control id="invoiceId" plaintext disabled defaultValue={log.QuickBooksInvoiceId} />
                        </Col>
                        <Col className="col-form-label" sm={2}>QB Txn</Col>
                        <Col style={{ paddingTop: "0px" }} sm={4}>
                            <Form.Control id="qbtxnId" plaintext disabled defaultValue={log.QuickBooksTxnId} />
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Error Log</Col>
                        <Col style={{ paddingTop: "5px" }} sm={8}>
                            <Form.Control as="textarea" id="notesId" rows="2" value={log.ErrorLog} readOnly />
                        </Col>
                    </Row>


                </div>
            );
        }
    }

}