import { ITranRequest } from "../classes/ITranRequest";
import * as React from "react";
import ITranState from "../classes/ITranState";
import ResponseMessage, { IResponseMessage } from "../classes/ResponseMessage";
import AxiosAgent from "../classes/AxiosAgent";
import OrderComponent from "./transaction/OrderComponent";
import OrderMaster, { IOrderMaster } from "../classes/dbEntities/IOrderMaster";
import * as ReactDOM from "react-dom";

export default class NewItemDisplay extends React.Component<ITranRequest, ITranState> {

    constructor(props) {
        super(props);
        this.state = {init:true, responseMessage:new ResponseMessage()}
    }

    async componentDidMount() {

        const exception = (msg: string) => {
            $('#errorDiv').show();
            $('#errorInfo').text(msg);
        };


    }

    render() {
        if (this.props.tranCode == "ORDER") {
            let orderDef: IOrderMaster = new OrderMaster(0);
            return (
                <div>
                <OrderComponent {...orderDef} />
                </div>
            );
        }
    }
}