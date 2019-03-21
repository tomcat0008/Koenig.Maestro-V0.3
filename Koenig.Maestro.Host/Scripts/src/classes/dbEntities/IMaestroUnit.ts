import { DbEntityBase } from "./DbEntityBase";
import { IMaestroUnitType } from "./IMaestroUnitType";

export interface IMaestroUnit extends DbEntityBase {

    Name: string;
    UnitTypeName: string;
    UnitTypeId: number;
    QuickBooksUnit: string;


}

export default class MaestroUnit implements IMaestroUnit {

    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    Name: string;    UnitTypeName: string;
    UnitTypeId: number;
    QuickBooksUnit: string;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;
    UnitType: IMaestroUnitType;


}