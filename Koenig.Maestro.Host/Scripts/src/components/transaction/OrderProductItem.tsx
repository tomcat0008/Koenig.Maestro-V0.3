import * as React from "react";
import { Button, Form, Col} from "react-bootstrap";
import { IMaestroUnit } from "../../classes/dbEntities/IMaestroUnit";
import EntityAgent from "../../classes/EntityAgent";
import { ICustomerProductUnit } from "../../classes/dbEntities/ICustomerProductUnit";

interface IOrderProductItemData {
    Quantity: number,
    ProductMapId:number,
    UnitId: number,
    Init:boolean
}

export interface IOrderProductItemProp {
    MapId:number,
    ProductLabel:string,
    UnitTypeLabel:string,
    Quantity: number,
    Units: IMaestroUnit[],
    Cpu:ICustomerProductUnit,
    UpdateOrderMethod: (mapId: number, unitId: number, quantity:number) => void;
}

export default class OrderProductItem extends React.Component<IOrderProductItemProp, IOrderProductItemData> {

    state = { Quantity: 0, UnitId: 0, ProductMapId: 0, Init: false };   

    constructor(props: IOrderProductItemProp) {
        super(props);
        this.state = {
            Quantity: props.Quantity,
            UnitId: ((props.Cpu == undefined || props.Cpu == null) ? 0 : props.Cpu.Id),
            ProductMapId: props.MapId, Init: false
        };   
    }

    componentDidMount() {
        //this.setState({ UnitId: parseInt((document.getElementById("unitNr_"+this.props.MapId) as HTMLSelectElement).value) });
    }

    increase = () => {
        let unitId: number = parseInt((document.getElementById("unitNr_" + this.props.MapId) as HTMLSelectElement).value);

        this.setState({
            Quantity: this.state.Quantity + 1,
            UnitId: unitId

        });
        this.props.UpdateOrderMethod(this.props.MapId, unitId, this.state.Quantity+1);
    }

    decrease = () => {
        let unitId: number = parseInt((document.getElementById("unitNr_" + this.props.MapId) as HTMLSelectElement).value);
        this.setState({
            Quantity: this.state.Quantity - 1,
            UnitId: unitId
        });
        this.props.UpdateOrderMethod(this.props.MapId, unitId, this.state.Quantity-1);
    }

    reset = () => {
        this.setState({ Quantity: 0 });

        this.props.UpdateOrderMethod(this.props.MapId, this.state.UnitId, 0);
    }

    onChangeUnit = () => {
        let input: HTMLSelectElement = document.getElementById("unitNr_" + this.props.MapId) as HTMLSelectElement;
        this.setState({ UnitId: parseInt(input.value) });
        this.props.UpdateOrderMethod(this.props.MapId, parseInt(input.value), this.state.Quantity);

    }

    onChangeQuantity = () => {
        let input: HTMLInputElement = document.getElementById("quantity_" + this.props.MapId) as HTMLInputElement;
        if(!$.isNumeric(input.value))
            input.value = "0";
        let unitId: number = parseInt((document.getElementById("unitNr_" + this.props.MapId) as HTMLSelectElement).value);

        this.setState({ Quantity: parseInt(input.value), UnitId: unitId });
        this.props.UpdateOrderMethod(this.props.MapId, unitId , parseInt(input.value));

    }

    render() {
        const renderTooltip = props => (
            <div
                {...props}
                style={{
                    backgroundColor: 'rgba(0, 0, 0, 0.85)',
                    padding: '2px 10px',
                    color: 'white',
                    borderRadius: 3,
                    ...props.style,
                }}
            >
                Simple tooltip
                            </div>
        );
        

        if (this.props.Units != null)
            if(this.props.Units.find(u=>u.Name.startsWith("--")) == undefined)
                this.props.Units.unshift(EntityAgent.GetFirstSelecItem("UNIT") as IMaestroUnit);


            return (
                <div className="container" style={{ textAlign:"left", width: "890px", padding:"4px" }}>
                    <Form>
                        <Form.Row>
                            <Col style={{ textAlign: "left" }}>
                                <Button id={"mapName_"+this.props.MapId} variant="secondary" onClick={this.increase} style={{ width: "320px" }}>{ this.props.ProductLabel}</Button>
                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <Form.Control as="select"
                                    id={"unitNr_" + this.props.MapId}
                                    disabled={this.props.Units == null}
                                    style={{ width: "100px" }}
                                    onChange={this.onChangeUnit}  >
                                    {
                                        (this.props.Units != null ?
                                            //this.props.Units.sort((a, b) => a.Name.localeCompare(b.Name)).filter(u=>this.props.ProductMaps.find(m=>m.UnitId==u.Id)!=undefined).map(u => <option value={u.Id}>{u.Name}</option>)
                                            this.props.Units.filter(u=>u.Name!="UNKNOWN").sort((a, b) => a.Name.localeCompare(b.Name)).map(u =>
                                                <option selected={ (this.props.Cpu == undefined ? false : (this.props.Cpu.UnitId == u.Id )) || (this.props.Units.length == 2 && u.Id >=0 ) } value={u.Id}>{u.Name}</option>)
                                            : null)
                                    }

                                </Form.Control>
                            </Col>
                            <Col style={{ textAlign: "left", paddingTop: "6px" }}><Form.Label tabIndex={-1} id="unitNameId" style={{ width: "50px" }}>{ this.props.UnitTypeLabel}</Form.Label></Col>
                            <Col style={{ textAlign: "left" }}>
                                <Form.Control id={"quantity_"+this.props.MapId}
                                    onChange={this.onChangeQuantity}
                                    value={this.state.Quantity.toString()} type="input"
                                    style={{ width: "80px", backgroundColor: this.state.Quantity == 0 ? "#ffffff" : "#fcea85" }} />
                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <Button tabIndex={-1} variant="secondary" id={"btnIncrease_"+this.props.MapId}
                                    onClick={this.increase} style={{ width: "40px" }}>+</Button>
                                

                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <Button tabIndex={-1} disabled={this.state.Quantity == 0} variant="secondary" id={"btnDecrease_" + this.props.MapId}
                                    onClick={this.decrease} style={{ width: "40px" }}>-</Button>

                            </Col>

                            <Col style={{ textAlign: "left" }}>
                                <Button tabIndex={-1} disabled={this.state.Quantity == 0} variant="danger" id={"btnDisable" + this.props.MapId}
                                    onClick={this.reset} style={{ width: "40px" }}>X</Button>

                            </Col>
                        </Form.Row>
                    </Form>
                </div>
            );

/*
 *                                 <OverlayTrigger overlay={<Tooltip id="increaseQuantityToolTip">Increase Quantity</Tooltip>}>
                                    <Button variant="secondary"
                                        onClick={this.increase} style={{ width: "40px" }}>+</Button>
                                </OverlayTrigger>
*/
    }

}