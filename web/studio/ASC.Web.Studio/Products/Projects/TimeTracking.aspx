<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>

<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    CodeBehind="timeTracking.aspx.cs" Inherits="ASC.Web.Projects.TimeTracking" %>

<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Src="~/Products/Projects/Controls/TimeSpends/TimeSpendActionView.ascx"
    TagPrefix="ctrl" TagName="TimeSpendActionView" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">

   <div id="MainPageContainer" runat="server">
       <%if (TaskID <= 0)
        {%>

            <div id="filterContainer">
                <div id="ProjectsAdvansedFilter"></div>
            </div>

            <div class="total-time-forFilter">
                <%= TimeTrackingResource.TotalTimeNote%>
                <span class="total-count">
                    <%=TimeTrackingResource.TotalTimeCommon %>
                    <b><span class="hours"></span> <%= TimeTrackingResource.Hours%>
                    <span class="minutes"></span> <%= TimeTrackingResource.Minutes%></b>
                </span>
                <span class="billed-count">
                    <%=TimeTrackingResource.TotalTimeBilled %> 
                    <b><span class="hours"></span> <%= TimeTrackingResource.Hours%>
                    <span class="minutes"></span> <%= TimeTrackingResource.Minutes%></b>
                </span>
            </div>
        <% }else
          { %>
            <div class="total-time-forFilter">
                <%= TimeTrackingResource.TimeSpentForTask%> 
                <span class="total-count">
                    <%=TimeTrackingResource.TotalTimeCommon %>
                    <b><span class="hours"></span> <%= TimeTrackingResource.Hours%> 
                    <span class="minutes"></span> <%= TimeTrackingResource.Minutes%></b>
                </span>
                <span class="billed-count">
                    <%=TimeTrackingResource.TotalTimeBilled %> 
                    <b><span class="hours"></span> <%= TimeTrackingResource.Hours%>
                    <span class="minutes"></span> <%= TimeTrackingResource.Minutes%></b>
                </span>
            </div>          
        <% } %>
        <asp:PlaceHolder runat="server" ID="_emptyScreens"></asp:PlaceHolder>
        
        <div id="mainContent" class="pm-headerPanel-splitter timeSpendsList">
            <%if (ShowMultiActionPanel)
              { %>
                <ul id="timeTrakingGroupActionMenu" class="clearFix contentMenu contentMenuDisplayAll">
                    <li class="menuAction menuActionSelectAll menuActionSelectLonely">
                        <div class="menuActionSelect">
                            <input id="selectAllTimers" type="checkbox" title="<%= TimeTrackingResource.GroupMenuSelectAll %>"/>
                        </div>
                    </li>
                    <li class="menuAction" data-status="billed" title="<%= TimeTrackingResource.PaymentStatusBilled %>">
                        <span><%= TimeTrackingResource.PaymentStatusBilled%></span>
                    </li>
                    <li class="menuAction" data-status="not-billed" title="<%= TimeTrackingResource.PaymentStatusNotBilled %>">
                        <span><%= TimeTrackingResource.PaymentStatusNotBilled%></span>
                    </li>
                    <li class="menuAction" data-status="not-chargeable" title="<%=TimeTrackingResource.PaymentStatusNotChargeable%>">
                        <span><%= TimeTrackingResource.PaymentStatusNotChargeable%></span>
                    </li>
                    <li class="menuAction" data-status="delete" title="<%= ProjectsCommonResource.Delete%>">
                        <span><%= ProjectsCommonResource.Delete%></span>
                    </li>

                    <li class="menu-action-checked-count">
                        <span></span>
                        <a id="deselectAllTimers" class="link dotline small">
                            <%= TimeTrackingResource.GroupMenuDeselectAll%>
                        </a>
                    </li>
                    <li class="menu-action-on-top">
                        <a class="on-top-link" onclick="javascript:window.scrollTo(0, 0);">
                            <%= TimeTrackingResource.GroupMenuOnTop%>
                        </a>
                    </li>
                </ul>
                <div class="header-menu-spacer"> </div>
            <%} %>
            <div class="clearFix"> </div>
            <table id="timeSpendsList" class="listContainer pm-tablebase<%if (TaskID==-1){%> forProject<%} %>">
                <thead>
                </thead>
                <tbody>
                </tbody>
            </table>
            <span id="showNext">
                <%= ProjectsCommonResource.ShowNext%></span>
            <div class="taskProcess" id="showNextProcess">
            </div>
        </div>

        <div id="timeActionPanel" class="studio-action-panel" objid="">
            <div class="corner-top right"></div>
            <ul class="dropdown-content">
                <li id="ta_edit" class="dropdown-item">
                    <span><%= TaskResource.Edit%></span>
                </li>
                <li id="ta_remove" class="dropdown-item">
                    <span><%= ProjectsCommonResource.Delete%></span>
                </li>
            </ul>
        </div>
        
        <div id="statusListContainer" class="studio-action-panel">
            <div class="corner-top left"></div>
            <ul id="statusList" class="dropdown-content">
                <li class="not-chargeable dropdown-item"><%= TimeTrackingResource.PaymentStatusNotChargeable%></li>
                <li class="not-billed dropdown-item"><%= TimeTrackingResource.PaymentStatusNotBilled%></li>
                <li class="billed dropdown-item"><%= TimeTrackingResource.PaymentStatusBilled%></li>
            </ul>
        </div>
        
        <ctrl:TimeSpendActionView runat="server" ID="_timeSpendActionView" />
    </div>

    
    <div id="questionWindowTimerRemove" style="display: none">
    <sc:Container ID="_popupTimerRemove" runat="server">
        <Header>
            <%= TimeTrackingResource.DeleteTimers%>
        </Header>
        <Body>
            <p>
                <%= TimeTrackingResource.DeleteTimersQuestion%>
            </p>
            <p>
                <%=ProjectsCommonResource.PopupNoteUndone %></p>
            <div class="errorBox display-none"></div>
            <div class="middle-button-container">
                <a id="deleteTimersButton" class="button blue middle"><%= TimeTrackingResource.DeleteTimers%></a> 
                <span class="splitter-buttons"></span>
                <a class="button gray middle cancel"><%= ProjectsCommonResource.Cancel%></a>
            </div>
        </Body>
    </sc:Container>
</div>
</asp:Content>

<asp:Content ID="projectsClientTemplatesResourcesPlaceHolder" ContentPlaceHolderID="projectsClientTemplatesResourcesPlaceHolder" runat="server">
</asp:Content>

