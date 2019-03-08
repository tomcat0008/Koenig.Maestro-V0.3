"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var axios_1 = require("axios");
var NaviAgent = /** @class */ (function () {
    function NaviAgent() {
    }
    NaviAgent.prototype.navigateList = function (listName, caption) {
        $("#content").text(caption);
        axios_1.default.post("/MainPage/List", { requestMessage: listName }).then(function (response) { alert(response); })
            .catch(function (error) { alert(error); });
    };
    NaviAgent.prototype.getMessage = function () {
        var message = {};
    };
    return NaviAgent;
}());
exports.default = NaviAgent;
//# sourceMappingURL=NaviAgent.js.map