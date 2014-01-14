<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SmsValidationSettings.ascx.cs"
 Inherits="ASC.Web.Studio.UserControls.Management.SmsValidationSettings" %>
<%@ Import Namespace="ASC.Web.Studio.Core.SMS" %>
<%@ Import Namespace="Resources" %>

<div class="clearFix">
     <div class="settings-block">
         <div class="header-base">
             <%= Resource.SmsAuthTitle %>
         </div>
         <div id="studio_smsValidationSettingsInfo"></div>
         
         <div class="sms-validation-settings">
            <asp:PlaceHolder runat="server" ID="SmsBuyHolder"></asp:PlaceHolder>
            <br />
            <br />
            <div class="clearFix">
                <input type="radio" id="chk2FactorAuthEnable" name="chk2FactorAuth" <%= StudioSmsNotificationSettings.Enable ? "checked=\"checked\"" : "" %> />
                <label for="chk2FactorAuthEnable">
                        <%= Resource.EnableUserButton %></label>
            </div>
             <div class="clearFix">
                 <input type="radio" id="chk2FactorAuthDisable" name="chk2FactorAuth" <%= !StudioSmsNotificationSettings.Enable ? "checked=\"checked\"" : "" %> />
                 <label for="chk2FactorAuthDisable">
                     <%= Resource.DisableUserButton %></label>
             </div>
            <div class="clearFix">
                <a id="chk2FactorAuthSave" class="button blue" >
                    <%= Resource.SaveButton %></a>
            </div>
        </div>
     </div>
     <div class="settings-help-block">
         <p>
             <%= String.Format(Resource.SmsAuthDescription, "<b>", "</b>", "<br/>", "<br/>", "<b>", "</b>")%>
         </p>
     </div>
</div>