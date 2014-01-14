<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ColorThemes.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ColorThemes" %>
<%@ Import Namespace="Resources" %>

<div class="clearFix">
    <div id="colorThemeBlock" class="settings-block">
        <div class="header-base clearFix"><%=Resources.Resource.ColorThemesTitle %></div>
        <% foreach (PortalColorTheme theme in ColorThemesList)
           {%>
            <div class="clearFix">
                <input id="chk-<% =theme.Value %>" value="<%= theme.Value %>" type="radio" <% = (theme.Value.Equals(ChosenTheme) ? "checked=\"checked\"":"") %> name="colorTheme"/>
                <label for="chk-<% =theme.Value %>"><% =theme.Title %></label>
            </div>
        <% } %>     
        <div class="preview-theme-image <%= ChosenTheme%>">
        </div>
         <div class="clearFix">
            <a class="button blue" id="saveColorThemeBtn">
                <%=Resources.Resource.SaveButton %></a>
        </div>
    </div>
    <div class="settings-help-block">
        <p><%=String.Format(Resources.Resource.HelpAnswerColorTheme,"<br />", "<b>", "</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx" target="_blank"><%=Resources.Resource.LearnMore%></a>
    </div>
</div>