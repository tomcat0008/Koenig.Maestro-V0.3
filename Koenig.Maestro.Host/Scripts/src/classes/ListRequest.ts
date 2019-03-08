import { ITranRequest } from "./ITranRequest";

export default class TranRequest implements ITranRequest
{
    tranCode: string;
    action: string;
    listTitle: string;
    msgExtension: { [key: string]: string };
}