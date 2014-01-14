<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccountsPage.ascx.cs" Inherits="ASC.Web.Mail.Controls.AccountsPage" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="id_accounts_page" class="hidden page_content">
    <div class="containerBodyBlock">
        <div class="content-header">
            <a title="<%=ASC.Web.Mail.Resources.MailResource.CreateNewAccountBtn%>" href="#" class="button gray" id="createNewAccount">
                <div class="plus" style="background-position: -2px 1px;"><%=ASC.Web.Mail.Resources.MailResource.CreateNewAccountBtn%></div>
            </a>
            <span class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'AccountsHelperBlock'});"></span>
        </div>
    </div>
</div>


<div class="popup_helper" id="AccountsHelperBlock">
    <p><%=ASC.Web.Mail.Resources.MailResource.AccountsCommonInformationText%></p>
    <p><%=ASC.Web.Mail.Resources.MailResource.AccountsCommonNotificationText%></p>
    <div class="cornerHelpBlock pos_top"></div>
</div>


<div id="accountActionMenu" class="studio-action-panel">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li><a class="activateAccount dropdown-item"><%=ASC.Web.Mail.Resources.MailResource.ActivateAccountLabel%></a></li>
        <li><a class="deactivateAccount dropdown-item"><%=ASC.Web.Mail.Resources.MailResource.DeactivateAccountLabel%></a></li>
        <li><a class="editAccount dropdown-item"><%=ASC.Web.Mail.Resources.MailResource.EditAccountLabel%></a></li>
        <li><a class="deleteAccount dropdown-item"><%=ASC.Web.Mail.Resources.MailResource.DeleteAccountLabel%></a></li>
    </ul>
</div>


<div id="questionWnd" style="display: none" delete_header="<%=ASC.Web.Mail.Resources.MailResource.DeleteAccountLabel%>"
                                            activate_header="<%=ASC.Web.Mail.Resources.MailResource.ActivateAccountLabel%>"
                                            deactivate_header="<%=ASC.Web.Mail.Resources.MailResource.DeactivateAccountLabel%>">
   <sc:Container ID="accountQuestionPopup" runat="server">
        <header>
        </header>
        <body>
            <div class="mail-confirmationAction">
                <p class="attentionText remove"><%=ASC.Web.Mail.Resources.MailResource.DeleteAccountAttention%></p>
                <p class="attentionText activate"><%=ASC.Web.Mail.Resources.MailResource.ActivateAccountAttention%></p>
                <p class="attentionText deactivate"><%=ASC.Web.Mail.Resources.MailResource.DeactivateAccountAttention%></p>
                <p class="questionText"></p>
            </div>
            <div class="buttons">
                <button class="button blue remove" type="button"><%=ASC.Web.Mail.Resources.MailResource.DeleteBtnLabel%></button>
                <button class="button blue activate" type="button"><%=ASC.Web.Mail.Resources.MailResource.ActivateBtnLabel%></button>
                <button class="button blue deactivate" type="button"><%=ASC.Web.Mail.Resources.MailResource.DeactovateBtnLabel%></button>
                <button class="button gray cancel" type="button"><%=ASC.Web.Mail.Resources.MailScriptResource.CancelBtnLabel%></button>
            </div>
        </body>
    </sc:Container>
</div>