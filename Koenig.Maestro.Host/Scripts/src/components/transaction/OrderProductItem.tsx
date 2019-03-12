import * as React from "react";
import { Button, Form, Col, Row, OverlayTrigger, Tooltip } from "react-bootstrap";

interface IOrderProductItemData {
    Quantity: number,
    ProductMapId:number,
    UnitId: number,
    Init:boolean
}

export default class OrderProductItem extends React.Component<any, IOrderProductItemData> {
    
    constructor() {
        super(null);
        this.state = { Quantity: 0, UnitId: 0, ProductMapId: 0, Init: false };   
    }

    increase = () => {
        this.setState({ Quantity: this.state.Quantity + 1 });         
    }

    decrease = () => {
        this.setState({ Quantity: this.state.Quantity - 1 });         
    }

    reset = () => {
        this.setState({ Quantity: 0 });
    }

    onChange = () => {
        let input: HTMLInputElement = document.getElementById('quantityId') as HTMLInputElement;
        if(!$.isNumeric(input.value))
            input.value = "0";
        this.setState({Quantity:parseInt(input.value)});

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
            return (
                <div className="container" style={{ width: "680px" }}>
                    <Form>
                        <Form.Row>
                            <Col style={{ textAlign: "left" }}>
                                <Button variant="secondary" onClick={this.increase} style={{ width: "240px" }}>Product Name</Button>
                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <Form.Control as="select" id="unitNr" style={{ width: "100px" }} >
                                    {
                                        //regions.map(rg => <option selected={rg.Id == cus.RegionId} value={rg.Id}>{rg.Name + " (" + rg.PostalCode + ")"} </option>)
                                    }

                                </Form.Control>
                            </Col>
                            <Col style={{ textAlign: "left", paddingTop: "6px" }}><Form.Label id="unitNameId" style={{ width: "50px" }}>Slab</Form.Label></Col>
                            <Col style={{ textAlign: "left" }}>
                                <Form.Control id="quantityId"
                                    onChange={this.onChange}
                                    value={this.state.Quantity.toString()} type="input"
                                    style={{ width: "80px", backgroundColor: this.state.Quantity == 0 ? "#ffffff" : "#fcea85" }} />
                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <OverlayTrigger overlay={<Tooltip id="increaseQuantityToolTip">Increase Quantity</Tooltip>}>
                                    <Button variant="secondary"
                                        onClick={this.increase} style={{ width: "40px" }}>+</Button>
                                </OverlayTrigger>
                            </Col>
                            <Col style={{ textAlign: "left" }}>
                                <OverlayTrigger overlay={<Tooltip id="decreaseQuantityToolTip">Decrease Quantity</Tooltip>}>
                                <Button disabled={this.state.Quantity == 0} variant="secondary"
                                        onClick={this.decrease} style={{ width: "40px" }}>-</Button>
                                    </OverlayTrigger>
                            </Col>

                            <Col style={{ textAlign: "left" }}>
                                <OverlayTrigger overlay={<Tooltip id="deleteToolTo[">Delete</Tooltip>}>
                                        <Button disabled={this.state.Quantity == 0} variant="danger"
                                            onClick={this.reset} style={{ width: "40px" }}>X</Button>
                                </OverlayTrigger>

                            </Col>
                        </Form.Row>
                    </Form>
                </div>
            );


    }

}