var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import RequestMessage from "./RequestMessage";
import MessageHeader from "./MessageHeader";
import axios from 'axios';
import ErrorInfo from "./ErrorInfo";
export default class AxiosAgent {
    getList(tranCode, msgExtension) {
        return __awaiter(this, void 0, void 0, function* () {
            let mde = {};
            let url = "/MainPage/List";
            let msgJson = this.getMessage(msgExtension, "List", tranCode, "", null);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    getImport(tranCode, msgExtension) {
        return __awaiter(this, void 0, void 0, function* () {
            let mde = {};
            let url = "/MainPage/Import";
            let msgJson = this.getMessage(msgExtension, "ImportQb", tranCode, "", null);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    getNewOrderId() {
        return __awaiter(this, void 0, void 0, function* () {
            let mde = { ["REQUEST_TYPE"]: "RequestNewId" };
            let url = "/MainPage/GetOrderId";
            let msgJson = this.getMessage(mde, "New", "ORDER", "", null);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    getItem(id, tranCode) {
        return __awaiter(this, void 0, void 0, function* () {
            let mde = { ["ID"]: id.toString() };
            let url = "/MainPage/GetItem";
            let msgJson = this.getMessage(mde, "Get", tranCode, "", null);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    updateItem(tranCode, item) {
        return __awaiter(this, void 0, void 0, function* () {
            let url = "/MainPage/UpdateItem";
            let mde = {};
            let itemList = [item];
            let msgJson = this.getMessage(mde, "Update", tranCode, "", itemList);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    createItem(tranCode, item, mde) {
        return __awaiter(this, void 0, void 0, function* () {
            let url = "/MainPage/CreateItem";
            if (mde == undefined || mde == null)
                mde = {};
            let itemList = [item];
            let msgJson = this.getMessage(mde, "New", tranCode, "", itemList);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    exportItemQb(tranCode, items, mde) {
        return __awaiter(this, void 0, void 0, function* () {
            let url = "/MainPage/ExportItem";
            if (mde == undefined || mde == null)
                mde = {};
            let itemList = items;
            let msgJson = this.getMessage(mde, "ExportQb", tranCode, "", itemList);
            let result = yield this.sendRequest(url, msgJson);
            return result;
        });
    }
    sendRequest(url, message) {
        return __awaiter(this, void 0, void 0, function* () {
            let result;
            const getErrorInfo = (msg) => {
                let errorInfo = new ErrorInfo();
                errorInfo.ActionType = this.actionType;
                errorInfo.UserFriendlyMessage = msg;
                return errorInfo;
            };
            yield axios.post(url, { requestMessage: message })
                .then(({ data }) => {
                result = data;
                if (this.actionType == 'List') {
                    if (data.TransactionResult == null) {
                        result.TransactionStatus = "ERROR";
                        result.ErrorInfo = getErrorInfo("Exception:Transaction returned empty resultset");
                    }
                    if (data.TransactionResult.length == 0) {
                        result.TransactionStatus = "ERROR";
                        result.ErrorInfo = getErrorInfo("Transaction returned empty resultset (length=0)");
                    }
                }
            })
                .catch(function (error) {
                result.TransactionStatus = "ERROR";
                if (typeof (error) == "string")
                    result.ErrorInfo = getErrorInfo(error);
                else
                    result.ErrorInfo = error;
            });
            return result;
        });
    }
    getMessage(mde, actionType, tranCode, tag, entityList) {
        var message = new RequestMessage();
        message.MessageHeader = new MessageHeader();
        message.MessageHeader.ActionType = actionType;
        message.MessageHeader.TransactionCode = tranCode;
        message.MessageHeader.ClientDate = this.getDate();
        message.MessageHeader.UserName = 'MAESTRO';
        message.MessageHeader.AgentInfo = navigator.userAgent;
        if (mde != null)
            message.MessageDataExtension = mde;
        if (entityList != null)
            message.TransactionEntityList = entityList;
        message.MessageTag = tag;
        return JSON.stringify(message);
    }
    getDate() {
        return (new Date()).toISOString().split('T')[0] + ' ' + (new Date()).toISOString().split('T')[1].split('.')[0];
    }
}
//# sourceMappingURL=AxiosAgent.js.map