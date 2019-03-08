import RequestMessage from "../classes/RequestMessage";
import MessageHeader from "../classes/MessageHeader";
import axios from 'axios';
export default class NaviAgent {
    navigateList(transactionCode, actionType, caption) {
        let message = this.getMessage(transactionCode);
        $("#content").text(caption);
        axios.post("/MainPage/List", { requestMessage: message }).then(function (response) {
            return response.data;
        }).catch(function (error) {
            alert(error);
        });
    }
    getMessage(transactionCode) {
        var message = new RequestMessage();
        message.MessageHeader = new MessageHeader();
        message.MessageHeader.ActionType = "List";
        message.MessageHeader.TransactionCode = transactionCode;
        message.MessageHeader.ClientDate = this.getDate();
        message.MessageHeader.HostName = 'STING';
        message.MessageHeader.UserName = 'TOMCAT';
        message.MessageHeader.AgentInfo = 'an agent';
        message.MessageDataExtension["ID"] = "1";
        message.MessageDataExtension["ID2"] = "X";
        message.MessageTag = '';
        return JSON.stringify(message);
    }
    getDate() {
        return (new Date()).toISOString().split('T')[0] + ' ' + (new Date()).toISOString().split('T')[1].split('.')[0];
    }
}
//# sourceMappingURL=NaviAgent.js.map