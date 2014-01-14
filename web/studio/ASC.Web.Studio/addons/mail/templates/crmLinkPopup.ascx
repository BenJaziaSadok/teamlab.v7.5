<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="crmLinkPopupTmpl" type="text/x-jquery-tmpl">
    <div id="crm_link_popup_container" class="popup popupMailBox crm_popup">
        <div>
            <p class="body"><%= MailResource.LinkConversationPopupInfoMessage %></p>
            <p class="body"><%= MailResource.LinkConversationPopupChooseMessage %></p>
            <div class="label"><%= MailResource.LinkConversationPopupLinkLabel %></div>
            <div>
                 <select id="entity-type" class="entity-type">
                     <option value="1" selected="selected"><%= MailResource.LinkConversationOptionContactsLbl %></option>
                     <option value="2"><%= MailResource.LinkConversationOptionCasesLbl %></option>
                     <option value="3"><%= MailResource.LinkConversationOptionOpportunitiesLbl %></option>
                 </select>
            </div>
            <div class="label"><%= MailResource.LinkConversationChooseContactsLabel %></div>
            <div>
                <table cellspacing="0" cellpadding="1" class="search_panel">
                    <tbody>
                        <tr>
                            <td width="16px" style="border-right: 0 none;">
                                <img align="absmiddle" id="searchImg_companySelector_0" src="/skins/default/ImagesCSS/search_16.png" class="search_icon"/>
                                <img align="absmiddle" id="searchImg_companySelector_0" src="/skins/default/Images/mini_loader.gif" class="loading_icon" style="display: none"/>
                            </td>
                            <td style="border-left: 0 none">
                                <div>
                                    <input id="link_search_panel" class="search_input" placeholder="<%= MailResource.LinkConversationSearchPlaceHolderLabel %>"/>
                                </div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
        <div class="linked_table_parent">
                <table class="linked_contacts_table" cellspacing="0" cellpadding="0">
                </table>
        </div>
        <div class="loader">
            <img src="/skins/default/images/loader.gif"/>
            <div><%= MailResource.LoadingLabel %></div>
        </div>
        <div class="buttons">
            <button class="button blue link_btn" id="tryagain" type="button"><%= MailResource.LinkConversationLinkButtonLabel %></button>
            <button class="button gray unlink_all" type="button"><%= MailResource.LinkConversationUnlinkAllBtnLabel %></button>
            <button class="button gray cancel" type="button"><%= MailScriptResource.CancelBtnLabel %></button>
        </div>
    </div>
</script>

<script id="crmUnlinkAllPopupTmpl" type="text/x-jquery-tmpl">
    <div id="crm_unlink_all_popup_message">
        <p>
            <%= MailResource.UnlinkAllInformationMessage %>
        </p>
        <p>
            <%= MailResource.UnlinkAllConfirmationMessage %>
        </p>
        <div class="buttons">
            <button class="button blue unlink" type="button"><%= MailResource.UnlinkAllPopupUnlinkBtnLabel %></button>
            <button class="button gray cancel" type="button"><%= MailScriptResource.CancelBtnLabel %></button>
        </div>
    </div>
</script>