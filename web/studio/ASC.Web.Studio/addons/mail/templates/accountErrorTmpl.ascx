<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="accountErrorTmpl" type="text/x-jquery-tmpl">
    <div id="account_error_container" class="popup popupMailBox">
        ${errorBodyHeader = '<%: MailScriptResource.AccountCreationErrorHeader %>', ""}
        ${errorBody = '<%: MailResource.AccountCreationErrorBody %>', ""}
        {{tmpl({
            errorBodyHeader     : errorBodyHeader,
            errorBody           : errorBody,
            errorBodyFooter     : $item.data.errorBodyFooter,
            errorAdvancedInfo   : $item.data.errorAdvancedInfo
        }) "errorBodyTmpl"}}
        <div class="buttons">
            <button class="button blue tryagain" id="tryagain" type="button"><%: MailScriptResource.TryAgainButton %></button>
            <button class="button gray cancel" type="button"><%: MailScriptResource.CancelBtnLabel %></button>
            <a id="advancedErrorLinkButton" class="anchorLinkButton"><%: MailResource.AdvancedLinkLabel %></a>
        </div>
    </div>
</script>

<script id="messageOpenErrorTmpl" type="text/x-jquery-tmpl">
    <div class="body-error">
        ${errorBodyHeader = '<%: MailScriptResource.ErrorOpenMessage %>', ""}
        ${errorBody = '<%= Server.HtmlEncode(MailScriptResource.ErrorOpenMessageHelp)
                                                    .Replace("{0}", "<a href=\"" + MailPage.GetMailSupportUri() + "\" target=\"_blank\">")
                                                    .Replace("{1}", "</a>")%>', ""}
        {{tmpl({
                errorBodyHeader : errorBodyHeader,
                errorBody       : errorBody
            }) "errorBodyTmpl"}}
    </div>
</script>

<script id="errorBodyTmpl" type="text/x-jquery-tmpl">
    <div class="error">
        <div class="header">${errorBodyHeader}</div>
        {{if typeof(errorBody)!=='undefined'}}
            <p class="body">{{html errorBody}}</p>
        {{/if}}
        {{if typeof(errorBodyFooter)!=='undefined'}}
            <p class="footer">{{html errorBodyFooter}}</p>
        {{/if}}
        {{if typeof(errorAdvancedInfo)!=='undefined'}}
            <div id="mail_advanced_error_container" class="yellow_help account_error" style="display:none;">{{html errorAdvancedInfo}}</div>
        {{/if}}
    </div>

</script>