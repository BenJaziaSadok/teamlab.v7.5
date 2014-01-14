<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TaskAction.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Tasks.TaskAction" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="addTaskPanel" style="display: none;">
    <sc:Container ID="addTaskContainer" runat="server">
        <Header>
            <%= TaskResource.AddTask%>
        </Header>
        <Body>
            <div class="pm-headerPanelSmall-splitter titlePanel requiredField">
                <span class="requiredErrorText title" error="<%= TaskResource.EachTaskMustHaveTitle%>">
                </span>
                <div class="headerPanelSmall">
                    <b>
                        <%= TaskResource.TaskTitle%>:</b>
                </div>
                <input id="addtask_title" style="width: 99%;" maxlength="250" />
            </div>
            <div class="pm-headerPanelSmall-splitter">
                <div class="headerPanelSmall">
                    <b><%= TaskResource.TaskDescription%>:</b>
                </div>
                <textarea style="width: 99%; resize: none; max-height: 200px" id="addtask_description"
                    cols="22" rows="3"></textarea>
            </div>
            <div id="pm-projectBlock">
                <div class="pm-headerLeft">
                    <div class="requiredField">
                        <div class="headerPanelSmall"><%= ProjectResource.Project%>:
                        </div>
                    </div>
                </div>
                <div class="pm-fieldRight">
                    <span class="requiredErrorText project" error="<%= TaskResource.ChooseProject%>">
                    </span>
                    <select id="taskProject" class="full-width left-align">
                        <option value="-1" class="hidden"><%= TaskResource.Select%></option>
                    </select>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <div id="pm-milestoneBlock">
                <div class="pm-headerLeft">
                    <%= MilestoneResource.Milestone%>:</div>
                <div class="pm-fieldRight">
                    <select id="taskMilestone" class="full-width left-align">
                        <option value="-1" class="hidden"><%= TaskResource.Select%></option>
                        <option value="0"><%= TaskResource.None%></option>
                    </select>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <div class="pm-headerLeft userAddHeader"><%= TaskResource.TaskResponsible%>:</div>
            <div class="pm-fieldRight userAdd">
                <div id="responsibleSelectContainer">
                </div>
                <div id="fullFormUserList">
                </div>
                <div class="adduser">
                </div>
                <select id="taskResponsible" class="full-width left-align">
                    <option value="-1" class="hidden"><%= TaskResource.Add%></option>
                </select>
                <div id="noActiveParticipantsTaskNote" class="float-right gray-text display-none"><%=ProjectsJSResource.NoActiveParticipantsNote%></div>
            </div>
            <div style="clear: both">
            </div>
            <div class="pm-headerLeft notify">
            </div>
            <div class="pm-fieldRight notify">
                <input type="checkbox" id="notify" checked="checked" />
                <label for="notify"><%= MessageResource.SubscribeUsers%></label>
            </div>
            <div style="clear: both" class="notify">
            </div>
            <div class="pm-headerLeft" style="line-height: 21px;"><%= TaskResource.TaskStartDate%>:</div>
            <div class="pm-fieldRight">
                <span class="requiredErrorText startDate-error" error="<%= ProjectsCommonResource.TaskCompareDateError%>" style="margin-right: 24px;">
                </span>
                <input type="text" id="taskStartDate" class="textEditCalendar" />
            </div>
            <div style="clear: both">
            </div>
            <div class="pm-headerLeft" style="line-height: 21px;"><%= TaskResource.DeadLine%>:</div>
            <div class="pm-fieldRight">
                <input type="text" id="taskDeadline" class="textEditCalendar"/>
                <span class="splitter"></span><span class="dottedLink deadline_left" data-value="0">
                    <%= ProjectsCommonResource.Today %>
                </span><span class="splitter"></span><span class="dottedLink deadline_left" data-value="3">
                    3
                    <%= GrammaticalResource.DayGenitiveSingular%>
                </span><span class="splitter"></span><span class="dottedLink deadline_left" data-value="7">
                    <%= ReportResource.Week %>
                </span>
            </div>
            <div style="clear: both">
            </div>
            <div>
                <div class="pm-headerLeft">
                    <%= TaskResource.Priority%>:</div>
                <div class="pm-fieldRight">
                    <input type="checkbox" name="priority" value="1" id="priority" /><label class="priority high"
                        for="priority"><%= TaskResource.HighPriority%></label>
                </div>
                <div style="clear: both">
                </div>
            </div>
            <div class="pm-action-block">
                <div class="okBox display-none"><%=ProjectsCommonResource.TaskAddedMessage%>
                    <a id="taskLink" href="javascript:void(0)"><%=TaskResource.GoToTheTask%></a>
                </div>
                <div class="errorBox display-none"></div>
                 <a id="saveTaskAction" href="javascript:void(0)" class="button blue middle" add="<%=ProjectsCommonResource.Save%>"
                    update="<%= ProjectsCommonResource.SaveChanges%>">
                    <%= ProjectsCommonResource.Save%>
                </a>
                <span class="splitter-buttons"></span>
                <a id="createTaskAndCreateNew" href="javascript:void(0)" class="button gray middle">
                    <%= ProjectsCommonResource.SaveAndAddMore%>
                </a>
               <span class="splitter-buttons"></span>
               <a id="closeTaskAction" class="button gray middle" href="javascript:void(0)">
                    <%= ProjectsCommonResource.Cancel%>
                </a>
            </div>
            <div class="pm-ajax-info-block display-none">
                <span class="text-medium-describe">
                    <%= TaskResource.Saving%></span><br/>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"/>
            </div>
        </Body>
    </sc:Container>
</div>

<div id="removeTaskLinksQuestionPopup" style="display: none">
    <sc:Container ID="_moveTaskOutMilestone" runat="server">
        <Header>
            <%= ProjectsCommonResource.MoveTaskHeader %>
        </Header>
        <Body>
            <p><%= String.Format(ProjectsCommonResource.TaskMoveNote, "<br/>") %> </p>
            <div class="middle-button-container">
                <a class="button blue middle one-move"><%=ProjectsCommonResource.OneTaskMoveButton %></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle cancel"><%= ProjectsCommonResource.Cancel %></a>
            </div>
        </Body>
    </sc:Container>
</div>