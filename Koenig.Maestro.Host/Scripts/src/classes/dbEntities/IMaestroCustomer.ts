import { IMaestroRegion } from "./IMaestroRegion";
import { DbEntityBase } from "./DbEntityBase";

export interface IMaestroCustomer extends DbEntityBase {
    Name: string;
    Title: string;
    Address: string;
    Phone: string;
    Email: string;
    QuickBooksId: string;
    QuickBoosCompany: string;
    MaestroRegion: IMaestroRegion;
    RegionId: number;
    DefaultPaymentType: string;
}

export default class MaestroCustomer implements IMaestroCustomer {

    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    IsNew: boolean;
    Name: string;
    Title: string;
    Address: string;
    Phone: string;
    Email: string;
    QuickBooksId: string;
    QuickBoosCompany: string;
    MaestroRegion: IMaestroRegion;
    RegionId: number;
    DefaultPaymentType: string;
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;


}