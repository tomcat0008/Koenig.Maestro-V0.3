var MessagingProvider = /** @class */ (function () {
    function MessagingProvider(tranCode, actionType) {
        this.tranCode = tranCode;
        this.actionType = actionType;
    }
    MessagingProvider.prototype.sendMessage = function () {
        $.ajax({
            url: '@Url.Action("GetReport", "Report")',
            type: 'POST',
            dataType: 'json',
            data: {
                TransactionCode: this.tranCode, AgentInfo: 'agentinfo'
            },
            success: function (response) {
                if (response == null)
                    alert('null');
                //var url = URL.createObjectURL(response);
                /*
                var $a = $('<a />', {
                    'href': url,
                    'download': 'descarga.xlsx',
                    'text': "click"
                }).hide().appendTo("body")[0].click();*/
            },
            error: function (error) {
                alert(error);
            }
        });
    };
    return MessagingProvider;
}());
//# sourceMappingURL=messagingProvider.js.map