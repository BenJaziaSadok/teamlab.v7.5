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

jq(function () {
    jq('#savePasswordSettingsBtn').click(PasswordSettingsManager.SaveSettings);
});

jq(document).ready(function () {
    PasswordSettingsManager.LoadSettings();
});

PasswordSettingsManager = new function () {
    this.TimeoutHandler = null;

    this.LoadSettings = function () {
        PasswordSettingsController.LoadPasswordSettings(function (result) {

            var res = result.value;

            if (res == null)
                return;

            var jsonObj = JSON.parse(res);

            jq('#slider').slider({
                range: "max",
                min: 6,
                max: 16,
                value: jsonObj.MinLength,
                slide: function (event, ui) {
                    jq("#count").html(ui.value);
                }
            });

            jq("#count").html(jsonObj.MinLength);
            jq("input#chkUpperCase").prop("checked", jsonObj.UpperCase);
            jq("input#chkDigits").prop("checked", jsonObj.Digits);
            jq("input#chkSpecSymbols").prop("checked", jsonObj.SpecSymbols);
        });
    };

    this.SaveSettings = function () {
        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        AjaxPro.onLoading = function (b) {
            if (b)
                jq('#studio_passwordSettings').block();
            else
                jq('#studio_passwordSettings').unblock();
        };

        var jsonObj = {
            "MinLength": jq('#count').html(),
            "UpperCase": jq("input#chkUpperCase").is(":checked"),
            "Digits": jq("input#chkDigits").is(":checked"),
            "SpecSymbols": jq("input#chkSpecSymbols").is(":checked")
        };

        PasswordSettingsController.SavePasswordSettings(JSON.stringify(jsonObj), function (result) {
            var res = result.value;

            if (res.Status == 1)
                jq('#studio_passwordSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_passwordSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');

            PasswordSettingsManager.TimeoutHandler = setTimeout(function () {
                jq('#studio_passwordSettingsInfo').html('');
            }, 4000);
        });
    };
};