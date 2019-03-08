import { IOrderItem } from "./IOrderItem";

export interface IOrderMaster extends DbEntityBase {
    ProductId: number;
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
    
}

export default class OrderMaster implements IOrderMaster {

    constructor(id: number) {
        this.Id = id;
    }

    IsNew: boolean;
    ProductId: number;
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

}