import { IResponseMessage } from "../classes/ResponseMessage";
import { IErrorInfo } from "../classes/ErrorInfo";
import { DbEntityBase } from "../classes/dbEntities/DbEntityBase";

export interface IModalContainerState {
    ResponseMessage: IResponseMessage,
    ShowModal: boolean,
    ShowError: boolean,
    ShowSuccess: boolean,
    SuccessMessage: string,
    ErrorInfo: IErrorInfo,
    ModalContent: any,
    ModalCaption: string,
    Action: string,
    Init: boolean,
    Entity: DbEntityBase,
    TranCode:string

}