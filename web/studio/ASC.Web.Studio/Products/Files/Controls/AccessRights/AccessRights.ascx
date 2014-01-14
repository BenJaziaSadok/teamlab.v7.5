<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AccessRights.ascx.cs" Inherits="ASC.Web.Files.Controls.AccessRights" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:PlaceHolder ID="_sharingContainer" runat="server"></asp:PlaceHolder>

<div class="display-none">
    <div id="shareSelectorBody" class="page-menu <%= CoreContext.Configuration.YourDocs ? "only-outside" : string.Empty %>">
        <ul id="shareSidePanel" class="menu-list">
            <li id="shareSidePortal" class="menu-item none-sub-list">
                <a class="menu-item-label outer-text text-overflow">
                    <span class="menu-item-label inner-text"><%= FilesUCResource.SharingListCaption %></span>
                </a>
            </li>
            <li id="shareSideOutside" class="menu-item none-sub-list">
                <a class="menu-item-label outer-text text-overflow">
                    <span class="menu-item-label inner-text"><%= FilesUCResource.SharingListCaptionOutside %></span>
                </a>
            </li>
            <% if (Global.EnableEmbedded)
               { %>
            <li id="shareSideEmbedded" class="menu-item none-sub-list">
                <a class="menu-item-label outer-text text-overflow">
                    <span class="menu-item-label inner-text"><%= FilesUCResource.SharingListCaptionEmbedded %></span>
                </a>
            </li>
            <% } %>
        </ul>
        <div id="shareLinkBody">
            <span class="header-base">
                <%= FilesUCResource.SharingLinkCaption %>
            </span>
            <div id="sharingLinkItem">
            </div>
            <div id="sharingLinkDeny" class="describe-text"><%= FilesUCResource.ShareLinkDeny %></div>
            <div id="shareLinkPanel">
                <span class="header-base-small"><%= FilesUCResource.Link %>:</span>
                <% if (!string.IsNullOrEmpty(Global.BitlyUrl))
                   { %>
                <span id="getShortenLink" class="baseLinkAction text-medium-describe"><%= FilesUCResource.GetShortenLink %></span>
                <% } %>
                <span id="shareLinkCopy" class="baseLinkAction text-medium-describe"><span><%= FilesUCResource.CopyToClipboard %></span></span>
                <textarea id="shareLink" class="textEdit" cols="10" rows="2" readonly="readonly"></textarea>

                <span class="header-base-small"><%= FilesUCResource.ShareLink %>:</span>
                <span id="shareViaMail" class="baseLinkAction"><%= FilesUCResource.LinkViaMail %></span>
                <div id="shareViaSocPanel">
                    <span><%= FilesUCResource.LinkViaSocial %>:</span>
                    <ul>
                        <li><a class="google" target="_blank" title="<%= FilesUCResource.ButtonViaGoogle %>"></a></li>
                        <li><a class="facebook" target="_blank" title="<%= FilesUCResource.ButtonViaFacebook %>"></a></li>
                        <li><a class="twitter" target="_blank" title="<%= FilesUCResource.ButtonViaTwitter %>"></a></li>
                    </ul>
                </div>
                <div id="shareMailPanel">
                    <div class="recipientMail clearFix">
                        <input type="email" class="textEdit" placeholder="<%= FilesUCResource.ShareLinkMail %>"/>
                        <span class="baseLinkAction"><%= FilesUCResource.ButtonDelete %></span>
                    </div>
                    <span id="shareLinkMailAdd" class="baseLinkAction"><%= FilesUCResource.LinkViaMailAdd %></span>
                    <textarea id="shareMailText" class="textEdit" cols="10" rows="2" placeholder="<%= FilesUCResource.ShareLinkMailMessage %>"></textarea>
                    <a id="shareSendLinkToEmail" class="button middle blue"><%= FilesUCResource.LinkViaMailSend %></a>
                    <span class="splitter-buttons"></span>
                    <a class="sharing-cancel-button button middle gray"><%= FilesUCResource.ButtonClose %></a>
                </div>
            </div>
            <% if (Global.EnableEmbedded)
               { %>
            <div id="shareEmbeddedPanel">
                <span class="header-base-small"><%= FilesUCResource.EmbedCode %>:</span>
                <span id="embeddedCopy" class="baseLinkAction text-medium-describe"><span><%= FilesUCResource.CopyToClipboard %></span></span>
                <textarea id="shareEmbedded" class="textEdit" cols="10" rows="3" readonly="readonly"></textarea>

                <span class="header-base-small"><%= FilesUCResource.EmbedSize %>:</span>
                <ul id="embeddedSizeTemplate" class="clearFix">
                    <li class="embedded-size-item">
                        <span class="text-medium-describe"><%= FilesUCResource.EmbedSizeAuto %></span>
                        <div class="embedded-size-template"></div>
                    </li>
                    <li class="embedded-size-item embedded-size-8x6">
                        <span class="text-medium-describe">800x600</span>
                        <div class="embedded-size-template"></div>
                    </li>
                    <li class="embedded-size-item embedded-size-6x4">
                        <span class="text-medium-describe">600x400</span>
                        <div class="embedded-size-template"></div>
                    </li>
                    <li class="embedded-size-custom">
                        <span class="text-medium-describe"><%= FilesUCResource.EmbedSizeCustom %></span>
                        <div class="embedded-size-descr">
                            <span><%= FilesUCResource.EmbedSizeHeight %>:</span>
                            <input type="text" class="textEdit" name="height" />
                        </div>
                        <div class="embedded-size-descr">
                            <span><%= FilesUCResource.EmbedSizeWidth %>:</span>
                            <input type="text" class="textEdit" name="width" />
                        </div>
                    </li>
                </ul>
            </div>
            <% } %>
            <a class="share-link-close sharing-cancel-button button middle gray"><%= FilesUCResource.ButtonClose %></a>
        </div>
    </div>
</div>

<% if (!CoreContext.Configuration.YourDocs)
   { %>
<div id="filesConfirmUnsubscribe" class="popup-modal display-none">
    <sc:Container id="confirmUnsubscribeDialog" runat="server">
        <header><%=FilesUCResource.ConfirmRemove%></header>
        <body>
            <div id="confirmUnsubscribeText">
                <%=FilesUCResource.ConfirmUnsubscribe%>
            </div>
            <div id="confirmUnsubscribeList" class="files-remove-list webkit-scrollbar">
                <dl>
                    <dt class="confirm-remove-folders">
                        <%=FilesUCResource.Folders%>:</dt>
                    <dd class="confirm-remove-folders">
                    </dd>
                    <dt class="confirm-remove-files">
                        <%=FilesUCResource.Documents%>:</dt>
                    <dd class="confirm-remove-files">
                    </dd>
                </dl>
            </div>
            <div class="middle-button-container">
                <a id="unsubscribeConfirmBtn" class="button blue middle">
                    <%=FilesUCResource.ButtonOk%>
                </a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </body>
    </sc:Container>
</div>
<% } %>