"use strict";
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
Object.defineProperty(exports, "__esModule", { value: true });
var React = require("react");
var MenuItem_1 = require("./MenuItem");
var MainMenu = /** @class */ (function (_super) {
    __extends(MainMenu, _super);
    function MainMenu() {
        return _super !== null && _super.apply(this, arguments) || this;
    }
    MainMenu.prototype.render = function () {
        return (React.createElement("div", { className: "container", style: { width: "800px", paddingTop: "50px" } },
            React.createElement("div", { className: "row" },
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Orders",
                        React.createElement(MenuItem_1.default, { imgName: "order_new.png", action: "NewOrder", caption: "New order", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem_1.default, { imgName: "icon-order.png", action: "MaestroList/Orders", caption: "Orders", itemType: "button", height: "70px", width: "100%" }))),
                React.createElement("div", { className: "col-6" },
                    React.createElement("div", { className: "plate" },
                        "Definitions",
                        React.createElement(MenuItem_1.default, { imgName: "cake.png", action: "MaestroList/Products", caption: "Products", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem_1.default, { imgName: "clients.png", action: "MaestroList/Customers", caption: "Customers", itemType: "button", height: "70px", width: "100%" }),
                        React.createElement(MenuItem_1.default, { imgName: "measure.png", action: "MaestroList/Units", caption: "Units", itemType: "button", height: "70px", width: "100%" }))))));
    };
    return MainMenu;
}(React.Component));
exports.default = MainMenu;
//# sourceMappingURL=MainMenu.js.map