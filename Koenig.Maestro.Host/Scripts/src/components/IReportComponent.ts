import { IResponseMessage } from "../classes/ResponseMessage";

export interface IReportComponent {
    Run(): Promise<void>;
}