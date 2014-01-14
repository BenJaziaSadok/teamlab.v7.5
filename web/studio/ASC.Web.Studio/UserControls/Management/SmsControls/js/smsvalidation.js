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
window.ASC.Controls.SmsValidationSettings = (function () {
    return {
        TimeoutHandler: null,

        SaveSmsValidationSettings: function () {
            if (ASC.Controls.SmsValidationSettings.TimeoutHandler) {
                clearInterval(ASC.Controls.SmsValidationSettings.TimeoutHandler);
            }

            AjaxPro.onLoading = function (loading) {
                if (loading) {
                    jq("#studio_smsValidationSettingsInfo").block();
                } else {
                    jq("#studio_smsValidationSettingsInfo").unblock();
                }
            };

            var smsEnable = jq("#chk2FactorAuthEnable").is(":checked");

            AjaxPro.SmsValidationSettingsController.SaveSettings(smsEnable, function (result) {
                var res = result.value || result.error;

                jq("#studio_smsValidationSettingsInfo").html(
                    "<div class=\"{0}\">{1}</div>".format(
                        res.Status ? "okBox" : "errorBox",
                        res.Message));

                ASC.Controls.SmsValidationSettings.TimeoutHandler = setTimeout(function () {
                    jq("#studio_smsValidationSettingsInfo").html("");
                }, 4000);
            });
        }
    };
})();

(function () {
    jq(function () {
        jq("#chk2FactorAuthSave").on("click", ASC.Controls.SmsValidationSettings.SaveSmsValidationSettings);
    });
})();