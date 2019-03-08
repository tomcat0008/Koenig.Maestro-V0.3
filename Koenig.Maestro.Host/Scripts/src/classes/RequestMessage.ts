import MessageHeader from './MessageHeader';

export default class RequestMessage {
    public MessageHeader: MessageHeader;
    public MessageTag: string;
    public TransactionEntityList: any[];
    public MessageDataExtension: { [key: string]: string } = {};
}