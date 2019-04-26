import { DbEntityBase } from "./DbEntityBase";

export interface IMaestroReportDefinition extends DbEntityBase {
    ReportCode: string;
    Description: string;
    ProcedureName: string;
    MetaDefinition: string;
    FileName: string;
    ReportType: string;
    CodeBase: string;
    TransactionCode: string;
}

export class MaestroReportDefinition implements IMaestroReportDefinition {

    constructor(id: number) {
        this.Id = id;
        this.TypeName = "ReportDefinition";
    }

    ReportCode: string;
    Description: string;
    ProcedureName: string;
    MetaDefinition: string;
    FileName: string;
    ReportType: string;
    CodeBase: string;
    TransactionCode: string;
    Actions: string[];
    Id: number;
    CreatedUser: string;
    UpdatedUser: string;
    CreateDate: string;
    UpdateDate: string;
    RecordStatus: string;
    TypeName: string;
    IsNew: boolean;
}