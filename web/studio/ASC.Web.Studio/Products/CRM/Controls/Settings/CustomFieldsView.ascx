<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CustomFieldsView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.CustomFieldsView" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%@ Register TagPrefix="vs" Namespace="ASC.Web.Studio.UserControls.Common.ViewSwitcher" Assembly="ASC.Web.Studio" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="manageField" style="display: none">
    <sc:Container ID="_manageFieldPopup" runat="server">
        <Header>
           <%= CRMSettingResource.CreateNewField%>
        </Header>
        <Body>
            <dl>
                <dt></dt>
                <dd>
                    <div class="requiredField">
                        <span class="requiredErrorText"><%= CRMSettingResource.EmptyLabelError %></span>
                        <div class="headerPanelSmall header-base-small" style="margin-bottom:5px;">
                            <%= CRMSettingResource.Label%>:
                        </div>
                        <input type="text" class="textEdit" maxlength="255"/>
                    </div>
                </dd>

                <dt><%= CRMSettingResource.Type %>:</dt>
                <dd>
                    <select onchange="ASC.CRM.SettingsPage.selectTypeEvent(this);" class="comboBox">
                        <option value="0">
                            <%= CRMSettingResource.TextField %>
                        </option>
                        <option value="1">
                            <%= CRMSettingResource.TextArea %>
                        </option>
                        <option value="2">
                            <%= CRMSettingResource.SelectBox%>
                        </option>
                        <option value="3">
                            <%= CRMSettingResource.CheckBox%>
                        </option>
                        <option value="4">
                            <%= CRMSettingResource.Heading%>
                        </option>
                        <option value="5">
                            <%= CRMSettingResource.Date%>
                        </option>
                    </select>
                </dd>
                <dt class="field_mask text_field" style="display: block;">
                    <%= CRMSettingResource.Size%>:
                </dt>
                <dd class="field_mask text_field" style="display: block;">
                    <input id="text_field_size" class="textEdit" value="<%= Global.DefaultCustomFieldSize %>" />
                </dd>
                <dt class="field_mask textarea_field">
                    <%= CRMSettingResource.Rows%>:
                </dt>
                <dd class="field_mask textarea_field">
                    <input id="textarea_field_rows" class="textEdit" value="<%= Global.DefaultCustomFieldRows %>" />
                </dd>
                <dt class="field_mask textarea_field">
                    <%= CRMSettingResource.Cols%>:
                </dt>
                <dd class="field_mask textarea_field">
                    <input id="textarea_field_cols" class="textEdit" value="<%= Global.DefaultCustomFieldCols %>" />
                </dd>
                <dt class="field_mask select_options">
                    <%= CRMSettingResource.SelectOptions%>:</dt>
                <dd class="field_mask select_options">
                    <ul>
                        <li style="display: none">
                            <input type="text" class="textEdit" maxlength="255"/>
                            <label class="deleteBtn"
                                alt="<%= CRMSettingResource.RemoveOption%>" title="<%= CRMSettingResource.RemoveOption%>"
                                onclick="jq(this).parent().remove()"></label>
                        </li>
                    </ul>
                    <span onclick="ASC.CRM.SettingsPage.toSelectBox(this)" title="<%= CRMSettingResource.AddOption%>"
                        id="addOptionButton" class="baseLinkAction">
                        <%= CRMSettingResource.AddOption%></span>
                </dd>
            </dl>

            <div class="crm-actionButtonsBlock">
                <a class="button blue middle"><%= CRMSettingResource.AddThisField%></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.EnableEsc = true;jq.unblockUI();">
                     <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div class="crm-actionProcessInfoBlock">
                <span class="text-medium-describe"><%= CRMSettingResource.CreateFieldInProgressing %></span>
                <br />
                <img alt="<%= CRMSettingResource.CreateFieldInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </sc:Container>
</div>

<div class="settingsHeaderWithViewSwitcher clearFix">
    <a id="createNewField" class="gray button display-none">
        <span class="plus"><%= CRMSettingResource.CreateNewFieldListButton%></span>
    </a>

    <vs:ViewSwitcher runat="server" ID="_switcherEntityType">
        <SortItems>
            <vs:ViewSwitcherLinkItem runat="server"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server"></vs:ViewSwitcherLinkItem>
            <vs:ViewSwitcherLinkItem runat="server"></vs:ViewSwitcherLinkItem>
        </SortItems>
    </vs:ViewSwitcher>
</div>
<br />

<ul id="customFieldList" class="clearFix ui-sortable">
</ul>

<div id="customFieldActionMenu" class="studio-action-panel" fieldid="">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li><a class="dropdown-item editField" onclick="ASC.CRM.SettingsPage.showEditFieldPanel();"><%= CRMSettingResource.EditCustomField %></a></li>
        <li><a class="dropdown-item deleteField" onclick="ASC.CRM.SettingsPage.deleteField();"><%= CRMSettingResource.DeleteCustomField %></a></li>
    </ul>
</div>