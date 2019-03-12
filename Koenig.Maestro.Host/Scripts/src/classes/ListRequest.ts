import { ITranRequest } from "./ITranRequest";

export default class TranRequest implements ITranRequest
{
    TranCode: string;
    Action: string;
    ListTitle: string;
    MsgExtension: { [key: string]: string };
}