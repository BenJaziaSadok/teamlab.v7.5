<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SharingSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.SharingSettings" %>

<%@ Import Namespace="ASC.Core" %>

<%@ Register TagPrefix="sa" Namespace="ASC.Web.Studio.Controls.Users" Assembly="ASC.Web.Studio" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="studio_sharingSettingsDialog" class="display-none">
    <sc:Container ID="_sharingDialogContainer" runat="server">
        <Header>
            <span class="share-container-head"><%= Resources.UserControlsCommonResource.SharingSettingsTitle %></span>
        </Header>
        <Body>
            <div id="sharingSettingsDialogBody">
            <% if (!CoreContext.Configuration.YourDocs) { %>
                <div class="header-base">
                    <%= Resources.UserControlsCommonResource.SharingSettingsItemsTitle %>
                </div>

                <div class="add-to-sharing-links borderBase clearFix">                
                    <sa:AdvancedUserSelector runat="server" id="shareUserSelector"></sa:AdvancedUserSelector>
                </div>

                <div id="sharingSettingsItems"></div>
                
                <% if(EnableShareMessage) { %>
                <div id="shareMessagePanel">
                    <label>
                        <input type="checkbox" id="shareMessageSend" checked="checked" />
                        <%= Resources.UserControlsCommonResource.SendShareNotify %>
                    </label>
                    <a id="shareAddMessage" class="baseLinkAction linkMedium">
                        <%= Resources.UserControlsCommonResource.AddShareMessage %></a> <a id="shareRemoveMessage" class="baseLinkAction linkMedium">
                            <%= Resources.UserControlsCommonResource.RemoveShareMessage %></a>
                    <textarea id="shareMessage"></textarea>
                </div>
                   
                <% } %>
                <% } %>
                <div class="middle-button-container clearFix">
                    <a id="sharingSettingsSaveButton" class="button blue middle"><%= Resources.Resource.SaveButton %></a>
                    <span class="splitter-buttons"></span>
                    <a class="sharing-cancel-button button middle gray"><%= Resources.Resource.CancelButton %></a>
                </div>
            </div>
        </Body>
    </sc:Container>
</div>