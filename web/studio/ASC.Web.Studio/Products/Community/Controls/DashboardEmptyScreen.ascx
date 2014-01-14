<%@ Assembly Name="ASC.Web.Community" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.Core.Users"%>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardEmptyScreen.ascx.cs" Inherits="ASC.Web.Community.Controls.DashboardEmptyScreen" %>


<div class="backdrop" blank-page=""></div>

<div id="content" blank-page="" class="dashboard-center-box community">
    <div class="header">
        <a href="<%=ProductStartUrl%>">
            <span class="close"></span>
        </a>
        <%=ASC.Web.Community.Resources.CommunityResource.DashboardTitle%>
    </div>
    <div class="content clearFix">
    
    <%if (ASC.Core.SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser))
      { %>
       <div class="module-block">
            <div class="img users"></div>
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleSecondLine%></li>
           </ul>
           <a id="addUsersDashboard" href="javascript:void(0)" class="links"><%=ASC.Web.Community.Resources.CommunityResource.UsersModuleLink%></a>
       </div>
       
       <%}
      else
      {%>
      <style type="text/css">
          .dashboard-center-box.community .content .module-block  {width: 265px;}
          .dashboard-center-box.community .content .module-block { margin-left: 55px;}
          .dashboard-center-box.community .module-block.wiki ul { margin-bottom: 14px;}
      </style>
      <%} %>
       <div class="module-block">
           <div class="img blogs"></div>
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleSecondLine%></li>
           </ul>
           <a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/addblog.aspx")%>"  class="links"><%=ASC.Web.Community.Resources.CommunityResource.BlogsModuleLink1%></a>
           
       </div>
       <div class="module-block wiki">
           <div class="img bookmarks"></div>
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleSecondLine%></li>
           </ul>
           <a href="<%=VirtualPathUtility.ToAbsolute("~/products/community/modules/bookmarking/createbookmark.aspx")%>" class="links"><%=ASC.Web.Community.Resources.CommunityResource.WikiModuleLink2%></a>
       </div>
       <div class="module-block">
           <div class="img helpcenter"></div>
           <div class="title"><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleTitle%></div>
           <ul>
               <li><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleFirstLine%></li>
               <li><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleSecondLine%></li>
           </ul>
           <a href="http://helpcenter.teamlab.com" target="_blank" class="links"><%=ASC.Web.Community.Resources.CommunityResource.HelpModuleLink%></a>
       </div>
    </div>
    
</div>
