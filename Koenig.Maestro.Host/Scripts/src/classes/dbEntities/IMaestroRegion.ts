import { DbEntityBase } from "./DbEntityBase";

export interface IMaestroRegion extends DbEntityBase {
    Name: string;
    Description: string;
    PostalCode: string;
    GreaterRegion: string;
}

export default class MaestroRegion implements IMaestroRegion {
    

    constructor(id: number) {
        this.Id = id;
        this.TypeName = "MaestroRegion";
        this.IsNew = id == 0;
    }

    IsNew: boolean;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;

    Name: string;
    Description: string;
    PostalCode: string;
    GreaterRegion: string;
    Actions: string[];

}