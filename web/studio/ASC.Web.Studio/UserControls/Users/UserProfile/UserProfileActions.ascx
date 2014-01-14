<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserProfileActions.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserProfileActions" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="Resources" %>

<% if (HasActions)
   { %>
<div id="userMenu" class="menu-small"></div>
        
<div id="actionMenu" class="studio-action-panel" data-id="<%= profileHelper.UserInfo.ID %>" data-email="<%= profileHelper.UserInfo.Email %>" 
    data-admin="<%= IsAdmin.ToString().ToLower()%>" data-name="<%= profileHelper.UserInfo.DisplayUserName() %>">
    <div class="corner-top left"></div>
    <ul class="dropdown-content">
        <% if (Actions.AllowEdit)
           { %>
        <li class="edit-user <%= (profileHelper.UserInfo.Status != EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a title="<%= Resource.EditButton %>" href="<%= profileEditLink %>"
                class="dropdown-item">
                <%= Resource.EditButton %>
            </a>
        </li>
        <% }
           if (Actions.AllowAddOrDelete)
           { %>
        <li class="enable-user <%= (profileHelper.UserInfo.Status == EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a class="dropdown-item"
                title="<%= CustomNamingPeople.Substitute<Resource>("EnableUserHelp").HtmlEncode() %>">
                <%= Resource.EnableUserButton %>
            </a>
        </li>
        <% }
           if (Actions.AllowEdit && profileHelper.UserInfo.ActivationStatus == EmployeeActivationStatus.Activated)
           { %>
        <li class="psw-change <%= (profileHelper.UserInfo.Status != EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a title="<%= Resource.PasswordChangeButton %>"
                class="dropdown-item">
                <%= Resource.PasswordChangeButton %>
            </a>
        </li>
        <% }
           if (Actions.AllowEdit && profileHelper.UserInfo.ActivationStatus == EmployeeActivationStatus.Activated)
           { %>
        <li class="email-change <%= (profileHelper.UserInfo.Status != EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a title="<%= Resource.EmailChangeButton %>"
                class="dropdown-item">
                <%= Resource.EmailChangeButton %>
            </a>
        </li>
        <% }
           if (Actions.AllowEdit && profileHelper.UserInfo.ActivationStatus != EmployeeActivationStatus.Activated)
           { %>
        <li class="email-activate <%= (profileHelper.UserInfo.Status != EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a title="<%= Resource.ActivateEmailAgain %>"
                class="dropdown-item">
                <%= Resource.ActivateEmailAgain %>
            </a>
        </li>
        <% }
           if (Actions.AllowEdit && UserHasAvatar && !ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
           { %>
        <li class="edit-photo">
            <a class="dropdown-item"
                title="<%= Resource.EditThumbnailPhoto %>">
                <%= Resource.EditThumbnailPhoto %>
            </a> 
        </li>
        <% }
           if (!MyStaff && Actions.AllowAddOrDelete)
           { %>
        <li class="disable-user <%= (profileHelper.UserInfo.Status != EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a class="dropdown-item"
                title="<%= CustomNamingPeople.Substitute<Resource>("DisableUserHelp").HtmlEncode() %>">
                <%= Resource.DisableUserButton %>
            </a>
        </li>
        <% }
           if (MyStaff && !profileHelper.UserInfo.IsOwner() && !CoreContext.Configuration.YourDocs)
           { %>
        <li class="delete-user">
            <a class="dropdown-item" title="<%= Resource.DeleteProfileButton %>">
                <%= Resource.DeleteProfileButton %>
            </a>
        </li>
        <% }
           if (Actions.AllowAddOrDelete)
           { %>
        <li class="delete-self <%= (profileHelper.UserInfo.Status == EmployeeStatus.Terminated) ? "" :  "display-none"%>">
            <a class="dropdown-item" title="<%= Resource.DeleteSelfProfile %>">
                <%= Resource.DeleteSelfProfile %>
            </a>
        </li>
        <% } %>
    </ul>
</div>

<% if (Actions.AllowAddOrDelete)
   { %>
<asp:PlaceHolder ID="_phConfirmationDeleteUser" runat="server"></asp:PlaceHolder>
    <% } %>
<% } %>