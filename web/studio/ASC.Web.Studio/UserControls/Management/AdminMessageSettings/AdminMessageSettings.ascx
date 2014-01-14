<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminMessageSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.AdminMessageSettings" %>
<div class="clearFix">
  <div class="settings-block">
    <div class="header-base clearFix" id="admMessSettingsTitle">
        <div class="title">
            <%= Resources.Resource.AdminMessageSettingsTitle %>
        </div>
    </div>
    <div id="studio_admMessSettingsInfo">
    </div>
    <div id="studio_admMessSettings">
        <div class="clearFix">    
                <div class="clearFix">
                    <input id="chk_studio_admMess" type="radio" <%=(_studioAdmMessNotifSettings.Enable?"checked=\"checked\"":"")%> 
                        name="ShowingAdmMessages" />
                     <label for="chk_studio_admMess">
                     <%= Resources.Resource.AdminMessageSettingsEnable %></label>
                </div>
                <div class="clearFix">
                    <input id="dont_chk_studio_admMess" type="radio" <%=(!_studioAdmMessNotifSettings.Enable?"checked=\"checked\"":"")%> 
                        name="ShowingAdmMessages" />
                    <label for="dont_chk_studio_admMess">
                        <%=Resources.Resource.DisableUserButton%></label>
                </div>
        </div>
        <div class="clearFix admMessSaveSettings">
            <a class="button blue float-left" onclick="AdmMess.SaveSettings(); return false;"
                href="javascript:void(0);">
                <%=Resources.Resource.SaveButton %></a>
        </div>
    </div>
  </div>
  <div class="settings-help-block">
        <p><%=String.Format(Resources.Resource.HelpAnswerAdminMessSettings,"<br />", "<b>", "</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
 </div>  
</div>    
