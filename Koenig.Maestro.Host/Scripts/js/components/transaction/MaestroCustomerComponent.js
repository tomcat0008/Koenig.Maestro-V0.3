var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import { Form, Col } from 'react-bootstrap';
import { Row } from 'react-bootstrap';
import MaestroCustomer from '../../classes/dbEntities/IMaestroCustomer';
import EntityAgent from '../../classes/EntityAgent';
import MaestroRegion from '../../classes/dbEntities/IMaestroRegion';
export default class MaestroCustomerComponent extends React.Component {
    //item:IMaestroCustomer;
    constructor(props) {
        super(props);
        this.state = { Customer: props, Regions: [], init: true };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cd = yield ea.GetCustomerDisplay(this.props.Id);
            this.Save = this.Save.bind(this);
            this.setState(cd);
            if (this.props.IsNew)
                document.getElementById("customerRegionId").value = '';
        });
    }
    Save() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cust = new MaestroCustomer(0);
            cust.Id = this.state.Customer.Id;
            cust.Address = document.getElementById("customerAddressId").value;
            cust.Email = document.getElementById("customerEmailId").value;
            cust.Name = document.getElementById("customerNameId").value;
            cust.Phone = document.getElementById("customerPhoneId").value;
            cust.Title = document.getElementById("customerTitleId").value;
            cust.QuickBooksId = document.getElementById("qbListId").value;
            cust.QuickBoosCompany = document.getElementById("qbCompanyId").value;
            cust.DefaultPaymentType = this.state.Customer.DefaultPaymentType == null ? "" : this.state.Customer.DefaultPaymentType;
            cust.MaestroRegion = new MaestroRegion(parseInt(document.getElementById("customerRegionId").value));
            let result = yield ea.SaveCustomer(cust);
            return result;
        });
    }
    render() {
        let cus = this.state.Customer;
        let regions = this.state.Regions;
        if (this.state.init) {
            return (React.createElement("p", null));
        }
        else {
            let qbCompany, qbId;
            if (this.props.IsNew) {
                qbCompany = React.createElement(Form.Control, { id: "qbCompanyId", type: "input" });
                qbId = React.createElement(Form.Control, { id: "qbListId", type: "input" });
            }
            else {
                qbCompany = React.createElement(Form.Control, { id: "qbCompanyId", plaintext: true, readOnly: true, defaultValue: cus.QuickBoosCompany });
                qbId = React.createElement(Form.Control, { id: "qbListId", plaintext: true, readOnly: true, defaultValue: cus.QuickBooksId });
            }
            return (React.createElement(Form, null,
                React.createElement(Form.Group, { as: Row, controlId: "customerId" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Customer Id"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { plaintext: true, readOnly: true, defaultValue: cus.IsNew ? "New customer" : cus.Id.toString() }))),
                React.createElement(Form.Group, { as: Row, controlId: "customerName" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Customer Name"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { id: "customerNameId", type: "input", defaultValue: cus.Name }))),
                React.createElement(Form.Group, { as: Row, controlId: "customerTitle" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Title"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { id: "customerTitleId", type: "input", defaultValue: cus.Title }))),
                React.createElement(Form.Group, { as: Row, controlId: "customerRegion2" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Region"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { as: "select", id: "customerRegionId" }, regions.map(rg => React.createElement("option", { selected: rg.Id == cus.RegionId, value: rg.Id },
                            rg.Name + " (" + rg.PostalCode + ")",
                            " "))))),
                React.createElement(Form.Group, { as: Row, controlId: "customerAddress" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Address"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { as: "textarea", id: "customerAddressId", rows: "3" }, cus.Address))),
                React.createElement(Form.Group, { as: Row, controlId: "customerEmail" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Email"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { type: "input", id: "customerEmailId", defaultValue: cus.Email }))),
                React.createElement(Form.Group, { as: Row, controlId: "customerPhone" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Phone"),
                    React.createElement(Col, { sm: "6" },
                        React.createElement(Form.Control, { type: "input", id: "customerPhoneId", defaultValue: cus.Phone }))),
                React.createElement(Form.Group, { as: Row, controlId: "customerQbCompany" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Qb Company"),
                    React.createElement(Col, { sm: "6" }, qbCompany)),
                React.createElement(Form.Group, { as: Row, controlId: "customerQbId" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "Qb Object Id"),
                    React.createElement(Col, { sm: "6" }, qbId)),
                React.createElement(Form.Group, { as: Row, controlId: "defaultPayment" },
                    React.createElement(Col, { className: "col-form-label", sm: 3, as: Form.Label }, "C.O.D."),
                    React.createElement(Col, { sm: "6" },
                        React.createElement("input", { id: "defaultPaymentId", type: "checkbox", checked: cus.DefaultPaymentType == "COD" })))));
        }
    }
}
//# sourceMappingURL=MaestroCustomerComponent.js.map