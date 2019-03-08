export interface IErrorInfo  {
    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
}
export default class ErrorInfo implements IErrorInfo{
    UserFriendlyMessage: string;
    StackTrace: string;
    TransactionCode: string;
    ActionType: string;
}