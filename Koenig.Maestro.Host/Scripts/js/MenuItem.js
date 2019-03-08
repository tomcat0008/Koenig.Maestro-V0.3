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
var MenuItem = /** @class */ (function (_super) {
    __extends(MenuItem, _super);
    function MenuItem(props) {
        return _super.call(this, props) || this;
    }
    MenuItem.prototype.render = function () {
        var itemBody;
        if (this.props.itemType == "button") {
            var styleSet = { padding: '1px', width: "" + this.props.width, height: "" + this.props.height };
            itemBody = React.createElement("div", { style: styleSet },
                React.createElement("a", { href: this.props.action, title: this.props.caption, className: "btn btn-lg btn-primary", style: { width: '100%', height: '100%', } },
                    React.createElement("img", { src: "../../../img/" + this.props.imgName, alt: this.props.caption }),
                    this.props.caption,
                    " "));
        }
        else {
        }
        return itemBody;
    };
    return MenuItem;
}(React.Component));
exports.default = MenuItem;
//# sourceMappingURL=MenuItem.js.map