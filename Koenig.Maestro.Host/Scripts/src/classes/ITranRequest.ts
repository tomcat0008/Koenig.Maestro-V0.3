export interface ITranRequest
{
    TranCode: string,
    Action: string,
    ListTitle: string,
    MsgExtension: { [key: string]: string }
}