import { IResponseMessage } from "./ResponseMessage";

export default interface ITranState {
    responseMessage: IResponseMessage,
    init: boolean
}
