<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TopStudioPanel.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.TopStudioPanel" %>

<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Studio" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Studio.UserControls.Statistics" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="Resources" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div class="studio-top-panel mainPageLayout clearFix try-welcome-top">
    <ul>
        <li class="studio-top-logo ">
            <a class="top-logo" title="<%= Resource.TeamLabOfficeTitle %>" href="<%= CommonLinkUtility.GetDefault() %>">
                <img alt="" src="<%= GetAbsoluteCompanyTopLogoPath() %>" />
            </a>
        </li>
        <% if (CoreContext.Configuration.YourDocsDemo && Page is Auth)
           { %>
            <li class="try-welcome-top-label header-base big"><%= Resource.LabelTopTextHtml5 %></li>
        <% } %>
        <% if (ShowTopPanelNavigation && DisplayModuleList)
           { %>
            <li class="product-menu <%= DisplayModuleList ? "with-subitem" : string.Empty %> ">
                <span class="active-icon <%= CurrentProductClassName %>"></span>
                <a class="product-cur-link baseLinkAction" title="<%= CurrentProductName %>">
                    <%= CurrentProductName.HtmlEncode() %>
                </a>
            </li>
        <% } %>
        <% if (!DisableTariffNotify)
           { %>
            <li class="studio-top-trial-period">
                <a href="<%= TenantExtra.GetTariffPageLink() %>" title="<%= TariffNotify %>" ><%= TariffNotify %></a>
            </li>
        <% } %>
        
        <asp:PlaceHolder runat="server" ID="_guestInfoHolder">
            <li class="">
                <a href="<%= VirtualPathUtility.ToAbsolute("~/auth.aspx") %>">
                    <%= Resource.LoginButton %>
                </a>
            </li>
        </asp:PlaceHolder>

        <asp:PlaceHolder runat="server" ID="_userInfoHolder">
            <%--my staff--%>

            <li class="staff-profile-box" onselectstart="return false;" onmousedown=" return false; ">
                <span class="userLink">
                    <span class="usr-prof baseLinkAction" title="<%= ASC.Core.Users.UserInfoExtension.DisplayUserName(CurrentUser) %>">
                        <%= ASC.Core.Users.UserInfoExtension.DisplayUserName(CurrentUser) %>
                    </span>
                </span>
            </li>
            <%= RenderCustomNavigation() %>
        </asp:PlaceHolder>
                   
        <% if (!DisableSearch)
           { %>
            <li class="top-item-box search">
                <span class="searchActiveBox inner-text" title="<%= Resource.Search %>"></span>
            </li>
        <% } %>

        <% if (IsAdministrator && !DisableSettings)
           { %>
            <li class="top-item-box settings" >
                <a class="inner-text" href="<%= CommonLinkUtility.GetAdministration(ManagementType.General) %>" title="<%= Resource.Administration %>"></a>
            </li>
        <% } %>

        <% if (!DisableTariff)
           { %>
            <li class="top-item-box tariffs">
                <a class="inner-text" href="<%= TenantExtra.GetTariffPageLink() %>" title="<%= Resource.TariffSettings %>"></a>
            </li>
        <% } %>
        
        <% if (!DisableVideo)
           { %>
            <li class="top-item-box video">
                <a class="videoActiveBox inner-text" href="<%= AllVideoLink %>" target="_blank" title="<%= Resource.VideoGuides %>" data-videoUrl="<%= AllVideoLink %>">
                    <span class="inner-label"></span>
                </a>
            </li>
        <% } %>
        
        <li class="clear"></li>
    </ul>
    
    <asp:PlaceHolder runat="server" ID="_productListHolder">
        <% if (ShowTopPanelNavigation && DisplayModuleList)
           { %>
            <div id="studio_productListPopupPanel" class="studio-action-panel modules">
                <div class="corner-top right"></div>
                <ul class="dropdown-content">
                    <asp:Repeater runat="server" ID="_productRepeater" ItemType="ASC.Web.Core.IWebItem">
                        <ItemTemplate>
                            <li class="<%# Item.ProductClassName + (Item.IsDisabled() ? " display-none" : string.Empty) %>">
                                <a href="<%# VirtualPathUtility.ToAbsolute(Item.StartURL) %>" class="dropdown-item menu-products-item">
                                    <%# (Item.Name).HtmlEncode() %>
                                </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>
                    <li class="dropdown-item-seporator"></li>
                    <asp:Repeater runat="server" ID="_addonRepeater" ItemType="ASC.Web.Core.IWebItem">
                        <ItemTemplate>
                            <li class="<%# Item.ProductClassName + (Item.IsDisabled() ? " display-none" : string.Empty) %>">
                                <a href="<%# VirtualPathUtility.ToAbsolute(Item.StartURL) %>" class="dropdown-item menu-products-item">
                                    <%# (Item.Name).HtmlEncode() %>
                                </a>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                    <li class="feed"><a href="<%= VirtualPathUtility.ToAbsolute("~/feed.aspx") %>" class="dropdown-item menu-products-item"><%= UserControlsCommonResource.FeedTitle %></a></li>

                    <% if (IsAdministrator)
                       { %>
                        <li class="dropdown-item-seporator"></li>
                        <li class="settings"><a href="<%= CommonLinkUtility.GetAdministration(ManagementType.General) %>" title="<%= Resource.Administration %>" class="dropdown-item menu-products-item"><%= Resource.Administration %></a></li>
                    <% } %>
                    <% if (!DisableTariff)
                       { %>
                    <li class="tarrifs"><a href="<%= TenantExtra.GetTariffPageLink() %>" title="<%= Resource.TariffSettings %>" class="dropdown-item menu-products-item"><%= Resource.TariffSettings %></a></li>
                    <% } %>
                </ul>
            </div>
        <% } %>
    </asp:PlaceHolder>

    <% if (!DisableSearch)
       { %>
        <div id="studio_searchPopupPanel" class="studio-action-panel">
            <div class="corner-top left"></div>
            <div class="dropdown-content">
                <div class="search-input-box">
                    <input type="text" id="studio_search" class="search-input textEdit" placeholder="<%= UserControlsCommonResource.SearchHld %>" maxlength="255" data-url="<%= VirtualPathUtility.ToAbsolute("~/search.aspx") %>" />
                    <button class="button blue search-btn"></button>
                </div>
                <div class="header-base small bold"><%= UserControlsCommonResource.SeeInModulesHdr %></div>
                <div class="search-options-box clearFix">
                    <% foreach (var product in SearchProducts)
                       { %>
                    <div class="search-option-box">
                        <label>
                            <input type="checkbox" data-product-id="<%= product.ID %>"/>
                            <%= product.Name %>
                        </label>
                    </div>
                    <% } %>
                </div>
            </div>
        </div>
    <% } %>
    
    <div id="studio_myStaffPopupPanel" class="studio-action-panel">
        <div class="corner-top right"> </div>
        <ul class="dropdown-content">
            <% if (!TenantStatisticsProvider.IsNotPaid() && !CoreContext.Configuration.YourDocs)
               { %>
                <li>
                    <a class="dropdown-item" href="<%= CommonLinkUtility.GetMyStaff() %>">
                        <%= Resource.Profile %>
                    </a>
                </li>
            
                <%--Logout--%>
            <% } %>
            <li class="dropdown-item-seporator"></li>
            <% if (DebugInfo.ShowDebugInfo) %>
            <%
               { %>
                <li>
                    <a class="dropdown-item" href="javascript:void(0);" onclick=" StudioBlockUIManager.blockUI('#debugInfoPopUp', 1000, 300, -300); ">
                        Debug Info
                    </a>
                    <div id="debugInfoPopUp" style="display: none">
                        <sc:Container ID="debugInfoPopUpContainer" runat="server">
                            <Header>
                                <span>Debug Info</span>
                            </Header>
                            <Body>
                                <div style="height: 500px; overflow-y: scroll;">
                                    <%= DebugInfo.DebugString.HtmlEncode().Replace("\n\r", "<br/>").Replace("\n", "<br/>") %>
                                </div>
                                <div class="middle-button-container">
                                    <a class="button blue middle" href="javascript:void(0)" onclick=" jq.unblockUI(); ">Ok</a>
                                </div>
                            </Body>
                        </sc:Container>
                    </div>
                </li>
            <% } %>
            <li><a class="dropdown-item" href="<%= CommonLinkUtility.Logout %>">
                    <%= UserControlsCommonResource.LogoutButton %></a></li>
        </ul>
    </div>

    <div id="studio_dropFeedsPopupPanel" class="studio-action-panel">
        <div class="corner-top right"></div>
        
        <div id="drop-feeds-box">
            <div class="list display-none"></div>
            <div class="loader"><span><%= FeedResource.LoadingMsg %></span></div>
            <div class="feeds-readed-msg"><span><%= FeedResource.FeedsReadedMsg %></span></div>
            <a class="see-all-btn" href="<%= VirtualPathUtility.ToAbsolute("~/feed.aspx") %>">
                <%= FeedResource.SeeAllFeedsBtn %>
            </a>
        </div>
    </div>
    
    <% if (!DisableVideo)
       { %>
        <div id="studio_videoPopupPanel" class="video-popup-panel studio-action-panel">
            <div class="corner-top right"></div>
            <div id="dropVideoList" class="drop-list">
                <ul class="video-list">
                    <% foreach (var data in VideoGuideItems)
                       { %>
                        <li>
                            <a class="link" id="<%= data.Id %>" href="<%= data.Link %>" target="_blank"><%= data.Title %></a>
                        </li>
                    <% } %>
                </ul>
            </div>
            <a class="video-link" href="<%= AllVideoLink %>">
                <%= Resource.SeeAllVideos %>
            </a> 
            <a id="markVideoRead" class="video-link" href="javascript:void(0);">
                <%= Resource.MarkAllAsRead %>
            </a>     
        </div>
    <% } %>
    <div id="MoreProductsPopupPanel" class="studio-action-panel">
        <div class="corner-top left"></div>
        <ul class="dropdown-content">
            <asp:Repeater runat="server" ID="MoreProductsRepeater" ItemType="ASC.Web.Core.IWebItem">
                <ItemTemplate>
                    <div>
                        <li>
                            <a href="<%# VirtualPathUtility.ToAbsolute(Item.StartURL) %>" class="dropdown-item">
                                <%# Item.Name.HtmlEncode() %>
                            </a>
                        </li>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    <li class="dropdown-item-seporator"></li>
                </FooterTemplate>
            </asp:Repeater>
            <li><a class="dropdown-item" href="<%= CommonLinkUtility.GetDefault() %>"><%= UserControlsCommonResource.AllProductsTitle %></a></li>
        </ul>
    </div>

    <asp:PlaceHolder runat="server" ID="_customNavControls"></asp:PlaceHolder>
</div>