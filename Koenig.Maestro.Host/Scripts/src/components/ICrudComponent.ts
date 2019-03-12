import { IResponseMessage } from "../classes/ResponseMessage";

export interface ICrudComponent {
    Save(): Promise<IResponseMessage>;
    Disable(): void;
}