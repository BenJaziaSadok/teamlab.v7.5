<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MainMenu.ascx.cs"
    Inherits="ASC.Web.Files.Controls.MainMenu" %>
<%@ Import Namespace="ASC.Web.Files.Import" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<ul id="mainMenuHolder" class="menu-actions">
    <li id="menuCreateNewButton" class="menu-main-button" title="<%= FilesUCResource.ButtonCreate %>">
        <span class="main-button-text"><%= FilesUCResource.ButtonCreate %></span>
        <span class="white-combobox">&nbsp;</span>
    </li>
    <% if (!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
       {%>
    <li id="buttonUpload" class="menu-upload-button" title="<%= FilesUCResource.ButtonUpload %>">
        <span class="menu-upload-icon">&nbsp;</span>
    </li>
    <% } %>
    <% if (EnableImport || EnableThirdParty)
       {%>
    <li id="buttonThirdparty" class="menu-gray-button" title="<%= FilesUCResource.ButtonAddThirdParty %>">
        <span class="btn_other-actions">...</span>
    </li>
    <% } %>
</ul>

<asp:PlaceHolder runat="server" ID="ControlHolder"></asp:PlaceHolder>

<ul id="treeSecondary" class="menu-list">

    <li id="treeSetting" class="menu-item sub-list add-block">
        <div class="category-wrapper">
            <span class="expander"></span>
            <a href="#setting" class="menu-item-label outer-text text-overflow" title="<%= FilesUCResource.SideCaptionSettings %>">
                <span class="menu-item-icon settings"></span>
                <span class="menu-item-label inner-text"><%= FilesUCResource.SideCaptionSettings %></span>
            </a>
        </div>
        <ul class="menu-sub-list">
            <li class="menu-sub-item settings-link-common">
                <a class="menu-item-label outer-text text-overflow" href="#setting" title="<%= FilesUCResource.ThirdPartyConnectAccounts %>">
                    <span class="menu-item-label inner-text"><%= FilesUCResource.CommonSettings %></span>
                </a>
            </li>
            <% if (EnableThirdParty)
               { %>
            <li class="menu-sub-item settings-link-thirdparty">
                <a class="menu-item-label outer-text text-overflow" href="#setting=thirdparty" title="<%= FilesUCResource.ThirdPartyConnectAccounts %>">
                    <span class="menu-item-label inner-text"><%= FilesUCResource.ThirdPartyConnectAccounts %></span>
                </a>
            </li>
            <% } %>
        </ul>
    </li>

    <asp:PlaceHolder runat="server" ID="sideHelpCenter"></asp:PlaceHolder>
    <asp:PlaceHolder runat="server" ID="sideSupport"></asp:PlaceHolder>
</ul>

<%-- popup window --%>
<div id="newDocumentPanel" class="studio-action-panel files-popup-win display-nonel">
    <div class="corner-top left"></div>
    <ul class="dropdown-content">
        <asp:PlaceHolder runat="server" ID="CreateMenuHolder"></asp:PlaceHolder>
    </ul>
</div>
<div id="thirdPartyListPanel" class="studio-action-panel files-popup-win display-none">
    <div class="corner-top left">
    </div>
    <ul class="dropdown-content">

        <% if (EnableImport)
           {%>
        <% if (ImportConfiguration.SupportGoogleImport)
           { %>
        <li id="importFromGoogle"><a class="dropdown-item">
            <%= FilesUCResource.ImportFromGoogle %></a></li>
        <% } %>
        <% if (ImportConfiguration.SupportZohoImport)
           { %>
        <li id="importFromZoho"><a class="dropdown-item">
            <%= FilesUCResource.ImportFromZoho %></a></li>
        <% } %>
        <% if (ImportConfiguration.SupportBoxNetImport)
           { %>
        <li id="importFromBoxNet"><a class="dropdown-item">
            <%= FilesUCResource.ImportFromBoxNet %></a></li>
        <% } %>
        <% } %>

        <% if (EnableImport && EnableThirdParty)
           {%>
        <li>
            <div class="dropdown-item-seporator"></div>
        </li>
        <% } %>


        <% if (EnableThirdParty)
           {%>
        <% if (ImportConfiguration.SupportDropboxInclusion)
           { %>
        <li id="addThirdpartyDropBox"><a class="dropdown-item">
            <%= FilesUCResource.ButtonAddDropBox %></a> </li>
        <% } %>
        <% if (ImportConfiguration.SupportGoogleInclusion)
           { %>
        <li id="addThirdpartyGoogle"><a class="dropdown-item">
            <%= FilesUCResource.ButtonAddGoogle %></a> </li>
        <% } %>
        <% if (ImportConfiguration.SupportBoxNetInclusion)
           { %>
        <li id="addThirdpartyBox"><a class="dropdown-item">
            <%= FilesUCResource.ButtonAddBoxNet %></a> </li>
        <% } %>
        <% if (ImportConfiguration.SupportSkyDriveInclusion)
           { %>
        <li id="addThirdpartySkydrive"><a class="dropdown-item">
            <%= FilesUCResource.ButtonAddSkyDrive %></a> </li>
        <% } %>
        <% } %>
    </ul>
</div>
