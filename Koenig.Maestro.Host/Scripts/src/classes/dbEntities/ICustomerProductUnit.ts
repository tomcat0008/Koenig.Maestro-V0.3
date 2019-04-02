import { DbEntityBase } from "./DbEntityBase";

export interface ICustomerProductUnit extends DbEntityBase {
    ProductId: number;
    ProductName: string;
    CustomerId: number;
    CustomerName: string;
    UnitId: number;
    UnitName: string;
    UnitTypeName: string;
    UnitTypeId: number;
}

export default class CustomerProductUnit implements ICustomerProductUnit
{
    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    ProductId: number;
    ProductName: string;
    CustomerId: number;
    CustomerName: string;
    UnitId: number;
    UnitName: string;
    UnitTypeName: string;
    UnitTypeId: number;

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