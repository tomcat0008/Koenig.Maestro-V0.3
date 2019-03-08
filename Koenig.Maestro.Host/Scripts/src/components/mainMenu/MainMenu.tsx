import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MenuItem from './MenuItem';

import GridDisplay from '../GridDisplay';
import TranRequest from '../../classes/ListRequest';

import { ITranRequest } from '../../classes/ITranRequest';
import TransactionDisplay from '../TransactionDisplay';
import NewItemDisplay from '../NewItemDisplay';
import { Container, Row, Col, Modal, Button, Alert } from 'react-bootstrap';
import { IModalContainerState } from '../IModalContainerState';
import ErrorInfo, { IErrorInfo } from '../../classes/ErrorInfo';
import ResponseMessage, { IResponseMessage } from '../../classes/ResponseMessage';
import EntityAgent from '../../classes/EntityAgent';
import MaestroCustomerComponent from '../transaction/MaestroCustomerComponent';
import OrderMaster from '../../classes/dbEntities/IOrderMaster';
import MaestroCustomer from '../../classes/dbEntities/IMaestroCustomer';
import OrderComponent from '../transaction/OrderComponent';
import { ICrudComponent } from '../ICrudComponent';


export default class MainMenu extends React.Component<any, IModalContainerState>  {

    tranComponent: ICrudComponent;

    constructor() {
        super(null);

        this.handleClose = this.handleClose.bind(this);
        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.state = {
            showError: false, errorInfo: errorInfo,
            showSuccess: false, successMessage: "",
            showModal: false, modalContent: null, modalCaption: "",
            responseMessage: new ResponseMessage()
        };
        this.saveFct = this.saveFct.bind(this);
        this.handleNew = this.handleNew.bind(this);

    }

    handleNew(tranCode:string) {

        let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
        this.displayModal("New " + tranCode.toLowerCase(), entity, "New");
    }

    private displayModal(caption: string, item: DbEntityBase, itemAction: string) {


        let data;
        if (this.props.tranCode == "CUSTOMER")
            data = <MaestroCustomerComponent ref={(comp) => this.tranComponent = comp} {...item as MaestroCustomer} />
        else if (this.props.tranCode == "ORDER")
            data = <OrderComponent ref={(comp) => this.tranComponent = comp} {...item as OrderMaster} />


        this.setState({ modalContent: data, showModal: true, modalCaption: caption });
    }

    saveFct = async () => {
        try {
            let response: IResponseMessage = await this.tranComponent.Save();
            this.setState({ showSuccess: true, successMessage: response.ResultMessage });
            this.handleClose();
        }
        catch (error) {
            this.setState({ errorInfo: error, showError: true });
        }

    }

    handleClose() {
        this.setState({ showModal: false });
    }

    handleClick(id: MenuItem) {

        //this.setState({ loading: true })
        let req: ITranRequest = new TranRequest();
        req.listTitle = id.props.caption;
        req.action = id.props.action;
        req.tranCode = id.props.tranCode;
        req.msgExtension = id.props.msgExtension;
        $('#mainMenu').hide();

        if (id.props.action == "New") {
            ReactDOM.render(<NewItemDisplay {...req} />, document.getElementById('content'));
        }

        if (id.props.action == "List")
        {
            $('#wait').show();
            ReactDOM.render(<GridDisplay {...req} />, document.getElementById('content'));
            
            return;
        }

        if (id.props.action == "ImportQb") {
            $('#wait').show();
            ReactDOM.render(<TransactionDisplay {...req} />, document.getElementById('content'));
            
            return;
        }
        //this.setState({ loading: false })
    }
    

    mainmenuRender = 
        (
            

            <div className="container" style={{ width: "800px", paddingTop: "50px" }}>
                <div className="row">
                    <div className="col-6">
                        <div className="plate">Orders
                            <MenuItem imgName="order_new.png" action="New" tranCode="ORDER" eventHandler={this.handleClick}
                                caption="New order" itemType="button" msgExtension={{} }
                                height="70px" width="100%" />
                            <MenuItem imgName="icon-order.png" action="List" tranCode="ORDER" eventHandler={this.handleClick}
                                caption="Orders" itemType="button" msgExtension={{['STATUS']:'CR' }}
                                height="70px" width="100%" />
                        </div>
                        <br/>
                        <div className="plate">Imports
                            <MenuItem imgName="qb_customers.png" action="ImportQb" tranCode="CUSTOMER" eventHandler={this.handleClick}
                                caption="Import Customers" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
                                height="70px" width="100%" />

                            <MenuItem imgName="qb_products.png" action="ImportQb" tranCode="PRODUCT" eventHandler={this.handleClick}
                                caption="Import Products" itemType="button" msgExtension={{ ["IMPORT_TYPE"]:'' }}
                                height="70px" width="100%" />
                            <MenuItem imgName="qb_invoices.png" action="ImportQb" tranCode="QUICKBOOKS_INVOICE" eventHandler={this.handleClick}
                                caption="Import Invoices" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
                                height="70px" width="100%" />
                        </div>
                        <br />
                        <div className="plate">Integration
                            <MenuItem imgName="invoice_export.png" action="Export" tranCode="QUICKBOOKS_INVOICE" eventHandler={this.handleClick}
                                caption="Export Orders to Quickbooks" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                        </div>
                    </div>
                
                    <div className="col-6">
                        <div className="plate">Definitions
                            
                            <MenuItem imgName="cake.png" action="List" tranCode="PRODUCT" eventHandler={this.handleClick}
                                caption="Products" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="clients.png" action="List" tranCode="CUSTOMER" eventHandler={this.handleClick}
                                caption="Customers" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="measure.png" action="List" tranCode="UNIT" eventHandler={this.handleClick}
                                caption="Units" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="units.png" action="List" tranCode="UNIT_TYPE" eventHandler={this.handleClick}
                                caption="Unit Types" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="map.png" action="List" tranCode="REGION" eventHandler={this.handleClick}
                                caption="Regions" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="cup.png" action="List" tranCode="CUSTOMER_PRODUCT_UNIT" eventHandler={this.handleClick}
                                caption="Customer Product Units" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                            <MenuItem imgName="cup.png" action="List" tranCode="QUICKBOOKS_INVOICE" eventHandler={this.handleClick}
                                caption="Invoices Logs" itemType="button" msgExtension={{}}
                                height="70px" width="100%" />
                        </div>
                    </div>
                </div>

                <Modal show={this.state.showModal} dialogClassName="modal-90w" onHide={this.handleClose} centered size="lg">
                    <Modal.Header closeButton>
                        <Modal.Title>{this.state.modalCaption}</Modal.Title>
                    </Modal.Header>
                    <Modal.Body>
                        <Alert id="modalAlertId" dismissible show={this.state.showError} variant="danger" data-dismiss="alert" >
                            <Alert.Heading id="modalAlertHeadingId">Exception occured</Alert.Heading>
                            <p id="modalAlertUserFriendlyId">
                                {this.state.errorInfo.UserFriendlyMessage}
                            </p>
                            <hr />
                            <p id="modalAlertStackTraceId">
                                {this.state.errorInfo.StackTrace}
                            </p>
                        </Alert>
                        <div id="modalRender">{this.state.modalContent}</div>
                    </Modal.Body>

                    <Modal.Footer>
                        <Button variant="secondary" onClick={() => this.setState({ showModal: false })} >Close</Button>
                        <Button variant="primary" onClick={() => this.saveFct()} >Save changes</Button>
                    </Modal.Footer>
                </Modal>

            </div>

        );
    

    render() {
        return this.mainmenuRender;

    }


}