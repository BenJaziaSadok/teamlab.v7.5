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
ASC.Controls.ConfirmMobileManager = function () {
    var sendAuthCode = function () {
        jq("#errorMobileActivate").hide();

        var mobilePhone = jq("#primaryPhone").val().trim();
        if (!mobilePhone.length) {
            jq("#errorMobileActivate").html(ASC.Resources.Master.Resource.ActivateMobilePhoneEmptyPhoneNumber).show();
            jq("#primaryPhone").val(mobilePhone);
            return;
        }

        jq("#sendPhoneButton").addClass("disable");

        AjaxPro.MobileActivationController.SaveMobilePhone(mobilePhone, sendAuthCodeCallback);
    };

    var sendAuthCodeAgain = function () {
        jq("#errorMobileActivate").hide();
        jq("#getCodeAgainButton, #sendCodeButton").addClass("disable");

        AjaxPro.MobileActivationController.SendSmsCodeAgain(sendAuthCodeCallback);
    };

    var sendAuthCodeCallback = function (result) {
        jq("#sendPhoneButton, #getCodeAgainButton, #sendCodeButton").removeClass("disable");

        var res = result.value || result.error;
        if (res.phoneNoise) {
            if (res.confirm) {
                jq("#mobileNumberPanel").remove();
                jq("#phoneNoise").html(res.phoneNoise);
                jq("#mobileCodePanel").show();
                jq("#phoneAuthcode").val("").blur();
            } else {
                location.href = res.RefererURL || "/";
            }
        } else {
            jq("#errorMobileActivate").html(res.Message).show();
        }
    };

    var validateAuthCode = function () {
        jq("#errorMobileActivate").hide();

        var code = jq("#phoneAuthcode").val().trim();
        if (!code.length) {
            jq("#errorMobileActivate").html(ASC.Resources.Master.Resource.ActivateMobilePhoneEmptyCode).show();
            return;
        }

        jq("#getCodeAgainButton, #sendCodeButton").addClass("disable");

        AjaxPro.MobileActivationController.ValidateSmsCode(code, function (result) {
            var res = result.value || result.error;
            if (typeof res.RefererURL != "undefined") {
                location.href = res.RefererURL || "/";
            } else {
                jq("#getCodeAgainButton, #sendCodeButton").removeClass("disable");
                jq("#errorMobileActivate").html(res.Message || "Error").show();
            }
        });
    };

    return {
        sendAuthCode: sendAuthCode,
        sendAuthCodeAgain: sendAuthCodeAgain,
        validateAuthCode: validateAuthCode
    };
}();

(function () {
    jq(function () {
        jq(".mobilephone-panel").on("click", "#sendPhoneButton:not(.disable)", function () {
            ASC.Controls.ConfirmMobileManager.sendAuthCode();
            return false;
        });

        if (jq("#primaryPhone").length) {
            var country = jq("#primaryPhone").attr("data-country");
            PhoneController.Init(jq("#primaryPhone"), CountriesManager.countriesList, [country, "US"]);
        }

        jq("#primaryPhone").keyup(function (event) {
            if (!e) {
                var e = event;
            }
            var code = e.keyCode || e.which;
            if (code == 13) {
                ASC.Controls.ConfirmMobileManager.sendAuthCode();
                return false;
            }
        });

        jq(".mobilephone-panel").on("click", "#getCodeAgainButton:not(.disable)", function () {
            ASC.Controls.ConfirmMobileManager.sendAuthCodeAgain();
            return false;
        });

        jq(".mobilephone-panel").on("click", "#sendCodeButton:not(.disable)", function () {
            ASC.Controls.ConfirmMobileManager.validateAuthCode();
            return false;
        });

        jq("#phoneAuthcode").keyup(function (event) {
            if (!e) {
                var e = event;
            }
            var code = e.keyCode || e.which;
            if (code == 13) {
                ASC.Controls.ConfirmMobileManager.validateAuthCode();
                return false;
            }
        });
    });
})();