<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="VersionSettings.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Management.VersionSettings.VersionSettings" %>
<div class="clearFix">
  <div class="settings-block">    
    <div class="header-base clearFix" id="versionSettingsTitle">
            <%=Resources.Resource.StudioVersionSettings%>
    </div>
    <div class="clearFix" id="studio_versionSetting">
        <div class="clearFix versionSettingBox">
            <div class="clearFix">
                <div class="versionSettingName" id="versionSelector">
                    <% foreach (var tenantVersion in ASC.Core.CoreContext.TenantManager.GetTenantVersions())
                       {%>
                    <div class="clearFix">
                        <input type="radio" name="version" id="radio<%=tenantVersion.Id%>" value="<%=tenantVersion.Id%>"
                            <%=ASC.Core.CoreContext.TenantManager.GetCurrentTenant(false).Version==tenantVersion.Id?"checked=\"checked\"":"" %>
                             />
                        <%if (ASC.Core.CoreContext.TenantManager.GetCurrentTenant(false).Version == tenantVersion.Id)
                          {%>
                        <label for="radio<%= tenantVersion.Id %>">
                            <strong>
                                <%= GetLocalizedName(tenantVersion.Name) %>
                            </strong>
                        </label>
                        <% }
                          else
                          {%>
                        <label for="radio<%= tenantVersion.Id %>">
                            <%= GetLocalizedName(tenantVersion.Name)%>
                        </label>
                        <%} %>
                    </div>
                    <%} %>
                </div>
            </div>
            <div id="studio_versionSetting_info">
            </div>
            <div class="clearFix versionSettinButton">
                <a class="button blue float-left" onclick="StudioVersionManagement.SwitchVersion();"
                    href="javascript:void(0);">
                    <%=Resources.Resource.SaveButton%></a>
            </div>
        </div>
    </div>
 </div>
 <div class="settings-help-block">
        <p><%=String.Format(Resources.Resource.HelpAnswerPortalVersion,"<br /> ", " <b>", "</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
 </div>  
</div>