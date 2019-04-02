import { DbEntityBase } from "./DbEntityBase";

export interface IMaestroUnitType extends DbEntityBase {
    Name: string;
    Description: string;
    CanHaveUnits: boolean;



}

export default class MaestroUnitType implements IMaestroUnitType {
    constructor(id: number) {
        this.Id = id;
    }

    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;

    Name: string;
    Description: string;
    CanHaveUnits: boolean;

    Actions: string[];

}