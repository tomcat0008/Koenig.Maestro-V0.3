import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MenuItem from './MenuItem';

import GridDisplay from '../GridDisplay';
import TranRequest from '../../classes/ListRequest';

import { ITranRequest } from '../../classes/ITranRequest';
import TransactionDisplay from '../TransactionDisplay';

import { IModalContainerState } from '../IModalContainerState';
import ErrorInfo, { IErrorInfo } from '../../classes/ErrorInfo';
import ResponseMessage, { IResponseMessage } from '../../classes/ResponseMessage';
import EntityAgent from '../../classes/EntityAgent';
import { ICrudComponent } from '../ICrudComponent';
import ModalContainer from '../ModalConatiner';
import { DbEntityBase } from '../../classes/dbEntities/DbEntityBase';
import { Alert, Modal, Row, Col, Button } from 'react-bootstrap';



export default class MainMenu extends React.Component<any, IModalContainerState>  {

    tranComponent: ICrudComponent;

    constructor() {
        super(null);
        this.state = {
            ShowError: false, ErrorInfo: new ErrorInfo(), Action: "", TranCode: "",
            ShowSuccess: false, SuccessMessage: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "", selected: [],
            ResponseMessage: new ResponseMessage(), ConfirmText: "", ConfirmShow: false, ButtonAction: "",
            MsgDataExtension: {}
        }
        this.saveFct = this.saveFct.bind(this);

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

    handleModalClose= ()=> {
        this.setState({ ShowModal: false });
        //$('#mainMenu').show();
        //$('#wait').hide();
        $("body").removeClass("loading");
    }

    handleClick = async (id: MenuItem)=>{
        
        //this.setState({ loading: true })
        let req: ITranRequest = new TranRequest();
        req.ListTitle = id.props.caption;
        req.Action = id.props.action;
        req.TranCode = id.props.tranCode;
        req.MsgExtension = id.props.msgExtension;
        req.ButtonList = id.props.buttonList;
        req.ListSelect = id.props.listSelect;

        if (id.props.action == "New") {
            let tranCode: string = id.props.tranCode;
            let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
            await this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
        }

        if (id.props.action == "List")
        {

            $('#mainMenu').hide();
            $("body").addClass("loading");
            ReactDOM.render(<GridDisplay {...req} />, document.getElementById('content'));
            
            return;
        }
        
        if (id.props.action == "ImportQb") {
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
            await this.setState({ TranCode: id.props.tranCode, Action: id.props.action, ConfirmShow: true });
            
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
    

    render() {
        let dt: Date = new Date();

        return (
            


                <div className="container" style={{ width: "800px", paddingTop: "50px" }}>
                    <div className="row">
                        <div className="col-6">
                            <div className="plate">Orders
                                <MenuItem imgName="order_new.png" action="New" tranCode="ORDER" eventHandler={this.handleClick}
                                caption="New order" itemType="button" msgExtension={{}} buttonList={[]} listSelect={false}
                                    height="70px" width="100%" />
                            <MenuItem imgName="icon-order.png" action="List" tranCode="ORDER" eventHandler={this.handleClick}
                                caption="Orders" itemType="button" msgExtension={{ ['PERIOD']: 'Month' }} height="70px" width="100%"
                                buttonList={["New", "Return","Today", "Week", "Month", "Year", "All" ]} listSelect={false}
                                />
                            </div>
                            <br />
                            <div className="plate">Imports
                            <MenuItem imgName="qb_customers.png" action="ImportQb" tranCode="CUSTOMER"
                                eventHandler={this.handleClick }
                                caption="Import Customers" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
                                buttonList={["Return"]} height="70px" width="100%" listSelect={false}/>

                            <MenuItem imgName="qb_products.png" action="ImportQb" tranCode="PRODUCT"
                                    eventHandler={ this.handleClick}
                                    caption="Import Products" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
                                buttonList={["Return"]} height="70px" width="100%" listSelect={false}/>
                            <MenuItem imgName="qb_invoices.png" action="ImportQb" tranCode="QUICKBOOKS_INVOICE"
                                eventHandler={this.handleClick}
                                    caption="Import Invoices" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }} buttonList={["Return"]}
                                height="70px" width="100%" listSelect={false}/>
                            </div>
                            <br />
                            <div className="plate">Integration
                            <MenuItem imgName="invoice_export.png" action="List" tranCode="QUICKBOOKS_INVOICE" eventHandler={this.handleClick}
                                caption="Export Orders to Quickbooks" itemType="button" msgExtension={{ ["NOT_INTEGRATED"]: 'True' }}
                                buttonList={["Return", "Today", "Week", "Month", "Year", "All", "QB"]}
                                height="70px" width="100%" listSelect={true}/>
                            </div>
                        </div>

                        <div className="col-6">
                            <div className="plate">Definitions
                               
                            <MenuItem imgName="cake.png" action="List" tranCode="PRODUCT" eventHandler={this.handleClick}
                                caption="Products" itemType="button" msgExtension={{}} buttonList={["Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="clients.png" action="List" tranCode="CUSTOMER" eventHandler={this.handleClick}
                                    caption="Customers" itemType="button" msgExtension={{}} buttonList={["Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="measure.png" action="List" tranCode="UNIT" eventHandler={this.handleClick}
                                    caption="Units" itemType="button" msgExtension={{}} buttonList={["New","Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="units.png" action="List" tranCode="UNIT_TYPE" eventHandler={this.handleClick}
                                    caption="Unit Types" itemType="button" msgExtension={{}} buttonList={["New", "Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="map.png" action="List" tranCode="REGION" eventHandler={this.handleClick}
                                    caption="Regions" itemType="button" msgExtension={{}} buttonList={["New", "Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="cup.png" action="List" tranCode="CUSTOMER_PRODUCT_UNIT" eventHandler={this.handleClick}
                                    caption="Customer Product Units" itemType="button" msgExtension={{}} buttonList={["New", "Return"]}
                                height="70px" width="100%" listSelect={false}/>
                                <MenuItem imgName="cup.png" action="List" tranCode="QUICKBOOKS_INVOICE" eventHandler={this.handleClick}
                                    caption="Invoices Logs" itemType="button" buttonList={["Return"]} listSelect={false}
                                    msgExtension={{
                                    ['BEGIN_DATE']: new Date(dt.getFullYear(), dt.getMonth(), 1).toUTCString(),
                                        ['END_DATE']: dt.toUTCString()

                                }}
                                    height="70px" width="100%" />
                            </div>
                        </div>

                    </div>
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
                            Close:this.handleModalClose 
                        }} />

            </div>
            );

    }


}