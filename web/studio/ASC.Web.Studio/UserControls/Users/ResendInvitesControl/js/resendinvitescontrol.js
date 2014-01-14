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
    jq('#resendBtn').click(function() {
        InvitesResender.Resend();
    });
 
    jq('#resendCancelBtn, #resendInvitesCloseBtn').click(function() {
        InvitesResender.Hide();
    });
});
var InvitesResender = new function() {
    this.Resend = function() {
        AjaxPro.onLoading = function(b) {
            if (b)
                jq('#inviteResender').block();
            else
                jq('#inviteResender').unblock();
        };
        InviteResender.Resend(function(result) {
            var res = result.value;
            jq('#resendInvitesResult').removeClass("display-none");
            jq('#resendInvitesContent').addClass("display-none");

            jq('#resendInvitesResultText').html(res.message);
            if (res.status == 1)
                jq('#resendInvitesResultText').attr('class', 'okBox');
            else
                jq('#resendInvitesResultText').attr('class', 'errorBox');
            PopupKeyUpActionProvider.EnterAction = "PopupKeyUpActionProvider.CloseDialog();";

        })
    }

    this.Show = function() {
        jq('#resendInvitesResult').addClass("display-none");
        jq('#resendInvitesContent').removeClass("display-none");

        StudioBlockUIManager.blockUI("#inviteResender", 320, 150, 0);
        PopupKeyUpActionProvider.ClearActions();
        PopupKeyUpActionProvider.EnterAction = "jq(\"#resendBtn\").click();";
    }
    
    this.Hide = function() {
        jq.unblockUI();
    }
}