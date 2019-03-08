import { IResponseMessage } from "../classes/ResponseMessage";
import { IErrorInfo } from "../classes/ErrorInfo";

export interface IModalContainerState {
    responseMessage: IResponseMessage,
    showModal: boolean,
    showError: boolean,
    showSuccess: boolean,
    successMessage: string,
    errorInfo: IErrorInfo,
    modalContent: any,
    modalCaption: string

}