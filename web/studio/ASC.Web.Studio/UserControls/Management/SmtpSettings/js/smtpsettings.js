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

SmtpSettingsManager = new function () {
    this.Initialize = function () {
        jq('#smtpSettingsButtonSave').click(function() {
            SmtpSettingsManager.Save();
        });

        jq('#smtpSettingsButtonTest').click(function() {
            SmtpSettingsManager.Test();
        });

        jq('#smtpSettingsAuthentication').change(function() {
            if (jq('#smtpSettingsAuthentication').is(':checked')) {
                jq('.smtp-settings-block .host-login .smtp-settings-field').attr('disabled', false);
                jq('.smtp-settings-block .host-password .smtp-settings-field').attr('disabled', false);
            } else {
                jq('.smtp-settings-block .host-login .smtp-settings-field').attr('disabled', true);
                jq('.smtp-settings-block .host-password .smtp-settings-field').attr('disabled', true);
            }
        });
    };

    this.Save = function () {
        jq('#smtpSettingsErrorBox').addClass('display-none');
        jq('#smtpSettingsContainer').block();
        SmtpSettings.Save(getSettings(), function(result) {
            if (result.error != null) {
                jq('#smtpSettingsErrorBox').text(result.error.Message);
                jq('#smtpSettingsErrorBox').removeClass('display-none');
            }
            jq('#smtpSettingsContainer').unblock();
        });
    };

    this.Test = function () {
        jq('#smtpSettingsErrorBox').addClass('display-none');
        jq('#smtpSettingsContainer').block();
        SmtpSettings.Test(getSettings(), function(result) {
            if (result.error != null) {
                jq('#smtpSettingsErrorBox').text(result.error.Message);
                jq('#smtpSettingsErrorBox').removeClass('display-none');
            }
            jq('#smtpSettingsContainer').unblock();
        });
    };

    var getSettings = function() {
        var data = {
            Host: jq('.smtp-settings-block .host .smtp-settings-field').val(),
            SenderDisplayName: jq('.smtp-settings-block .display-name .smtp-settings-field').val(),
            SenderAddress: jq('.smtp-settings-block .email-address .smtp-settings-field').val(),
            EnableSSL: jq('#smtpSettingsEnableSsl').is(':checked')
        };

        var port = jq('.smtp-settings-block .port .smtp-settings-field').val();
        if (port != '') {
            data.Port = port;
        }

        var requireAuthentication = jq('#smtpSettingsAuthentication').is(':checked');
        if (requireAuthentication) {
            data.CredentialsUserName = jq('.smtp-settings-block .host-login .smtp-settings-field').val();
            data.CredentialsUserPassword = jq('.smtp-settings-block .host-password .smtp-settings-field').val();
        }

        return data;
    };
};

jq(function() {
    SmtpSettingsManager.Initialize();
});