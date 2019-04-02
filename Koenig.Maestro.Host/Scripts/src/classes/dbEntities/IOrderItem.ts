import { DbEntityBase } from "./DbEntityBase";

export interface IOrderItem extends DbEntityBase {
    OrderId: number;
    ProductId: number;
    ProductName: string;
    Quantity: number;
    UnitId: number;
    UnitName: string;
    ItemPrice: number;
    MapId: number;
}

export class OrderItem implements IOrderItem {
    

    constructor(orderId: number, id:number) {
        this.OrderId = orderId;
        this.Id = id;
        this.TypeName = "OrderItem";
        this.ItemPrice = 0;
        this.Quantity = 0;
        this.MapId = 0;
        this.UnitId = 0;
        this.ProductId = 0;
    }

    IsNew: boolean;
    OrderId: number;
    ProductId: number;
    ProductName: string;
    Quantity: number;
    UnitId: number;
    UnitName: string;
    ItemPrice: number;
    MapId: number;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;

    Actions: string[];

}