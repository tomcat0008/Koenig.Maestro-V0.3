var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from "react";
import ErrorInfo from "../../classes/ErrorInfo";
import EntityAgent from "../../classes/EntityAgent";
import { Row, Col, Form } from "react-bootstrap";
import DatePicker from "react-datepicker";
import AxiosAgent from "../../classes/AxiosAgent";
export default class ReportFilterComponent extends React.Component {
    constructor(props) {
        super(props);
        this.state = { Init: true, ErrorInfo: new ErrorInfo(), ReportCode: "", StartDate: new Date(), EndDate: new Date(), ReportDefinitions: null };
        this.state = { ReportCode: "", StartDate: new Date(), EndDate: new Date(), Init: true, ErrorInfo: new ErrorInfo(), ReportDefinitions: null };
    }
    Run() {
        return __awaiter(this, void 0, void 0, function* () {
            const getError = (msg) => {
                let result = new ErrorInfo();
                result.UserFriendlyMessage = msg;
                result.DisableAction = false;
                return result;
            };
            let reportCode = document.getElementById("reportTypeId").value;
            if (reportCode == "") {
                throw getError("Please select a report");
            }
            $("body").addClass("loading");
            let reportDefs = this.state.ReportDefinitions;
            let rdef = reportDefs.find(r => r.ReportCode == reportCode);
            let mde = { ["REPORT_CODE"]: rdef.ReportCode };
            let begin = this.state.StartDate;
            let end = this.state.EndDate;
            let dateString = [begin.getFullYear(), (begin.getMonth() + 1 > 9 ? '' : '0') + begin.getMonth() + 1, (begin.getDay() > 9 ? '' : '0') + begin.getDay()].join('');
            if (begin > end) {
                $("body").removeClass("loading");
                throw getError("Begin date can not be later than end date");
            }
            else if (begin == end) {
                end.setDate(begin.getDate() + 1);
            }
            else {
                let endDateString = [end.getFullYear(), (end.getMonth() + 1 > 9 ? '' : '0') + end.getMonth() + 1, (end.getDay() > 9 ? '' : '0') + end.getDay()].join('');
                dateString += "_" + endDateString;
            }
            mde["BEGIN_DATE"] = begin.toISOString();
            mde["END_DATE"] = end.toISOString();
            let ax = new AxiosAgent();
            let response = yield ax.runReport(rdef.TransactionCode, reportCode, mde);
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
        });
    }
    DisableEnable(disable) {
        document.getElementById("startDateId").disabled = disable;
        document.getElementById("endDateId").disabled = disable;
        document.getElementById("reportTypeId").disabled = disable;
    }
    componentDidMount() {
        return __awaiter(this, void 0, void 0, function* () {
            let ea = new EntityAgent();
            let cd = yield ea.GetReportFilterDisplay();
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
        });
    }
    render() {
        let reportDefs = this.state.ReportDefinitions;
        if (reportDefs != null) {
            reportDefs.sort((a, b) => { return a.Description.localeCompare(b.Description); });
            if (reportDefs.length > 1) {
                if (reportDefs.find(c => c.Id == -1) == undefined)
                    reportDefs.unshift(EntityAgent.GetFirstSelecItem("REPORT"));
            }
        }
        return (React.createElement("div", { className: "container" },
            React.createElement(Row, null,
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Report"),
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 6 },
                    React.createElement(Form.Control, { as: "select", id: "reportTypeId" }, reportDefs == null ? null : reportDefs.map(rd => React.createElement("option", { value: rd.ReportCode }, rd.Description))))),
            React.createElement(Row, null,
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "Start Date"),
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 },
                    React.createElement(Form.Control, { as: DatePicker, id: "startDateId", dateFormat: "dd/MM/yyyy", selected: this.state.StartDate, onChange: (dt) => { this.setState({ StartDate: dt }); } })),
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 }, "End Date"),
                React.createElement(Col, { style: { paddingTop: "5px" }, sm: 2 },
                    React.createElement(Form.Control, { as: DatePicker, id: "endDateId", dateFormat: "dd/MM/yyyy", selected: this.state.EndDate, onChange: (dt) => { this.setState({ EndDate: dt }); } })))));
    }
}
//# sourceMappingURL=ReportFilterComponent.js.map