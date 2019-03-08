import * as React from 'react';
import * as ReactDOM from 'react-dom';
import MenuItem from './MenuItem';
import GridDisplay from './GridDisplay';
export default class MainMenu extends React.Component {
    constructor() {
        //state = { clickedOperation: MenuItem2, isInit:bool }
        super(...arguments);
        this.mainmenuRender = (React.createElement("div", { className: "container", style: { width: "800px", paddingTop: "50px" } },
            React.createElement("div", { className: "row" },
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Orders",
                        React.createElement(MenuItem, { imgName: "order_new.png", action: "New", tranCode: "ORDER", eventHandler: this.handleClick, caption: "New order", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "icon-order.png", action: "List", tranCode: "ORDER", eventHandler: this.handleClick, caption: "Orders", itemType: "button", height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Imports",
                        React.createElement(MenuItem, { imgName: "qb_customers.png", action: "Import", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Import Customers", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_products.png", action: "Import", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Import Products", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "qb_invoices.png", action: "Import", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Import Invoices", itemType: "button", height: "70px", width: "100%" })),
                    React.createElement("br", null),
                    React.createElement("div", { className: "plate" },
                        "Integration",
                        React.createElement(MenuItem, { imgName: "invoice_export.png", action: "Export", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Export Orders to Quickbooks", itemType: "button", height: "70px", width: "100%" }))),
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Definitions",
                        React.createElement(MenuItem, { imgName: "cake.png", action: "List", tranCode: "PRODUCT", eventHandler: this.handleClick, caption: "Products", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "clients.png", action: "List", tranCode: "CUSTOMER", eventHandler: this.handleClick, caption: "Customers", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "measure.png", action: "List", tranCode: "UNIT", eventHandler: this.handleClick, caption: "Units", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "units.png", action: "List", tranCode: "UNIT_TYPE", eventHandler: this.handleClick, caption: "Unit Types", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "map.png", action: "List", tranCode: "REGION", eventHandler: this.handleClick, caption: "Regions", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "CUSTOMER_PRODUCT_UNIT", eventHandler: this.handleClick, caption: "Customer Product Units", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem, { imgName: "cup.png", action: "List", tranCode: "QUICKBOOKS_INVOICE", eventHandler: this.handleClick, caption: "Invoices Logs", itemType: "button", height: "70px", width: "100%" }))))));
    }
    handleClick(id) {
        ReactDOM.render(React.createElement(GridDisplay, { listTitle: id.props.caption, action: id.props.action, tranCode: id.props.tranCode }), document.getElementById('content'));
        $('#mainMenu').hide();
    }
    render() {
        return this.mainmenuRender;
    }
}
//# sourceMappingURL=MainMenu.js.map