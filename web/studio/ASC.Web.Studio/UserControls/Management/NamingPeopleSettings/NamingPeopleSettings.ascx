<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NamingPeopleSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.NamingPeopleSettings" %>

<div class="clearFix">
    <div class="settings-block">
        <div class="header-base clearFix" id="namingPeopleTitle">
            <%=Resources.Resource.NamingPeopleSettings%>
        </div>

        <div id="studio_namingPeopleInfo">
        </div>
        <div id="studio_namingPeopleBox">
            
            <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
            <div class="small-button-container clearFix">
                <a id="saveNamingPeopleBtn" class="button blue" href="javascript:void(0);"><%=Resources.Resource.SaveButton %></a>
            </div>
        </div>
    </div>
    <div class="settings-help-block">
        <p><%=String.Format(Resources.Resource.HelpAnswerTeamTemplate, "<br />", "<b>", "</b>")%></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#CustomizingPortal_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
    </div>
</div>