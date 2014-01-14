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

ASC.Controls.FirstTimeManager = function() {

    this.OnAfterSaveRequiredData = null;
    this.OnAfterSaveData = null;

    this.SaveRequiredData = function(parentCalllback) {
        ASC.Controls.EmailAndPasswordManager.SaveRequiredData(parentCalllback);
    };
};

ASC.Controls.EmailAndPasswordManager = new function() {

    this.PassText = '';
    this.changeIt = '';
    this.ok = '';
    this.wrongPass = '';
    this.emptyPass = '';
    this.wrongEmail = '';

    this.init = function (changeItText, okText, wrongPassText, emptyPassText, wrongEmailText) {
        ASC.Controls.EmailAndPasswordManager.changeIt = changeItText;
        ASC.Controls.EmailAndPasswordManager.ok = okText;
        ASC.Controls.EmailAndPasswordManager.wrongPass = wrongPassText;
        ASC.Controls.EmailAndPasswordManager.emptyPass = emptyPassText;
        ASC.Controls.EmailAndPasswordManager.wrongEmail = wrongEmailText;
    };

    this.ShowChangeEmailAddress = function() {
        var email = jQuery.trim(jq('.emailAddress').html());
        jq('.emailAddress').html('');
        jq('.emailAddress').append('<input type="textbox" id="newEmailAddress" maxlength="64" class="textEdit newEmail">');
        jq('.emailAddress #newEmailAddress').val(email);
        jq('.changeEmail').html('');
    };

    this.AcceptNewEmailAddress = function() {
        var email = jq('.changeEmail #dvChangeMail #newEmailAddress').val();

        if (email == '')
            return;

        jq('#requiredStep .emailBlock .email .emailAddress').html(email);
        jq('.changeEmail #dvChangeMail').html('');
        jq('.changeEmail #dvChangeMail').append('<a class="info baseLinkAction" onclick="ASC.Controls.EmailAndPasswordManager.ShowChangeEmailAddress();">' + ASC.Controls.EmailAndPasswordManager.changeIt + '</a>');
    };

    this.SaveRequiredData = function(parentCallback) {

        var email = jQuery.trim(jq('#requiredStep .emailBlock .email .emailAddress #newEmailAddress').val()); //
        if (email == '' || email == null)
            email = jQuery.trim(jq('#requiredStep .emailBlock .email .emailAddress').html());
        var pwd = jq('.passwordBlock .pwd #newPwd').val();
        var cpwd = jq('.passwordBlock .pwd #confPwd').val();
        var promocode = jq('.passwordBlock .promocode #promocode_input').val();

        if (email == '' || !jq.isValidEmail(email)) {
            var res = { "Status": 0, "Message": ASC.Controls.EmailAndPasswordManager.wrongEmail };
            if (parentCallback != null)
                parentCallback(res);
            return;
        }

        if (pwd != cpwd || pwd == '') {

            if (pwd != cpwd) {
                jq(".passwordBlock .pwd #newPwd ,.passwordBlock .pwd #confPwd").css("border-color", "#DF1B1B");
            }

            if (pwd == '') {
                jq(".passwordBlock .pwd #newPwd").css("border-color", "#DF1B1B");
            }

            var res = { "Status": 0, "Message": pwd == '' ? ASC.Controls.EmailAndPasswordManager.emptyPass : ASC.Controls.EmailAndPasswordManager.wrongPass };
            if (parentCallback != null)
                parentCallback(res);
            return;
        }
        window.onbeforeunload = null;
        AjaxPro.timeoutPeriod = 1800000;
        EmailAndPasswordController.SaveData(email, pwd, jq('#studio_lng').val(), jq('#demo_data').is(':checked'), promocode, function(result) {

            if (parentCallback != null)
                parentCallback(result.value);
        });
    };
};

jq(function() {
    if (jQuery.trim(jq('.emailAddress').html()) == '') {
        ASC.Controls.EmailAndPasswordManager.ShowChangeEmailAddress();
    }
});