import { DbEntityBase } from "./dbEntities/DbEntityBase";
import { IErrorInfo } from "./ErrorInfo";

export interface ITranComponentProp {
    Entity: DbEntityBase;
    ExceptionMethod: (error: IErrorInfo) => void;
    ButtonSetMethod: (actions:string[]) => void;
}