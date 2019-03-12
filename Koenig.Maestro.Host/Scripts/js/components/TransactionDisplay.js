var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import AxiosAgent from "../classes/AxiosAgent";
import ResponseMessage from "../classes/ResponseMessage";
import * as React from "react";
export default class TransactionDisplay extends React.Component {
    constructor(props) {
        super(props);
        let msg = new ResponseMessage();
        //msg.toStringNew = msg.toStringNew.bind(msg);
        this.state = { responseMessage: msg, init: true };
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            const exception = (msg) => {
                $('#errorDiv').show();
                $('#errorInfo').text(msg);
            };
            try {
                let response = yield new AxiosAgent().getImport(this.props.TranCode, this.props.MsgExtension);
                this.setState({ responseMessage: response, init: false });
                console.log(response);
            }
            catch (err) {
                exception(err);
            }
            $('#wait').hide();
        });
    }
    render() {
        if (!this.state.init)
            return (React.createElement("div", null, this.state.responseMessage.ResultMessage));
        else {
            return (React.createElement("div", null));
        }
    }
}
//# sourceMappingURL=TransactionDisplay.js.map