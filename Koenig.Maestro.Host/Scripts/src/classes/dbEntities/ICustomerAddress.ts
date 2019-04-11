import { DbEntityBase } from "./DbEntityBase";

export interface ICustomerAddress extends DbEntityBase {
    CustomerId: number;
    AddressType;
    AddressCode: string;
    QbName: string;
    QbListID: string;
    Line1: string;
    Line2: string;
    Line3: string;
    Line4: string;
    Line5: string;
    City: string;
    Province: string;
    PostalCode: string;
}


export default class CustomerAddress implements ICustomerAddress {



    constructor(id: number) {
        this.Id = id;
        this.IsNew = id == 0;
    }

    CustomerId: number;
    AddressType;
    AddressCode: string;
    QbName: string;
    QbListID: string;
    Line1: string;
    Line2: string;
    Line3: string;
    Line4: string;
    Line5: string;
    City: string;
    Province: string;
    PostalCode: string;

    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    IsNew: boolean;
    TypeName: string;
    Actions: string[];
}