import ErrorInfo, { IErrorInfo } from "./ErrorInfo";

export interface IResponseMessage {
    ErrorInfo: IErrorInfo;
    ResultMessage: string;
    TransactionResult: any;
    TransactionStatus: string;
    TransactionCode: string;
    TransactionDuration: number;
    Warnings: string[];
    GridDisplayMembers: Array<any>;
    getLength();
    toStringNew();
}

export default class ResponseMessage implements IResponseMessage {

    constructor() {
        this.ErrorInfo = new ErrorInfo();
        this.TransactionResult = new Array<string>();
    }

    getLength() {
        if (this.TransactionResult == null)
            return 0;
        else
            return this.TransactionResult.length;
    }

    toStringNew():string  {
        let result:string = '';
        result += 'ResultMessage:' + this.ResultMessage + '\n';
        result += 'TransactionCode' + this.TransactionCode + '\n';
        result += 'TransactionDuration'+ this.TransactionDuration + '\n';
        //result += this.Warnings.join('\n');
        return result;
    }

    readonly ErrorInfo: IErrorInfo;
    ResultMessage: string;
    TransactionResult: Array<any>;
    TransactionStatus: string;
    TransactionCode: string;
    TransactionDuration: number;
    GridDisplayMembers: string[];
    Warnings: string[];
}

