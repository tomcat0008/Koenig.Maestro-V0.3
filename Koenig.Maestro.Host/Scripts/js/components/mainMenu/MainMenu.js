var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MenuItem from './MenuItem';
import GridDisplay from '../GridDisplay';
import TranRequest from '../../classes/ListRequest';
import TransactionDisplay from '../TransactionDisplay';
import ErrorInfo from '../../classes/ErrorInfo';
import ResponseMessage from '../../classes/ResponseMessage';
import EntityAgent from '../../classes/EntityAgent';
import ModalContainer from '../ModalConatiner';
import OrderProductItem from '../OrderProductItem';
export default class MainMenu extends React.Component {
    constructor() {
        super(null);
        /*
        handleNew(tranCode:string) {
    
            let entity: DbEntityBase = EntityAgent.FactoryCreate(tranCode);
            this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity:entity });
        }
    
        private displayModal(caption: string, item: DbEntityBase, itemAction: string, tranCode:string) {
    
            this.setState({ ShowModal: true, ModalCaption: caption, Action: itemAction, TranCode:tranCode });
        }
        */
        this.saveFct = () => __awaiter(this, void 0, void 0, function* () {
            try {
                let response = yield this.tranComponent.Save();
                this.setState({ ShowSuccess: true, SuccessMessage: response.ResultMessage });
                this.handleModalClose;
            }
            catch (error) {
                this.setState({ ErrorInfo: error, ShowError: true });
            }
        });
        this.handleModalClose = () => {
            this.setState({ ShowModal: false });
        };
        this.handleClick = (id) => __awaiter(this, void 0, void 0, function* () {
            //this.setState({ loading: true })
            let req = new TranRequest();
            req.ListTitle = id.props.caption;
            req.Action = id.props.action;
            req.TranCode = id.props.tranCode;
            req.MsgExtension = id.props.msgExtension;
            if (id.props.action == "New") {
                let tranCode = id.props.tranCode;
                let entity = EntityAgent.FactoryCreate(tranCode);
                yield this.setState({ ShowModal: true, ModalCaption: "New " + tranCode.toLowerCase(), Action: "New", TranCode: tranCode, Entity: entity });
            }
            if (id.props.action == "List") {
                $('#mainMenu').hide();
                $('#wait').show();
                ReactDOM.render(React.createElement(GridDisplay, Object.assign({}, req)), document.getElementById('content'));
                return;
            }
            if (id.props.action == "ImportQb") {
                $('#wait').show();
                ReactDOM.render(React.createElement(TransactionDisplay, Object.assign({}, req)), document.getElementById('content'));
                return;
            }
            //this.setState({ loading: false })
        });
        let errorInfo = new ErrorInfo();
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
    render() {
        console.debug(this.state);
        return (React.createElement("div", { className: "container", style: { width: "800px", paddingTop: "50px" } },
            React.createElement("div", { className: "row" },
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Orders",
                        React.createElement(MenuItem, { imgName: "order_new.png", action: "New", tranCode: "ORDER", eventHandler: this.handleClick, caption: "New order", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "icon-order.png", action: "List", tranCode: "ORDER", eventHandler: this.handleClick, caption: "Orders", itemType: "button", msgExtension: { ['STATUS']: 'CR' }, height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Imports",
                        React.createElement(MenuItem, { imgName: "qb_customers.png", action: "ImportQb", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Import Customers", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_products.png", action: "ImportQb", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Import Products", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_invoices.png", action: "ImportQb", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Import Invoices", itemType: "button", msgExtension: { ["IMPORT_TYPE"]: '' }, height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Integration",
                        React.createElement(MenuItem, { imgName: "invoice_export.png", action: "Export", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Export Orders to Quickbooks", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }))),
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Definitions",
                        React.createElement(MenuItem, { imgName: "cake.png", action: "List", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Products", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "clients.png", action: "List", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Customers", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "measure.png", action: "List", tranCode: "UNIT", eventHandler: this.handleClick, caption: "Units", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "units.png", action: "List", tranCode: "UNIT_TYPE", eventHandler: this.handleClick, caption: "Unit Types", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "map.png", action: "List", tranCode: "REGION", eventHandler: this.handleClick, caption: "Regions", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "CUSTOMER_PRODUCT_UNIT", eventHandler: this.handleClick, caption: "Customer Product Units", itemType: "button", msgExtension: {}, height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Invoices Logs", itemType: "button", msgExtension: {}, height: "70px", width: "100%" })))),
            React.createElement(ModalContainer, Object.assign({}, {
                TranCode: (this.state == null ? "" : this.state.TranCode),
                Action: (this.state == null ? "" : this.state.Action),
                Entity: (this.state == null ? null : this.state.Entity),
                Show: (this.state == null ? false : this.state.ShowModal),
                Caption: (this.state == null ? "" : "New " + this.state.TranCode.toLowerCase()),
                Close: this.handleModalClose
            })),
            React.createElement(OrderProductItem, null),
            React.createElement(OrderProductItem, null)));
    }
}
//# sourceMappingURL=MainMenu.js.map