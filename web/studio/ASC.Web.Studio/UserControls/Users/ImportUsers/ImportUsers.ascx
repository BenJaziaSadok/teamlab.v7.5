<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportUsers.ascx.cs"
    Inherits="ASC.Web.Studio.UserControls.Users.ImportUsers" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Management" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Statistics" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>


<!--[if IE]>
<style>
#wizard_users .checkall
{
    border:0;
    margin-top:13px;
}
</style>
<![endif]-->

<!--[if lte IE 8]>
<style>
.fistable {
   width:540px;
   padding: 0 12% 0 22%;
   cellpadding:20%;
   display:block;
}
#wizard_users #userList .userItem .name {
    width: 436px;
    padding-left:6px;
}
.fistable .desc {
    width:450px;
}
#wizard_users {
    width:750px;
    padding-left:6px;
}
#wizard_users #userList .userItem .check {
    padding:0 0 0 2px;
}
</style>
<![endif]-->

<!--[if IE 9]>
<style>
#wizard_users #userList .userItem .check input {
    margin:0 3px 0 2px;
}

#wizard_users #userList .userItem .name {
    width:438px;
}

#wizard_users #userList .userItem .name .firstname,
#wizard_users #userList .userItem .name .lastname
{
    float:left;
    padding-right:14px;
    vertical-align:top;
}
</style>
<![endif]-->

<div id="importUsers">
    <div class="blockUI blockMsg blockElement" id="upload"><img/></div>
        <div class="desc">
            <%= String.Format(ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("ImportContactsDescription"),"<span class=\"starStyle\">*</span>")%>
        </div>
        <div class="smallDesc"><span class="starStyle">*</span> <%= Resources.Resource.ImportContactsSmallDescription %></div>
        <div class="clearFix importUsers" id="panel">
            <div class="frame <%= IsMobileVersion?"framePad":"" %>">
                <iframe src="<%= SetupInfo.GetImportServiceUrl() %>" style="border: none; width: <%= IsMobileVersion?"100%":"505px" %>; height: 50px; overflow: hidden; filter: alpha(opacity=100);" frameborder="0" id="ifr"></iframe>
            </div>
        <div class="file" onclick="ImportUsersManager.ChangeVisionFileSelector();" title="<%= Resources.Resource.ImportContactsFromFile %>" style="display: <%= IsMobileVersion?"none":"block" %>;">
            <%= Resources.Resource.ImportContactsFromFile %>
            <div class="innerBox float-right">
            </div>
            <div class="fileSelector studio-action-panel" onclick="ImportUsersManager.ChangeVisionFileSelector();">
				<div class="corner-top right"> </div>
                <ul class="dropdown-content">
                    <li id="import_flatUploader"><a href="javascript:void(0);" class="dropdown-item"><%= Resources.Resource.ImportContactsFromFileCSV %></a></li>
                    <li id="import_msUploader"><a href="javascript:void(0);" class="dropdown-item"><%= Resources.Resource.ImportContactsFromFileMS %></a></li>
                </ul>
            </div>
        </div>
    </div>

    <div id="wizard_users">
        <div class="clearFix <%= IsMobileVersion?"mob":"" %>" id="addUserBlock">
            <div class="checkall">
                <input type="checkbox" id="checkAll" onclick="ImportUsersManager.ChangeAll()" />
            </div>
            <div class="nameBox">
                <div class="error" id="fnError">
                    <%= Resources.Resource.ImportContactsErrorFirstName %>
                </div>
                <input type="text" id="firstName" class="textEdit" maxlength="64" />
            </div>
            <div class="lastnameBox">
                <div class="error" id="lnError">
                   <%= Resources.Resource.ImportContactsErrorLastName %>
                </div>
                <input type="text" id="lastName" class="textEdit" maxlength="64" />
            </div>
            <div class="emailBox">
                <div class="error" id="eaError">
                    <%= Resources.Resource.ImportContactsErrorEmail %>
                </div>
                <input type="text" id="email" class="textEdit" maxlength="64" />
            </div>
            <div class="<%= IsMobileVersion?"mobBtn":"btn" %>">
                <div class="btnBox">
                    <a class="button gray" id="saveSettingsBtn" href="javascript:void(0);" onclick="ImportUsersManager.AddUser();">
                        <%= Resources.Resource.ImportContactsAddButton %></a>
                </div>
            </div>
        </div>
        <div class="restr  <%= IsMobileVersion?"mob":"" %>">
            <table id="userList">
            </table>
        </div>
    </div>
    <div class="desc">
        <input type="checkbox" id="importAsCollaborators" onclick="ImportUsersManager.ChangeInviteLinkType();"
            <%= EnableInviteLink ? "" : "disabled=\"disabled\" checked=\"checked\"" %> />
        <label for="importAsCollaborators">
            <%= Resources.Resource.InviteUsersAsCollaborators%>
        </label>
        <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'answerForHelpInviteGuests',position: 'fixed'});"></div>
    </div>
    <div class="buttons clearFix">
        <a class="button blue disable float-left impBtn" onclick="ImportUsersManager.ImportList();">
            <%=Resources.Resource.ImportContactsSaveButton%>
        </a>
        <a class="button gray disable buttonsImportContact cncBtn" onclick="ImportUsersManager.DeleteSelected();">
            <%= Resources.Resource.ImportContactsDeleteButton %>
        </a>
        <a class="button gray buttonsImportContact" onclick="ImportUsersManager.HideImportWindow();">
            <%= Resources.Resource.ImportContactsCancelButton %>
        </a>
        <div class="inviteLabel">
            <div class="invite-text">
                <%= Resources.Resource.ImportContactsInviteLinkLabel %>:
            </div>
            <div>
                <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'answerForHelpInv',position: 'fixed'});">
                </div>
                <div class="popup_helper" id="answerForHelpInv">
                    <p>
                        <%=Resources.Resource.ImportContactsInviteHint%></p>
                </div>
                <div class="popup_helper" id="answerForHelpInviteGuests">
                    <p>
                        <%=string.Format(Resources.Resource.NoteForInviteCollaborator, "<b>","</b>")%>
                        <a href="http://helpcenter.teamlab.com" target="_blank">
                            <%=Resources.Resource.LearnMore%></a></p>
                </div>
            </div>

            <div id="inviteLinkPanel" class="inputBox">
                <%if (!IsMobileVersion) { %>
                <a id="inviteLinkCopy" class="invite-copy-link link dotline small gray"><span><%= Resources.Resource.CopyToClipboard %></span></a>
                <%} %>
                <input id="inviteUserLink" type="text" readonly="readonly" class="textEdit" value="<%= EnableInviteLink ? InviteLink.GenerateLink(EmployeeType.User): InviteLink.GenerateLink(EmployeeType.Visitor) %>"
                    <% if (EnableInviteLink) { %> data-invite-user-link="<%=InviteLink.GenerateLink(EmployeeType.User)%>"
                    data-invite-visitor-link="<%=InviteLink.GenerateLink(EmployeeType.Visitor) %>"
                    <%} %> />
            </div>
        </div>
    </div>

