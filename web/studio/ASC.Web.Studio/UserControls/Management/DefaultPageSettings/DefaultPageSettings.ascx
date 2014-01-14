<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultPageSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.DefaultPageSettings" %>
<div class="clearFix">
  <div class="settings-block">
    <div class="header-base clearFix">
        <div class="title">
            <%= Resources.Resource.DefaultPageSettingsTitle %>
        </div>
    </div>
    <div id="studio_defaultPegeSettings">
    </div>
    <div>
        <div class="clearFix">
            <% foreach (var defaultPage in DefaultPages) %>
            <% { %>
            <div class="clearFix">
                <input id="chk_studio_default_<%= defaultPage.ProductName %>"
                    value="<%= defaultPage.ProductID%>" type="radio" name="defaultPage"
                    <%=(defaultPage.IsSelected?"checked=\"checked\"":"")%>/>
                <label for="chk_studio_default_<%= defaultPage.ProductName %>">
                    <%= defaultPage.DisplayName %>
                </label>
            </div>
            <% } %>
        </div>
        <div class="clearFix admMessSaveSettings">
            <a class="button blue float-left" onclick="DefaultPage.SaveSettings(); return false;" href="javascript:void(0);">
                <%=Resources.Resource.SaveButton %>
            </a>
        </div>
    </div>
  </div>
  <div class="settings-help-block">
        <p>
            <%=String.Format(Resources.Resource.HelpAnswerDefaultPageSettings, "<b>", "</b>")%>
        </p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank">
            <%=Resources.Resource.LearnMore%>
        </a>
 </div>  
</div>