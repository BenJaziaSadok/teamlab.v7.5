<%@ Assembly Name="ASC.Web.People" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/products/people/Masters/PeopleBaseTemplate.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ASC.Web.People.Default" EnableViewState="false" %>

<%@ Register Assembly="ASC.Web.Studio" Namespace=" ASC.Web.Studio.Controls.Common" TagPrefix="sc" %>

<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Data.Storage" %>
<%@ Import Namespace="ASC.Web.People.Core" %>
<%@ Import Namespace="ASC.Web.Studio" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.People.Resources" %>

<asp:Content ID="_peopleContent" runat="server" ContentPlaceHolderID="PeoplePageContent">
    <div class="clearFix profile-title header-with-menu">
        <span class="header text-overflow"><%= Resources.Resource.People %></span>
        <% if (IsAdmin) { %>
        <span class="menu-small" style="display:none;"></span>
        <% } %>
    </div>

    <% if (IsAdmin) { %>
    <div id="actionGroupMenu" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <li><a class="dropdown-item update-group"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("DepEditHeader").HtmlEncode()%></a></li>
            <li><a class="dropdown-item delete-group"><%= PeopleResource.DeleteButton %></a></li>
        </ul>
    </div>
    <% } %>

    
    <div id="filterContainer">
        <div id="peopleFilter"></div>
    </div>

    <br />

    <div id="peopleContent">
        <% if (IsAdmin) { %>
        <ul id="peopleHeaderMenu" class="clearFix contentMenu contentMenuDisplayAll">
            <li class="menuAction menuActionSelectAll menuActionSelectLonely">
                <div class="menuActionSelect">
                    <input id="mainSelectAll" type="checkbox" title="<%= PeopleResource.SelectAll %>" onclick="ASC.People.PeopleController.selectAll(this);"/>
                </div>
            </li>
            <li class="menuAction menuChangeType" title="<%= PeopleResource.ChangeType %>">
                <span><%= PeopleResource.ChangeType %></span>
                <div class="down_arrow"></div>
            </li>
            <li class="menuAction menuChangeStatus" title="<%= PeopleResource.ChangeStatus %>">
                <span><%= PeopleResource.ChangeStatus %></span>
                <div class="down_arrow"></div>
            </li>
            <li class="menuAction menuSendInvite" title="<%= PeopleResource.LblSendActivation%>">
                <span><%= PeopleResource.LblSendActivation%></span>
            </li>
            <li class="menuAction menuRemoveUsers" title="<%= PeopleResource.DeleteButton%>">
                <span><%= PeopleResource.DeleteButton%></span>
            </li>
            <li class="menu-action-simple-pagenav">
            </li>
            <li class="menu-action-checked-count">
                <span></span>
                <a id="mainDeselectAll" class="link dotline small">
                    <%= PeopleResource.DeselectAll %>
                </a>
            </li>
            <li class="menu-action-on-top">
                <a class="on-top-link">
                    <%= PeopleResource.OnTop%>
                </a>
            </li>
        </ul>
        <div class="header-menu-spacer" style="display: none;"></div>
        <% } %>

        <table id="peopleData" class="table-list height48" cellpadding="7" cellspacing="0">
            <tbody>
            </tbody>
        </table>


        <table id="tableForPeopleNavigation" cellpadding="0" cellspacing="0" border="0">
            <tbody>
                <tr>
                    <td>
                        <sc:PageNavigator ID="peoplePageNavigator" EntryCount="0" CurrentPageNumber="1" EntryCountOnPage="25" VisibleOnePage="false" runat="server" />
                    </td>
                    <td style="text-align:right;">
                        <span class="gray-text"><%= PeopleResource.TotalCount %>:&nbsp;</span>
                        <span class="gray-text" id="totalUsers"></span>
                        <span class="page-nav-info">
                            <span class="gray-text"><%= PeopleResource.ShowOnPage %>:&nbsp;</span>
                            <select id="countOfRows" class="top-align">
                                <option value="25">25</option>
                                <option value="50">50</option>
                                <option value="75">75</option>
                                <option value="100">100</option>
                            </select>
                        </span>
                    </td>
                </tr>
            </tbody>
        </table>

    </div>

    <asp:PlaceHolder runat="server" ID="emptyScreen"></asp:PlaceHolder>

    <div id="peopleActionMenu" class="studio-action-panel">
        <div class="corner-top right"></div>
        <ul class="dropdown-content">
            <% if (Actions.AllowEdit) { %>
            <li><a class="edit-profile dropdown-item"><%= PeopleResource.LblEdit %></a></li>
            <% } %>
            <% if (Actions.AllowAddOrDelete) { %>
            <li><a class="enable-profile dropdown-item"><%= PeopleResource.EnableUserButton%></a></li>
            <% } %>
            <% if (Actions.AllowEdit) { %>
            <li><a class="change-password dropdown-item"><%= PeopleResource.LblChangePassword %></a></li>
            <% } %>
            <li><a class="change-email dropdown-item"><%= PeopleResource.LblChangeEmail %></a></li>
            <li><a class="email-activation dropdown-item"><%= PeopleResource.LblSendActivation%></a></li>
            <% if (Actions.AllowAddOrDelete) { %>
            <li><a class="block-profile dropdown-item"><%= PeopleResource.DisableUserButton%></a></li>
            <li><a class="delete-profile dropdown-item"><%= PeopleResource.LblDeleteProfile %></a></li>
            <% } %>
        </ul>
    </div>

    <div id="confirmationDeleteDepartmentPanel" style="display: none;">
        <sc:Container id="_confirmationDeleteDepartmentPanel" runat="server">
            <header>
                <%= PeopleResource.Confirmation%>
            </header>
            <body>
                <div class="confirmationAction">
                </div>
                <div class="middle-button-container">
                    <a class="button blue middle"><%= PeopleResource.LblOKButton%></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle" onclick="jq.unblockUI();"><%= PeopleResource.LblCancelButton%></a>
                </div>
            </body>
        </sc:Container>
    </div>

    <asp:PlaceHolder ID="userEmailChange" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="userPwdChange" runat="server"></asp:PlaceHolder>
    <asp:PlaceHolder ID="userConfirmationDelete" runat="server"></asp:PlaceHolder>

    <% if (IsAdmin) { %>
    <div id="changeTypePanel" class="studio-action-panel group-actions">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <li>
                <a class="dropdown-item" data-type="<%= (int)EmployeeType.User %>">
                    <%= CustomNamingPeople.Substitute<Resources.Resource>("User").HtmlEncode() %>
                </a>
            </li>
            <li>
                <a class="dropdown-item" data-type="<%= (int)EmployeeType.Visitor %>">
                    <%= CustomNamingPeople.Substitute<Resources.Resource>("Guest").HtmlEncode() %>
                </a>
            </li>
        </ul>
    </div>

    <div id="changeTypeDialog" style="display: none;">
        <sc:Container id="_changeTypeDialog" runat="server">
            <header>
                <%= PeopleResource.ChangeTypeDialogHeader %>
            </header>
            <body>
                <div id="userTypeInfo">
                    <div class="tariff-limit"></div>
                    <div>
                        <%= PeopleResource.ChangeTypeDialogConstraint %>
                    </div>
                    <div>
                        <%= CustomNamingPeople.Substitute<PeopleResource>("ChangeTypeDialogToUser").HtmlEncode() %>
                    </div>
                </div>
                <div id="visitorTypeInfo">
                    <div>
                        <%= CustomNamingPeople.Substitute<PeopleResource>("ChangeTypeDialogToGuest").HtmlEncode() %>
                    </div>
                </div>
                <div>
                    <%= PeopleResource.ChangeTypeDialogRestriction %>
                </div>
                <a class="link dotline showBtn">
                    <%= PeopleResource.ShowSelectedUserList %>
                </a>
                <a class="link dotline hideBtn display-none">
                    <%= PeopleResource.HideSelectedUserList %>
                </a>
                <div class="user-list-for-group-operation display-none">
                </div>
                <div class="loader-popup display-none">
                    <%= Resources.Resource.PleaseWaitMessage %>
                </div>
                <div class="error-popup display-none"></div>
                <div class="big-button-container">
                    <a class="button blue middle"><%= PeopleResource.LblOKButton%></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle"><%= PeopleResource.LblCancelButton%></a>
                </div>
            </body>
        </sc:Container>
    </div>

    <div id="changeStatusPanel" class="studio-action-panel group-actions">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <li>
                <a class="dropdown-item" data-status="<%= (int)EmployeeStatus.Active %>">
                    <%= PeopleResource.LblActive %>
                </a>
            </li>
            <li>
                <a class="dropdown-item" data-status="<%= (int)EmployeeStatus.Terminated %>">
                    <%= PeopleResource.LblTerminated %>
                </a>
            </li>
        </ul>
    </div>
    
    <div id="changeStatusDialog" style="display: none;">
        <sc:Container id="_changeStatusDialog" runat="server">
            <header>
                <%= PeopleResource.ChangeStatusDialogHeader %>
            </header>
            <body>
                <div id="activeStatusInfo">
                    <div class="tariff-limit"></div>
                    <div>
                        <%= PeopleResource.ChangeStatusDialogConstraint %>
                    </div>
                    <div>
                        <%= PeopleResource.ChangeStatusDialogToActive %>
                    </div>
                </div>
                <div id="terminateStatusInfo" class="display-none">
                    <div>
                        <%= PeopleResource.ChangeStatusDialogToTerminate %>
                    </div>
                </div>
                <div>
                    <%= PeopleResource.ChangeStatusDialogRestriction %>
                </div>
                <a class="link dotline showBtn">
                    <%= PeopleResource.ShowSelectedUserList %>
                </a>
                <a class="link dotline hideBtn display-none">
                    <%= PeopleResource.HideSelectedUserList %>
                </a>
                <div class="user-list-for-group-operation display-none">
                </div>
                <div class="loader-popup display-none">
                    <%= Resources.Resource.PleaseWaitMessage %>
                </div>
                <div class="error-popup display-none"></div>
                <div class="big-button-container">
                    <a class="button blue middle"><%= PeopleResource.LblOKButton%></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle"><%= PeopleResource.LblCancelButton%></a>
                </div>
            </body>
        </sc:Container>
    </div>

    <div id="resendInviteDialog" style="display: none;">
        <sc:Container id="_resendInviteDialog" runat="server">
            <header>
                <%= PeopleResource.ResendInviteDialogHeader %>
            </header>
            <body>
                <div>
                    <%= PeopleResource.ResendInviteDialogTargetUsers %>
                </div>
                <div>
                    <%= PeopleResource.ResendInviteDialogAfterActivation %>
                </div>
                <a class="link dotline showBtn">
                    <%= PeopleResource.ShowSelectedUserList %>
                </a>
                <a class="link dotline hideBtn display-none">
                    <%= PeopleResource.HideSelectedUserList %>
                </a>
                <div class="user-list-for-group-operation display-none">
                </div>
                <div class="loader-popup display-none">
                    <%= Resources.Resource.PleaseWaitMessage %>
                </div>
                <div class="error-popup display-none"></div>
                <div class="big-button-container">
                    <a class="button blue middle"><%= PeopleResource.LblOKButton%></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle"><%= PeopleResource.LblCancelButton%></a>
                </div>
            </body>
        </sc:Container>
    </div>

    <div id="deleteUsersDialog" style="display: none;">
        <sc:Container id="_deleteUsersDialog" runat="server">
            <header>
                <%= PeopleResource.DeleteUserProfiles %>
            </header>
            <body>
               <div>
                    <%= PeopleResource.DeleteUsersDescription %>
                </div>
                <a class="link dotline showBtn">
                    <%= PeopleResource.ShowSelectedUserList %>
                </a>
                <a class="link dotline hideBtn display-none">
                    <%= PeopleResource.HideSelectedUserList %>
                </a>
                <div class="user-list-for-group-operation display-none">
                </div>
                <div class="loader-popup display-none">
                    <%= Resources.Resource.PleaseWaitMessage %>
                </div>
                <div class="error-popup display-none"></div>
                <div class="big-button-container">
                    <a class="button blue middle"><%= PeopleResource.LblOKButton%></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle"><%= PeopleResource.LblCancelButton%></a>
                </div>
            </body>
        </sc:Container>
    </div>
    <% } %>
</asp:Content>