export interface ITranRequest
{
    tranCode: string,
    action: string,
    listTitle: string,
    msgExtension: { [key: string]: string }
}