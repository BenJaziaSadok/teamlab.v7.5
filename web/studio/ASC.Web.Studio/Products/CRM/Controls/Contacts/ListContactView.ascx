<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListContactView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Contacts.ListContactView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div id="mainContactList">
    <div class="clearFix">
        <div id="contactsFilterContainer">
            <div id="contactsAdvansedFilter"></div>
        </div>
        <br />
        <div id="companyListBox" style="display: none">
            <ul id="contactsHeaderMenu" class="clearFix contentMenu contentMenuDisplayAll">
                <li class="menuAction menuActionSelectAll menuActionSelectLonely">
                    <div class="menuActionSelect">
                        <input type="checkbox" id="mainSelectAll" title="<%=CRMCommonResource.SelectAll%>" onclick="ASC.CRM.ListContactView.selectAll(this);" />
                    </div>
                </li>
                <% if (CRMSecurity.IsAdmin) %>
                <% { %>
                <li class="menuAction menuActionSendEmail" title="<%= CRMCommonResource.SendEmail %>">
                    <span><%= CRMCommonResource.SendEmail%></span>
                    <div class="down_arrow"></div>
                </li>
                <% } %>
                <li class="menuAction menuActionAddTag" title="<%= CRMCommonResource.AddNewTag %>">
                    <span><%=CRMCommonResource.AddNewTag%></span>
                    <div class="down_arrow"></div>
                </li>
                <%--<li class="menuAction menuActionPermissions" title="<%= CRMCommonResource.SetPermissions %>">
                    <span><%=CRMCommonResource.SetPermissions%></span>
                </li>--%>
                <li class="menuAction menuActionAddTask" title="<%= CRMTaskResource.AddNewTaskButtonText %>">
                    <span><%=CRMTaskResource.AddNewTaskButtonText%></span>
                </li>
                <li class="menuAction menuActionDelete" title="<%= CRMCommonResource.Delete %>">
                    <span><%= CRMCommonResource.Delete%></span>
                </li>
                <li class="menu-action-simple-pagenav">
                </li>
                <li class="menu-action-checked-count">
                    <span></span>
                    <a class="linkDescribe baseLinkAction" style="margin-left:10px;" onclick="ASC.CRM.ListContactView.deselectAll();">
                        <%= CRMCommonResource.DeselectAll%>
                    </a>
                </li>
                <li class="menu-action-on-top">
                    <a class="on-top-link" onclick="javascript:window.scrollTo(0, 0);">
                        <%= CRMCommonResource.OnTop%>
                    </a>
                </li>
            </ul>
            <div class="header-menu-spacer">&nbsp;</div>

            <table id="companyTable" class="tableBase" cellpadding="4" cellspacing="0">
                <colgroup>
                    <col style="width: 30px;"/>
                    <col style="width: 40px;"/>
                    <col/>
                    <col style="width: 200px;"/>
                    <col style="width: 200px;"/>
                    <col style="width: 200px;"/>
                    <col style="width: 40px;"/>
                </colgroup>
                <tbody>
                </tbody>
            </table>

            <table id="tableForContactNavigation" class="crm-navigationPanel" cellpadding="4" cellspacing="0" border="0">
                <tbody>
                <tr>
                    <td>
                        <div id="divForContactPager">
                        </div>
                    </td>
                    <td style="text-align:right;">
                        <span class="gray-text"><%= CRMContactResource.TotalContacts%>:</span>
                        <span class="gray-text" id="totalContactsOnPage"></span>
                        <span class="page-nav-info">
                            <span class="gray-text"><%= CRMCommonResource.ShowOnPage%>:&nbsp;</span>
                            <select class="top-align">
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
    </div>
</div>

<div id="permissionsContactsPanelInnerHtml" class="display-none">
    <% if (!CRMSecurity.IsAdmin) %>
    <% { %>
    <div style="margin-top:10px">
        <b><%= CRMCommonResource.AccessRightsLimit%></b>
    </div>
    <% } %>
    <asp:PlaceHolder runat="server" ID="_phPrivatePanel"></asp:PlaceHolder>
</div>


<% if (CRMSecurity.IsAdmin) %>
<% { %>
<div id="sendEmailDialog" class="studio-action-panel">
    <div class="corner-top left"></div>
    <ul class="dropdown-content">
        <%--<li>
            <a class="dropdown-item sendMailByTl" href="" target="_blank">
                <%=CRMCommonResource.TeamlabMail%>
            </a>
        </li>--%>
        <li>
            <a class="dropdown-item" onclick="ASC.CRM.ListContactView.showCreateLinkPanel()">
                <%=CRMSettingResource.ExternalClient%>
            </a>
        </li>
        <li>
            <a class="dropdown-item" onclick="ASC.CRM.ListContactView.showSenderPage()">
                <%=String.Format(CRMSettingResource.InternalSMTP, MailSender.GetQuotas())%>
            </a>
        </li>
    </ul>
</div>
<% } %>

<div id="addTagDialog" class="studio-action-panel addTagDialog">
    <div class="corner-top left"></div>
    <ul class="dropdown-content mobile-overflow"></ul>
    <div class="h_line">&nbsp;</div>
    <div style="margin-bottom:5px;"><%= CRMCommonResource.CreateNewTag%>:</div>
    <input type="text" maxlength="50" class="textEdit" />
    <a onclick="ASC.CRM.ListContactView.addNewTag();" class="button blue" id="addThisTag">
        <%= CRMCommonResource.OK%>
    </a>
</div>

<div id="contactActionMenu" class="studio-action-panel">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li><a class="showProfileLink dropdown-item"><%= CRMContactResource.ShowContactProfile%></a></li>
        <li><a class="addPhoneLink dropdown-item"><%= CRMJSResource.AddNewPhone%></a></li>
        <li><a class="addEmailLink dropdown-item"><%= CRMJSResource.AddNewEmail%></a></li>
        <li><a class="sendEmailLink dropdown-item" target="_blank"><%= CRMContactResource.WriteEmail%></a></li>
        <li><a class="addTaskLink dropdown-item"><%= CRMTaskResource.AddNewTask%></a></li>
        <li><a class="addDealLink dropdown-item"><%= CRMDealResource.CreateNewDeal %></a></li>
        <li><a class="addCaseLink dropdown-item"><%= CRMCasesResource.CreateNewCase %></a></li>
        <%--<li><a class="setPermissionsLink dropdown-item"><%= CRMCommonResource.SetPermissions%></a></li>--%>
        <li><a class="editContactLink dropdown-item"><%= CRMContactResource.EditContact%></a></li>
        <li><a class="deleteContactLink dropdown-item"><%= CRMContactResource.DeleteContact%></a></li>
    </ul>
</div>

<asp:PlaceHolder ID="_phDashboardEmptyScreen" runat="server"></asp:PlaceHolder>