<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileViewer.ascx.cs" Inherits="ASC.Web.Files.Controls.FileViewer" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<div id="fileViewerDialog">
    <div id="viewerOtherActions" class="display-none">
        <ul>
            <% if (!ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
               { %>
            <li>
                <a id="imageDownload" class="action-download" title="<%= FilesUCResource.ButtonDownload %>">
                    <%= FilesUCResource.ButtonDownload %>
                </a>
            </li>
            <% } %>
            <li>
                <a id="imageDelete" class="action-delete" title="<%= FilesUCResource.ButtonDelete %>">
                    <%= FilesUCResource.ButtonDelete %>
                </a>
            </li>
        </ul>
    </div>
    <img id="imageViewerContainer" class="display-none" src="" alt="" height="10px" width="10px" />
    <img id="imageViewerPreload" class="display-none" alt="" />
    <div id="imageViewerToolbox" class="display-none">
        <div class="image-info describe-text">
            &nbsp;
        </div>
        <div class="toolbox-wrapper">
            <ul>
                <li>
                    <a id="imageZoomIn" title="<%= FilesUCResource.ButtonZoomIn %>"></a>
                </li>
                <li>
                    <a id="imageZoomOut" title="<%= FilesUCResource.ButtonZoomOut %>"></a>
                </li>
                <li>
                    <a id="imageFullScale" title="<%= FilesUCResource.ButtonFullScale %>"></a>
                </li>
                <li class="action-block-wrapper"></li>
                <li>
                    <a id="imagePrev" title="<%= FilesUCResource.ButtonPrevImg %>"></a>
                </li>
                <li>
                    <a id="imageNext" title="<%= FilesUCResource.ButtonNextImg %>"></a>
                </li>
                <li class="action-block-wrapper"></li>
                <li>
                    <a id="imageRotateLeft" title="<%= FilesUCResource.ButtonRotateLeft %>"></a>
                </li>
                <li>
                    <a id="imageRotateRight" title="<%= FilesUCResource.ButtonRotateRight %>"></a>
                </li>
                <li>
                    <a id="viewerOtherActionsSwitch" title="<%= FilesUCResource.ButtonOtherAction %>"></a>
                </li>
            </ul>
        </div>
    </div>

    <div id="imageViewerInfo" class="display-none">
        <span>100%</span>
    </div>

    <div id="imageViewerClose" class="display-none" title="<%= FilesUCResource.ButtonClose %>">
    </div>

    <div id="imageBatchLoader" class="display-none">
    </div>
</div>