import { DbEntityBase } from "./DbEntityBase";
import { IMaestroUnitType } from "./IMaestroUnitType";

export interface IMaestroProduct extends DbEntityBase {
    
    Name: string;
    Description: string;
    QuickBooksProductId: string;
    Price: number;
    MinimumOrderQuantity: number;
    MaestroUnitType: IMaestroUnitType;
    GroupId: number;
    UnitTypeName: string;
    UnitTypeId: number;
    UnitTypeCanHaveUnits: boolean;
}

export default class MaestroProduct implements IMaestroProduct
{
    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    IsNew: boolean;
    Name: string;    Description: string;
    QuickBooksProductId: string;
    Price: number;
    MinimumOrderQuantity: number;
    MaestroUnitType: IMaestroUnitType;
    GroupId: number;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    UnitTypeName: string;
    UnitTypeId: number;
    UnitTypeCanHaveUnits: boolean;
}