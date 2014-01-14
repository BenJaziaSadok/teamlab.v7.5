<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskList.ascx.cs" Inherits="ASC.Web.Projects.Controls.Tasks.TaskList" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div class="simplePageNavigator"></div>

<div id="SubTasksBody" data-task-count="<%=AllTasksCount %>">
    <div class="taskList"></div>

    <div class="taskProcess" id="showNextTaskProcess"></div>

    <table id="tableForNavigation" cellpadding="4" cellspacing="0">
        <tbody>
        <tr>
            <td>
                <div id="divForTaskPager" class="divPager">
                </div>
            </td>
            <td style="text-align:right;">
                <span class="gray-text"><%= ProjectsCommonResource.Total%> : </span>
                <span class="gray-text" style="margin-right: 20px;" id="totalCount"><%=AllTasksCount%></span>
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

<div id="questionWindow" style="display: none">
    <sc:Container ID="_hintPopup" runat="server">
        <Header>
            <%= TaskResource.ClosingTheTask%>
        </Header>
        <Body>
            <p><%= TaskResource.TryingToCloseTheTask%>.</p>
            <p><%= TaskResource.BetterToReturn%>.</p>
            <div class="middle-button-container">
                <a class="button blue middle end">
                    <%= TaskResource.EndAllSubtasksCloseTask%></a> 
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle cancel">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </Body>
    </sc:Container>
</div>

<div id="questionWindowTaskRemove" style="display: none">
    <sc:Container ID="_hintPopupTaskRemove" runat="server">
        <Header>
            <%= TaskResource.RemoveTask%>
        </Header>
        <Body>
            <p>
                <%= TaskResource.RemoveTaskPopup%>
            </p>
            <p>
                <%=ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="errorBox display-none"></div>
            <div class="middle-button-container">
                <a class="button blue middle remove"><%= TaskResource.RemoveTask%></a> 
                <span class="splitter-buttons"></span>
                <a class="button gray middle cancel"><%= ProjectsCommonResource.Cancel%></a>
            </div>
        </Body>
    </sc:Container>
</div>

<div id="moveTaskPanel" style="display: none;">
    <sc:Container ID="moveTaskContainer" runat="server">
        <Header>
            <%= TaskResource.MoveTaskToAnotherMilestone%>
        </Header>
        <Body>
            <div class="describe-text"><%= TaskResource.Task %></div>
            <div class="taskTitls ms"><b id="moveTaskTitles"></b></div>
            <div class="describe-text ms"><%= TaskResource.WillBeMovedToMilestone%>:</div>
            <div class="milestonesList">
                <div class="milestonesButtons">
                    <input id="ms_0" type="radio" name="milestones" value="0" />
                    <label for="ms_0"><%= TaskResource.None%></label>
                </div>
            </div>

            <div class="pm-action-block">
                <a href="javascript:void(0)" class="button blue middle">
                    <%= TaskResource.MoveToMilestone%>
                </a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" href="javascript:void(0)">
                    <%= ProjectsCommonResource.Cancel%>
                </a>
            </div>
            <div class='pm-ajax-info-block' style="display: none;">
                <span class="text-medium-describe"><%= TaskResource.ExecutingGroupOperation%></span>
                <br />
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")  %>" />
            </div>
        </Body>
    </sc:Container>
</div>

<div id="taskActionPanel" class="studio-action-panel" objid="">
    <div class="corner-top right"></div>
    <ul class="dropdown-content">
        <li id="ta_edit" class="dropdown-item"><span title="<%= TaskResource.Edit%>"><%= TaskResource.Edit%></span></li>
        <li id="ta_subtask" class="dropdown-item"><span title="<%= TaskResource.AddSubtask%>"><%= TaskResource.AddSubtask%></span></li>
        <li id="ta_accept" class="dropdown-item"><span title="<%= TaskResource.AcceptSubtask%>"><%= TaskResource.AcceptSubtask%></span></li>
        <li id="ta_move" class="dropdown-item"><span title="<%= TaskResource.MoveToMilestone%>"><%= TaskResource.MoveToMilestone%></span></li>
        <li id="ta_mesres" class="dropdown-item"><span title="<%= TaskResource.MessageResponsible%>"><%= TaskResource.MessageResponsible%></span></li>
        <li id="ta_time" class="dropdown-item"><span title="<%= TaskResource.TrackTime %>"><%= TaskResource.TrackTime %></span></li>
        <li id="ta_remove" class="dropdown-item"><span title="<%= ProjectsCommonResource.Delete%>"><%= ProjectsCommonResource.Delete%></span></li>
    </ul>
</div>

<div id="othersPanel" class="studio-action-panel">
    <div class="corner-top right"></div>
    <ul id="othersListPopup" class="dropdown-content">
    </ul>
</div>

<div id="taskDescrPanel" class="studio-action-panel" objid="">
    <div class="corner-top left"></div>
    <div>
        <div class="date">
            <div class="param"><%= TaskResource.CreatingDate%>:</div>
            <div class="value"></div>
        </div>
        <div class="createdby">
            <div class="param"><%= TaskResource.CreatedBy%>:</div>
            <div class="value"></div>
        </div>
        <div class="startdate">
            <div class="param"><%= TaskResource.TaskStartDate%>:</div>
            <div class="value"></div>
        </div>
        <div class="closed">
            <div class="param"><%= TaskResource.Closed%>:</div>
            <div class="value"></div>
        </div>
        <div class="closedby">
            <div class="param"><%= TaskResource.ClosedBy%>:</div>
            <div class="value"></div>
        </div>
        <% if (!RequestContext.IsInConcreteProject) %>
        <%{%>
        <div class="project">
            <div class="param"><%= TaskResource.Project%>:</div>
            <div class="value"></div>
        </div>
        <%}%>
        <div class="milestone">
            <div class="param"><%= TaskResource.Milestone%>:</div>
            <div class="value"></div>
        </div>
        <div class="descr">
            <div class="param"><%= TaskResource.Description%>:</div>
            <div class="value">
                <div class="descrValue"></div>
                <a class="readMore"><%=ProjectsCommonResource.ReadMore %></a>
            </div>
        </div>
    </div>
</div>

<div id="statusListContainer" class="studio-action-panel">
    <div class="corner-top left"></div>
    <ul id="statusList" class="dropdown-content">
        <li class="open dropdown-item"><%= TaskResource.Open%></li>
        <li class="closed dropdown-item"><%= TaskResource.Closed%></li>
    </ul>
</div>

<div id="remindAboutTask" style="display: none">
    <sc:Container ID="_hintPopupTaskRemind" runat="server">
        <Header>
            <%= TaskResource.MessageResponsible%>
        </Header>
        <Body>
            <p><%= TaskResource.MessageSend%></p>
            <div class="middle-button-container">
                <a class="button blue middle ok"><%= ProjectResource.OkButton%></a> 
            </div>
        </Body>
    </sc:Container>
</div>