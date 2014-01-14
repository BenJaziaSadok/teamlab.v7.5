<%@ Assembly Name="ASC.Web.Files" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ThirdParty.ascx.cs" Inherits="ASC.Web.Files.Controls.ThirdParty" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.Files.Classes" %>
<%@ Import Namespace="ASC.Web.Files.Import" %>
<%@ Import Namespace="ASC.Web.Files.Resources" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="thirdPartyAccountContainer">

    <% if (ImportConfiguration.SupportInclusion)%>
    <% { %>
    <div class="clearFix">
        <div class="third-party-connect-account-container">
            <a id="thirdPartyConnectAccount" class="button middle blue">
                <%= FilesUCResource.ThirdPartyConnectAccount %>
            </a>
        </div>
    </div>
    <% } %>

    <div id="thirdPartyAccountList"></div>

</div>

<asp:PlaceHolder runat="server" ID="EmptyScreenThirdParty" />


<div id="thirdPartyEditor" class="popup-modal display-none">
    <sc:Container id="ThirdPartyEditorTemp" runat="server">
        <header>
            <span id="thirdPartyDialogCaption"></span>
        </header>
        <body>
            <div id="thirdPartyPanel">
                <div id="thirdPartyNamePass">
                    <div><%=FilesUCResource.Login%></div>
                    <input type="text" id="thirdPartyName" maxlength="100" class="textEdit" />
                    <div id="thirdPartyPassText"><%=FilesUCResource.Password%></div>
                    <input type="password" id="thirdPartyPass" maxlength="100" class="textEdit" />
                </div>
                <div id="thirdPartyGetToken">
                    <a id="thirdPartyGetTokenButton" class="button gray middle"></a>
                    <span><%=FilesUCResource.TakeToken%></span>
                    <input type="hidden" />
                </div>

                <div><%=FilesUCResource.ThirdPartyFolderTitle%></div>
                <input type="text" id="thirdPartyTitle" maxlength="<%=Global.MaxTitle%>" class="textEdit" />

                <%if (Global.IsAdministrator && !CoreContext.Configuration.YourDocs) %>
                <% { %>
                <label id="thirdPartyLabelCorporate" >
                    <input type="checkbox" id="thirdPartyCorporate" /><%= FilesUCResource.ThirdPartySetCorporate %></label>
                <% } %>
            </div>
            <div class="middle-button-container">
                <a id="submitThirdParty" class="button blue middle">
                    <%=FilesUCResource.ButtonOk%>
                </a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.CloseDialog();return false;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
            <div class="ajax-info-block display-none">
                <span class="text-medium-describe">
                    <%=FilesUCResource.ProcessAuthentificate%>
                </span>
                <br />
                <div class="ajax-progress-loader"></div>
            </div>
        </body>
    </sc:Container>
</div>

<div id="thirdPartyDelete" class="popup-modal display-none">
    <sc:Container runat="server" id="ThirdPartyDeleteTmp">
        <Header>
            <%=FilesUCResource.ThirdPartyDeleteCaption%>
        </Header>
        <Body>
            <div id="thirdPartyDeleteDescr"></div>
            <div class="middle-button-container">
                <a id="deleteThirdParty" class="button blue middle">
                    <%=FilesUCResource.ButtonOk%>
                </a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="PopupKeyUpActionProvider.CloseDialog();return false;">
                    <%=FilesUCResource.ButtonCancel%>
                </a>
            </div>
        </Body>
    </sc:Container>
</div>

<div id="thirdPartyNewAccount" class="popup-modal display-none">
    <sc:Container runat="server" id="ThirdPartyNewAccountTmp">
        <Header>
            <%=FilesUCResource.ThirdPartyConnectingAccount%>
        </Header>
        <Body>
            <div>
                <%=FilesUCResource.ThirdPartyConnectAccountsDescription%>
            </div>
            <div>
                <table cellpadding="0" cellspacing="0" width="100%">
                    <tbody>
                        <tr>
                            <% if (ImportConfiguration.SupportDropboxInclusion) %>
                            <% { %>
                            <td align="center">
                                <div class="account-popup-icon DropBoxBig"></div>
                                <a id="thirdpartyDropBox" class="button middle blue">
                                    <%= FilesUCResource.ThirdPartyDropBox %>
                                </a>
                            </td>
                            <% } %>
                            <% if (ImportConfiguration.SupportGoogleInclusion) %>
                            <% { %>
                            <td align="center">
                                <div class="account-popup-icon GoogleBig"></div>
                                <a id="thirdpartyGoogle" class="button middle blue">
                                    <%= FilesUCResource.ThirdPartyGoogleDrive %>
                                </a>
                            </td>
                            <% } %>
                            <% if (ImportConfiguration.SupportBoxNetInclusion) %>
                            <% { %>
                            <td align="center">
                                <div class="account-popup-icon BoxNetBig"></div>
                                <a id="thirdpartyBoxNet" class="button middle blue">
                                    <%= FilesUCResource.ThirdPartyBoxNet %>
                                </a>
                            </td>
                            <% } %>
                            <% if (ImportConfiguration.SupportSkyDriveInclusion) %>
                            <% { %>
                            <td align="center">
                                <div class="account-popup-icon SkyDriveBig"></div>
                                <a id="thirdpartySkyDrive" class="button middle blue">
                                    <%= FilesUCResource.ThirdPartySkyDrive %>
                                </a>
                            </td>
                            <% } %>
                        </tr>
                    </tbody>
                </table>
            </div>
        </Body>
    </sc:Container>
</div>

<div id="thirdPartyActionPanel" class="studio-action-panel files-popup-win">
    <div class="corner-top right">
    </div>
    <ul class="dropdown-content">
        <li id="accountEditLinkContainer">
            <a class="dropdown-item">
                <%= FilesUCResource.ButtonEdit %>
            </a>
        </li>
        <li id="accountDeleteLinkContainer">
            <a class="dropdown-item">
                <%= FilesUCResource.ButtonDelete %>
            </a>
        </li>
    </ul>
</div>
