import { DbEntityBase } from "./DbEntityBase";

export interface IMaestroProductGroup extends DbEntityBase {
    Name: string;
    Description: string;
}

export default class MaestroProductGroup implements IMaestroProductGroup {

    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    Name: string;
    Description: string;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;

}