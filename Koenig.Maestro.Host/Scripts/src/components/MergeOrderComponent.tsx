import * as React from 'react';
import { IOrderMaster } from '../classes/dbEntities/IOrderMaster';
import { Button, Image, Card } from 'react-bootstrap';
import { IOrderItem } from '../classes/dbEntities/IOrderItem';

export interface IMergeOrderComponent {
    RenderOrderMaster: IOrderMaster;
    Location: string;
    ClickFct: (id: number, location: string, displayStatus: boolean) => void;
    Display: boolean;
}

export interface MergeOrderState {
    Show: boolean;
}

export default class MergeOrderComponent extends React.Component<IMergeOrderComponent, MergeOrderState> {


    state = { Show: true }

    constructor(props: IMergeOrderComponent) {
        super(props);
        //this.state = { Show: this.props.Display };
    }

    ChangeState = async () => {
        let disp: boolean = !this.state.Show;
        await this.setState({ Show: disp });

        this.props.ClickFct(this.props.RenderOrderMaster.Id, this.props.Location, disp);
    }

    render() {
        let om: IOrderMaster = this.props.RenderOrderMaster;
        let items: IOrderItem[] = om.OrderItems;
        var itemBody = <div id={this.props.Location+om.Id} style={{ display: this.props.Display ? "" : "none"  }}>
            <Card className="mergeOrderCard" onClick={this.ChangeState} >
                <Card.Body>
                    <Card.Title><Image src="/Maestro/img/order_new.png"></Image>Order No:{om.Id}</Card.Title>
                    <Card.Subtitle className="mb-2 text-muted">Delivery Date:{om.DeliveryDate.toLocaleString().substr(0,10)}</Card.Subtitle>
                    <Card.Text>
                        <div>
                        {
                            items.map(i => <li>{i.Quantity + " " + i.MapDescription}</li>)
                        }
                        </div>
                    </Card.Text>
                </Card.Body>
            </Card>

        </div>
        return itemBody;
    }


}