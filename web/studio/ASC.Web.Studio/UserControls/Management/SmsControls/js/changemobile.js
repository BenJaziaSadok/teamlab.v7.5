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
window.ASC.Controls.UserMobilePhoneManager = (function () {
    var openDialog = function () {
        jq("#changeMobileContent").show();
        jq("#changeMobileProgress, #changeMobileResult").hide();

        StudioBlockUIManager.blockUI("#studio_mobilePhoneChangeDialog", 400, 300);

        PopupKeyUpActionProvider.EnterAction = "jq(\"#changeMobileSend\").click();";
    };

    var sendNotify = function () {
        jq("#changeMobileContent").hide();
        jq("#changeMobileProgress").show();

        var userInfoId = jq("#hiddenUserInfoId").val();
        AjaxPro.ChangeMobileNumber.SendNotificationToChange(userInfoId, function (result) {
            if (result.error) {
                alert(result.error.Message);
                PopupKeyUpActionProvider.CloseDialog();
            } else {
                if (userInfoId) {
                    jq("#changeMobileProgress").hide();
                    jq("#changeMobileResult").show();
                }

                if (result.value.length) {
                    window.location.href = result.value;
                } else {
                    window.location.reload(true);
                }
            }
        });
    };

    return {
        openDialog: openDialog,
        sendNotify: sendNotify
    };
})();

(function () {
    jq(function () {
        jq("#changeMobileSend").on("click", function () {
            ASC.Controls.UserMobilePhoneManager.sendNotify();
        });
    });
})();