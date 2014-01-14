<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagSettingsView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.TagSettingsView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%@ Register TagPrefix="vs" Namespace="ASC.Web.Studio.UserControls.Common.ViewSwitcher" Assembly="ASC.Web.Studio" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="manageTag" style="display: none">
    <sc:Container ID="_manageTagPopup" runat="server">
        <Header>
           <%= CRMSettingResource.CreateNewTag%>
        </Header>
        <Body>
            <div class="requiredField">
                <span class="requiredErrorText"></span>
                <div class="headerPanelSmall header-base-small" style="margin-bottom:5px;">
                    <%= CRMSettingResource.Label%>:
                </div>
                <input id="tagTitle" type="text" class="textEdit" style="width:100%" maxlength="50"/>
            </div>

            <div class="crm-actionButtonsBlock">
                <a class="button blue middle" onclick="ASC.CRM.TagSettingsView.createTag();"><%= CRMSettingResource.AddThisTag%></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                     <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div class="crm-actionProcessInfoBlock">
                <span class="text-medium-describe"> <%= CRMSettingResource.CreateTagInProgressing%> </span>
                <br />
                <img alt="<%= CRMSettingResource.CreateTagInProgressing %>"
                    title="<%= CRMSettingResource.CreateTagInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </sc:Container>
</div>

<div class="settingsHeaderWithViewSwitcher clearFix">
    <a id="createNewTagSettings" class="gray button display-none">
        <span class="plus"><%= CRMSettingResource.CreateNewTagListButton %></span>
    </a>
    <% if (entityType == ASC.CRM.Core.EntityType.Contact) { %>
    <div class="cbx_AddTagWithoutAskingContainer display-none">
        <input type="checkbox" style="float: left;" id="cbx_AddTagWithoutAsking"
            <% if (ASC.Web.CRM.Classes.Global.TenantSettings.AddTagToContactGroupAuto != null) { %>checked="checked"<% } %> />
        <label style="float:left; padding: 2px 0 0 4px;" for="cbx_AddTagWithoutAsking">
            <%= CRMSettingResource.AddTagWithoutAskingSettingsLabel %>
        </label>
    </div>
    <% } %>



    <vs:ViewSwitcher runat="server" ID="_switcherEntityType">
        <SortItems>
            <vs:ViewSwitcherLinkItem runat="server" id="_forContacts"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server" id="_forDeals"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server" id="_forCases"></vs:ViewSwitcherLinkItem>
        </SortItems>
    </vs:ViewSwitcher>
</div>
<br />

<ul id="tagList" class="clearFix">
</ul>