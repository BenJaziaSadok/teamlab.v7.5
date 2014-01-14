<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManagementNavigation.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.NavigationSidePanel.ManagementNavigation" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>

<div class="page-menu">
    <ul class="menu-list">
        <li class="menu-item none-sub-list <%if(CurrentPage=="common"){%>active<%}%>">            
            <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.General) %>" title="<%=Resources.Resource.GeneralSettings%>">
                <span class="menu-item-icon general"></span>
                <span class="menu-item-label inner-text"><%=Resources.Resource.GeneralSettings%></span>
            </a>
        </li>
        <li class="menu-item none-sub-list <%if(CurrentPage=="1"){%>active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.ProductsAndInstruments) %>" title="<%=Resources.Resource.ProductsAndInstruments%>">
                <span class="menu-item-icon modules"></span>
                <span class="menu-item-label inner-text"><%=Resources.Resource.ProductsAndInstruments%></span>
            </a>
        </li>
        <li class="menu-item none-sub-list <%if(CurrentPage=="8"){%>active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.AccessRights) %>" title="<%=Resources.Resource.AccessRights%>">
                <span class="menu-item-icon access"></span>
                <span class="menu-item-label inner-text"><%=Resources.Resource.AccessRights%></span>
            </a>
        </li>
        <li class="menu-item none-sub-list <%if(CurrentPage=="7"){%>active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.Customization) %>" title="<%=Resources.Resource.Customization%>">
                <span class="menu-item-icon customization"></span>
                <span class="menu-item-label inner-text"><%=Resources.Resource.Customization%></span>
            </a>
        </li>
        <% if (ASC.Web.Studio.Core.SetupInfo.IsVisibleSettings("Backup"))
           { %>
            <li class="menu-item none-sub-list <%if(CurrentPage=="6"){%>active<%}%>" title="<%=Resources.Resource.Backup%>">
                <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.Account) %>">
                    <span class="menu-item-icon backup"></span>
                    <span class="menu-item-label inner-text"><%=Resources.Resource.Backup%></span>
                </a>
            </li>
        <% } %>
        <li class="menu-item none-sub-list <%if(CurrentPage=="5"){%>active<%}%>">
            <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.Statistic) %>" title="<%=Resources.Resource.StatisticsTitle%>">
                <span class="menu-item-icon statistics"></span>
                <span class="menu-item-label inner-text"><%=Resources.Resource.StatisticsTitle%></span>
            </a>
        </li>
        <% if (ASC.Core.CoreContext.Configuration.Standalone)
           { %>
            <li class="menu-item none-sub-list <% if (CurrentPage == "3"){ %>active<% } %>">
                <a class="menu-item-label outer-text text-overflow" href="<%= CommonLinkUtility.GetAdministration(ManagementType.AuthorizationKeys) %>" title="<%= Resources.Resource.StatisticsTitle %>">
                    <span class="menu-item-icon authorization"></span>
                    <span class="menu-item-label inner-text"><%= Resources.Resource.ThirdPartyAuthorization %></span>
                </a>
            </li>
        <% } %>

        <asp:PlaceHolder ID="HelpHolder" runat="server"></asp:PlaceHolder>
        <asp:PlaceHolder ID="SupportHolder" runat="server"></asp:PlaceHolder>
    </ul>
</div>