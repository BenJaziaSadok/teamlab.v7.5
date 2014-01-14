<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectNavigatePanel.ascx.cs" Inherits="ASC.Web.Projects.Controls.Projects.ProjectNavigatePanel" %>
<%@ Register TagPrefix="uc" Namespace="ASC.Web.Studio.UserControls.Common.TabsNavigator" Assembly="ASC.Web.Studio" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div class="project-info-container">
    <div class="project-total-info">
        <div class="project-title header-with-menu">
            <% if (InConcreteProjectModule) { %>      
                <%if ((CurrentPage == "messages" || CurrentPage == "tasks") && !string.IsNullOrEmpty(UpLink))
                  {%> 
                  <a class="header-back-link" href="<%= UpLink %>"></a>
                  <%}%>          
                <span class="main-title-icon <%=CurrentPage %>"></span>                 
            <% } else {%>         
                <span class="main-title-icon projects" ></span> 
                <% if (Project.Private){%><span class="private"></span><% } %>
            <% } %>
            <span id="essenceTitle" class="text-overflow" title="<%= HttpUtility.HtmlEncode(EssenceTitle) %>"><%= HttpUtility.HtmlEncode(EssenceTitle)%></span>
            <span class="header-status" data-text="(<%=TaskResource.Closed.ToLower() %>)"><%= !string.IsNullOrEmpty(EssenceStatus) ? string.Format("({0})", EssenceStatus) : ""%></span>
             <% if (!InConcreteProjectModule)
                { %>
                 <%if(!IsInTeam)
                     if(!IsFollowed){%>
                    <a id="followProject" class="follow-status unsubscribed" data-text="<%= ProjectsCommonResource.Unfollow %>" title="<%= ProjectsCommonResource.Follow %>"></a>
                    <% }else{ %>
                    <a id="followProject"  class="follow-status subscribed" data-followed="followed" data-text="<%= ProjectsCommonResource.Follow %>" title="<%= ProjectsCommonResource.Unfollow %>"></a>
                  <% } %>
            <% }else 
            if(CurrentPage == "messages"){%>            
                <a id="changeSubscribeButton" subscribed="<%= IsSubcribed ? "1": "0" %>" class="follow-status <%= IsSubcribed ? "subscribed" : "unsubscribed"%>" title="<%= IsSubcribed ? ProjectsCommonResource.UnSubscribeOnNewComment : ProjectsCommonResource.SubscribeOnNewComment%>"></a>            
               <% } else 
            if(CurrentPage == "tasks"){%>              
                <a id="followTaskActionTop" class="follow-status <%= IsSubcribed ? "subscribed" : "unsubscribed"%>" textvalue="<%= IsSubcribed ? TaskResource.FollowTask :TaskResource.UnfollowTask%>" onclick="ASC.Projects.TaskDescroptionPage.subscribeTask();" title="<%=IsSubcribed ? TaskResource.UnfollowTask : TaskResource.FollowTask%>"></a>           
            <% } %>
            <span class="menu-small <% if (!InConcreteProjectModule && IsInTeam){ %> vertical-align-middle<%} %> <%if (Page.Participant.IsVisitor && Request["id"] != null){  %> visibility-hidden<% } %>"></span>    
        </div>
        <% if (ShowGanttChartFlag){ %>
            <a class="button blue middle gant-chart-link"  href="ganttchart.aspx?prjID=<%=Project.ID %>" target="_blank"><%=ProjectResource.GanttGart %></a>
        <% } %>
    </div>

    <uc:TabsNavigator ID="Tabs" runat="server" BlockID="projectTabs">
        <TabItems></TabItems>
    </uc:TabsNavigator>
</div>
    <div id="projectActions" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <% if (CanEditProject && !Page.Participant.IsVisitor){%>
                <li><a class="dropdown-item" href="projects.aspx?prjID=<%= Project.ID %>&action=edit"><%= ProjectsCommonResource.Edit %></a></li>
            <% }%>
            <% if (CanDeleteProject){%>
                <li><a id="deleteProject" class="dropdown-item"><%= ProjectsCommonResource.Delete %></a></li>
            <% } %>
            <li><a id="viewDescription" class="dropdown-item"><%= ProjectsCommonResource.ViewProjectInfo%></a></li>
        </ul>
    </div>

    <div id="questionWindowDelProj" style="display: none">
        <sc:Container ID="_hintPopup" runat="server">
            <header><%= ProjectResource.DeleteProject %></header>
            <body>
                <p>
                    <%= ProjectResource.DeleteProjectPopup %>
                </p>
                <p>
                    <%= ProjectsCommonResource.PopupNoteUndone %>
                </p>
                <div class="middle-button-container">
                    <a class="button blue middle remove">
                        <%= ProjectResource.DeleteProject %>
                    </a> 
                    <span class="splitter-buttons"></span>
                    <a class="button gray middle cancel">
                        <%= ProjectsCommonResource.Cancel %>
                    </a>
                </div>
                <div class="pm-ajax-info-block display-none">
                    <span class="text-medium-describe"><%= ProjectResource.LoadingWait%></span><br/>
                    <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"/>
                </div>
            </body>
        </sc:Container>
    </div>
    
    <div class="projectDescriptionPopup">
        <sc:Container ID="_projectDescriptionPopup" runat="server">
            <header><%= ProjectsCommonResource.ProjectInformation%></header>
            <body>
                <div class="descriptionContainer">
                    <div class="section-name"><span><%=ProjectsCommonResource.ProjectName%></span> 
                        <%=HttpUtility.HtmlEncode(Project.Title)%></div>
                    <div class="section-name"><span><%=ProjectResource.ProjectLeader%></span> 
                        <%=ProjectLeaderName%></div>
                    <div class="section-name"><span><%=ProjectsFilterResource.ByCreateDate %></span> 
                        <%=HttpUtility.HtmlEncode(Project.CreateOn.ToShortDateString()) %></div>
                    <%if (!string.IsNullOrEmpty(Project.Description))
                      {%>
                    <div class="section-name"><span><%= ProjectsCommonResource.Description %></span>
                        <p id="prjInfoDescription" data-description="<%= HttpUtility.HtmlEncode(Project.Description) %>"><%= HttpUtility.HtmlEncode(Project.Description) %></p></div>
                    <% } %>
                </div>
            </body>
        </sc:Container>
    </div>
