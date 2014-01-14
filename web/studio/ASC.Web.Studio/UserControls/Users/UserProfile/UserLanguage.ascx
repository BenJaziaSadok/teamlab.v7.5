<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserLanguage.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserLanguage" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="System.Globalization" %>
<div class="field">
    <span class="field-title describe-text "><%=Resource.Language%>:</span>
    <span class="field-value usrLang <%= GetCurrentLanguage().Name %>">
        <span class="val"><%= GetCurrentLanguage().DisplayName %></span>
        <span class="selector"></span>        
    </span>
    <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'NotFoundLanguage'});"></div>
    <div class="popup_helper" id="NotFoundLanguage">
        <p>
            <%=string.Format(Resources.Resource.NotFoundLanguage, "<a href=\"mailto:documentation@teamlab.com\">", "</a>")%>
            <a href="http://helpcenter.teamlab.com/guides/become-translator.aspx" target="_blank">
                <%=Resources.Resource.LearnMore%></a></p>
</div>
</div>

<div id="languageMenu" class="languageMenu studio-action-panel">
    <div class="corner-top left"></div>
    <ul class="options dropdown-content">
    <% foreach (var ci in SetupInfo.EnabledCultures)
       { %>
    <li class="option dropdown-item <%= ci.Name %> <%= String.Equals(CultureInfo.CurrentCulture.Name, ci.Name) ? "selected" : "" %>" data="<%= ci.Name %>">
        <a><%= ci.DisplayName %></a>
    </li>
    <% } %>
    </ul>
</div>