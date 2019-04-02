import { DbEntityBase } from "./DbEntityBase";

export interface IQbInvoiceLog extends DbEntityBase {
    OrderId: number;
    CustomerId: number;
    CustomerName: string;
    IntegrationStatus: string;
    IntegrationDate: Date;
    BatchId: number;
    QuickBooksInvoiceId: string;
    QuickBooksTxnId: string;
    QuickBooksCustomerId: string;
    ErrorLog: string;

}

export default class QbInvoiceLog implements IQbInvoiceLog {

    constructor(id: number) {
        this.Id = id;
    }

    OrderId: number;
    CustomerId: number;
    CustomerName: string;
    IntegrationStatus: string;
    IntegrationDate: Date;
    BatchId: number;
    QuickBooksInvoiceId: string;
    QuickBooksTxnId: string;
    QuickBooksCustomerId: string;
    ErrorLog: string;

    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;

    Actions: string[];

}