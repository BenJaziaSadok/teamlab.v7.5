<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmActivation.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Management.ConfirmActivation" %>
<%@ Import Namespace="ASC.Web.Studio" %>
<asp:Panel runat="server" ID="passwordSetter" Visible="false">
    <h2>
        <%=Type==ConfirmType.PasswordChange?Resources.Resource.PassworResetTitle:Resources.Resource.AccountActivationTitle %></h2>
    <p>
        <%=Resources.Resource.AccountActivationMessage %></p>
    <%--Pwd--%>
    <div class="clearFix" style="margin-top: 15px;">
        <div style="text-align: left; width: 400px;">
            <%=Resources.Resource.InvitePassword%>:
        </div>
        <div style="margin-top: 3px;">
            <input type="password" id="studio_confirm_pwd" value="" style="width: 360px;" name="pwdInput"
                class="pwdLoginTextbox" />
        </div>
    </div>
    <%--RePwd--%>
    <div class="clearFix" style="margin-top: 15px;">
        <div style="text-align: left; width: 400px;">
            <%=Resources.Resource.RePassword%>:
        </div>
        <div style="margin-top: 3px;">
            <input type="password" id="studio_confirm_repwd" value="" style="width: 360px;" name="repwdInput"
                class="pwdLoginTextbox" />
        </div>
    </div>
    <div class="clearFix" align="center" style="margin-top: 20px;">
        <asp:Button runat="server" ID="ButtonEmailAndPasswordOK" 
            CssClass="button blue" Style="width: 100px;" OnClick="LoginToPortal" />
    </div>
</asp:Panel>
<asp:Panel runat="server" ID="emailChange" Visible="false">
    <h2 style="padding-top:5px;">
        <%=Resources.Resource.MessageEmailAddressChanging%></h2>
    <p id="currentEmailText">
        <%=Resources.Resource.MessageCurrentEmailAddressIs%>
        <div style="margin-top:5px;">
        <a href="mailto:<%=User.Email %>">
            <%=User.Email%></a>
            </div>
    </p>
    <div id="studio_confirmMessage" style="text-align: left; width: 400px; margin: 10px 0px 20px 0px;
        display: none;">
        <div id="studio_confirmMessage_successtext" style="text-align: center; display: none;">
        </div>
        <div id="studio_confirmMessage_errortext" class="errorBox" style="display: none;">
        </div>
    </div>
    <div id="emailInputContainer">
        <%--Email--%>
        <div class="clearFix" style="margin-top: 15px;">
            <div style="text-align: left; width: 400px;">
                <%=Resources.Resource.TypeNewEmail%>:
            </div>
            <div style="margin-top: 3px;">
                <input type="email" id="email1" value="" style="width: 360px;" name="emailInput" class="pwdLoginTextbox" />
            </div>
        </div>
        <%--ReEmail--%>
        <div class="clearFix" style="margin-top: 15px;">
            <div style="text-align: left; width: 400px;">
                <%=Resources.Resource.ReEmail%>:
            </div>
            <div style="margin-top: 3px;">
                <input type="email" id="email2" value="" style="width: 360px;" name="reEmailInput"
                    class="pwdLoginTextbox" />
            </div>
        </div>
        <div class="clearFix" align="center" style="margin-top: 20px;">
            <asp:Button ID="btChangeEmail" runat="server" 
                CssClass="button blue" OnClientClick="window.btChangeEmailOnClick(); return false;" />
        </div>
    </div>
</asp:Panel>
