import { IResponseMessage } from "../classes/ResponseMessage";

export interface ICrudComponent {
    Save(): Promise<IResponseMessage>;
    Cancel(): Promise<IResponseMessage>;
    Integrate(): Promise<IResponseMessage>;
    DisableEnable(disable: boolean): void;
}