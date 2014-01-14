<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DealMilestoneView.ascx.cs" Inherits="ASC.Web.CRM.Controls.Settings.DealMilestoneView" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<p style="margin-bottom: 10px;"><%= CRMSettingResource.DescriptionTextDealMilestone %></p>
<p style="margin-bottom: 20px;"><%= CRMSettingResource.DescriptionTextDealMilestoneEditDelete %></p>

 <div id="manageDealMilestone" style="display: none">
    <sc:Container ID="_manageDealMilestonePopup" runat="server">
        <Header>
           <%= CRMSettingResource.CreateNewDealMilestone%>
        </Header>
        <Body>
            <dl>
                <dt class="selectedColor">&nbsp;</dt>
                <dd>
                    <span class="baseLinkAction crm-withArrowDown change_color" onclick="ASC.CRM.DealMilestoneView.showColorsPanelToSelect();">
                        <%= CRMSettingResource.ChangeColor %>
                    </span>
                    <div id="popup_colorsPanel" class="studio-action-panel colorsPanelSettings">
                        <div class="corner-top left"></div>
                        <span class="style1" colorstyle="1"></span><span class="style2" colorstyle="2"></span><span class="style3" colorstyle="3"></span><span class="style4" colorstyle="4"></span><span class="style5" colorstyle="5"></span><span class="style6" colorstyle="6"></span><span class="style7" colorstyle="7"></span><span class="style8" colorstyle="8"></span>
                        <span class="style9" colorstyle="9"></span><span class="style10" colorstyle="10"></span><span class="style11" colorstyle="11"></span><span class="style12" colorstyle="12"></span><span class="style13" colorstyle="13"></span><span class="style14" colorstyle="14"></span><span class="style15" colorstyle="15"></span><span class="style16" colorstyle="16"></span>
                    </div>
                </dd>

                <dt></dt>
                <dd>
                    <div class="requiredField">
                        <span class="requiredErrorText"><%= CRMSettingResource.EmptyTitleError %></span>
                        <div class="headerPanelSmall header-base-small" style="margin-bottom:5px;">
                            <%= CRMSettingResource.TitleItem %>:
                        </div>
                        <input type="text" class="textEdit title" maxlength="255"/>
                    </div>
                </dd>

                <dt><%= CRMSettingResource.Description %>:</dt>
                <dd>
                    <textarea rows="4" style="resize: none;"></textarea>
                </dd>

                <dt><%= CRMSettingResource.Likelihood %>:</dt>
                <dd>
                    <input type="text" class="textEdit probability" style="width: 30px;"  /> %
                </dd>

                <dt><%= CRMDealResource.DealMilestoneType%>:</dt>
                <dd>
                    <ul>
                        <li>
                            <input type="radio" id="dealMilestoneStatusOpen" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.Open %>" />
                            <label for="dealMilestoneStatusOpen"><%=DealMilestoneStatus.Open.ToLocalizedString() %></label>
                        </li>
                        <li>
                            <input type="radio" id="dealMilestoneStatusClosedAndWon" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.ClosedAndWon %>"
                                onclick="javascript:jq('#manageDealMilestone .probability').val('100');"/>
                            <label for="dealMilestoneStatusClosedAndWon"><%=DealMilestoneStatus.ClosedAndWon.ToLocalizedString()%></label>
                        </li>
                        <li>
                            <input type="radio" id="dealMilestoneStatusClosedAndLost" name="deal_milestone_status" value="<%= (Int32)DealMilestoneStatus.ClosedAndLost %>"
                                onclick="javascript:jq('#manageDealMilestone .probability').val('0');"/>
                            <label for="dealMilestoneStatusClosedAndLost"><%=DealMilestoneStatus.ClosedAndLost.ToLocalizedString()%></label>
                        </li>
                    </ul>
                </dd>
            </dl>

            <div class="crm-actionButtonsBlock">
                <a class="button blue middle"><%= CRMSettingResource.AddThisDealMilestone%></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">
                    <%= CRMCommonResource.Cancel %>
                </a>
            </div>
            <div class="crm-actionProcessInfoBlock">
                <span class="text-medium-describe"><%= CRMSettingResource.CreateDealMilestoneInProgressing%></span>
                <br />
                <img alt="<%= CRMSettingResource.CreateDealMilestoneInProgressing %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
        </Body>
    </sc:Container>
</div>

<a id="createNewDealMilestone" class="gray button">
    <span class="plus"><%= CRMSettingResource.CreateNewDealMilestoneListButton%></span>
</a>
<br/><br/>

<ul id="dealMilestoneList">
</ul>

<div id="dealMilestoneActionMenu" class="studio-action-panel" dealmilestoneid="">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li><a class="dropdown-item" onclick="ASC.CRM.DealMilestoneView.showEditDealMilestonePanel();"><%= CRMSettingResource.EditDealMilestone%></a></li>
        <li><a class="dropdown-item" onclick="ASC.CRM.DealMilestoneView.deleteDealMilestone();"><%= CRMSettingResource.DeleteDealMilestone%></a></li>
    </ul>
</div>

<div id="colorsPanel" class="studio-action-panel colorsPanelSettings">
    <div class="corner-top left"></div>
    <span class="style1" colorstyle="1"></span><span class="style2" colorstyle="2"></span><span class="style3" colorstyle="3"></span><span class="style4" colorstyle="4"></span><span class="style5" colorstyle="5"></span><span class="style6" colorstyle="6"></span><span class="style7" colorstyle="7"></span><span class="style8" colorstyle="8"></span>
    <span class="style9" colorstyle="9"></span><span class="style10" colorstyle="10"></span><span class="style11" colorstyle="11"></span><span class="style12" colorstyle="12"></span><span class="style13" colorstyle="13"></span><span class="style14" colorstyle="14"></span><span class="style15" colorstyle="15"></span><span class="style16" colorstyle="16"></span>
</div>