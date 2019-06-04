import * as React from "react";
import { ListGroup, Image, CardDeck, Card, CardColumns, Alert, Modal, Row, Col, Button, CardGroup } from "react-bootstrap";
import Chart from 'react-google-charts';
import ResponseMessage, { IResponseMessage } from "../../classes/ResponseMessage";
import AxiosAgent from "../../classes/AxiosAgent";
import { IModalContainerState } from "../IModalContainerState";
import { ICrudComponent } from "../ICrudComponent";
import ErrorInfo, { IErrorInfo } from "../../classes/ErrorInfo";
import ModalContainer from "../ModalConatiner";
import { ITranRequest } from "../../classes/ITranRequest";
import TranRequest from "../../classes/ListRequest";
import TransactionDisplay from "../TransactionDisplay";
import * as ReactDOM from "react-dom";
import { DbEntityBase } from "../../classes/dbEntities/DbEntityBase";
import GridDisplay from "../GridDisplay";
import EntityAgent from "../../classes/EntityAgent";
import MenuItem from "../mainMenu/MenuItem";
import { IDashboardMenuItem } from "../../classes/DashboardMenuItem";

export default class Dashboard extends React.Component<any, IModalContainerState>  {

    tranComponent: ICrudComponent;

    state = {
        ShowError: false, ErrorInfo: new ErrorInfo(), Entity: null,
        ShowSuccess: false, SuccessMessage: "",
        ResponseMessage: new ResponseMessage(), TranCode: "", ConfirmText: "", ConfirmShow: false,
        Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
        MsgDataExtension: {}, selected: []

    };

