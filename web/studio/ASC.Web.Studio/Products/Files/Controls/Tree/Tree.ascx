<%@ Assembly Name="ASC.Web.Files" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Tree.ascx.cs" Inherits="ASC.Web.Files.Controls.Tree" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<%-- TREE --%>
<div id="treeViewContainer" class="jstree <%= FolderIDCurrentRoot != null ? "jstree-inprojects" : string.Empty %> webkit-scrollbar">
    <% if (FolderIDCurrentRoot == null)
       {%>
    <ul>
        <% if (!IsVisitor)
           { %>
        <li data-id="<%= Global.FolderMy %>" class="tree-node jstree-closed">
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderMy %>" title="<%= FilesUCResource.MyFiles %>" href="#<%= Global.FolderMy %>">
                <span class="jstree-icon myFiles"></span><%= FilesUCResource.MyFiles %>
            </a>
            <span class="is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderMy %>"></span>
        </li>
        <% } %>
        <% if (!CoreContext.Configuration.YourDocs)
           { %>
        <li data-id="<%= Global.FolderShare %>" class="tree-node jstree-closed access-read">
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderShare %>" title="<%= FilesUCResource.SharedForMe %>" href="#<%= Global.FolderShare %>">
                <span class="jstree-icon shareformeFiles"></span><%= FilesUCResource.SharedForMe %>
            </a>
            <span class="is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderShare %>"></span>
        </li>
        <li data-id="<%= Global.FolderCommon %>" class="tree-node jstree-closed <%= Global.IsAdministrator ? string.Empty : "access-read" %>">
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderCommon %>" title="<%= FilesUCResource.CorporateFiles %>" href="#<%= Global.FolderCommon %>">
                <span class="jstree-icon corporateFiles"></span><%= FilesUCResource.CorporateFiles %>
            </a>
            <span class="is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderCommon %>"></span>
        </li>
        <% if (Global.FolderProjects != null)
           { %>
        <li data-id="<%= Global.FolderProjects %>" class="tree-node jstree-closed access-read">
            <span class="jstree-icon jstree-expander"></span>
            <a data-id="<%= Global.FolderProjects %>" title="<%= FilesUCResource.ProjectFiles %>" href="#<%= Global.FolderProjects %>">
                <span class="jstree-icon projectFiles"></span><%= FilesUCResource.ProjectFiles %>
            </a>
            <span class="is-new" title="<%= FilesUCResource.RemoveIsNew %>" data-id="<%= Global.FolderProjects %>"></span>
        </li>
        <% } %>
        <% } %>
        <% if (!CoreContext.Configuration.YourDocsDemo)
           { %>
        <li data-id="<%= Global.FolderTrash %>" class="tree-node jstree-closed">
            <span class="jstree-icon visibility-hidden" ></span>
            <a data-id="<%= Global.FolderTrash %>" title="<%= FilesUCResource.Trash %>" href="#<%= Global.FolderTrash %>">
                <span class="jstree-icon trashFiles"></span><%= FilesUCResource.Trash %>
            </a>
        </li>
        <% } %>
    </ul>
    <% } %>
</div>
<% if (SecurityContext.IsAuthenticated)
   { %>
<div id="treeViewPanelSelector" class="studio-action-panel webkit-scrollbar files-popup-win">
    <div class="corner-top"></div>
    <div class="select-folder-header">
        <b><%= FilesUCResource.SelectFolder %></b>
    </div>
    <div class="jstree">
        <ul id="treeViewSelector">
            <% if (FolderIDCurrentRoot == null)
               { %>
            <% if (!IsVisitor)
               { %>
            <li data-id="<%= Global.FolderMy %>" class="stree-node jstree-closed">
                <span class="jstree-icon jstree-expander"></span>
                <a data-id="<%= Global.FolderMy %>" title="<%= FilesUCResource.MyFiles %>" href="#<%= Global.FolderMy %>">
                    <span class="jstree-icon myFiles"></span><%= FilesUCResource.MyFiles %>
                </a>
            </li>
            <% } %>
            <% if(!CoreContext.Configuration.YourDocs)
               { %>
            <li data-id="<%= Global.FolderShare %>" class="stree-node jstree-closed access-read">
                <span class="jstree-icon jstree-expander"></span>
                <a data-id="<%= Global.FolderShare %>" title="<%= FilesUCResource.SharedForMe %>" href="#<%= Global.FolderShare %>">
                    <span class="jstree-icon shareformeFiles"></span><%= FilesUCResource.SharedForMe %>
                </a>
            </li>
            <li data-id="<%= Global.FolderCommon %>" class="stree-node jstree-closed <%= Global.IsAdministrator ? string.Empty : "access-read" %>">
                <span class="jstree-icon jstree-expander"></span>
                <a data-id="<%= Global.FolderCommon %>" title="<%= FilesUCResource.CorporateFiles %>" href="#<%= Global.FolderCommon %>">
                    <span class="jstree-icon corporateFiles"></span><%= FilesUCResource.CorporateFiles %>
                </a>
            </li>
            <% if (Global.FolderProjects != null)
               { %>
            <li data-id="<%= Global.FolderProjects %>" class="stree-node jstree-closed access-read">
                <span class="jstree-icon jstree-expander"></span>
                <a data-id="<%= Global.FolderProjects %>" title="<%= FilesUCResource.ProjectFiles %>" href="#<%= Global.FolderProjects %>">
                    <span class="jstree-icon projectFiles"></span><%= FilesUCResource.ProjectFiles %>
                </a>
            </li>
            <% } %>
            <% } %>
            <% }
               else
               { %>
            <li data-id="<%= FolderIDCurrentRoot %>" class="stree-node jstree-closed">
                <span class="jstree-icon jstree-expander"></span>
                <a data-id="<%= FolderIDCurrentRoot %>" title="<%= FilesUCResource.ProjectFiles %>" href="#<%= FolderIDCurrentRoot %>">
                    <span class="jstree-icon projectFiles"></span><%= FilesUCResource.ProjectFiles %>
                </a>
            </li>
            <% } %>
        </ul>
    </div>
</div>
<% } %>