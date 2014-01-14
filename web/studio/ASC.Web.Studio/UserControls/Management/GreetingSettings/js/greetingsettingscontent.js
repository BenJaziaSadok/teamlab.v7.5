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
    if (jq('#studio_logoUploader').length > 0) {
        new AjaxUpload('studio_logoUploader', {
            action: 'ajaxupload.ashx?type=ASC.Web.Studio.UserControls.Management.LogoUploader,ASC.Web.Studio',
            onSubmit: jq.blockUI,
            onComplete: GreetingSettingsUploadManager.SaveGreetingLogo
        });
    }
});

var GreetingSettingsUploadManager = new function() {
    this.BeforeEvent = null;
    this.SaveGreetingLogo = function (file, response) {
        var GreetManager1 = new GreetingSettingsContentManager();
        GreetManager1.SaveGreetingLogo(file, response);
    };
};

var GreetingSettingsContentManager = function() {

    this.OnAfterSaveData = null;
    this.TimeoutHandler = null;

    this.SaveGreetingLogo = function(file, response) {
        jq.unblockUI();

        var result = eval("(" + response + ")");

        if (result.Success) {
            jq('#studio_greetingLogo').attr('src', result.Message);
            jq('#studio_greetingLogoPath').val(result.Message);
        } else {
            if (this.OnAfterSaveData != null) {
                this.OnAfterSaveData(result);
            } else {
                jq('#studio_setInfGreetingSettingsInfo').html('<div class="errorBox">' + result.Message + '</div>');
                this.TimeoutHandler = setTimeout(function() {
                    jq('#studio_setInfGreetingSettingsInfo').html('');
                }, 4000);
            }
        }
    };

    this.SaveGreetingOptions = function(parentCallback) {
        GreetingSettingsController.SaveGreetingSettings(jq('#studio_greetingLogoPath').val(),
                                                jq('#studio_greetingHeader').val(),
                                                function(result) {
                                                    //clean logo path input
                                                    jq('#studio_greetingLogoPath').val('');
                                                    if (parentCallback != null)
                                                        parentCallback(result.value);
                                                });
    };

    this.RestoreGreetingOptions = function(parentCallback) {
        GreetingSettingsController.RestoreGreetingSettings(function(result) {
            //clean logo path input
            jq('#studio_greetingLogoPath').val('');

            if (result.value.Status == 1) {
                jq('#studio_greetingHeader').val(result.value.CompanyName);
                jq('#studio_greetingLogo').attr('src', result.value.LogoPath);
            }

            if (parentCallback != null) {
                parentCallback(result.value);
            }
        });
    };
}