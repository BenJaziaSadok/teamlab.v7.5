<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PwdTool.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.PwdTool" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>
<div id="studio_pwdReminderDialog" class="display-none">
    <sc:Container runat="server" ID="_pwdRemainderContainer">
        <Header>
            <span id="pswdRecoveryDialogPopupHeader" class="display-none">
                <%=Resources.Resource.PasswordRecoveryTitle%>
            </span><span id="pswdChangeDialogPopupHeader" class="display-none">
                <%=Resources.Resource.PasswordChangeTitle%>
            </span>
        </Header>
        <Body>
            <div id="studio_pwdReminderContent">
                <input type="hidden" id="studio_pwdReminderInfoID" value="<%=_pwdRemainderContainer.ClientID%>_InfoPanel" />
                <div class="clearFix">
                    <div id="pswdRecoveryDialogText" class="display-none">
                        <div>
                            <%=Resources.Resource.MessageSendPasswordRecoveryInstructionsOnEmail%>
                        </div>
                        <div style="margin: 3px 0px;">
                            <input type="text" id="studio_emailPwdReminder" class="textEdit" style="width: 99%;" />
                        </div>
                    </div>
                    <div id="pswdChangeDialogText" class="display-none">
                        <%= String.Format(Resources.Resource.MessageSendPasswordChangeInstructionsOnEmail, "<div style='margin-top:2px;'><a name='userEmail'></a></div>")%>
                    </div>
                </div>
                <div class="clearFix" style="margin-top: 26px;" id="pwd_rem_panel_buttons">
                    <a class="button blue middle" style="float: left;" href="javascript:AuthManager.RemindPwd();">
                        <%=Resources.Resource.SendButton%></a> <a class="button gray middle" style="float: left;
                            margin-left: 8px;" href="javascript:AuthManager.ClosePwdReminder();">
                            <%=Resources.Resource.CancelButton%></a>
                </div>
                <div style="padding-top: 16px; display: none;" id="pwd_rem_action_loader" class="clearFix">
                    <div class="text-medium-describe">
                        <%=Resources.Resource.PleaseWaitMessage%>
                    </div>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
                </div>
            </div>
            <div id="studio_pwdReminderMessage" style="padding: 20px 0px; text-align: center;
                display: none;">
            </div>
        </Body>
    </sc:Container>
</div>
