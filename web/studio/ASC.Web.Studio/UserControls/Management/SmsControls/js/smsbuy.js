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

;
window.ASC.Controls.SmsBuy = (function () {
    var isInit = false;
    var init = function () {
        if (isInit) {
            return;
        }

        jq("#smsBuy").click(showSmsBuyDialog);

        jq("[name='smsPackageOption']").on("change", selectSmsPackage);
    };

    var showSmsBuyDialog = function () {
        StudioBlockUIManager.blockUI("#smsBuyDialog", 350, 400);
        selectSmsPackage();
    };

    var selectSmsPackage = function () {
        var link = jq("[name='smsPackageOption']:checked").attr("data-quota-link");

        jq("#smsPay").attr("href", link);
    };
    
    return {
        init:init
    };
})();

(function () {
    jq(function () {
        ASC.Controls.SmsBuy.init();
    });
})();