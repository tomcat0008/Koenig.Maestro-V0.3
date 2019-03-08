import * as React from 'react';
import { Form, Col } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
export default class MaestroCustomerComponent extends React.Component {
    //item:IMaestroCustomer;
    constructor(props) {
        super(props);
        this.state = props;
    }
    /*
        Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;

    Name: string;
    Title: string;
    Address: string;
    Phone: string;
    Email: string;
    QuickBooksId: string;
    QuickBoosCompany: string;
    MaestroRegion: number;
    DefaultPaymentType: string;
    */
    render() {
        return (React.createElement(Form, null,
            React.createElement(Form.Group, { as: Row, controlId: "customerId" },
                React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Customer Id"),
                React.createElement(Col, { sm: "6" },
                    React.createElement(Form.Control, { plaintext: true, readOnly: true, defaultValue: this.state.Id.toString() }))),
            React.createElement(Form.Group, { as: Row, controlId: "customerName" },
                React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Customer Name"),
                React.createElement(Col, { sm: "6" },
                    React.createElement(Form.Control, { type: "input", defaultValue: this.state.Name })))));
    }
}
//# sourceMappingURL=MaestroCustomerComponent.js.map