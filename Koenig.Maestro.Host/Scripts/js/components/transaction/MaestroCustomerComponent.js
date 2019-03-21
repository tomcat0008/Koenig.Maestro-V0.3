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
import MaestroCustomer from '../../classes/dbEntities/IMaestroCustomer';
import EntityAgent from '../../classes/EntityAgent';
import MaestroRegion from '../../classes/dbEntities/IMaestroRegion';
import ErrorInfo from '../../classes/ErrorInfo';
export default class MaestroCustomerComponent extends React.Component {
    //item:IMaestroCustomer;
    constructor(props) {
        super(props);
        this.state = { Customer: null, Regions: [], Init: true, ErrorInfo: new ErrorInfo() };
        //this.setState({ Customer: props.Entity as IMaestroCustomer, Regions: [], Init: true, ErrorInfo: new ErrorInfo()  });
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cd = yield ea.GetCustomerDisplay(this.props.Entity.Id);
            this.Save = this.Save.bind(this);
            this.setState(cd);
            if (this.props.Entity.IsNew)
                document.getElementById("customerId").value = '';
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
            cust.DefaultPaymentType = document.getElementById("defaultPaymentId").checked ? "COD" : "";
            let result = yield ea.SaveCustomer(cust);
            this.DisableEnable(true);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
                //this.props.ExceptionMethod(result.ErrorInfo);
            }
            else {
                if (cust.Id <= 0) {
                    cust.Id = (result.TransactionResult.Id);
                    document.getElementById("customerIdId").value = cust.Id.toString();
                }
                cust.IsNew = false;
                this.DisableEnable(true);
                return result;
            }
        });
    }
    DisableEnable(disable) {
        document.getElementById("customerIdId").disabled = disable;
        document.getElementById("qbCompanyId").disabled = disable;
        document.getElementById("customerNameId").disabled = disable;
        document.getElementById("qbListId").disabled = disable;
        document.getElementById("customerTitleId").disabled = disable;
        document.getElementById("customerRegionId").disabled = disable;
        document.getElementById("customerAddressId").disabled = disable;
        document.getElementById("customerEmailId").disabled = disable;
        document.getElementById("customerPhoneId").disabled = disable;
        document.getElementById("defaultPaymentId").disabled = disable;
    }
    render() {
        let cus = this.state.Customer;
        let regions = this.state.Regions;
        if (this.state.Init) {
            return (React.createElement("p", null));
        }
        else {
            let qbCompany, qbId;
            if (this.props.Entity.IsNew) {
                qbCompany = React.createElement(Form.Control, { id: "qbCompanyId", type: "input" });
                qbId = React.createElement(Form.Control, { id: "qbListId", type: "input" });
            }
            else {
                qbCompany = React.createElement(Form.Control, { id: "qbCompanyId", plaintext: true, readOnly: true, className: "modal-disabled", defaultValue: cus.QuickBoosCompany });
                qbId = React.createElement(Form.Control, { id: "qbListId", plaintext: true, readOnly: true, className: "modal-disabled", defaultValue: cus.QuickBooksId });
            }
            return (React.createElement(Form, null,
                React.createElement(Form.Row, null,
                    React.createElement(Form.Group, { as: Col, controlId: "customerId" },
                        React.createElement(Form.Label, null, "Customer Id"),
                        React.createElement(Form.Control, { className: "modal-disabled", id: "customerIdId", plaintext: true, readOnly: true, defaultValue: cus.IsNew ? "New customer" : cus.Id.toString() }))),
                React.createElement(Form.Row, null,
                    React.createElement(Form.Group, { as: Col, controlId: "customerName" },
                        React.createElement(Form.Label, null, "Customer Name"),
                        React.createElement(Form.Control, { id: "customerNameId", type: "input", defaultValue: cus.Name })),
                    React.createElement(Form.Group, { as: Col, controlId: "customerTitle" },
                        React.createElement(Form.Label, null, "Title"),
                        React.createElement(Form.Control, { id: "customerTitleId", type: "input", defaultValue: cus.Title }))),
                React.createElement(Form.Row, null,
                    React.createElement(Form.Group, { as: Col, controlId: "customerEmail" },
                        React.createElement(Form.Label, null, "Email"),
                        React.createElement(Form.Control, { type: "input", id: "customerEmailId", defaultValue: cus.Email })),
                    React.createElement(Form.Group, { as: Col, controlId: "customerPhone" },
                        React.createElement(Form.Label, null, "Phone"),
                        React.createElement(Form.Control, { type: "input", id: "customerPhoneId", defaultValue: cus.Phone })),
                    React.createElement(Form.Group, { as: Col, controlId: "customerRegion2" },
                        React.createElement(Form.Label, null, "Region"),
                        React.createElement(Form.Control, { as: "select", id: "customerRegionId" }, regions.map(rg => React.createElement("option", { selected: rg.Id == cus.RegionId, value: rg.Id },
                            rg.Name + " (" + rg.PostalCode + ")",
                            " "))))),
                React.createElement(Form.Row, null,
                    React.createElement(Form.Group, { as: Col, controlId: "customerAddress" },
                        React.createElement(Form.Label, null, "Address"),
                        React.createElement(Form.Control, { as: "textarea", id: "customerAddressId", rows: "3" }, cus.Address))),
                React.createElement(Form.Row, null,
                    React.createElement(Form.Group, { as: Col, controlId: "customerQbCompany" },
                        React.createElement(Form.Label, null, "QB Company"),
                        qbCompany),
                    React.createElement(Form.Group, { as: Col, controlId: "customerQbId" },
                        React.createElement(Form.Label, null, "QB Object Id"),
                        qbId)),
                React.createElement(Form.Group, { id: "defaultPayment" },
                    React.createElement(Form.Check, { type: "checkbox", id: "defaultPaymentId", label: "C.O.D.", defaultChecked: cus.DefaultPaymentType == "COD" }))));
        }
    }
}
//# sourceMappingURL=MaestroCustomerComponent.js.map