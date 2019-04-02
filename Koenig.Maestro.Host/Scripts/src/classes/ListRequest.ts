import { ITranRequest } from "./ITranRequest";

export default class TranRequest implements ITranRequest
{
    ListSelect: boolean;
    ButtonList: string[];
    TranCode: string;
    Action: string;
    ListTitle: string;
    MsgExtension: { [key: string]: string };
}