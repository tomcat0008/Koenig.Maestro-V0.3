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
export default class UnitTypeComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { UnitType: null, Init: true, ErrorInfo: new ErrorInfo() };
        this.state = { UnitType: props.Entity, Init: true, ErrorInfo: new ErrorInfo() };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let display = yield ea.GetUnitTypeDisplay(this.props.Entity.Id);
            this.Save = this.Save.bind(this);
            this.setState(display);
            this.props.ButtonSetMethod(display.UnitType.Actions);
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
            let unitType = this.state.UnitType;
            unitType.Name = document.getElementById("unitTypeNameId").value;
            unitType.Description = document.getElementById("unitTypeDescId").value;
            unitType.CanHaveUnits = document.getElementById("chkCanHaveUnits").checked;
            let result = yield ea.SaveUnitType(unitType);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
                //this.props.ExceptionMethod(result.ErrorInfo);
            }
            else {
                if (unitType.Id <= 0) {
                    unitType.Id = (result.TransactionResult.Id);
                    document.getElementById("unitTypeId").value = unitType.Id.toString();
                }
                unitType.IsNew = false;
            }
            return result;
        });
    }
    DisableEnable(disable) {
        document.getElementById("unitTypeNameId").disabled = disable;
        document.getElementById("unitTypeDescId").disabled = disable;
        document.getElementById("chkCanHaveUnits").disabled = disable;
    }
    render() {
        let unitType = this.state.UnitType;
        if (this.state.Init) {
            return (React.createElement("p", null));
        }
        else {
            return (React.createElement("div", { className: "container" },
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Type Id"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "unitTypeId", plaintext: true, readOnly: true, defaultValue: unitType.IsNew ? "New Unit Type" : unitType.Id.toString() }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Type Name"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "unitTypeNameId", type: "input", defaultValue: unitType.Name }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Unit Type Description"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "unitTypeDescId", type: "input", defaultValue: unitType.Description }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Can have units"),
                    React.createElement(Col, null,
                        React.createElement(Form.Check, { "aria-label": "chkCanHaveUnits", id: "chkCanHaveUnits", checked: unitType.CanHaveUnits })))));
        }
    }
}
//# sourceMappingURL=UnitTypeComponent.js.map