var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from "react";
import ResponseMessage from "../classes/ResponseMessage";
import OrderComponent from "./transaction/OrderComponent";
import OrderMaster from "../classes/dbEntities/IOrderMaster";
export default class NewItemDisplay extends React.Component {
    constructor(props) {
        super(props);
        this.state = { init: true, responseMessage: new ResponseMessage() };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const exception = (msg) => {
                $('#errorDiv').show();
                $('#errorInfo').text(msg);
            };
        });
    }
    render() {
        if (this.props.TranCode == "ORDER") {
            let orderDef = new OrderMaster(0);
            return (React.createElement("div", null,
                React.createElement(OrderComponent, Object.assign({}, orderDef))));
        }
    }
}
//# sourceMappingURL=NewItemDisplay.js.map