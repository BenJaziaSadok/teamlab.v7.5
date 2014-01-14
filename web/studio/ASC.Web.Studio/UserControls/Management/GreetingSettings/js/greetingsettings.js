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

var GreetingSettingsManager = new function () {
    this.TimeoutHandler = null;
    this.OnAfterSaveData = function (result, type) {
        if (type == 1) {
            if (!result.Success) {
                jq('div[id^="studio_setInf"]').html('');
                jq('#studio_setInfGreetingSettingsInfo').html('<div class="errorBox">' + result.Message + '</div>');
                GreetingSettingsManager.TimeoutHandler = setTimeout(function () { jq('#studio_setInfGreetingSettingsInfo').html(''); }, 4000);
            }
        }
    };

    this.SaveGreetingLogo = function (file, response) {
        jq.unblockUI();

        if (GreetingSettingsManager.TimeoutHandler) {
            clearTimeout(GreetingSettingsManager.TimeoutHandler);
        }

        var result = eval("(" + response + ")");
        if (result.Success) {
            jq('#studio_greetingLogo').attr('src', result.Message);
            jq('#studio_greetingLogoPath').val(result.Message);
        } else {
            jq('div[id^="studio_setInf"]').html('');
            jq('#studio_setInfGreetingSettingsInfo').html('<div class="errorBox">' + result.Message + '</div>');
            GreetingSettingsManager.TimeoutHandler = setTimeout(function () { jq('#studio_setInfGreetingSettingsInfo').html(''); }, 4000);
        }
    };

    this.SaveGreetingOptionsCallback = function (result) {
        //clean logo path input
        jq('#studio_greetingLogoPath').val('');
        jq('div[id^="studio_setInf"]').html('');

        var classCSS = (result.Status == 1 ? "okBox" : "errorBox");
        jq('#studio_setInfGreetingSettingsInfo').html('<div class="' + classCSS + '">' + result.Message + '</div>');
        GreetingSettingsManager.TimeoutHandler = setTimeout(function () { jq('#studio_setInfGreetingSettingsInfo').html(''); }, 4000);
    };

    this.SaveGreetingOptions = function () {
        if (this.TimeoutHandler) {
            clearTimeout(this.TimeoutHandler);
        }

        jq('#studio_greetingSettingsInfo').html('');
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_greetingSettingsBox').block();
            } else {
                jq('#studio_greetingSettingsBox').unblock();
            }
        };
        var greetManager = new GreetingSettingsContentManager();
        greetManager.SaveGreetingOptions(GreetingSettingsManager.SaveGreetingOptionsCallback);
    };

    this.RestoreGreetingOptions = function () {
        if (this.TimeoutHandler) {
            clearTimeout(this.TimeoutHandler);
        }

        jq('#studio_greetingSettingsInfo').html('');
        AjaxPro.onLoading = function (b) {
            if (b) {
                jq('#studio_greetingSettingsBox').block();
            } else {
                jq('#studio_greetingSettingsBox').unblock();
            }
        };

        var GreetManager = new GreetingSettingsContentManager();
        GreetManager.RestoreGreetingOptions(GreetingSettingsManager.RestoreGreetingOptionsCallback);
    };

    this.RestoreGreetingOptionsCallback = function (result) {
        jq('div[id^="studio_setInf"]').html('');
        var classCSS = (result.Status == 1 ? "okBox" : "errorBox");
        jq('#studio_setInfGreetingSettingsInfo').html('<div class="' + classCSS + '">' + result.Message + '</div>');
        GreetingSettingsManager.TimeoutHandler = setTimeout(function () { jq('#studio_setInfGreetingSettingsInfo').html(''); }, 4000);
    };
};

jq(function () {
    jq('#saveGreetSettingsBtn').click(GreetingSettingsManager.SaveGreetingOptions);
    jq('#restoreGreetSettingsBtn').click(GreetingSettingsManager.RestoreGreetingOptions);
});