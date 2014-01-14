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

if (typeof (ASC) == 'undefined')
    ASC = {};
if (typeof (ASC.Controls) == 'undefined')
    ASC.Controls = {};

ASC.Controls.FirstTimeView = new function() {
    this.TimeoutHandler = null;

    this.Finish = function() {
        window.onbeforeunload = null;
        EventTracker.Track('wizard_finish');
        location.href = "welcome.aspx";
    }

    this.SaveRequiredStep = function() {

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('.step').block();
            else
                jq('.step').unblock();
        };

        var FTManager = new ASC.Controls.FirstTimeManager();
        FTManager.SaveRequiredData(this.SaveRequiredStepCallback);
    }

    this.SaveRequiredStepCallback = function(result) {

        if (result.Status == 1) {
            var time = new TimeAndLanguageContentManager();
            time.SaveTimeLangSettings(ASC.Controls.FirstTimeView.SaveTimeLangSettingsCallback);
        } else {
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
        }
    }

    this.SaveTimeLangSettingsCallback = function(result) {
        if (result.Status == 2 || result.Status == 1) {
            ASC.Controls.FirstTimeView.Finish(); // enter redirect on the dash of modules
        } else {
            ASC.Controls.FirstTimeView.ShowOperationInfo(result);
            window.onbeforeunload = function(e) {
                return ASC.Resources.Master.Resource.WizardCancelConfirmMessage;
            };
        }
    }

    this.ShowOperationInfo = function(result) {
        var classCSS = (result.Status == 1 ? "okText" : "errorText");
        jq('#wizard_OperationInfo').html('<div class="' + classCSS + '">' + result.Message + '</div>');
        ASC.Controls.FirstTimeView.TimeoutHandler = setTimeout(function() { jq('#wizard_OperationInfo').html(''); }, 4000);
    }
};


jq(document).ready(function() {
    jq(document).keyup(function (e) {
    if (e.which == 13) {
        ASC.Controls.FirstTimeView.SaveRequiredStep();
        }
    });
    window.onbeforeunload = function(e) {
    return ASC.Resources.Master.Resource.WizardCancelConfirmMessage;
    };
});
