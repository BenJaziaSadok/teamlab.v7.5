<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactsSearchView.ascx.cs" Inherits="ASC.Web.CRM.Controls.SocialMedia.ContactsSearchView" EnableViewState="false" %>

<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="divSMContactsSearchContainer" style="display: none;">
    <sc:Container ID="_ctrlContactsSearchContainer" runat="server">
        <Header>
            <%= CRMSocialMediaResource.ProfilesInSocialMedia %>
        </Header>
        <Body>
            <div id="divModalContent" style="max-height: 500px; overflow: auto;">
                <div id="divContactDescription">
                </div>

                <div style="display: none;" id="divCrbsContactConfirm">
                    <div class="h_line" style="margin-top: 15px;">&nbsp;</div>

                    <a href="javascript:void(0);" class="button blue"
                        onclick="ASC.CRM.SocialMedia.ConfirmCrunchbaseContact(); return false;">
                            <%= CRMSocialMediaResource.Confirm %></a>
                    <span class="splitter">&nbsp;</span>
                    <a href="" class="button gray" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;">
                            <%= CRMCommonResource.Cancel %></a>

                    <div style="float:right;">
                        <span class="text-medium-describe"><%=CRMSocialMediaResource.InformationProvidedBy %></span>
                        <a target="_blank" href="http://crunchbase.com/" class="link blue bold">CrunchBase</a>
                    </div>
                </div>
            </div>
            <div class="divWaitForSearching">
                <span class="text-medium-describe"><%= CRMSocialMediaResource.PleaseWaitForSearching %></span>
                <br />
                <img alt="<%= CRMSocialMediaResource.PleaseWait %>" title="<%= CRMSocialMediaResource.PleaseWaitForSearching %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>

            <div class="divWaitForAdding">
                <span class="text-medium-describe">
                    <%= CRMSocialMediaResource.PleaseWait %></span>
                <br />
                <img alt="<%= CRMSocialMediaResource.PleaseWait %>" title="<%= CRMSocialMediaResource.PleaseWait %>"
                    src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>

            <div class="divNoProfiles"><%= CRMSocialMediaResource.NoAccountsHasBeenFound %></div>
        </Body>
    </sc:Container>
</div>