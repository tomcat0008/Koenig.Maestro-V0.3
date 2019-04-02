var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from "react";
import ErrorInfo from "../../classes/ErrorInfo";
import { Form, Row, Col } from "react-bootstrap";
export default class QbInvoiceLogComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { InvoiceLog: null, Init: true, ErrorInfo: new ErrorInfo() };
        this.state = { InvoiceLog: props.Entity, Init: false, ErrorInfo: new ErrorInfo() };
    }
    Save() {
        return __awaiter(this, void 0, void 0, function* () { return null; });
    }
    DisableEnable(disable) { }
    Cancel() {
        return __awaiter(this, void 0, void 0, function* () {
            return null;
        });
    }
    Integrate() {
        return __awaiter(this, void 0, void 0, function* () {
            return null;
        });
    }
    render() {
        let log = this.state.InvoiceLog;
        this.props.ButtonSetMethod(log.Actions);
        if (this.state.Init) {
            return (React.createElement("p", null));
        }
        else {
            return (React.createElement("div", { className: "container" },
                React.createElement(Row, null,
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "Log Id"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 4 },
                        React.createElement(Form.Control, { id: "logId", plaintext: true, disabled: true, defaultValue: log.Id.toString() }))),
                React.createElement(Row, null,
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "Customer"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 8 },
                        React.createElement(Form.Control, { id: "customerId", plaintext: true, disabled: true, defaultValue: log.CustomerName + " (" + log.QuickBooksCustomerId + ")" }))),
                React.createElement(Row, null,
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "Integration Status"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 2 },
                        React.createElement(Form.Control, { id: "integrationStatusId", plaintext: true, disabled: true, defaultValue: log.IntegrationStatus })),
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "Integration Date"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 4 },
                        React.createElement(Form.Control, { id: "integrationDateId", plaintext: true, disabled: true, defaultValue: log.IntegrationDate.toString().replace("T", " ") }))),
                React.createElement(Row, null,
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "QB Invoice No"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 2 },
                        React.createElement(Form.Control, { id: "invoiceId", plaintext: true, disabled: true, defaultValue: log.QuickBooksInvoiceId })),
                    React.createElement(Col, { className: "col-form-label", sm: 2 }, "QB Txn"),
                    React.createElement(Col, { style: { paddingTop: "0px" }, sm: 4 },
                        React.createElement(Form.Control, { id: "qbtxnId", plaintext: true, disabled: true, defaultValue: log.QuickBooksTxnId }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Error Log"),
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 8 },
                        React.createElement(Form.Control, { as: "textarea", id: "notesId", rows: "2", value: log.ErrorLog, readOnly: true })))));
        }
    }
}
//# sourceMappingURL=QbInvoiceLogComponent.js.map