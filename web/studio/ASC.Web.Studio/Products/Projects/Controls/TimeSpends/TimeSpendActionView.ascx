<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeSpendActionView.ascx.cs" Inherits="ASC.Web.Projects.Controls.TimeSpends.TimeSpendActionView" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>


<div id="timeTrakingPopup" style="display:none;">
    <sc:Container id="_timetrackingContainer" runat="server">
    
    <header>    
        <%= ProjectsCommonResource.TimeTracking %>
    </header>
    
    <body>
        
        <div id="TimeLogTaskTitle" class="header-base pm-headerPanelSmall-splitter"></div>
           
        <div class="addLogPanel-infoPanel">
            <div class="addLogPanel-infoPanelBody">
                <span class="header-base gray-text">
                    <%= ProjectsCommonResource.SpentTotally %>
                </span>
                <span class="splitter-buttons"></span>
                <span id="TotalHoursCount" class="header-base"></span>
                <span class="splitter-buttons"></span>
            </div>
        </div> 
        <div class="warnBox" style="display:none;" id="timeTrakingErrorPanel"></div>
        <div class="pm-headerPanelSmall-splitter" style="float:right">
            <div class="headerPanelSmall">
                <b><%= TaskResource.TaskResponsible %>:</b>
            </div>
            <select style="width: 220px;" class="comboBox pm-report-select" id="teamList"></select>
        </div>
        
        <div>   
        <div class="pm-headerPanelSmall-splitter" style="float:left;margin-right:20px">
            <div class="headerPanelSmall">
                <b><%= ProjectsCommonResource.Time%>:</b>
            </div>
            <input id="inputTimeHours" type="text" placeholder="<%=ProjectsCommonResource.WatermarkHours %>" class="textEdit" maxlength="2" />
            <span class="splitter">:</span>
            <input id="inputTimeMinutes" type="text" placeholder="<%=ProjectsCommonResource.WatermarkMinutes %>" class="textEdit" maxlength="2" />
        </div>
           
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <b><%= ProjectsCommonResource.Date %>:</b>
            </div>
            <input id="timeTrakingDate" class="textEditCalendar" style="margin-right: 3px"/>
        </div>
        </div>
        
        <div style="clear:both"></div>
           
        <div class="pm-headerPanelSmall-splitter">
            <div class="headerPanelSmall">
                <b><%= ProjectResource.ProjectDescription %>:</b>
            </div>
            <textarea id="timeDescription" rows="7" cols="20"></textarea>
        </div>
         
        <div class="middle-button-container">
            <a href="javascript:void(0)" class="button blue middle">
                <%= ProjectsCommonResource.SaveChanges%>
            </a>
            <span class="splitter-buttons"></span>
            <a class="button gray middle" href="javascript:void(0)" onclick="javascript: jq.unblockUI();">
                <%= ProjectsCommonResource.Cancel%>
            </a>
        </div>
           
    </body>
</sc:Container>

</div>
