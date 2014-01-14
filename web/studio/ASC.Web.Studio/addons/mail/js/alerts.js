/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

window.mailAlerts = (function($) {
    var _lock = false;
    var _repeat = true;
    var _alerts = [];

    var init = function() {
        _init_flag = true;
        _check();
    };

    var _unlock = function() {
        _lock = false;
        if (_repeat)
            setTimeout(function() {
                _repeat = true;
                _check();
            }, 180000);
    };

    var check = function() {
        _repeat = false;
        _check();
    };

    var _check = function() {
        if (true === _lock)
            return;
        _lock = true;
        serviceManager.getAlerts({}, { success: _onGetAlerts });
    };

    var _onGetAlerts = function(options, alerts) {
        $.each(alerts, function(index, value) {
            _storeAlert(value);
        });
        if (0 == _alerts.length) {
            _unlock();
            return;
        }
        $.each(_alerts, function(index, value) {
            if (_alerts.length - 1 != index) {
                popup.addBig(value.header, value.body, function() {
                    _deleteAlert(value.id);
                });
            } else {
                popup.addBig(value.header, value.body, function() {
                    _deleteAlert(value.id);
                    _unlock();
                });
            }
        });
        _alerts = [];
    };

    var _storeAlert = function(alert) {
        var header, body, data = $.parseJSON(alert.data);
        switch (data.type) {
            case 1:
                header = MailScriptResource.DeliveryFailurePopupHeader;
                body = "<div class=\"popup\"><div class=\"error\"><p>";
                body += MailScriptResource.DeliveryFailurePopupBody
                    .replace(/{separator}/g, "</p><p>")
                    .replace(/{subject}/g, data.subject)
                    .replace(/{account_name}/g, data.from)
                    .replace(/{faq_link_open_tag}/g, "<a target=\"blank\" href=\"" + TMMail.getFaqLink(data.from) + "\">")
                    .replace(/{faq_link_close_tag}/g, "</a>");
                body += "</p></div></div>";
                body += "<div class=\"buttons\">";
                body += "<a href=\"#draftitem/" + data.message_id + "/\" class=\"button blue tryagain\">" + MailScriptResource.TryAgainButton + "</a>";
                body += "<a class=\"button gray cancel\">" + MailScriptResource.CancelBtnLabel + "</a></div></div>";
                body = $(body);
                body.find('.tryagain').click(function() { popup.hide(); });
                break;
            case 2:
                header = MailScriptResource.LinkFailurePopupHeader;
                body = "<div class=\"popup\"><div class=\"error\"><p>";
                body += MailScriptResource.LinkFailurePopupName;
                body += "</p><p>"
                body += MailScriptResource.LinkFailurePopupText;
                body += "</p></div>"
                body += "<div class=\"buttons\">";
                body += "<a class=\"button blue cancel\">" + MailScriptResource.OkBtnLabel + "</a></div></div>";
                break;
            case 3:
                header = MailScriptResource.ExportFailurePopupHeader;
                body = "<div class=\"popup\"><div class=\"error\"><p>";
                body += MailScriptResource.ExportFailurePopupName;
                body += "</p><p>"
                body += MailScriptResource.ExportFailurePopupText;
                body += "</p></div>"
                body += "<div class=\"buttons\">";
                body += "<a class=\"button blue cancel\">" + MailScriptResource.OkBtnLabel + "</a></div></div>";
                break;
        };
        if (header && body)
            _alerts.push({ header: header, body: body, id: alert.id });
    };

    var _handleAlert = function(alert) {
        if (header && body)
            popup.addBig(header, body, function() { _deleteAlert(alert.id); });
    };

    var _deleteAlert = function(id) {
        serviceManager.deleteAlert(id);
    };

    return {
        init: init,
        check: check
    };
})(jQuery);