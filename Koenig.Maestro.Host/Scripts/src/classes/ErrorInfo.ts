export interface IErrorInfo  {
    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
}
export default class ErrorInfo implements IErrorInfo{

    constructor() {
        this.UserFriendlyMessage="";
        this.StackTrace="";
        this.TransactionCode="";
        this.ActionType="";
    }

    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
}