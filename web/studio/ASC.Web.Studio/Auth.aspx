<%@ Page Language="C#" MasterPageFile="~/Masters/basetemplate.master" AutoEventWireup="true" EnableViewState="false" CodeBehind="Auth.aspx.cs" Inherits="ASC.Web.Studio.Auth" Title="TeamLab" %>
<%@ MasterType TypeName="ASC.Web.Studio.Masters.BaseTemplate" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="Resources" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnableScriptGlobalization="True" EnableScriptLocalization="True"></asp:ScriptManager>

    <% if (CoreContext.Configuration.YourDocsDemo)
       { %>

    <div class="auth-form-with">
        <style type="text/css">
            .try-welcome-top {
                height: 67px;
                padding: 17px 64px 0;
            }

            .try-welcome-top.studio-top-panel .studio-top-logo {
                margin-top: 0;
            }

            .try-welcome-top .try-welcome-top-label {
                color: #D2D4DA;
                float: right;
                margin-top: 10px;
            }

            .auth-form-with {
                margin: 85px auto 0;
                text-align: center;
                width: 500px;
            }

            .try-welcome {
                color: #83888D;
                font-size: 22px;
                padding-bottom: 8px;
            }

            .auth-form-with .login-with-label {
                color: #333;
                font-size: 18px;
            }

            .auth-form-with #accountLinks {
                text-align: center;
            }

            .auth-form-with .social-login #accountLinks li {
                height: 117px;
            }

            .auth-form-with .social-login #accountLinks li a {
                margin: 0 9px;
                height: 96px;
                width: 96px;
            }

            .label-try {
                margin-bottom: 14px;
            }

            .try-editor {
                margin: 10px auto 0;
                width: 200px;
            }

            .try-editor a {
                color: #fff;
                text-decoration: none;
            }

            .try-editor a span {
                background: #3e4b6c;
                border-radius: 3px;
                -moz-border-radius: 3px;
                -webkit-border-radius: 3px;
                display: block;
                font-size: 18px;
                height: 40px;
                line-height: 40px;
                text-align: center;
            }

            .try-editor.blue a span {
                background: #3e4b6c;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#7688b6', endColorstr='#3e4b6c');
                background: linear-gradient(bottom top, #3e4b6c, #7688b6);
                background: -webkit-gradient(linear, left bottom, left top, from(#3e4b6c), to(#7688b6)) #3e4b6c;
                background: -o-linear-gradient(bottom, #3e4b6c, #7688b6);
                background: -moz-linear-gradient(bottom, #3e4b6c,#7688b6);
            }

            .try-editor.blue a:hover span {
                background: #4c5b80;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#7f91be', endColorstr='#4e5d83');
                background: linear-gradient(bottom top, #4e5d83, #7f91be);
                background: -webkit-gradient(linear, left bottom, left top, from(#4e5d83), to(#7f91be)) #4e5d83;
                background: -o-linear-gradient(bottom, #4e5d83, #7f91be);
                background: -moz-linear-gradient(bottom, #4e5d83,#7f91be);
            }

            .try-editor.green a span {
                background: #598001;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#a0ba5d', endColorstr='#70843e');
                background: linear-gradient(bottom top, #70843e, #a0ba5d);
                background: -webkit-gradient(linear, left bottom, left top, from(#70843e), to(#a0ba5d)) #70843e;
                background: -o-linear-gradient(bottom, #70843e, #a0ba5d);
                background: -moz-linear-gradient(bottom, #70843e,#a0ba5d);
            }

            .try-editor.green a:hover span {
                background: #6c901b;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#aabe75', endColorstr='#7b9046');
                background: linear-gradient(bottom top, #7b9046, #aabe75);
                background: -webkit-gradient(linear, left bottom, left top, from(#7b9046), to(#aabe75)) #7b9046;
                background: -o-linear-gradient(bottom, #7b9046, #aabe75);
                background: -moz-linear-gradient(bottom, #7b9046,#aabe75);
            }

            .try-editor.orange a span {
                background: #E59241;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#e86e2e', endColorstr='#ae5321');
                background: linear-gradient(bottom top, #ae5321, #e86e2e);
                background: -webkit-gradient(linear, left bottom, left top, from(#ae5321), to(#e86e2e)) #ae5321;
                background: -o-linear-gradient(bottom, #ae5321, #e86e2e);
                background: -moz-linear-gradient(bottom, #ae5321,#e86e2e);
            }

            .try-editor.orange a:hover span {
                background: #eea14a;
                filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#e6814c', endColorstr='#bd5820');
                background: linear-gradient(bottom top, #bd5820, #e6814c);
                background: -webkit-gradient(linear, left bottom, left top, from(#bd5820), to(#e6814c)) #bd5820;
                background: -o-linear-gradient(bottom, #bd5820, #e6814c);
                background: -moz-linear-gradient(bottom, #bd5820,#e6814c);
            }
        </style>
        <div class="try-welcome"><%= string.Format(Resource.LabelWelcomeTry, "<br />") %></div>
        <asp:PlaceHolder runat="server" ID="HolderLoginWithThirdParty"></asp:PlaceHolder>

        <div class="header-base label-try"><%= Resource.LabelTry %></div>
        <div class="try-editor blue">
            <a href="<%= CommonLinkUtility.GetFileWebEditorTryUrl(FileType.Document) %>" target="_blank">
                <span><%= Resource.TryEditorNow %></span>
            </a>
        </div>
        <div class="try-editor green">
            <a href="<%= CommonLinkUtility.GetFileWebEditorTryUrl(FileType.Spreadsheet) %>" target="_blank">
                <span><%= Resource.TrySpreadsheetNow %></span>
            </a>
        </div>
        <div class="try-editor orange">
            <a href="<%= CommonLinkUtility.GetFileWebEditorTryUrl(FileType.Presentation) %>" target="_blank">
                <span><%= Resource.TryPresentationNow %></span>
            </a>
        </div>
    </div>

    <% }
       else
       { %>
    <div class="auth-form-page">
        <div id="GreetingBlock" class="authForm <% if (!withHelpBlock) {%>help-block-none <%}%>">
            <%--header and logo--%>
            <div class="header header-base-big blue-text clearFix">
                <% if (!CoreContext.Configuration.YourDocs)
                   { %>
                <img class="logo" src="<%= LogoPath %>" alt="<%= CoreContext.TenantManager.GetCurrentTenant().Name.HtmlEncode() %>" />
                <% } %>
                <h1 class="title"><%= CoreContext.TenantManager.GetCurrentTenant().Name.HtmlEncode() %></h1>
            </div>

            <asp:PlaceHolder runat="server" ID="AuthorizeHolder"></asp:PlaceHolder>

            <div class="help-block-signin">
                <asp:PlaceHolder runat="server" ID="CommunitationsHolder"></asp:PlaceHolder>
            </div>
        </div>
    </div>
    <% } %>
</asp:Content>

<asp:Content ContentPlaceHolderID="FooterContent" runat="server">
    <div class="footerAuth">
        <%=Resource.PoweredBy%>
        <a href="http://www.teamlab.com/" title="www.teamlab.com" class="link underline" target="_blank">www.teamlab.com</a>
    </div>
</asp:Content>
