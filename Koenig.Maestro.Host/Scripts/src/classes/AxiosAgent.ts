import RequestMessage from "./RequestMessage";
import MessageHeader from "./MessageHeader";
import ResponseMessage, { IResponseMessage } from "./ResponseMessage";
import axios from 'axios';
import ErrorInfo, { IErrorInfo } from "./ErrorInfo";
import { DbEntityBase } from "./dbEntities/DbEntityBase";
import { string } from "prop-types";

export default class AxiosAgent {

    private actionType: string;

    public async runReport(tranCode: string, reportCode: string, msgExtension: { [key: string]: string }): Promise<IResponseMessage> {

        let url: string = "/MainPage/RunReport";
        let msgJson = this.getMessage(msgExtension, "Report", tranCode, "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async getList(tranCode: string, msgExtension: { [key: string]: string } ): Promise<IResponseMessage> {

        let mde: { [key: string]: string } = {};
        let url: string = "/MainPage/List";
        let msgJson = this.getMessage(msgExtension, "List", tranCode, "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
       
        return result;
    }

    public async getImport(tranCode: string, msgExtension: { [key: string]: string }): Promise<IResponseMessage> {

        let mde: { [key: string]: string } = {};
        let url: string = "/MainPage/Import";
        let msgJson = this.getMessage(msgExtension, "ImportQb", tranCode, "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async getNewOrderId():Promise<IResponseMessage> {
        let mde: { [key: string]: string } = { ["REQUEST_TYPE"]: "RequestNewId"};
        let url: string = "/MainPage/GetOrderId";
        let msgJson = this.getMessage(mde, "New", "ORDER", "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async getItem(id: number, tranCode: string): Promise<IResponseMessage> {
        let mde: { [key: string]: string } = { ["ID"]: id.toString() };
        let url: string = "/MainPage/GetItem";
        let msgJson = this.getMessage(mde, "Get", tranCode, "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async updateItem(tranCode: string, item:DbEntityBase):Promise<IResponseMessage> {
        let url: string = "/MainPage/UpdateItem";
        let mde: { [key: string]: string } = {};
        let itemList = [item];
        let msgJson = this.getMessage(mde, "Update", tranCode, "", itemList );
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async createItem(tranCode: string, item: DbEntityBase, mde?: { [key: string]: string }): Promise<IResponseMessage> {
        let url: string = "/MainPage/CreateItem";
        if (mde == undefined || mde == null)
            mde = {};
        let itemList = [item];
        let msgJson = this.getMessage(mde, "New", tranCode, "", itemList);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async exportItemQb(tranCode: string, items: DbEntityBase[], mde?: { [key: string]: string }): Promise<IResponseMessage> {
        let url: string = "/MainPage/ExportItem";
        if (mde == undefined || mde == null)
            mde = {};
        let itemList = items;
        let msgJson = this.getMessage(mde, "ExportQb", tranCode, "", itemList);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async cancelItem(tranCode: string, item:DbEntityBase) {
        let url: string = "/MainPage/DeleteItem";
        let mde: { [key: string]: string } = { ["ID"]: item.Id.toString() };
        let itemList = [item];
        let msgJson = this.getMessage(mde, "Delete", tranCode, "", itemList);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }

    public async createInvoice(invoiceList: number[]) {
        let url: string = "/MainPage/CreateInvoice";

        let idList: string = "";
        invoiceList.forEach(i => idList += i + ",");

        let mde: { [key: string]: string } = {
            ["INVOICE_LIST"]: idList
        };

        let msgJson = this.getMessage(mde, "ExportQb", "QUICKBOOKS_INVOICE", "", null);
        let result: IResponseMessage = await this.sendRequest(url, msgJson);
        return result;
    }


    private async sendRequest(url:string, message:string): Promise<IResponseMessage> {

        let result: IResponseMessage;
        const getErrorInfo = (msg: string): ErrorInfo => {
            let errorInfo: ErrorInfo = new ErrorInfo();
            errorInfo.ActionType = this.actionType;
            errorInfo.UserFriendlyMessage = msg;
            return errorInfo;
        }
        await axios.post<IResponseMessage>(url, { requestMessage: message })
            .then(
                ({ data }) => {
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
            .catch(function (error)
            {
                result.TransactionStatus = "ERROR";
                if (typeof (error) == "string")

                    result.ErrorInfo = getErrorInfo(error);
                else
                    result.ErrorInfo = error;
            });
        return result;

    }

    private getMessage(mde: { [key: string]: string }, actionType: string, tranCode: string, tag:string, entityList:any[] ) {

        var message = new RequestMessage();
        message.MessageHeader = new MessageHeader()
        message.MessageHeader.ActionType = actionType;
        message.MessageHeader.TransactionCode = tranCode;
        message.MessageHeader.ClientDate = this.getDate();
        message.MessageHeader.UserName = 'MAESTRO';
        message.MessageHeader.AgentInfo = navigator.userAgent;
        if (mde != null)
            message.MessageDataExtension = mde;
        if(entityList != null)
            message.TransactionEntityList = entityList;
        message.MessageTag = tag;
        return JSON.stringify(message);
    }

    private getDate() {
        return (new Date()).toISOString().split('T')[0] + ' ' + (new Date()).toISOString().split('T')[1].split('.')[0]
    }


}