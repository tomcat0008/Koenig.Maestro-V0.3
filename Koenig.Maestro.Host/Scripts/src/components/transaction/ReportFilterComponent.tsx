import * as React from "react";
import ErrorInfo, { IErrorInfo } from "../../classes/ErrorInfo";
import EntityAgent, { IReportFilterDisplay } from "../../classes/EntityAgent";
import { IReportComponent } from "../IReportComponent";
import { IResponseMessage } from "../../classes/ResponseMessage";
import { Row, Col, Form } from "react-bootstrap";
import DatePicker from "react-datepicker";
import { ITranComponentProp } from "../../classes/ITranComponentProp";
import { IMaestroReportDefinition } from "../../classes/dbEntities/IReportDefinition";
import AxiosAgent from "../../classes/AxiosAgent";

export default class ReportFilterComponent extends React.Component<ITranComponentProp, IReportFilterDisplay> implements IReportComponent {

    state = { Init: true, ErrorInfo: new ErrorInfo(), ReportCode: "", StartDate:new Date(), EndDate:new Date(), ReportDefinitions:null }

    constructor(props: ITranComponentProp) {
        super(props);
        this.state = { ReportCode:"", StartDate:new Date(), EndDate:new Date(), Init: true, ErrorInfo: new ErrorInfo(), ReportDefinitions:null }
    }


    async Run(): Promise<void> {

        const getError = (msg: string): IErrorInfo => {
            let result: ErrorInfo = new ErrorInfo();
            result.UserFriendlyMessage = msg;
            result.DisableAction = false;
            return result;
        };

        let reportCode: string = (document.getElementById("reportTypeId") as HTMLSelectElement).value;

        if (reportCode == "") {
            throw getError("Please select a report");
        }


        $("body").addClass("loading");
        

        let reportDefs: IMaestroReportDefinition[] = this.state.ReportDefinitions;
        let rdef:IMaestroReportDefinition = reportDefs.find(r => r.ReportCode == reportCode);

        let mde: { [key: string]: string } = { ["REPORT_CODE"]: rdef.ReportCode };

        let begin: Date = this.state.StartDate;
        let end: Date = this.state.EndDate;
        
        let dateString:string = [begin.getFullYear(), (begin.getMonth() + 1 > 9 ? '' : '0') + begin.getMonth() + 1, (begin.getDay() > 9 ? '' : '0') + begin.getDay()].join('');

        if (begin > end) {
            $("body").removeClass("loading");
            throw getError("Begin date can not be later than end date");
        } else if (begin == end) {
            end.setDate(begin.getDate() + 1);

        } else {
            let endDateString: string = [end.getFullYear(), (end.getMonth() + 1 > 9 ? '' : '0') + end.getMonth() + 1, (end.getDay() > 9 ? '' : '0') + end.getDay()].join('');
            dateString += "_" + endDateString; 
        }

        mde["BEGIN_DATE"] = begin.toISOString();
        mde["END_DATE"] = end.toISOString();

        let ax: AxiosAgent = new AxiosAgent();
        let response: IResponseMessage = await ax.runReport(rdef.TransactionCode, reportCode, mde);

        var base64 = response.TransactionResult;
        var slicesize = 512;
        var bytechars = atob(base64);
        var bytearrays = [];
        for (var offset = 0; offset < bytechars.length; offset += slicesize) {
            var slice = bytechars.slice(offset, offset + slicesize);
            var bytenums = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                bytenums[i] = slice.charCodeAt(i);
            }
            var bytearray = new Uint8Array(bytenums);
            bytearrays[bytearrays.length] = bytearray;
        }
        
        $("body").removeClass("loading");
        var a = window.document.createElement('a');
        a.href = window.URL.createObjectURL(new Blob(bytearrays, { type: 'application/zip' }));

        let fileName = rdef.FileName.replace("{0}", dateString) + ".zip";

        a.download = fileName;
        a.click();
        
        
    }


    DisableEnable(disable: boolean): void {
        (document.getElementById("startDateId") as HTMLInputElement).disabled = disable;
        (document.getElementById("endDateId") as HTMLInputElement).disabled = disable;
        (document.getElementById("reportTypeId") as HTMLSelectElement).disabled = disable;
    }

    async componentDidMount() {

        let ea: EntityAgent = new EntityAgent();

        let cd: IReportFilterDisplay = await ea.GetReportFilterDisplay();
        if (cd.ErrorInfo != null) {
            if (cd.ErrorInfo.UserFriendlyMessage != "") {
                this.props.ExceptionMethod(cd.ErrorInfo);
            }
            else {
                cd.Init = false;
                this.setState(cd);
            }
        }
        else {
            cd.Init = false;
            this.setState(cd);
        }

        this.props.ButtonSetMethod(["Run"]);
    }

    render() {

        let reportDefs: IMaestroReportDefinition[] = this.state.ReportDefinitions;
        if (reportDefs != null) {
            reportDefs.sort((a, b) => { return a.Description.localeCompare(b.Description); });

            if (reportDefs.length > 1) {
                if (reportDefs.find(c => c.Id == -1) == undefined)
                    reportDefs.unshift(EntityAgent.GetFirstSelecItem("REPORT") as IMaestroReportDefinition);
            }
        }

        return (
            
            <div className="container">
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Report</Col>
                        <Col style={{ paddingTop: "5px" }} sm={6}>
                            <Form.Control as="select" id="reportTypeId">
                                {
                                    reportDefs == null ? null : reportDefs.map(rd => <option value={rd.ReportCode}>{rd.Description}</option>)
                                }

                            </Form.Control>
                        </Col>
                    </Row>
                    <Row>
                        <Col style={{ paddingTop: "5px" }} sm={2}>Start Date</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                        <Form.Control as={DatePicker} id="startDateId"
                            dateFormat="dd/MM/yyyy"
                                selected={this.state.StartDate}
                                onChange={(dt) => { this.setState({ StartDate: dt }) }} />
                        </Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>End Date</Col>
                        <Col style={{ paddingTop: "5px" }} sm={2}>
                        <Form.Control as={DatePicker} id="endDateId"
                            dateFormat="dd/MM/yyyy"
                                selected={this.state.EndDate}
                                onChange={(dt) => { this.setState({ EndDate: dt }) }} />
                        </Col>
                    </Row>
            </div>
            );

    }

}