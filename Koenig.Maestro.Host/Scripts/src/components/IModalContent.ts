import { DbEntityBase } from "../classes/dbEntities/DbEntityBase";

export interface IModalContent {
    TranCode: string,
    Entity: DbEntityBase,
    Show: boolean,
    Action: string,
    Caption:string,
    Close();
}