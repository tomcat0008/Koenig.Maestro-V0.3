import { IOrderItem } from "./IOrderItem";
import { DbEntityBase } from "./DbEntityBase";

export interface IOrderMaster extends DbEntityBase {
    CustomerId: number;
    OrderDate: Date;
    DeliveryDate: Date;
    PaymentType: string;
    Notes: string;
    OrderStatus: string;
    ItemCount: number;
    OrderItems: IOrderItem[];
    IntegrationStatus: string;
    CustomerName: string;
    CreateInvoiceOnQb: boolean;
}

export default class OrderMaster implements IOrderMaster {

    constructor(id: number) {
        this.Id = id;
        this.OrderDate = new Date();
        this.DeliveryDate = new Date();
        this.DeliveryDate.setDate(this.OrderDate.getDate() + 1);
    }

    IsNew: boolean;
    CustomerId: number;
    OrderDate: Date;
    DeliveryDate: Date;
    PaymentType: string;
    Notes: string;
    OrderStatus: string;
    ItemCount: number;
    OrderItems: IOrderItem[];
    IntegrationStatus: string;
    CustomerName: string;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    CreateInvoiceOnQb: boolean;
}