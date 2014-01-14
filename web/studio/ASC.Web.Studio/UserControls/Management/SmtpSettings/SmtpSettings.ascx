<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmtpSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.SmtpSettings" %>
<%@ Import Namespace="ASC.Core" %>

<div id="smtpSettingsContainer">
    <div class="header-base"><%=Resources.Resource.SmtpSettings %></div>

    <p class="smtp-settings-text"><% =Resources.Resource.SmtpSettingsText %> </p>

    <div class="smtp-settings-block clearFix">
        <div class="smtp-settings-item host">
            <div class="smtp-settings-title"><%=Resources.Resource.HostName %>:</div>
            <input type="text" class="smtp-settings-field textEdit" value="<%= CoreContext.Configuration.SmtpSettings.Host %>"/>
        </div>
        <div class="smtp-settings-item port">
            <div class="smtp-settings-title"><%=Resources.Resource.Port %>:</div>
            <input type="text" class="smtp-settings-field textEdit" value="<%= CoreContext.Configuration.SmtpSettings.Port %>"/>
            <input id="smtpSettingsAuthentication" type="checkbox" 
                <% if (!string.IsNullOrEmpty(CoreContext.Configuration.SmtpSettings.CredentialsUserName)
                       && !string.IsNullOrEmpty(CoreContext.Configuration.SmtpSettings.CredentialsUserPassword))
                   { %>  
                    checked="checked" 
                <% } %> />
            <label for="smtpSettingsAuthentication"><%=Resources.Resource.Authentication %></label>
        </div>
        <div class="smtp-settings-item host-login">
            <div class="smtp-settings-title"><%=Resources.Resource.HostLogin %>:</div>
            <input type="text" class="smtp-settings-field textEdit" value="<%= CoreContext.Configuration.SmtpSettings.CredentialsUserName %>"/>
        </div>
        <div class="smtp-settings-item host-password">
            <div class="smtp-settings-title"><%=Resources.Resource.HostPassword %>:</div>
            <input type="password" class="smtp-settings-field textEdit" value=""/>
        </div>
        <div class="smtp-settings-item display-name">
            <div class="smtp-settings-title"><%=Resources.Resource.SenderName %>:</div>
            <input type="text" class="smtp-settings-field textEdit" value="<%= CoreContext.Configuration.SmtpSettings.SenderDisplayName %>"/>
        </div>
        <div class="smtp-settings-item email-address">
            <div class="smtp-settings-title"><%=Resources.Resource.SenderEmailAddress %>:</div>
            <input type="text" class="smtp-settings-field textEdit" value="<%= CoreContext.Configuration.SmtpSettings.SenderAddress %>"/>
        </div>
        <div class="smtp-settings-item">
            <input id="smtpSettingsEnableSsl" type="checkbox" <% if (CoreContext.Configuration.SmtpSettings.EnableSSL) { %> checked="checked" <% } %>/>
            <label for="smtpSettingsEnableSsl"><%=Resources.Resource.EnableSSL %></label>
        </div>
    </div>

    <div class="middle-button-container">
        <div id="smtpSettingsErrorBox" class="errorBox display-none"></div>
        <a id="smtpSettingsButtonSave" class="button blue" href="javascript:void(0);"><%= Resources.Resource.SaveButton %></a>
        <span class="splitter-buttons"></span>
        <a id="smtpSettingsButtonTest" class="button gray" href="javascript:void(0);"><%= Resources.Resource.SendTestMail %></a>
    </div>
</div>