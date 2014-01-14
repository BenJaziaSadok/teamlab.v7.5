<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ListDealView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Deals.ListDealView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<div id="dealFilterContainer">
    <div id="dealsAdvansedFilter"></div>
    <br />
</div>

<div id="dealList" class="clearFix" style="min-height: 400px;">

    <table id="tableForDealNavigation" class="crm-navigationPanel" cellpadding="4" cellspacing="0" border="0">
        <tbody>
        <tr>
            <td>
                <div id="divForDealPager">
                </div>
            </td>
            <td style="text-align:right;">
                <a style="margin-right: 25px;" class="baseLinkAction showTotalAmount"
                    onclick="ASC.CRM.ListDealView.showExchangeRatePopUp();" href="javascript:void(0)">
                        <%=CRMDealResource.ShowTotalAmount %>
                </a>
                <span class="gray-text"><%= CRMDealResource.TotalDeals %>:</span>
                <span class="gray-text" id="totalDealsOnPage"></span>

                <span class="gray-text"><%= CRMCommonResource.ShowOnPage %>:&nbsp;</span>
                <select class="top-align">
                    <option value="25">25</option>
                    <option value="50">50</option>
                    <option value="75">75</option>
                    <option value="100">100</option>
                </select>
            </td>
        </tr>
        </tbody>
    </table>
</div>

<div id="files_hintStagesPanel" class="hintDescriptionPanel">
    <div class="popup-corner"></div>
    <%=CRMDealResource.TooltipStages%>
    <a href="http://www.teamlab.com/help/tipstricks/opportunity-stages.aspx" target="_blank"><%=CRMCommonResource.ButtonLearnMore%></a>
</div>

<div id="hiddenBlockForContactSelector" style="display:none;width:300px;"></div>

<div id="addTagDealsDialog" class="studio-action-panel addTagDialog">
    <div class="corner-top left"></div>
    <ul class="dropdown-content mobile-overflow"></ul>
    <div class="h_line">&nbsp;</div>
    <div style="margin-bottom:5px;"><%= CRMCommonResource.CreateNewTag%>:</div>
    <input type="text" maxlength="50" class="textEdit" />
    <a onclick="ASC.CRM.ListDealView.addNewTag();" class="button blue" id="addThisTag">
        <%= CRMCommonResource.OK%>
    </a>
</div>

<div id="permissionsDealsPanelInnerHtml" class="display-none">
    <% if (!CRMSecurity.IsAdmin) %>
    <% { %>
    <div style="margin-top:10px">
        <b><%= CRMCommonResource.AccessRightsLimit%></b>
    </div>
    <% } %>
    <asp:PlaceHolder runat="server" ID="_phPrivatePanel"></asp:PlaceHolder>
</div>

<div id="dealActionMenu" class="studio-action-panel">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li><a class="showProfileLink dropdown-item"><%= CRMDealResource.ShowDealProfile %></a></li>
        <li><a class="setPermissionsLink dropdown-item"><%= CRMCommonResource.SetPermissions %></a></li>
        <% if (Global.CanCreateProjects()) %>
        <% { %>
        <li><a class="createProject dropdown-item" target="_blank"><%= CRMCommonResource.CreateNewProject %></a></li>
        <% } %>
        <li><a class="editDealLink dropdown-item"><%= CRMDealResource.EditThisDealButton %></a></li>
        <li><a class="deleteDealLink dropdown-item"><%= CRMDealResource.DeleteDeal %></a></li>
    </ul>
</div>