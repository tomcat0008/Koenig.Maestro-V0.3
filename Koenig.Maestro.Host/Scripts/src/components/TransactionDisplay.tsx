import AxiosAgent from "../classes/AxiosAgent";
import ResponseMessage, { IResponseMessage } from "../classes/ResponseMessage";
import * as React from "react";
import { ITranRequest } from "../classes/ITranRequest";
import ITranState from "../classes/ITranState";

export default class TransactionDisplay extends React.Component<ITranRequest, ITranState> {

    

    constructor(props) {
        super(props);
        let msg: IResponseMessage = new ResponseMessage();  
        //msg.toStringNew = msg.toStringNew.bind(msg);
        this.state = { responseMessage:msg, init:true } ;
    }

    async componentDidMount() {

        const exception = (msg: string) => {
            $('#errorDiv').show();
            $('#errorInfo').text(msg);
        };


        try {
            let response: IResponseMessage = await new AxiosAgent().getImport(this.props.TranCode, this.props.MsgExtension);
            this.setState({ responseMessage: response, init: false } );
            console.log(response);

        }
        catch (err) {
            exception(err);
        }
        $('#wait').hide();

    }

    render() {
        if(!this.state.init)
            return (<div>{this.state.responseMessage.ResultMessage}</div>);
        else {
            return (<div></div>);

        }
    }

}