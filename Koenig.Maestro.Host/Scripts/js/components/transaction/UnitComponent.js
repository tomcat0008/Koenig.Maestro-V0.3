var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from "react";
import EntityAgent from "../../classes/EntityAgent";
import ErrorInfo from "../../classes/ErrorInfo";
import { Form, Row, Col } from "react-bootstrap";
export default class UnitComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Unit: null, UnitTypes: [], Init: true, ErrorInfo: new ErrorInfo() };
        this.state = { Unit: props.Entity, UnitTypes: [], Init: true, ErrorInfo: new ErrorInfo() };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let display = yield ea.GetUnitDisplay(this.props.Entity.Id);
            this.Save = this.Save.bind(this);
            this.setState(display);
            this.props.ButtonSetMethod(display.Unit.Actions);
        });
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
            this.DisableEnable(true);
            let ea = new EntityAgent();
            let unit = this.state.Unit;
            unit.UnitTypeId = parseInt(document.getElementById("unitTypeId").value);
            unit.Name = document.getElementById("unitNameId").value;
            unit.QuickBooksUnit = document.getElementById("qbUnitId").value;
            let result = yield ea.SaveUnit(unit);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
            }
            else {
                if (unit.Id <= 0) {
                    unit.Id = (result.TransactionResult.Id);
                    document.getElementById("unitId").value = unit.Id.toString();
                }
                unit.IsNew = false;
            }
            return result;
        });
    }
    DisableEnable(disable) {
        document.getElementById("unitNameId").disabled = disable;
        document.getElementById("unitTypeId").disabled = disable;
        document.getElementById("qbUnitId").disabled = disable;
    }
    render() {
        let unit = this.state.Unit;
        let unitTypes = this.state.UnitTypes.filter(u => u.CanHaveUnits).sort((a, b) => a.Name.localeCompare(b.Name));
        if (unitTypes.find(c => c.Id == -1) == undefined)
            unitTypes.unshift(EntityAgent.GetFirstSelecItem("UNIT_TYPE"));
        if (this.state.Init) {
            return (React.createElement("p", null));
        }
        else {
            return (React.createElement("div", { className: "container" },
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Id"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "unitId", plaintext: true, readOnly: true, defaultValue: unit.IsNew ? "New Region" : unit.Id.toString() }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Name"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "unitNameId", type: "input", defaultValue: unit.Name }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Type"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { as: "select", id: "unitTypeId" }, unitTypes.map(ut => React.createElement("option", { selected: ut.Id == unit.UnitTypeId, value: ut.Id },
                            ut.Name,
                            " "))))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Quickbooks Unit"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "qbUnitId", type: "input", defaultValue: unit.QuickBooksUnit })))));
        }
    }
}
//# sourceMappingURL=UnitComponent.js.map