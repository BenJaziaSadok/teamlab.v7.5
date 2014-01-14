<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="crmExportPopupTmpl" type="text/x-jquery-tmpl">
    <div id="crm_export_popup_container" class="popup popupMailBox crm_popup">
        <div>
            <p class="body"><%= MailResource.ExportMessagePopupInfoMessage %></p>
            <p class="body"><%= MailResource.ExportMessagePopupChooseMessage %></p>
            <div class="label"><%= MailResource.ExportMessagePopupExportLabel %></div>
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
        <div class="buttons">
            <button class="button blue export_btn" id="export" type="button"><%= MailResource.ExportMessagePopupExportBtnLbl %></button>
            <button class="button gray cancel" type="button"><%= MailScriptResource.CancelBtnLabel %></button>
        </div>
    </div>
</script>