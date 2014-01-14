<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ChunkUploadDialog.ascx.cs" Inherits="ASC.Web.Files.Controls.ChunkUploadDialog" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Utils" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>

<%@ Register TagPrefix="su" Namespace="ASC.Web.Studio.Controls.FileUploader" Assembly="ASC.Web.Studio" %>

<su:FileUploaderRegistrator runat="server" />

<div id="chunkUploadDialog" class="progress-dialog display-none">

    <div class="progress-dialog-header menu-upload-icon">
        <a class="actions-container close"></a>
        <a class="actions-container minimize"></a>
        <a class="actions-container maximize"></a>
        <span id="chunkUploadDialogHeader"></span>
    </div>

    <div class="progress-dialog-body">
        <div class="settings-container clearFix">
            <span id="uploadSettingsSwitcher">
                <a class="baseLinkAction gray-text"><%=FilesUCResource.SideCaptionSettings%></a>
                <span class="sort-down-gray"></span>
            </span>
            <a id="abortUploadigBtn" class="linkMedium gray-text">
                <%= FilesUCResource.ButtonCancelAll %>
            </a>
        </div>
        <div class="files-container">
            <table id="uploadFilesTable" class="tableBase" cellspacing="0" cellpadding="5">
                <colgroup>
                    <col style="width: 30px;"/>
                    <col/>
                    <col style="width: 110px;"/>
                    <col style="width: 90px;"/>
                </colgroup>
                <tbody></tbody>
            </table>
        </div>
        <% if (FileConverter.EnableAsUploaded) %>
        <% { %>
        <label class="gray-text">
            <input type="checkbox" class="store-original" <%= FilesSettings.StoreOriginalFiles ? "checked=\"checked\"" : string.Empty %> />
            <%= FilesUCResource.ConfirmStoreOriginalUploadCbxLabelText %>
        </label>
        <% } %>
        <div class="info-container">
            <% if (FileConverter.EnableAsUploaded) %>
            <% { %>
            <div class="info-content">
                <%= FilesUCResource.ConfirmStoreOriginalUploadTitle %>
            </div>
            <% } %>
            <span class="free-space gray-text"></span>
            <% if (!CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor())
               { %>
            <span class="splitter"></span>
            <a class="link underline gray" target="_blank" href="<%= TenantExtra.GetTariffPageLink() %>">
                <%= FilesUCResource.UpgradeYourPlan %>
            </a>
            <% } %>
        </div>
        <div id="uploadSettingsPanel" class="studio-action-panel files-popup-win">
            <div class="corner-top left"></div>
            <ul class="dropdown-content">
                <li>
                    <label class="gray-text">
                        <input class="update-if-exist" type="checkbox" <%= FilesSettings.UpdateIfExist ? "checked=\"checked\"" : string.Empty %>>
                        <%= FilesUCResource.UpdateIfExist%>
                    </label>
                </li>
                <li>
                    <label class="gray-text">
                        <input id="uploadCompactViewCbx" type="checkbox">
                        <%= FilesUCResource.ShowThisWindowMinimized %>
                    </label>
                </li>
            </ul>
        </div>
        <input id="flashSwfUrl" type="hidden" value="<%= FileUploaderRegistrator.GetFlashUrl() %>"/>
    </div>

</div>

<script id="fileUploaderRowTmpl" type="text/x-jquery-tmpl">
    <tr id="${id}" class="fu-row">
        <td class="borderBase">
            <div class="${fileTypeCssClass}"></div>
        </td>
        <td class="borderBase">
            <div class="fu-title-cell" title="${name}">${name}</div>
        </td>
        <td class="borderBase">
            <div class="fu-progress-cell">
                <div class="upload-progress">
                    {{if showAnim == true}}
                        <span class="progress-slider progress-color-anim">&nbsp;</span>
                    {{/if}}
                    {{if showAnim == false}}
                        <span class="progress-slider progress-color">&nbsp;</span>
                    {{/if}}
                    <span class="progress-label">${actionText}</span>
                </div>
                <span class="upload-complete gray-text">${completeActionText}</span>
                <span class="upload-canceled gray-text"><%= FilesUCResource.Canceled %></span>
                <span class="upload-error red-text expl"><%= FilesUCResource.Error %></span>
                <div class="popup_helper files-popup-win"></div>
            </div>
        </td>
        <td class="borderBase">
            <div class="fu-action-cell">
                <a class="linkMedium gray-text abort-file-uploadig"><%= FilesUCResource.ButtonCancel %></a>
                {{if canShare === true}}
                    <a class="linkMedium gray-text share"><%= FilesUCResource.Share %></a>
                {{/if}}
            </div>
        </td>
    </tr>
</script>

