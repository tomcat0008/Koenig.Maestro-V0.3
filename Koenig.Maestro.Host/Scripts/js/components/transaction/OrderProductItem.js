import * as React from "react";
import { Button, Form, Col } from "react-bootstrap";
import EntityAgent from "../../classes/EntityAgent";
export default class OrderProductItem extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Quantity: 0, UnitId: 0, ProductMapId: 0, Init: false };
        this.increase = () => {
            let unitId = parseInt(document.getElementById("unitNr_" + this.props.MapId).value);
            this.setState({
                Quantity: this.state.Quantity + 1,
                UnitId: unitId
            });
            this.props.UpdateOrderMethod(this.props.MapId, unitId, this.state.Quantity + 1);
        };
        this.decrease = () => {
            let unitId = parseInt(document.getElementById("unitNr_" + this.props.MapId).value);
            this.setState({
                Quantity: this.state.Quantity - 1,
                UnitId: unitId
            });
            this.props.UpdateOrderMethod(this.props.MapId, unitId, this.state.Quantity - 1);
        };
        this.reset = () => {
            this.setState({ Quantity: 0 });
            this.props.UpdateOrderMethod(this.props.MapId, this.state.UnitId, 0);
        };
        this.onChangeUnit = () => {
            let input = document.getElementById("unitNr_" + this.props.MapId);
            this.setState({ UnitId: parseInt(input.value) });
            this.props.UpdateOrderMethod(this.props.MapId, parseInt(input.value), this.state.Quantity);
        };
        this.onChangeQuantity = () => {
            let input = document.getElementById("quantity_" + this.props.MapId);
            if (!$.isNumeric(input.value))
                input.value = "0";
            let unitId = parseInt(document.getElementById("unitNr_" + this.props.MapId).value);
            this.setState({ Quantity: parseInt(input.value), UnitId: unitId });
            this.props.UpdateOrderMethod(this.props.MapId, unitId, parseInt(input.value));
        };
        this.state = {
            Quantity: props.Quantity,
            UnitId: ((props.Cpu == undefined || props.Cpu == null) ? 0 : props.Cpu.Id),
            ProductMapId: props.MapId, Init: false
        };
    }
    componentDidMount() {
        //this.setState({ UnitId: parseInt((document.getElementById("unitNr_"+this.props.MapId) as HTMLSelectElement).value) });
    }
    render() {
        const renderTooltip = props => (React.createElement("div", Object.assign({}, props, { style: Object.assign({ backgroundColor: 'rgba(0, 0, 0, 0.85)', padding: '2px 10px', color: 'white', borderRadius: 3 }, props.style) }), "Simple tooltip"));
        if (this.props.Units != null)
            if (this.props.Units.find(u => u.Name.startsWith("--")) == undefined)
                this.props.Units.unshift(EntityAgent.GetFirstSelecItem("UNIT"));
        return (React.createElement("div", { className: "container", style: { textAlign: "left", width: "890px", padding: "4px" } },
            React.createElement(Form, null,
                React.createElement(Form.Row, null,
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Button, { id: "mapName_" + this.props.MapId, variant: "secondary", onClick: this.increase, style: { width: "320px" } }, this.props.ProductLabel)),
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Form.Control, { as: "select", id: "unitNr_" + this.props.MapId, disabled: this.props.Units == null, style: { width: "100px" }, onChange: this.onChangeUnit }, (this.props.Units != null ?
                            //this.props.Units.sort((a, b) => a.Name.localeCompare(b.Name)).filter(u=>this.props.ProductMaps.find(m=>m.UnitId==u.Id)!=undefined).map(u => <option value={u.Id}>{u.Name}</option>)
                            this.props.Units.sort((a, b) => a.Name.localeCompare(b.Name)).map(u => React.createElement("option", { selected: (this.props.Cpu == undefined ? false : (this.props.Cpu.UnitId == u.Id)) || (this.props.Units.length == 2 && u.Id >= 0), value: u.Id }, u.Name))
                            : null))),
                    React.createElement(Col, { style: { textAlign: "left", paddingTop: "6px" } },
                        React.createElement(Form.Label, { tabIndex: -1, id: "unitNameId", style: { width: "50px" } }, this.props.UnitTypeLabel)),
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Form.Control, { id: "quantity_" + this.props.MapId, onChange: this.onChangeQuantity, value: this.state.Quantity.toString(), type: "input", style: { width: "80px", backgroundColor: this.state.Quantity == 0 ? "#ffffff" : "#fcea85" } })),
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Button, { tabIndex: -1, variant: "secondary", id: "btnIncrease_" + this.props.MapId, onClick: this.increase, style: { width: "40px" } }, "+")),
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Button, { tabIndex: -1, disabled: this.state.Quantity == 0, variant: "secondary", id: "btnDecrease_" + this.props.MapId, onClick: this.decrease, style: { width: "40px" } }, "-")),
                    React.createElement(Col, { style: { textAlign: "left" } },
                        React.createElement(Button, { tabIndex: -1, disabled: this.state.Quantity == 0, variant: "danger", id: "btnDisable" + this.props.MapId, onClick: this.reset, style: { width: "40px" } }, "X"))))));
        /*
         *                                 <OverlayTrigger overlay={<Tooltip id="increaseQuantityToolTip">Increase Quantity</Tooltip>}>
                                            <Button variant="secondary"
                                                onClick={this.increase} style={{ width: "40px" }}>+</Button>
                                        </OverlayTrigger>
        */
    }
}
//# sourceMappingURL=OrderProductItem.js.map