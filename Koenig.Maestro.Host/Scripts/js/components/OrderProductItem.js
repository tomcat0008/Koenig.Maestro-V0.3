import * as React from "react";
import { Button, Form } from "react-bootstrap";
export default class OrderProductItem extends React.Component {
    constructor() {
        super(null);
        this.increase = () => {
        };
        this.decrease = () => {
        };
    }
    render() {
        return (React.createElement("div", null,
            React.createElement(Form.Group, null,
                React.createElement(Button, { variant: "secondary", onClick: this.increase }, "Product Name"),
                React.createElement(Form.Control, { as: "select", id: "unitNr" }),
                React.createElement(Form.Label, { id: "unitNameId" }, "Slab"),
                React.createElement(Form.Control, { id: "quantityId", type: "input" }),
                React.createElement(Button, { variant: "secondary", onClick: this.decrease }, "-"))));
    }
}
//# sourceMappingURL=OrderProductItem.js.map