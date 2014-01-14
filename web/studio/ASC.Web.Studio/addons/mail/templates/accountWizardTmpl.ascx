<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>


<script id="accountWizardTmpl" type="text/x-jquery-tmpl">
    <table>
            <tbody>
                <tr>
                    <td style="vertical-align: top; max-width: 308px; padding-right: 24px">
                        <div id="account_simple_container" class="popup popupMailBox">
                            <div id="mail_EMailContainer" class="requiredField">
                                <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
                                <div class="headerPanelSmall" style="font-weight: bold;">
                                    <%: MailResource.EMailLabel %>
                                </div>
                                <div>
                                    <input type="email" id="email" value="" class="textEdit" style="width:277px" />
                                </div>
                                <div style="clear: both;" />
                            </div>
                            <div id="mail_PasswordContainer" class="requiredField">
                                <span class="requiredErrorText"><%: MailScriptResource.ErrorEmptyField %></span>
                                <div class="headerPanelSmall" style="font-weight: bold;">
                                    <%: MailResource.Password %>
                                </div>
                                <div>
                                    <input type="password" id="password" value="" class="textEdit" style="width:277px" />
                                </div>
                                <div class="progressContainer">
                                    <div class="loader" style="display: none"></div>
                                </div>
                                <div style="clear: both;" />
                            </div>
                            <div class="buttons new-account">
                                <button class="button blue" id="save" type="button"><%: MailScriptResource.AddAccountBtnLabel %></button>
                                <button class="button gray cancel" id="cancel" type="button"><%: MailScriptResource.CancelBtnLabel %></button>
                                <a class="anchorLinkButton" id="advancedLinkButton"><%: MailResource.AdvancedLinkLabel %></a>
                            </div>
                        </div>
                    </td>
                    <% if (MailPage.IsTurnOnOAuth()) 
                       { %>
                        <td style="width:1px; border-left: solid 1px #D1D1D1;" />
                        <td style="vertical-align: top; max-width: 92px; padding-left: 24px">
                            <div class="popup popupMailBox">
                                <div class="headerPanelSmall" style="font-weight: bold;">
                                    <%: MailResource.OAuthLabel %>
                                </div>
                                <div id="oauth_frame_blocker" style="position: absolute;display: none;width: 90px;height: 150px;opacity: 0.1; background-color: aliceblue;"></div>
                                <iframe src="<%: MailPage.GetImportOAuthAccessUrl() %>" style="border: none; width: 100%; height: 100%; overflow: hidden; filter: alpha(opacity=100);" id="ifr"></iframe>
                            </div>
                        </td>
                    <% } %>
                </tr>
            </tbody>
        </table>
</script>