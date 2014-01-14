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

window.watchdog = (function($) {
    var popup_queue = [],
        is_init = false;

    function init(timeout_ms){
        if (true === is_init)
            return;

        is_init = true;

        serviceManager.bind(window.Teamlab.events.getMailAccounts, function(params, accounts) {
            checkAccounts(accounts);
        });

        setInterval(function() {
            serviceManager.getMailAccounts();
        }, timeout_ms*10);
    };

    function addPopup(header_text, body_text) {
        var body = "<div class=\"popup\"><p class=\"error\">";
        body += body_text;
        body += "</p>";
        body += "<div class=\"buttons\"><a class=\"button gray cancel\">" + window.MailScriptResource.CancelBtnLabel + "</a></div></div>";
        popup_queue.push({ header: header_text, body: body });
    }

    function checkAccounts(accounts) {
        var quota_error_accounts = [];
        var auth_error_accounts = [];

        $.each(accounts, function(index, value) {
            if (!value.enabled)
                return;
            if (value.quotaError)
                quota_error_accounts.push(value);
            if (value.authError)
                auth_error_accounts.push(value);
        });

        if (quota_error_accounts.length > 0) {
            addPopup(window.MailScriptResource.QuotaPopupHeader, window.MailScriptResource.QuotaPopupBody);
        }

        $.each(auth_error_accounts, function(index, value) {
            var body_text = window.MailScriptResource.AuthErrorPopupBody;
            body_text = body_text.replace('{0}', (value.name != "") ? value.name : value.address);
            body_text = body_text.replace('{1}', '<a href="' + TMMail.getFaqLink(value.address) + '">');
            body_text = body_text.replace('{2}', '</a>');
            addPopup(window.MailScriptResource.AuthErrorPopupHeader, body_text);
        });

        var item;
        while (item = popup_queue.pop())
            popup.addBig(item.header, item.body, item.onHide);
    }

    return {
        init: init
    };

})(jQuery);