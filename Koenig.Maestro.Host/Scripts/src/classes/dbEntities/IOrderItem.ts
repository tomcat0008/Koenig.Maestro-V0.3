import { DbEntityBase } from "./DbEntityBase";

export interface IOrderItem extends DbEntityBase {
    OrderId: number;
    ProductId: number;
    ProductName: string;
    Quantity: number;
    UnitId: number;
    UnitName: string;
    ItemPrice: number;

}

export class OrderItem implements IOrderItem {
    

    constructor(orderId: number, id:number) {
        this.OrderId = orderId;
        this.Id = id;
        this.IsNew = id == 0;
    }
    IsNew: boolean;
    OrderId: number;    ProductId: number;
    ProductName: string;
    Quantity: number;
    UnitId: number;
    UnitName: string;
    ItemPrice: number;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;


}