</div>

<div id="importUserLimitPanel">
    <sc:Container ID="limitPanel" runat="server">
        <Header><%=Resources.Resource.ImportUserLimitTitle%></Header>
        <Body>
            <div class="tariff-limitexceed-users">
                <div class="header-base-medium">
                    <%=PeopleLimit > 0 ? String.Format(Resources.Resource.ImportUserLimitHeader, PeopleLimit) : Resources.Resource.ImportUserOverlimitHeader%>
                </div>
                <br/>
                <br/>
                <%=PeopleLimit > 0 ? Resources.Resource.ImportUserLimitReason : Resources.Resource.ImportUserOverlimitReason%>
                <br/>
                <br/>
                <%=Resources.Resource.ImportUserLimitDecision%>
            </div>
            <div class="middle-button-container">
                <div id="cnfrm_action_conainer">
                    <a class="blue button" onclick="ImportUsersManager.ConfirmationLimit();">
                        <%=Resources.Resource.ImportUserLimitOkButtons%>
                    </a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray" onclick="ImportUsersManager.HideImportUserLimitPanel();">
                        <%= Resources.Resource.ImportContactsCancelButton %>
                    </a>
                </div>
                <div id="cnfrm_action_loader" class="display-none">
                    <div class="text-medium-describe"><%=Resources.Resource.PleaseWaitMessage%></div>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
                </div>
            </div>
        </Body>
    </sc:Container>
</div>

<table id="donor" class="display-none">
    <tr>
        <td class="fistable">
            <div class="desc">
                <%= Resources.Resource.ImportContactsFirstable %>
            </div>
        </td>
    </tr>
</table>

<sc:Container ID="icon" runat="server">
    <Header><%= Resources.Resource.ImportContactsErrorHeader %></Header>
    <Body>
        <div id="infoMessage">
        </div>
        <div class="clearFix okImportUsers">
            <a class="button blue" onclick="ImportUsersManager.HideInfoWindow('okcss');">
                <%= Resources.Resource.ImportContactsOkButton %>
            </a>
        </div>
    </Body>
</sc:Container>
<div class="blockUpload display-none" id="blockProcess"></div>