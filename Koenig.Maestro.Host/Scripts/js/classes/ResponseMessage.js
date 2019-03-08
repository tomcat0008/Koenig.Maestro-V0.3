import ErrorInfo from "./ErrorInfo";
export default class ResponseMessage {
    constructor() {
        this.ErrorInfo = new ErrorInfo();
        this.TransactionResult = new Array();
    }
    getLength() {
        if (this.TransactionResult == null)
            return 0;
        else
            return this.TransactionResult.length;
    }
    toStringNew() {
        let result = '';
        result += 'ResultMessage:' + this.ResultMessage + '\n';
        result += 'TransactionCode' + this.TransactionCode + '\n';
        result += 'TransactionDuration' + this.TransactionDuration + '\n';
        //result += this.Warnings.join('\n');
        return result;
    }
}
//# sourceMappingURL=ResponseMessage.js.map