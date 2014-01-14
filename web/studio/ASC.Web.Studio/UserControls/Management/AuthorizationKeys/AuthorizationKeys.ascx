<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AuthorizationKeys.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AuthorizationKeys" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Management" %>

<% if (AuthServiceList.Count > 0)
   { %>
<div id="authKeysContainer">
    <div class="header-base"><%=Resources.Resource.AuthorizationKeys %></div>

    <p class="auth-service-text"><% =Resources.Resource.AuthorizationKeysText %> <br />
        <a href="http://helpcenter.teamlab.com/tipstricks/authorization-keys.aspx" target="_blank"><% = Resources.Resource.LearnMore %></a>
    </p>

    <div class="auth-service-block clearFix">
        <% foreach (AuthService service in AuthServiceList)
           { %>
            <div class="auth-service-item">
                <span class="auth-service-name"><%= service.Title %></span>
                <% if (service.Id != null)
                   { %>
                    <input id="<%= service.Id.Name %>" type="text" class="auth-service-id textEdit" placeholder="<%= service.Id.Title %>" value="<%= service.Id.Value %>"/>
                <% } %>
                <% if (service.Key != null)
                   { %>
                    <input id="<%= service.Key.Name %>" type="text" class="auth-service-key textEdit" placeholder="<%= service.Key.Title %>" value="<%= service.Key.Value %>"/>
                <% } %>
            </div>
        <% } %>
    </div>
    
    <div class="middle-button-container">
        <div class="errorBox display-none"></div>
        <a id="authKeysButtonSave" class="button blue" href="javascript:void(0);"><%= Resources.Resource.SaveButton %></a>
    </div>
</div>
<% } %>