<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TariffLicenseOver.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.TariffLicenseOver" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>
<%@ Import Namespace="Resources" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="tariffLimitExceedLicense" style="display: none">
    <sc:Container runat="server" ID="tariffLimitExceedLicense">
        <Header><%= UserControlsCommonResource.TariffLicenseOverHeader %></Header>
        <Body>
            <div class="tariff-limit-license">
                <span class="header-base-medium"><%= UserControlsCommonResource.TariffLicenseOver %></span>
                <br />
                <br />
                <%= UserControlsCommonResource.TariffLicenseOverReason %>
            </div>

            <div class="middle-button-container">
                <a class="blue button medium" href="<%= TenantExtra.GetTariffPageLink() %>">
                    <%= UserControlsCommonResource.TariffLimitOkButtonLicense %></a>
                <span class="splitter-buttons"></span>
                <a class="gray button medium" onclick="PopupKeyUpActionProvider.CloseDialog(); return false;">
                    <%= Resource.CancelButton %></a>
            </div>
        </Body>
    </sc:Container>
</div>