    constructor(props) {
        super(props);

        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            ShowError: false, ErrorInfo: errorInfo, Entity: null,
            ShowSuccess: false, SuccessMessage: "",
            ResponseMessage: new ResponseMessage(), TranCode: "", ConfirmText: "", ConfirmShow: false,
            Init: true, ShowModal: false, ModalContent: null, ModalCaption: "", Action: "", ButtonAction: "",
            MsgDataExtension: {}, selected: []
        };

    }

    async loadDashboardData() {
        try {
            //$('#content').hide();
            $("body").addClass("loading");

            let msgExt: { [key: string]: string } = { ["LIST_CODE"]: 'DashboardSummary' };
            let response: IResponseMessage = await new AxiosAgent().getList("ORDER", msgExt);
            this.setState({ ResponseMessage: response, Init: false });
            if (response.TransactionStatus == "ERROR") {
                $("body").removeClass("loading");
                throw (response.ErrorInfo);
            }
        }
        catch (error) {
            this.setState({ ErrorInfo: error, ShowError: true });
            //console.error(error);
            
        }
        $("body").removeClass("loading");
    }

    handleModalClose = async () => {
        this.setState({ ShowModal: false });
        await this.loadDashboardData();
    }

    async componentDidMount() {
        await this.loadDashboardData();
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
            this.handleModalClose;
        }
        catch (error) {
            this.setState({ ErrorInfo: error, ShowError: true });
        }

    }

    handleClick = async (id: IDashboardMenuItem) => {

        //this.setState({ loading: true })
        let req: ITranRequest = new TranRequest();
        req.ListTitle = id.caption;
        req.Action = id.action;
        req.TranCode = id.tranCode;
        req.MsgExtension = id.msgExtension;
        req.ButtonList = id.buttonList;
        req.ListSelect = id.listSelect;
        let tranCode: string = id.tranCode;

        if (id.action == "New") {
            let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
            await this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
        }

        if (id.action == "List") {

            $('#mainMenu').hide();
            $("body").addClass("loading");
            ReactDOM.render(<GridDisplay {...req} />, document.getElementById('content'));

            return;
        }

        if (id.action == "Report") {
            await this.setState({ TranCode: id.tranCode, Action: id.action, ConfirmShow: false, ShowModal: true });
            return;
        }

        if (id.action == "Merge") {
            
            if (tranCode == "QUICKBOOKS_INVOICE") {
                let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
                await this.setState({ ShowModal: true, ModalCaption: "Merge " + tranCode.toLowerCase(), Action: "Merge", TranCode: tranCode, Entity: entity });
            }
            return;
        }

        if (id.action == "ImportQb") {
            switch (this.state.TranCode) {
                case "PRODUCT":
                    await this.setState({
                        ConfirmText: "Do you want to import products from QB ?"
                    });
                    break;
                case "CUSTOMER":
                    await this.setState({
                        ConfirmText: "Do you want to import customers from QB ?"
                    });
                    break;
                case "QUICKBOOKS_INVOICE":
                    await this.setState({
                        ConfirmText: "Do you want to import invoices from QB ?"
                    });
                    break;
            }
            await this.setState({ TranCode: id.tranCode, Action: id.action, ConfirmShow: true });

            return;
        }

    }


    onYes = () => {
        this.setState({ ConfirmShow: false });

        let req: ITranRequest = new TranRequest();
        req.ListTitle = "";
        req.Action = this.state.Action;
        req.TranCode = this.state.TranCode;
        req.MsgExtension = { ["IMPORT_TYPE"]: '' };
        req.ButtonList = [];
        req.ListSelect = false;


        $('#wait').show();
        ReactDOM.render(<TransactionDisplay {...req} />, document.getElementById('content'));

    }
    onNo = () => {

        this.setState({ ConfirmShow: false, ButtonAction: "", ConfirmText: "" });
    }

    prepareTimeLineData = () => {
        let orderHistory = this.state.ResponseMessage.TransactionResult["YTD_ORDER_TOTAL"];
        const rows = [];
        const columns = [
            { type: 'date', label: 'Date' },
            { type: 'number', label: 'Revenue' },
        ]

        for (let i = 0; i < orderHistory.length; i++) {
            let dtString: string = orderHistory[i]["ORDER_DATE"];
            let dt: Date = new Date(parseInt(dtString.substr(6, 4)), parseInt(dtString.substr(3, 2)) - 1, parseInt(dtString.substr(0, 2)));
            let total: number = parseFloat(orderHistory[i]["TOTAL"]);
            rows.push([dt, total]);
        }
        let chartData = [columns, ...rows];
        return chartData;
    }

    prepareMonthlySummary = () => {
        let data = this.state.ResponseMessage.TransactionResult["YEAR_MONTH_TOTAL"];
        const rows = [];
        const columns = [
            { type: 'string', label: 'Month' },
            { type: 'number', label: new Date().getFullYear()-1 },
            { type: 'number', label: new Date().getFullYear() }
        ];

        for (let i = 0; i < data.length; i++) {
            rows.push([
                data[i]["ORDER_MONTH"],
                parseFloat(data[i][new Date().getFullYear()-1]),
                parseFloat(data[i][new Date().getFullYear()])]);
        }
        let chartData = [columns, ...rows];
        //console.log(chartData);
        return chartData;
    }
    renderNextDayOrders() {
        let data = this.state.ResponseMessage.TransactionResult["ORDERS_NEXT_DAY"];
        let dt: Date = new Date();
        dt.setDate(dt.getDate() + 1);
        
        let tomorrow: string = "Orders for tomorrow <br/>" +dt.toLocaleDateString("en-GB") + " " + dt.toLocaleDateString("en-GB", { weekday: 'long' });
        let orderList: string = "<div class=\"listCap\">" + tomorrow + "</div>";

        let orderId: number = 0;
        let customerName: string = "";
        if (data.length > 0) {
            for (let i = 0; i < data.length; i++) {
                let id: number = parseInt(data[i]["ID"]);
                let quantity: number = parseInt(data[i]["QUANTITY"]);
                let product: string = data[i]["QB_DESCRIPTION"];
                if (orderId != id) {
                    orderId = id;
                    customerName = data[i]["QB_COMPANY"];
                    orderList += "<br/><p><b>" + customerName + " (" + id + ")</b></p>";
                    //orderList.push("<p><b>" + customerName + " (" + id + ")</b></p>")
                }
                orderList += "<li>" + quantity + " " + product + "</li>";
                //orderList.push("<li>" + quantity + " " + product + "</li>")

            }
        }
        else {
            orderList += "<br/><br/><div class=\"listCap\"><center><b>Oh no...no orders found !!!</b></center></div>";
        }

        return (
            <div className="dbInnerTable" dangerouslySetInnerHTML={{ __html: orderList}}>
            </div>        
        );
    }
        
    render() {

        if (!this.state.Init) {
            let dt: Date = new Date();
            let c = this.state.ResponseMessage.TransactionResult["YTD_CUSTOMER_TOTAL"];
            let p = this.state.ResponseMessage.TransactionResult["YTD_PRODUCT_TOTAL"];
            let orderHistory = this.prepareTimeLineData();
            let prepareMonthlySummary = this.prepareMonthlySummary();
            return (
                <div className="dashboard">
                    <div className="dbrdMenuCol">
                        <ListGroup>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "ORDER",
                                    action: "New",
                                    caption: "New order",
                                    msgExtension: {},
                                    buttonList: [],
                                    listSelect: false

                                });
                            }}>
                            <Image src="/Maestro/img/order_new.png" />New Order
                            </ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "ORDER",
                                    action: "List",
                                    caption: "Orders",
                                    msgExtension: { ['PERIOD']: 'Month' },
                                    buttonList: ["New", "Return", "Today", "Week", "Month", "Year", "All"],
                                    listSelect: false
                                });
                            }}>
                            <Image src="/Maestro/img/icon-order.png" />Orders</ListGroup.Item>
                        </ListGroup>
                        <br />
                        <ListGroup>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "REPORT",
                                    action: "Report",
                                    caption: "Reports",
                                    msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                    buttonList: ["Return"],
                                    listSelect: true
                                });
                            }}>
                            <Image src="/Maestro/img/report.png" />Reports</ListGroup.Item>

                            <ListGroup.Item action onClick={() => {
                                    this.handleClick({
                                        tranCode: "QUICKBOOKS_INVOICE",
                                        action: "List",
                                        caption: "Export Orders to Quickbooks",
                                        msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                        buttonList: ["Return", "Today", "Week", "Month", "Year", "All", "QB"],
                                        listSelect: true
                                    });
                                }}>

                            <Image src="/Maestro/img/invoice_export.png" />Export Invoices
                            </ListGroup.Item>

                                <ListGroup.Item action onClick={() => {
                                    this.handleClick({
                                        tranCode: "QUICKBOOKS_INVOICE",
                                        action: "Merge",
                                        caption: "Merge Invoices",
                                        msgExtension: { ["LIST_CODE"]: 'MERGE_INVOICE' },
                                        buttonList: ["Return"],
                                        listSelect: false
                                    });
                                }}>

                                <Image src="/Maestro/img/invoice_export.png" />Merge Invoices
                            </ListGroup.Item>

                        </ListGroup>
                        <br />
                        <ListGroup>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "PRODUCT",
                                    action: "List",
                                    caption: "Products",
                                    msgExtension: { },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}>
                                <Image src="/Maestro/img/cake.png" />Products
                            </ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "CUSTOMER",
                                    action: "List",
                                    caption: "Customers",
                                    msgExtension: {  },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/clients.png" />Customers</ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "CUSTOMER_PRODUCT_UNIT",
                                    action: "List",
                                    caption: "Customer Product Units",
                                    msgExtension: {  },
                                    buttonList: ["New","Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/cup.png" />Customer-Product-Unit</ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "UNIT_TYPE",
                                    action: "List",
                                    caption: "Unit Types",
                                    msgExtension: {  },
                                    buttonList: ["New","Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/units.png" />Unit Types</ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "UNIT",
                                    action: "List",
                                    caption: "Units",
                                    msgExtension: { },
                                    buttonList: ["New","Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/measure.png" />Units</ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "REGION",
                                    action: "List",
                                    caption: "Regions",
                                    msgExtension: { },
                                    buttonList: ["Region","Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/map.png" />Regions</ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "List",
                                    caption: "Invoices",
                                    msgExtension: {
                                        ['BEGIN_DATE']: new Date(dt.getFullYear(), dt.getMonth(), 1).toUTCString(),
                                        ['END_DATE']: dt.toUTCString()
                                    },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}><Image src="/Maestro/img/invoices.png" />Invoices</ListGroup.Item>

                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "QB_PRODUCT_MAP",
                                    action: "List",
                                    caption: "QB Product Maps",
                                    msgExtension: {  },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}>
                                <Image src="/Maestro/img/cake2.png" />Product Maps
                            </ListGroup.Item>

                        </ListGroup>
                        <br />
                        <ListGroup>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "CUSTOMER",
                                    action: "ImportQb",
                                    caption: "Import Customers",
                                    msgExtension: { ["IMPORT_TYPE"]: '' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}>
                                <Image src="/Maestro/img/qb_customers.png" />Import Customers
                            </ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "PRODUCT",
                                    action: "ImportQb",
                                    caption: "Import Products",
                                    msgExtension: { ["IMPORT_TYPE"]: '' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}>
                                <Image src="/Maestro/img/qb_products.png" />Import Products
                            </ListGroup.Item>
                            <ListGroup.Item action onClick={() => {
                                this.handleClick({
                                    tranCode: "QUICKBOOKS_INVOICE",
                                    action: "ImportQb",
                                    caption: "Import Invoices",
                                    msgExtension: { ["NOT_INTEGRATED"]: 'True' },
                                    buttonList: ["Return"],
                                    listSelect: false
                                });
                            }}>
                                <Image src="/Maestro/img/qb_invoices.png" />Import Invoices
                            </ListGroup.Item>

                        </ListGroup>

                    </div>
                    <div className="dbrdColumn">
                        <table cellPadding="6" cellSpacing="6" style={{ width: "100%", backgroundColor:"whitesmoke" }}>
                            <tr >
                                <td style={{ width: "75%" }}>
                                    <CardGroup>
                                    <Card bg="success" text="white">
                                        <Card.Title><Image src="/Maestro/img/clients.png" />Top 5 Customers (Revenue YTD)</Card.Title>
                                        <Card.Body>
                                            <div style={{ textAlign: "left", marginLeft: "10px" }}>
                                                {"1." + c[0]["CUSTOMER_NAME"] + " - " + c[0]["TOTAL"] + " $"}
                                                <br />{"2." + c[1]["CUSTOMER_NAME"] + " - " + c[1]["TOTAL"] + " $"}
                                                <br />{"3." + c[2]["CUSTOMER_NAME"] + " - " + c[2]["TOTAL"] + " $"}
                                                <br />{"4." + c[3]["CUSTOMER_NAME"] + " - " + c[3]["TOTAL"] + " $"}
                                                <br />{"5." + c[4]["CUSTOMER_NAME"] + " - " + c[4]["TOTAL"] + " $"}
                                            </div>
                                        </Card.Body>
                                    </Card>
                                    <Card bg="warning" text="dark">
                                        <Card.Title><Image src="/Maestro/img/cake.png" />Top 5 Products (Revenue YTD)</Card.Title>
                                        <Card.Body>
                                            <div style={{ textAlign: "left", marginLeft: "10px" }}>
                                                {"1." + p[0]["PRODUCT_NAME"] + " - " + p[0]["TOTAL"] + " $"}
                                                <br />{"2." + p[1]["PRODUCT_NAME"] + " - " + p[1]["TOTAL"] + " $"}
                                                <br />{"3." + p[2]["PRODUCT_NAME"] + " - " + p[2]["TOTAL"] + " $"}
                                                <br />{"4." + p[3]["PRODUCT_NAME"] + " - " + p[3]["TOTAL"] + " $"}
                                                <br />{"5." + p[4]["PRODUCT_NAME"] + " - " + p[4]["TOTAL"] + " $"}
                                            </div>
                                        </Card.Body>
                                        </Card>
                                    </CardGroup>
                                </td>
                                <td style={{ width: "25%" }} rowSpan={3}>{this.renderNextDayOrders() } </td>
                            </tr>
                            <tr><td align="center">
                                <div className="dbColDiv">
                                <Chart
                                    width={"100%"}
                                    height={180}
                                    chartType="Calendar"
                                    loader={<div>Loading Chart</div>}
                                    data={orderHistory}
                                    options={{
                                        title: 'YTD Daily Revenue',
                                        colorAxis: { colors: ['#ffffcc', '#33cc33'], maxValue: 10000 }
                                    }}
                                    />
                                </div>
                            </td><td></td></tr>
                            <tr><td align="center">
                                <div className="dbColDiv">
                                <Chart
                                    width={"100%"}
                                    height={'600px'}
                                    chartType="Line"
                                    loader={<div>Loading Chart</div>}
                                    data={prepareMonthlySummary}
                                    options={{
                                        chart: {
                                            title: 'Revenues in CAD'
                                        },
                                        colors: ["#ddf6b2","#3cce39"],
                                    }}
                                    />
                                </div>
                            </td><td>

                            </td></tr>
                        </table>
                    </div>
                    <div>
                    <Alert id="mmAlertId" dismissible show={this.state == null ? false : this.state.ShowError} variant="danger" data-dismiss="alert" >
                        <Alert.Heading id="mmAlertHeadingId">Exception occured</Alert.Heading>
                        <div className="errorStackTrace">
                            <p id="mmAlertUserFriendlyId">
                                {this.state == null ? "" : this.state.ErrorInfo.UserFriendlyMessage}
                            </p>
                        </div>
                        <hr />
                        <div className="errorStackTrace">
                            <p id="mmAlertStackTraceId">
                                {this.state == null ? "" : this.state.ErrorInfo.StackTrace}
                            </p>
                        </div>
                    </Alert>
                    <Alert variant="success" dismissible show={this.state == null ? false : this.state.ShowSuccess} data-dismiss="alert">
                        <p id="mmSuccess">{this.state == null ? "" : this.state.SuccessMessage}</p>
                    </Alert>
                    <Modal
                        size="sm"
                        centered
                        show={this.state.ConfirmShow}
                        aria-labelledby="example-modal-sizes-title-sm"
                        dialogClassName="modal-300p"
                    >
                        <Modal.Header><Modal.Title ></Modal.Title></Modal.Header>
                        <Modal.Body>
                            <Row><Col>{this.state.ConfirmText}</Col></Row>

                            <Row style={{ marginTop: "20px" }}>
                                <Col><Button variant="primary" id="btnYes" onClick={this.onYes} >Yes</Button></Col>
                                <Col><Button style={{ float: "right" }} variant="primary" id="btnNo" onClick={this.onNo}>No</Button></Col>
                            </Row>
                        </Modal.Body>
                    </Modal>

                    <ModalContainer  {
                        ...{
                            TranCode: (this.state == null ? "" : this.state.TranCode),
                            Action: (this.state == null ? "" : this.state.Action),
                            Entity: (this.state == null ? null : this.state.Entity),
                            Show: (this.state == null ? false : this.state.ShowModal),
                            Caption: (this.state == null ? "" : "New " + this.state.TranCode.toLowerCase()),
                            Close: this.handleModalClose
                            }} />
                    </div>

                </div>
            );
        }
        else {
            return (<div></div>);
        }
    }

}