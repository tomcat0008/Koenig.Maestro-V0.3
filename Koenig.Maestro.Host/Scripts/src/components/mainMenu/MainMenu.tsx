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



export default class MainMenu extends React.Component<any, IModalContainerState>  {

    tranComponent: ICrudComponent;

    constructor() {
        super(null);

        
        let errorInfo: IErrorInfo = new ErrorInfo();
        errorInfo.StackTrace = "";
        errorInfo.UserFriendlyMessage = "";
        this.setState({
            ShowError: false, ErrorInfo: new ErrorInfo(), Action: "", TranCode: "",
            ShowSuccess: false, SuccessMessage: "", Init: true, Entity: null,
            ShowModal: false, ModalContent: null, ModalCaption: "",
            ResponseMessage: new ResponseMessage()
        });
        this.saveFct = this.saveFct.bind(this);

    }
    /*
    handleNew(tranCode:string) {

        let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
        this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity:entity });
    }

    private displayModal(caption: string, item: DbEntityBase, itemAction: string, tranCode:string) {

        this.setState({ ShowModal: true, ModalCaption: caption, Action: itemAction, TranCode:tranCode });
    }
    */
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
    }

    handleClick = async (id: MenuItem)=>{
        
        //this.setState({ loading: true })
        let req: ITranRequest = new TranRequest();
        req.ListTitle = id.props.caption;
        req.Action = id.props.action;
        req.TranCode = id.props.tranCode;
        req.MsgExtension = id.props.msgExtension;

        if (id.props.action == "New") {
            let tranCode: string = id.props.tranCode;
            let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
            await this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
        }

        if (id.props.action == "List")
        {
            $('#mainMenu').hide();
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
    

    render() {
        return (
            


                <div className="container" style={{ width: "800px", paddingTop: "50px" }}>
                    <div className="row">
                        <div className="col-6">
                            <div className="plate">Orders
                            <MenuItem imgName="order_new.png" action="New" tranCode="ORDER" eventHandler={this.handleClick}
                                    caption="New order" itemType="button" msgExtension={{}}
                                    height="70px" width="100%" />
                                <MenuItem imgName="icon-order.png" action="List" tranCode="ORDER" eventHandler={this.handleClick}
                                    caption="Orders" itemType="button" msgExtension={{ ['STATUS']: 'CR' }}
                                    height="70px" width="100%" />
                            </div>
                            <br />
                            <div className="plate">Imports
                            <MenuItem imgName="qb_customers.png" action="ImportQb" tranCode="CUSTOMER" eventHandler={this.handleClick}
                                    caption="Import Customers" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
                                    height="70px" width="100%" />

                                <MenuItem imgName="qb_products.png" action="ImportQb" tranCode="PRODUCT" eventHandler={this.handleClick}
                                    caption="Import Products" itemType="button" msgExtension={{ ["IMPORT_TYPE"]: '' }}
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