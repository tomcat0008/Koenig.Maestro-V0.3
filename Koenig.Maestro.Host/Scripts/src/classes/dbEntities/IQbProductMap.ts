import { DbEntityBase } from "./DbEntityBase";

export interface IQbProductMap extends DbEntityBase {
    ProductId: number;
    ProductName: string;
    Price: number;
    FullName: string;
    QuickBooksDescription: string;
    UnitId: number;
    UnitName: string;
    UnitTypeName: string;
    QuickBooksParentListId: string;
    QuickBooksParentCode: string;
    UnitTypeCanHaveUnits: boolean;
    ProductGroupId: number;
    QuickBooksCode: string;
}

export class QbProductMap implements IQbProductMap {
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;

    ProductId: number;
    ProductName: string;
    Price: number;
    FullName: string;
    QuickBooksDescription: string;
    UnitTypeCanHaveUnits: boolean;
    UnitId: number;
    UnitName: string;
    UnitTypeId: number;
    UnitTypeName: string;
    QuickBooksParentListId: string;
    QuickBooksParentCode: string;
    ProductGroupId: number;
    QuickBooksCode: string;

    Actions: string[];
}