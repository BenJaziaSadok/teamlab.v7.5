<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GreetingSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.GreetingSettings" %>

<div class="clearFix">
    <div class="settings-block">
        <div class="header-base greetingTitle clearFix">
            <%=Resources.Resource.GreetingSettingsTitle%>
        </div>
        <div id="studio_setInfGreetingSettingsInfo"></div>
        <div id="studio_greetingSettingsBox" class="clearFix">
            <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
            <div class="small-button-container clearFix">
                <a id="saveGreetSettingsBtn" class="button blue"  href="javascript:void(0);" ><%=Resources.Resource.SaveButton %></a>
                <span class="splitter-buttons"></span>
                <a id="restoreGreetSettingsBtn" class="button gray" href="javascript:void(0);" ><%=Resources.Resource.RestoreDefaultButton%></a>
            </div>
        </div>
    </div>
    <div class="settings-help-block">
        <p><%=String.Format(Resources.Resource.HelpAnswerGreetingSettings, "<br />","<b>","</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#CustomizingPortal_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
    </div>
</div>