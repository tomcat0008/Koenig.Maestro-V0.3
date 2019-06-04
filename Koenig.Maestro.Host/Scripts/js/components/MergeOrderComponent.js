var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import { Image, Card } from 'react-bootstrap';
export default class MergeOrderComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Show: true };
        this.ChangeState = () => __awaiter(this, void 0, void 0, function* () {
            let disp = !this.state.Show;
            yield this.setState({ Show: disp });
            this.props.ClickFct(this.props.RenderOrderMaster.Id, this.props.Location, disp);
        });
        //this.state = { Show: this.props.Display };
    }
    render() {
        let om = this.props.RenderOrderMaster;
        let items = om.OrderItems;
        var itemBody = React.createElement("div", { id: this.props.Location + om.Id, style: { display: this.props.Display ? "" : "none" } },
            React.createElement(Card, { className: "mergeOrderCard", onClick: this.ChangeState },
                React.createElement(Card.Body, null,
                    React.createElement(Card.Title, null,
                        React.createElement(Image, { src: "/Maestro/img/order_new.png" }),
                        "Order No:",
                        om.Id),
                    React.createElement(Card.Subtitle, { className: "mb-2 text-muted" },
                        "Delivery Date:",
                        om.DeliveryDate.toLocaleString().substr(0, 10)),
                    React.createElement(Card.Text, null,
                        React.createElement("div", null, items.map(i => React.createElement("li", null, i.Quantity + " " + i.MapDescription)))))));
        return itemBody;
    }
}
//# sourceMappingURL=MergeOrderComponent.js.map