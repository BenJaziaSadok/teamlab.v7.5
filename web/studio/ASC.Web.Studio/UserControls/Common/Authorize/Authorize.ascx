<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Authorize.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.Authorize" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Users.UserProfile" %>

<div id="authMessage" class="auth-form_message"><%= ErrorMessage + LoginMessage %></div>

<div id="authForm" class="auth-form">
    <%--login by email email--%>
    <div id="_AuthByEmail" class="login" runat="server">
        <input maxlength="64" class="pwdLoginTextbox" type="email" placeholder="<%=Resources.Resource.RegistrationEmailWatermark %>" id="login" name="login"
            <%= String.IsNullOrEmpty(Login)
                ? ""
                : ("value=\"" + Login.HtmlEncode() + "\"") %> />
    </div>
    
    <%--password--%>
    <div class="auth-form_password">
        <input type="password" id="pwd" class="pwdLoginTextbox" name="pwd" maxlength="64" placeholder="<%= Resources.Resource.Password %>" />
    </div>
    <%--buttons--%>
    <div class="auth-form_submenu clearFix">
        <div class="auth-form_submenu_login clearFix">
            <a id="loginButton" class="button blue big signIn" href="javascript:void(0);" onclick="AuthManager.Login(); return false;">
                <%= Resources.Resource.LoginButton %>
            </a>
            <% if (AccountLinkControl.IsNotEmpty)
               { %>
            <div id="social" class="social_nets clearFix" style="display: <%= SetupInfo.ThirdPartyAuthEnabled ? "block" : "none" %>">
                <span><%= Resources.Resource.LoginWithAccount %></span>
                <div class="float-right">
                    <asp:PlaceHolder ID="signInPlaceholder" runat="server" />
                </div>
            </div>
            <% } %>
            <asp:PlaceHolder ID="pwdReminderHolder" runat="server" />
        </div>
        <div class="auth-form_subtext">
            <span class="link gray underline" onclick="AuthManager.ShowPwdReminderDialog()">
                <%= Resources.Resource.ForgotPassword %>
            </span>
        </div>
    </div>
</div>