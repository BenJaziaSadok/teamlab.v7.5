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

jq(function() {
    jq('input[name="studio_settingsSMTPSwitch"]').click(MailSettingsManager.SwitchSMTPMode);
    jq('#saveMailSettingsBtn').click(MailSettingsManager.SaveSmtpSettings);
    jq('#sendTestMailBtn').click(MailSettingsManager.TestSmtpSettings);
})

var MailSettingsManager = new function() {

    this.TimeoutHandler = null;

    this.SaveSmtpSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_smtpSettingsBox').block();
            else
                jq('#studio_smtpSettingsBox').unblock();
        };

        var isCorporate = (jq('#studio_corporateSMTPButton').is(':checked'))

        MailSettingsController.SaveSmtpSettings(jq('#studio_smtpAddress').val(),
                                        jq('#studio_smtpPort').val(), //port
                                        jq('#studio_smtpEnableSSL').is(":checked"), //secure
                                        jq('#studio_smtpSenderEmail').val(),
                                        jq('#studio_smtpSenderDisplayName').val(),
                                        isCorporate ? "" : jq('#studio_smtpCredentialsDomain').val(),
                                        isCorporate ? "" : jq('#studio_smtpCredentialsUserName').val(),
                                        isCorporate ? "" : jq('#studio_smtpCredentialsUserPwd').val(),
        function(result) {
            jq('div[id^="studio_setInf"]').html('');
            var res = result.value;
            jq('#studio_smtpPort').val(res.Port);
            if (res.Status == 1)
                jq('#studio_setInfSmtpSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_setInfSmtpSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');

            MailSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_setInfSmtpSettingsInfo').html(''); }, 4000);
        });
    };

    this.TestSmtpSettings = function() {
        if (this.TimeoutHandler)
            clearInterval(this.TimeoutHandler);

        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#studio_smtpSettingsBox').block();
            else
                jq('#studio_smtpSettingsBox').unblock();
        };

        MailSettingsController.TestSmtpSettings(jq('#studio_smtpRecipientAddress').val(),
            function(result) {            
                jq('div[id^="studio_setInf"]').html('');
                var res = result.value;
                if (res.Status == 1)
                    jq('#studio_setInfSmtpSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
                else
                    jq('#studio_setInfSmtpSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');
                MailSettingsController.TimeoutHandler = setTimeout(function() { jq('#studio_setInfSmtpSettingsInfo').html(''); }, 4000);
            });
    };

    this.SwitchSMTPMode = function() {

        if (jq('#studio_corporateSMTPButton').is(':checked'))
            jq('#studio_smtp_personal').hide();
        else
            jq('#studio_smtp_personal').show();
    };

}