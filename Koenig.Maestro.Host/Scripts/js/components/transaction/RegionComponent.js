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
export default class RegionComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Region: null, Init: true, ErrorInfo: new ErrorInfo() };
        this.state = { Region: props.Entity, Init: true, ErrorInfo: new ErrorInfo() };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let display = yield ea.GetRegionDisplay(this.props.Entity.Id);
            this.Save = this.Save.bind(this);
            this.setState(display);
            this.props.ButtonSetMethod(display.Region.Actions);
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
            let region = this.state.Region;
            region.Name = document.getElementById("regionNameId").value;
            region.Description = document.getElementById("regionDescId").value;
            region.PostalCode = document.getElementById("regionPKId").value;
            region.GreaterRegion = document.getElementById("greaterRegionId").value;
            let result = yield ea.SaveRegion(region);
            if (result.ErrorInfo != null) {
                this.DisableEnable(false);
                throw result.ErrorInfo;
            }
            else {
                if (region.Id <= 0) {
                    region.Id = (result.TransactionResult.Id);
                    document.getElementById("regionId").value = region.Id.toString();
                }
                region.IsNew = false;
            }
            return result;
        });
    }
    DisableEnable(disable) {
        document.getElementById("regionNameId").disabled = disable;
        document.getElementById("regionPKId").disabled = disable;
        document.getElementById("regionDescId").disabled = disable;
        document.getElementById("greaterRegionId").disabled = disable;
    }
    render() {
        let region = this.state.Region;
        if (this.state.Init) {
            return (React.createElement("p", null));
        }
        else {
            return (React.createElement("div", { className: "container" },
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Region Id"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "regionId", plaintext: true, readOnly: true, defaultValue: region.IsNew ? "New Region" : region.Id.toString() }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Region Name"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "regionNameId", type: "input", defaultValue: region.Name }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Postal Code"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "regionPKId", type: "input", defaultValue: region.PostalCode }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Region Description"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "regionDescId", type: "input", defaultValue: region.Description }))),
                React.createElement(Row, null,
                    React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Greater Region"),
                    React.createElement(Col, { style: { paddingTop: "5px" } },
                        React.createElement(Form.Control, { id: "greaterRegionId", type: "input", defaultValue: region.GreaterRegion })))));
        }
    }
}
//# sourceMappingURL=RegionComponent.js.map