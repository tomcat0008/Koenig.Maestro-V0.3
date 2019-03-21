export interface IErrorInfo  {
    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
    DisableAction: boolean;
}
export default class ErrorInfo implements IErrorInfo{

    constructor() {
        this.UserFriendlyMessage="";
        this.StackTrace="";
        this.TransactionCode="";
        this.ActionType = "";
        this.DisableAction = true;
    }

    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
    DisableAction: boolean;
}