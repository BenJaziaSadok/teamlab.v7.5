<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="Masters/BasicTemplate.Master"
            CodeBehind="Milestones.aspx.cs" Inherits="ASC.Web.Projects.Milestones" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<%@ Import Namespace="ASC.Web.Projects.Classes"%>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>


<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">

    <div id="filterContainer">
        <div id="ProjectsAdvansedFilter"></div>
    </div>
    <div id="questionWindowTasks" style="display: none">
        <sc:Container ID="_hintPopupTasks" runat="server">
            <Header>
                <%= MilestoneResource.CloseMilestone %>
            </Header>
            <Body>
                <p><%= MilestoneResource.NotCloseMilWithActiveTasks %></p>
                <div class="middle-button-container">
                    <a class="button blue middle" id="linkToTasks"><%= ProjectResource.ViewActiveTasks %></a>
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle"><%= ProjectsCommonResource.Cancel %></a>
                </div>
            </Body>
        </sc:Container>
    </div>

    <div id="questionWindowDeleteMilestone" style="display: none">
        <sc:Container ID="_hintPopupTaskRemove" runat="server">
        <Header>
            <%= MilestoneResource.DeleteMilestone %>
        </Header>
        <Body>
            <p><%= MilestoneResource.DeleteMilestonePopup %> </p>
            <p><%= ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="middle-button-container">
                <a class="button blue middle remove"><%= MilestoneResource.DeleteMilestone %></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle cancel"><%= ProjectsCommonResource.Cancel %></a>
            </div>
        </Body>
        </sc:Container>
    </div>

    <div class="simplePageNavigator">
    </div>
    <div id="milestonesListContainer" data-milestones-count="<%=AllMilestonesCount %>" data-is-admin="<%= IsAdmin %>">
        <asp:PlaceHolder runat="server" ID="_emptyScreenPlaceHolder"></asp:PlaceHolder>
        <table id="milestonesList">
            <thead>
            </thead>
            <tbody>
            </tbody>
        </table>
        <table id="tableForNavigation" cellpadding="4" cellspacing="0">
            <tbody>
            <tr>
                <td>
                    <div id="divForTaskPager" class="divPager">
                    </div>
                </td>
                <td style="text-align:right;">
                    <span class="gray-text"><%= ProjectsCommonResource.Total%> : </span>
                    <span class="gray-text" style="margin-right: 20px;" id="totalCount"></span>
                    <span class="gray-text"><%= ProjectsCommonResource.ShowOnPage%> : </span>
                    <select id="countOfRows" class="top-align">
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

    <div id="statusListContainer" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul id="statusList" class="dropdown-content">
            <li class="open dropdown-item"><%= MilestoneResource.StatusOpen %></li>
            <li class="closed dropdown-item"><%= MilestoneResource.StatusClosed %></li>
        </ul>
    </div>

    <div id="descriptionPanel" class="studio-action-panel" objid="">
        <div class="corner-top left"></div>
        <div class="param">
            <% if (!RequestContext.IsInConcreteProject) { %>
            <div class="project"><%= ProjectResource.Project %>:</div>
            <% } %>
            <div class="createdby"><%= MilestoneResource.CreatedBy %>:</div>
            <div class="created"><%= MilestoneResource.CreatingDate %>:</div>
            <div class="description"><%= MilestoneResource.Description %>:</div>
        </div>
        <div class="value">
            <% if (!RequestContext.IsInConcreteProject) { %>
            <div class="project"><a></a></div>
            <% } %>
            <div class="createdby"></div>
            <div class="created"></div>
            <div class="description"></div>
        </div>
    </div>

    <div id="milestoneActionContainer" class="studio-action-panel">
        <div class="corner-top right"></div>
        <ul class="dropdown-content">
            <li class="dropdown-item" id="updateMilestoneButton"><span title="<%= ProjectsCommonResource.Edit %>"><%= ProjectsCommonResource.Edit %></span></li>
            <li class="dropdown-item" id="removeMilestoneButton"><span title="<%= ProjectsCommonResource.Delete %>"><%= ProjectsCommonResource.Delete %></span></li>
            <li class="dropdown-item" id="addMilestoneTaskButton"><span title="<%= TaskResource.AddTask %>"><%= TaskResource.AddTask %></span></li>
        </ul>
    </div>
</asp:Content>
<asp:Content ID="projectsClientTemplatesResourcesPlaceHolder" ContentPlaceHolderID="projectsClientTemplatesResourcesPlaceHolder" runat="server">
</asp:Content>