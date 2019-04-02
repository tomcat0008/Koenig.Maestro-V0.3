var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import CustomerProductUnit from '../../classes/dbEntities/ICustomerProductUnit';
import EntityAgent from '../../classes/EntityAgent';
import { Form, Col } from 'react-bootstrap';
import ErrorInfo from '../../classes/ErrorInfo';
export default class CustomerProductUnitsComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Customers: [], Products: [], Units: [], Init: true, ErrorInfo: new ErrorInfo(), ProductId: 0, Entity: new CustomerProductUnit(0), UnitTypeId: 0 };
        this.onProductChange = (evt) => {
            let product = this.state.Products.find(p => p.Id == parseInt(evt.target.value));
            this.setState({ ProductId: evt.target.value, UnitTypeId: product.UnitTypeId });
            document.getElementById("cpuUnitId").disabled = false;
        };
        this.state = {
            ProductId: props.Entity.Id, Customers: [], Products: [], Units: [], Init: true, ErrorInfo: new ErrorInfo(),
            Entity: this.props.Entity, UnitTypeId: this.props.Entity.UnitTypeId
        };
        //props.Entity.IsNew = props.Entity.Id > 0;
    }
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
    Save() {
        return __awaiter(this, void 0, void 0, function* () {
            let cpu = new CustomerProductUnit(0);
            let ea = new EntityAgent();
            cpu.Id = this.props.Entity.Id;
            cpu.ProductId = parseInt(document.getElementById("cpuProductId").value);
            cpu.UnitId = parseInt(document.getElementById("cpuUnitId").value);
            cpu.CustomerId = parseInt(document.getElementById("cpuCustomerId").value);
            this.DisableEnable(true);
            let result = yield ea.SaveCustomerProductUnit(cpu);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
                //this.props.ExceptionMethod(result.ErrorInfo);
            }
            else {
                if (cpu.Id <= 0) {
                    cpu.Id = (result.TransactionResult.Id);
                    document.getElementById("cpuId").value = cpu.Id.toString();
                }
                cpu.IsNew = false;
                return result;
            }
        });
    }
    DisableEnable(disable) {
        document.getElementById("cpuProductId").disabled = disable;
        document.getElementById("cpuUnitId").disabled = disable;
        document.getElementById("cpuCustomerId").disabled = disable;
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cd = yield ea.GetCustomerProductUnitDisplay();
            if (cd.ErrorInfo != null) {
                if (cd.ErrorInfo.UserFriendlyMessage != "") {
                    this.props.ExceptionMethod(cd.ErrorInfo);
                }
                else {
                    cd.Init = false;
                    if (this.props.Entity.Id > 0) {
                        cd.Entity = this.props.Entity;
                        cd.ProductId = this.props.Entity.ProductId;
                        cd.UnitTypeId = this.props.Entity.UnitTypeId;
                    }
                    this.setState(cd);
                }
            }
            else {
                cd.Init = false;
                if (this.props.Entity.Id > 0) {
                    cd.Entity = this.props.Entity;
                    cd.ProductId = this.props.Entity.ProductId;
                    cd.UnitTypeId = this.props.Entity.UnitTypeId;
                }
                this.setState(cd);
            }
            this.props.ButtonSetMethod(cd.Entity.Actions);
        });
    }
    render() {
        //units.filter(u => u.UnitTypeId == products.find(p => p.Id == this.state.ProductId).UnitTypeId).map(unit => <option value={unit.Id}>{unit.Name} </option>)
        if (!this.state.Init) {
            let customers = this.state.Customers;
            let products = this.state.Products.filter(p => p.UnitTypeCanHaveUnits);
            let units = this.state.Units;
            let cpu = this.state.Entity;
            if (customers.find(c => c.Id == -1) == undefined)
                customers.unshift(EntityAgent.GetFirstSelecItem("CUSTOMER"));
            if (products.find(c => c.Id == -1) == undefined)
                products.unshift(EntityAgent.GetFirstSelecItem("PRODUCT"));
            if (units.find(c => c.Id == -1) == undefined)
                units.unshift(EntityAgent.GetFirstSelecItem("UNIT"));
            products.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            customers.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            units.sort((a, b) => { return a.Name.localeCompare(b.Name); });
            let options;
            if (this.state.UnitTypeId > 0) {
                //(document.getElementById("cpuUnitId") as HTMLSelectElement).disabled = false;
                options = units.filter(u => u.UnitTypeId == this.state.UnitTypeId || u.Id == -1).map(unit => React.createElement("option", { selected: unit.Id == cpu.UnitId, value: unit.Id },
                    unit.Name,
                    " "));
            }
            return (React.createElement("div", { className: "container" },
                React.createElement(Form, null,
                    React.createElement(Form.Group, { as: Col, controlId: "itemId" },
                        React.createElement(Form.Label, null, "Customer Product Unit Id"),
                        React.createElement(Form.Control, { className: "modal-disabled", id: "cpuId", plaintext: true, readOnly: true, defaultValue: cpu.IsNew ? "New record" : cpu.Id.toString() })),
                    React.createElement(Form.Group, { as: Col, controlId: "customerDropDownId" },
                        React.createElement(Form.Label, null, "Customer"),
                        React.createElement(Form.Control, { as: "select", id: "cpuCustomerId" }, customers.map(customer => React.createElement("option", { selected: customer.Id == cpu.CustomerId, value: customer.Id },
                            customer.Name + " (" + customer.QuickBoosCompany + ")",
                            " ")))),
                    React.createElement(Form.Group, { as: Col, controlId: "productDropDownId" },
                        React.createElement(Form.Label, null, "Product"),
                        React.createElement(Form.Control, { as: "select", id: "cpuProductId", onChange: this.onProductChange }, products.map(product => React.createElement("option", { selected: product.Id == cpu.ProductId, value: product.Id },
                            product.Name,
                            " ")))),
                    React.createElement(Form.Group, { as: Col, controlId: "unitDropDownId" },
                        React.createElement(Form.Label, null, "Unit"),
                        React.createElement(Form.Control, { as: "select", id: "cpuUnitId", disabled: cpu.IsNew }, options)))));
        }
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=CustomerProductUnitsComponent.js.map