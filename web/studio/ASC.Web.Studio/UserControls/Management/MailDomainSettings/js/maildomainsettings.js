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
    jq('#saveMailDomainSettingsBtn').click(MailDomainSettingsManager.SaveSettings);
    jq('input[name="signInType"]').click(MailDomainSettingsManager.SwitchSignInType);
    jq('#addTrustDomainBtn').click(MailDomainSettingsManager.AddTrustedDomain);

})

MailDomainSettingsManager = new function() {

    this.TimeoutHandler = null;

    this.SwitchSignInType = function() {

        if (jq('#trustedMailDomains').is(':checked')) {

            jq('#offMailDomainsDescription').hide();
            jq('#allMailDomainsDescription').hide();
            jq('#trustedMailDomainsDescription').show();
        }
        else if (jq('#allMailDomains').is(':checked')) {
            jq('#offMailDomainsDescription').hide();
            jq('#trustedMailDomainsDescription').hide();
            jq('#allMailDomainsDescription').show();
        }
        else {
            jq('#trustedMailDomainsDescription').hide();
            jq('#allMailDomainsDescription').hide();
            jq('#offMailDomainsDescription').show();
        }
    }

    this.RemoveTrustedDomain = function(number) {
        jq('#studio_domain_box_' + number).remove();
        var count = jq('div[id^="studio_domain_box_"]').length;
        if (count < 10)
            jq('#addTrustDomainBtn').show();
    };

    this.AddTrustedDomain = function() {
        var maxNumb = -1;
        jq('div[id^="studio_domain_box_"]').each(function(i, pel) {

            var n = parseInt(jq(this).attr('name'));
            if (n > maxNumb)
                maxNumb = n + 1;
        });

        maxNumb++;
        var sb = new String();
        sb += '<div name="' + maxNumb + '" id="studio_domain_box_' + maxNumb + '" class="clearFix" style="margin-bottom:15px;">';
        sb += '<input type="text" value="" id="studio_domain_box_' + maxNumb + '" class="textEdit" maxlength="60" style="width:300px;"/>'
        sb += '<a class="removeDomain" href="javascript:void(0);" onclick="MailDomainSettingsManager.RemoveTrustedDomain(\'' + maxNumb + '\');"><img alt="" align="absmiddle" border="0" src="' + StudioManager.GetImage("trash_16.png") + '"/></a>';
        sb += '</div>';

        jq('#studio_domainListBox').append(sb);

        var count = jq('div[id^="studio_domain_box_"]').length;
        if (count >= 10)
            jq('#addTrustDomainBtn').hide();
    }

    this.SaveSettings = function() {

        if (this.TimeoutHandler)
            clearTimeout(this.TimeoutHandler);

        var domains = new Array();
        var type = '';
        if (jq('#trustedMailDomains').is(':checked')) {

            type = jq('#trustedMailDomains').val();
            jq('input[id^="studio_domain_"]').each(function(i, pel) {
                domains.push(jq(this).val());
            });
        }
        else if (jq('#allMailDomains').is(':checked')) {
            type = jq('#allMailDomains').val();
        }
        else
            type = jq('#offMailDomains').val();

        var inviteUsersAsVisitors = jq("#cbxInviteUsersAsVisitors").is(":checked");

        AjaxPro.onLoading = function(b) {
                if (b)
                    jq('#studio_mailDomainSettingsBox').block();
                else
                    jq('#studio_mailDomainSettingsBox').unblock();
        };

        MailDomainSettingsController.SaveMailDomainSettings(type, domains, inviteUsersAsVisitors, function(result) {

            var res = result.value;
            if (res.Status == 1)
                jq('#studio_domainSettingsInfo').html('<div class="okBox">' + res.Message + '</div>');
            else
                jq('#studio_domainSettingsInfo').html('<div class="errorBox">' + res.Message + '</div>');

            MailDomainSettingsManager.TimeoutHandler = setTimeout(function() { jq('#studio_domainSettingsInfo').html(''); }, 4000);
        });
    }

}