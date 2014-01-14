<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardEmptyScreen.ascx.cs" Inherits="ASC.Web.CRM.Controls.Common.DashboardEmptyScreen" %>

<div class="backdrop" blank-page=""></div>

<div id="content" blank-page="" class="dashboard-center-box crm">
    <div class="header">
        <span class="close"></span><%=CRMCommonResource.DashboardTitle %>
    </div>
    <div class="content clearFix">
       <div class="module-block">
           <div class="img contacts"></div>
           <div class="title"><%=String.Format(CRMCommonResource.ManageContacts,"<br />") %></div>
           <ul>
               <li><%=CRMCommonResource.DashContactsImport %></li>
               <li><%=CRMCommonResource.DashContactsDetails %></li>
               <li><%=CRMCommonResource.DashContactsMail %></li>
           </ul>
       </div>
       <div class="module-block">
           <div class="img sales"></div>
           <div class="title"><%= String.Format(CRMCommonResource.TrackSales,"<br />") %></div>
           <ul>
               <li><%=CRMCommonResource.DashSalesTasks %></li>
               <li><%=CRMCommonResource.DashSalesOpportunities %></li>
               <li><%=CRMCommonResource.DashSalesParticipants %></li>
           </ul>
       </div>
       <div class="module-block">
           <div class="img integration"></div>
           <div class="title"><%= String.Format(CRMCommonResource.ModulesIntegration,"<br />") %></div>
           <ul>
               <li><%=CRMCommonResource.DashIntegrationCalendar %></li>
               <li><%=CRMCommonResource.DashIntegrationProjects %></li>
               <li><%=CRMCommonResource.DashIntegrationDocs %></li>
           </ul>
       </div>
       <div class="module-block">
           <div class="img customize"></div>
           <div class="title"><%=String.Format(CRMCommonResource.SetUpCustomize,"<br />") %></div>
           <ul>
               <li><%=CRMCommonResource.DashCustomizeCRM %></li>
               <li><%=CRMCommonResource.DashCustomizeFields %></li>
               <li><%=CRMCommonResource.DashCustomizeWebsite %></li>
           </ul>
       </div>
    </div>
    <div class="dashboard-buttons">
        <a class="button huge create-button" href="default.aspx?action=manage&type=people">
          <%=CRMCommonResource.CreateNewContact %>          
        </a>
    </div>
</div>