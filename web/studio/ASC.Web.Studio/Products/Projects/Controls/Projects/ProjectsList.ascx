<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectsList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Projects.ProjectsList" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>


<div id="filterContainer">
    <div id="ProjectsAdvansedFilter"></div>
</div>
<div id="questionWindowTasks" style="display: none">
    <sc:Container ID="_hintPopupTasks" runat="server">
        <header>
    <%= ProjectResource.CloseProject%>
    </header>
        <body>
            <p>
                <%=ProjectResource.NotClosePrjWithActiveTasks%></p>
            <div class="middle-button-container">
                <a class="button blue middle" id="linkToTasks">
                    <%= ProjectResource.ViewActiveTasks%></a> 
                <span class="splitter-buttons"></span>
                <a class="button gray middle">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </sc:Container>
</div>
<div id="questionWindowMilestone" style="display: none">
    <sc:Container ID="_hintPopupMilestones" runat="server">
        <header>
    <%= ProjectResource.CloseProject%>
    </header>
        <body>
            <p>
                <%=ProjectResource.NotClosedPrjWithActiveMilestone%></p>
            <div class="middle-button-container">
                <a class="button blue middle" id="linkToMilestines">
                    <%= ProjectResource.ViewActiveMilestones%></a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle cancel">
                    <%= ProjectsCommonResource.Cancel%></a>
            </div>
        </body>
    </sc:Container>
</div>

<div class="simplePageNavigator">
</div>
<div id="tableListProjectsContainer">
    <table id="tableListProjects">
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
                <span class="gray-text"><%= ProjectsCommonResource.Total %> : </span>
                <span class="gray-text" style="margin-right: 20px;" id="totalCount"></span>
                <span class="gray-text"><%= ProjectsCommonResource.ShowOnPage %> : </span>
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

<div id="containerStatusList" class="studio-action-panel">
    <div class="corner-top left"></div>
    <ul id="statusList" class="dropdown-content">
        <li class="open dropdown-item" onclick="ASC.Projects.AllProject.changeStatus(this);">
            <%=ProjectResource.ActiveProject%></li>
        <li class="paused dropdown-item" onclick="ASC.Projects.AllProject.changeStatus(this);">
            <%=ProjectResource.PausedProject%></li>
        <li class="closed dropdown-item" onclick="ASC.Projects.AllProject.changeStatus(this);">
            <%=ProjectResource.ClosedProject%></li>
    </ul>
</div>
