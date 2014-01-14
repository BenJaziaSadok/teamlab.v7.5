<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentsPopup.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.DocumentsPopup.DocumentsPopup" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="Resources" %>
<%@ Assembly Name="ASC.Web.Files" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<script id="getfolderstree" type="text/x-jquery-tmpl">
    <ul>
        {{each folders}}
            <li class="tree-node jstree-closed{{if access == 0}} access-read{{/if}}{{if foldersCount == 0}} jstree-empty{{/if}}" data-id="${id}">
                    <span class="jstree-icon jstree-expander" > </span>
                    <a href="javascript:;" title="${title}" data-id="${id}" onclick="javascript:DocumentsPopup.openFolder('${id}');return false;">
                        <span class="jstree-icon"></span>
                        ${title}
                    </a>
            </li>
        {{/each}}
    </ul>
</script>

<script>
    window.DP_Files_Constants = {
        FOLDER_ID_MY_FILES: "<%= Global.FolderMy %>",
        FOLDER_ID_COMMON_FILES: "<%= Global.FolderCommon %>",
        FOLDER_ID_SHARE: "<%= Global.FolderShare %>",
        FOLDER_ID_PROJECT: "<%= Global.FolderProjects %>",
        FOLDER_ID_TRASH: "<%= Global.FolderTrash %>"
    };
</script>

<sc:Container id="_documentUploader" runat="server">
        <header>
        <%= PopupName %>
        </header>
        <body>
            <!--<div class="popupContainerBreadCrumbs">
                
            </div>-->
            <table>
                <tbody>
                    <tr>
                        <td style="vertical-align: top; max-width: 220px;">
                            <div id="treeViewContainer" class="jstree">
                                <ul>
                                    <li data-id="<%= Global.FolderMy %>" class="tree-node jstree-closed">
                                        <span class="jstree-icon jstree-expander"></span>
                                        <a data-id="<%= Global.FolderMy %>" title="<%= FilesUCResource.MyFiles %>" href="javascript:;" onclick="javascript:DocumentsPopup.openFolder(<%= Global.FolderMy %>);return false;">
                                            <span class="jstree-icon myFiles"></span>
                                            <%= FilesUCResource.MyFiles %>
                                        </a>
                                    </li>
                                    <li data-id="<%= Global.FolderShare %>" class="tree-node jstree-closed access-read">
                                        <span class="jstree-icon jstree-expander"></span>
                                        <a data-id="<%= Global.FolderShare %>" title="<%= FilesUCResource.SharedForMe %>" href="javascript:;" onclick="javascript:DocumentsPopup.openFolder(<%= Global.FolderShare %>);return false;">
                                            <span class="jstree-icon shareformeFiles"></span>
                                            <%= FilesUCResource.SharedForMe %>
                                        </a>
                                    </li>
                                    <li data-id="<%= Global.FolderCommon %>" class="tree-node jstree-closed <%= Global.IsAdministrator ? string.Empty : "access-read" %>">
                                        <span class="jstree-icon jstree-expander"></span>
                                        <a data-id="<%= Global.FolderCommon %>" title="<%= FilesUCResource.CorporateFiles %>" href="javascript:;" onclick="javascript:DocumentsPopup.openFolder(<%= Global.FolderCommon %>);return false;">
                                            <span class="jstree-icon corporateFiles"></span>
                                            <%= FilesUCResource.CorporateFiles %>
                                        </a>
                                    </li>
                                    <li data-id="<%= Global.FolderProjects %>" class="tree-node jstree-closed access-read">
                                        <span class="jstree-icon jstree-expander"></span>
                                        <a data-id="<%= Global.FolderProjects %>" title="<%= FilesUCResource.ProjectFiles %>" href="javascript:;" onclick="javascript:DocumentsPopup.openFolder(<%= Global.FolderProjects %>);return false;">
                                            <span class="jstree-icon projectFiles"></span>
                                            <%= FilesUCResource.ProjectFiles %>
                                        </a>
                                    </li>
                                </ul>
                            </div>
                        </td>
                        <td style="width:10px; border-left: solid 1px #D1D1D1;" />
                        <td style="max-width: 380px; width: 416px;">
                            <!--<p class="containerCheckAll display-none">
                                <input type="checkbox" onchange="DocumentsPopup.selectAll();" title="Select all" id="checkAll"/>
                                <label for="checkAll"><%=UserControlsCommonResource.CheckAll%></label>
                            </p>-->
                            <div class="fileContainer">
                                <img class="loader" src="<%= WebImageSupplier.GetAbsoluteWebPath("loader.gif")%>"/>
                                <div id="emptyFileList" class="display-none">
                                    <asp:PlaceHolder runat="server" ID="_phEmptyDocView"></asp:PlaceHolder>
                                </div>
                                <div id="filesViewContainer">
                                    <ul class='fileList'>
                                    </ul>
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="buttonContainer" style="padding-top: 20px;">
                <button id="attach_btn" class="button blue disable" type="button"><%=UserControlsCommonResource.AttachFiles %></button>
                <span class="splitter-buttons"></span>
                <button id="cancel_btn" class="button gray" type="button"><%=UserControlsCommonResource.CancelButton %></button>
            </div>
        </body>
</sc:Container>