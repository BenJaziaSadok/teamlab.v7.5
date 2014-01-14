<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NavigationSidePanel.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.NavigationSidePanel" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div class="page-menu">
    <ul class="menu-actions">
        <li id="menuCreateNewButton" class="menu-main-button without-separator <%= MobileVer ? "big" : "middle" %>" title="<%= CRMCommonResource.CreateNew %>">
            <span class="main-button-text"><%= CRMCommonResource.CreateNew %></span>
            <span class="white-combobox"></span>
        </li>
        <% if (!MobileVer) %>
        <% { %>
        <li id="menuOtherActionsButton" class="menu-gray-button" title="<%=Resources.Resource.MoreActions %>">
            <span class="btn_other-actions">...</span>
        </li>
        <% } %>
    </ul>

    <%-- popup windows --%>
    <div id="createNewButton" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <li><a class="dropdown-item" href="default.aspx?action=manage"><%= CRMContactResource.Company %></a></li>
            <li><a class="dropdown-item" href="default.aspx?action=manage&type=people"><%= CRMContactResource.Person %></a></li>
            <li><a id="menuCreateNewTask" class="dropdown-item" href="javascript:void(0);"><%= CRMTaskResource.Task %></a></li>
            <li><a id="menuCreateNewDeal" class="dropdown-item" href="deals.aspx?action=manage"><%= CRMDealResource.Deal %></a></li>
            <li><a class="dropdown-item" href="cases.aspx?action=manage"><%= CRMCasesResource.Case %></a></li>
        </ul>
    </div>

    <% if (!MobileVer) %>
    <% { %>
    <div id="otherActions" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <li><a id="importContactLink" class="dropdown-item" href="default.aspx?action=import"><%= CRMContactResource.ImportContacts%></a></li>
            <li><a id="importTasksLink" class="dropdown-item" href="tasks.aspx?action=import"><%= CRMTaskResource.ImportTasks %></a></li>
            <li><a id="importDealsLink" class="dropdown-item" href="deals.aspx?action=import"><%= CRMDealResource.ImportDeals %></a></li>
            <li><a id="importCasesLink" class="dropdown-item" href="cases.aspx?action=import"><%= CRMCasesResource.ImportCases %></a></li>

            <% if (CRMSecurity.IsAdmin) %>
            <% { %>
            <li><a id="exportListToCSV" class="dropdown-item"><%= CRMCommonResource.ExportCurrentListToCsvFile %></a></li>
            <li><a id="openListInEditor" class="dropdown-item"><%= CRMCommonResource.OpenCurrentListInTheEditor%></a></li>
            <% } %>
        </ul>
    </div>
    <% } %>

    <ul class="menu-list">
        <li class="menu-item sub-list<%if(CurrentPage=="contacts"){%> active currentCategory<%}else
            if (CurrentPage=="companies" || CurrentPage=="persons"){%> currentCategory<%}%>">
            <div class="category-wrapper">
                <span class="expander"></span>
                <a class="menu-item-label outer-text text-overflow" href=".">
                    <span class="menu-item-icon group"></span><span class="menu-item-label inner-text"><%=CRMContactResource.Contacts%></span>
                </a>
                <span id="feed-new-contacts-count" class="feed-new-count"></span>
            </div>
            <ul class="menu-sub-list">
                <li class="menu-sub-item<%if(CurrentPage=="companies"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow companies-menu-item" href="default.aspx#eyJpZCI6InNvcnRlciIsInR5cGUiOiJzb3J0ZXIiLCJwYXJhbXMiOiJleUpwWkNJNkltTnlaV0YwWldRaUxDSmtaV1lpT25SeWRXVXNJbVJ6WXlJNmRISjFaU3dpYzI5eWRFOXlaR1Z5SWpvaVpHVnpZMlZ1WkdsdVp5SjkifTt7ImlkIjoicGVyc29uIiwidHlwZSI6ImNvbWJvYm94IiwicGFyYW1zIjoiZXlKMllXeDFaU0k2SW1OdmJYQmhibmtpTENKMGFYUnNaU0k2SWlBZ0lDQWdJQ0FnSUNCRGIyMXdZVzVwWlhNZ0lDQWdJQ0FpTENKZlgybGtJam80TnpJd01UUjkifQ==">
                        <span class="menu-item-label inner-text"><%=CRMContactResource.Companies%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="persons"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow persons-menu-item" href="default.aspx#eyJpZCI6InNvcnRlciIsInR5cGUiOiJzb3J0ZXIiLCJwYXJhbXMiOiJleUpwWkNJNkltTnlaV0YwWldRaUxDSmtaV1lpT25SeWRXVXNJbVJ6WXlJNmRISjFaU3dpYzI5eWRFOXlaR1Z5SWpvaVpHVnpZMlZ1WkdsdVp5SjkifTt7ImlkIjoicGVyc29uIiwidHlwZSI6ImNvbWJvYm94IiwicGFyYW1zIjoiZXlKMllXeDFaU0k2SW5CbGNuTnZiaUlzSW5ScGRHeGxJam9pSUNBZ0lDQWdJQ0FnSUZCbGNuTnZibk1nSUNBZ0lDQWlMQ0pmWDJsa0lqbzROekl3TVRSOSJ9">
                        <span class="menu-item-label inner-text"><%=CRMContactResource.Persons%></span>
                    </a>
                </li>
            </ul>
        </li>
        <li class="menu-item none-sub-list<%if(CurrentPage=="tasks"){%> active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="tasks.aspx">
                <span class="menu-item-icon tasks"></span><span class="menu-item-label inner-text"><%=CRMCommonResource.TaskModuleName%></span>
            </a>
            <span id="feed-new-crmTasks-count" class="feed-new-count"></span>
        </li>
        <li class="menu-item  none-sub-list<%if(CurrentPage=="deals"){%> active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="deals.aspx">
                <span class="menu-item-icon opportunities"></span><span class="menu-item-label inner-text"><%=CRMCommonResource.DealModuleName%></span>
            </a>
            <span id="feed-new-deals-count" class="feed-new-count"></span>
        </li>
        <li class="menu-item  none-sub-list<%if(CurrentPage=="cases"){%> active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="cases.aspx">
                <span class="menu-item-icon cases"></span><span class="menu-item-label inner-text"><%=CRMCommonResource.CasesModuleName%></span>
            </a>
            <span id="feed-new-cases-count" class="feed-new-count"></span>
        </li>
        <% if (CRMSecurity.IsAdmin) %>
        <% { %>
        <li id="menuSettings" class="menu-item add-block sub-list<%if(CurrentPage.IndexOf("settings_", StringComparison.Ordinal)>-1){%> currentCategory<%}%>">
            <div class="category-wrapper">
                <span class="expander"></span>
                <a class="menu-item-label outer-text text-overflow<%if(CurrentPage.IndexOf("settings_", System.StringComparison.Ordinal)==-1){%> gray-text<%}%>" href="settings.aspx">
                    <span class="menu-item-icon settings"></span><span class="menu-item-label inner-text"><%=CRMCommonResource.SettingModuleName%></span>
                </a>
            </div>
            <ul class="menu-sub-list">
                <li class="menu-sub-item<%if(CurrentPage=="settings_common"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=common">
                        <span class="menu-item-label inner-text"><%=CRMSettingResource.CommonSettings%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_custom_field"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=custom_field">
                        <span class="menu-item-label inner-text"><%=CRMSettingResource.CustomFields%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_deal_milestone"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=deal_milestone">
                        <span class="menu-item-label inner-text"><%=CRMDealResource.DealMilestone%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_contact_stage"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=contact_stage">
                        <span class="menu-item-label inner-text"><%=CRMContactResource.ContactStages%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_contact_type"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=contact_type">
                        <span class="menu-item-label inner-text"><%=CRMContactResource.ContactType%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_task_category"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=task_category">
                        <span class="menu-item-label inner-text"><%=CRMTaskResource.TaskCategories%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_history_category"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=history_category">
                        <span class="menu-item-label inner-text"><%=CRMSettingResource.HistoryCategories%></span>
                    </a>
                </li>
                <li class="menu-sub-item<%if(CurrentPage=="settings_tag"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=tag">
                        <span class="menu-item-label inner-text"><%=CRMCommonResource.Tags%></span>
                    </a>
                </li>
                <li id="menuCreateWebsite" class="menu-sub-item<%if(CurrentPage=="settings_web_to_lead_form"){%> active<%}%>">
                    <a class="menu-item-label outer-text text-overflow" href="settings.aspx?type=web_to_lead_form">
                        <span class="menu-item-label inner-text"><%=CRMSettingResource.WebToLeadsForm%></span>
                    </a>
                </li>
                <li id="menuAccessRights" class="menu-sub-item">
                    <a class="menu-item-label outer-text text-overflow" href="<%=VirtualPathUtility.ToAbsolute("~/management.aspx")+ "?type=8#crm"%>">
                        <span class="menu-item-label inner-text"><%=CRMSettingResource.AccessRightsSettings%></span>
                    </a>
                </li>
               
            </ul>
        </li>
        <% } %> 
        <asp:PlaceHolder ID="HelpHolder" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="SupportHolder" runat="server"></asp:PlaceHolder>
    </ul>
</div>