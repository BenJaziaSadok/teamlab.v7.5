<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%-- Empty screen control --%>
<script id="emptyScrTmpl" type="text/x-jquery-tmpl">
    <div id="${ID}" class="noContentBlock emptyScrCtrl{{if typeof(CssClass)!="undefined"}} ${CssClass}{{/if}}" >
        {{if typeof(ImgSrc)!="undefined" && ImgSrc != null && ImgSrc != ""}}
        <table>
            <tr>
                <td>
                    <img src="${ImgSrc}" alt="" class="emptyScrImage" />
                </td>
                <td>
                    <div class="emptyScrTd">
        {{/if}}
                    {{if typeof(Header)!="undefined" && Header != null && Header != ""}}
                        <div class="header-base-big">${Header}</div>
                    {{/if}}
                    {{if typeof(HeaderDescribe)!="undefined" && HeaderDescribe != null && HeaderDescribe != ""}}
                        <div class="emptyScrHeadDscr">${HeaderDescribe}</div>
                    {{/if}}
                    {{if typeof(Describe)!="undefined" && Describe != null && Describe != ""}}
                        <div class="emptyScrDscr">{{html Describe}}</div>
                    {{/if}}
                    {{if typeof(ButtonHTML)!="undefined" && ButtonHTML != null && ButtonHTML != ""}}
                        <div class="emptyScrBttnPnl">{{html ButtonHTML}}</div>
                    {{/if}}
        {{if typeof(ImgSrc)!="undefined" && ImgSrc != null && ImgSrc != ""}}
                    </div>
                </td>
            </tr>
        </table>
        {{/if}}
    </div>
</script>

<%-- Tag view control --%>
<script id="tagViewTmpl" type="text/x-jquery-tmpl">
    <div id="tagContainer">
        <span>
            <span id="addNewTag" class="baseLinkAction crm-withArrowDown" title="<%= CRMCommonResource.AddNewTag %>"><%= CRMCommonResource.AddNewTag %></span>

            <div id="addTagDialog" class="studio-action-panel addTagDialog">
                <div class="corner-top left"></div>
                <ul class="dropdown-content mobile-overflow">
                    {{each availableTags}}
                        {{tmpl({"tagLabel": $value}) 'tagInAllTagsTmpl'}}
                    {{/each}}
                </ul>
                <div class="h_line">&nbsp;</div>
                <div style="margin-bottom:5px;"><%= CRMCommonResource.CreateNewTag%>:</div>
                <input type="text" class="textEdit" maxlength="50"/>
                <a id="addThisTag" class="button blue" title="<%= CRMCommonResource.OK %>"><%= CRMCommonResource.OK %></a>
            </div>
        </span>
        <div style="display:inline;">
            {{each tags}}
                {{tmpl({"tagLabel": $value}) 'taqTmpl'}}
            {{/each}}
        </div>

        <img class="adding_tag_loading" alt="<%= CRMSettingResource.AddTagInProgressing %>"
                title="<%= CRMSettingResource.AddTagInProgressing %>"
                src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>" />
    </div>
</script>

<script id="taqTmpl" type="text/x-jquery-tmpl">
    <span class="tag_item">
        <span class="tag_title" data-value="${jQuery.base64.encode(tagLabel)}">${ASC.CRM.Common.convertText(tagLabel, true)}</span>
        <a class="delete_tag" alt="<%= CRMCommonResource.DeleteTag %>" title="<%= CRMCommonResource.DeleteTag %>"
            onclick="ASC.CRM.TagView.deleteTag(jq(this).parent())"></a>
     </span>
</script>

<script id="tagInAllTagsTmpl" type="text/x-jquery-tmpl">
    <li><a class="dropdown-item" onclick="ASC.CRM.TagView.addExistingTag(this);" data-value="${jQuery.base64.encode(tagLabel)}">${ASC.CRM.Common.convertText(tagLabel, true)}</a></li>
</script>

<%--HistoryView--%>

<script id="historyRowTmpl" type="text/x-jquery-tmpl">
    <tr id="event_${id}">
        <td class="borderBase entityPlace">
            <a name="${id}"></a>
            {{if typeof(category) != "undefined"}}
            <label title="${category.title}" alt="${category.title}" class="event_category ${category.cssClass}"></label>
            {{/if}}
        </td>
        <td class="borderBase title">
            {{if contact != null && contact.id != ASC.CRM.HistoryView.ContactID }}
                <a class="link underline gray" href="default.aspx?id=${contact.id}">${contact.displayName}</a>
                {{if entity != null && entity.entityId != ASC.CRM.HistoryView.EntityID}}
                    &nbsp;/&nbsp;
                {{/if}}
            {{/if}}
            {{if entity != null && entity.entityId != ASC.CRM.HistoryView.EntityID }}
                ${entityType}: <a class="link underline gray" href="${entityURL}">${entity.entityTitle}</a>
            {{/if}}


            {{if typeof message != "undefined" && message != null}}
            <div>
                <%= CRMCommonResource.TheEmail %>
                "<span class="baseLinkAction historyEventMailSubj">${content}</span>"
                {{if message.is_sended === true}}
                <%= CRMCommonResource.HistoryEventOutboxMail %>
                {{else}}
                <%= CRMCommonResource.HistoryEventInboxMail %>
                {{/if}}
            </div>
            {{else}}
            <div>
                {{html Encoder.XSSEncode(content).replace(/\n/g, "<br/>") }}
            </div>
            {{/if}}

            <span class="text-medium-describe">${createdDate} <%= CRMCommonResource.Author %>: ${createdBy.displayName}</span>
        </td>
        <td class="borderBase activityData describe-text">
            {{if files != null }}
                <label class="event_category event_category_attach_file" align="absmiddle"></label>
                <a id="eventAttachSwither_${id}" class="baseLinkAction linkMedium">
                    <%= CRMCommonResource.ViewFiles %>
                </a>
                <div id="eventAttach_${id}" class="studio-action-panel">
                    <div class="corner-top right"></div>
                    <ul class="dropdown-content">
                        {{each(i, file) files}}
                            <li id="fileContent_${file.id}">
                                <a target="_blank" href="${file.webUrl}" class="dropdown-item">
                                    ${file.title} 
                                </a>
                                {{if $data.canEdit == true }}
                                <img align="absmiddle" title="<%= CRMCommonResource.Delete %>"
                                    onclick="ASC.CRM.HistoryView.deleteFile(${file.id}, ${$data.id})"
                                    style="cursor:pointer;margin-left: 3px;"
                                    src="<%=WebImageSupplier.GetAbsoluteWebPath("trash_12.png")%>" />
                                {{/if}}
                            </li>
                        {{/each}}
                    </ul>
                </div>
            {{/if}}
        </td>
        <td class="borderBase" style="width: 20px;">
          {{if canEdit == true }}
            <label id="eventTrashImg_${id}" class="crm-deleteBtn" title="<%= CRMCommonResource.DeleteHistoryEvent %>"
               onclick="ASC.CRM.HistoryView.deleteEvent(${id})"></label>
            <img src="<%=WebImageSupplier.GetAbsoluteWebPath("mini_loader.gif")%>"
                id="eventLoaderImg_${id}" style="display:none;" />
          {{/if}}
      </td>
    </tr>
</script>


<%--ImportFromCSVView--%>

<script id="previewImportDataTemplate" type="text/x-jquery-tmpl">
    <tr>
        <td class="borderBase">
          <input type="checkbox" checked="checked" />
        </td>
        <td class="borderBase">
          <img src="${default_image}" />
        </td>
        <td style="width:100%" class="borderBase">
              ${contact_title}
        </td>
    </tr>
</script>

<script id="columnSelectorTemplate" type="text/x-jquery-tmpl">
   {{if isHeader}}
       <option name="is_header">${title}</option>
   {{else !isHeader}}
        <option name="${name}" >${title}</option>
   {{/if}}
</script>

<script id="columnMappingTemplate" type="text/x-jquery-tmpl">
    {{each firstContactFields}}
        <tr>
            <td class="borderBase">
              ${headerColumns[$index]}
            </td>
            <td class="borderBase">
             {{html $item.renderSelector($index)}}
            </td>
            <td class="borderBase">
               ${$value}
            </td>
        </tr>
    {{/each}}
</script>

<%--ConfirmationPanel--%>

<script id="blockUIPanelTemplate" type="text/x-jquery-tmpl">
    <div style="display:none;" id="${id}">
        <div class="popupContainerClass">
            <div class="containerHeaderBlock">
                <table cellspacing="0" cellpadding="0" border="0" style="width:100%; height:0px;">
                    <tbody>
                        <tr valign="top">
                            <td>${headerTest}</td>
                            <td class="popupCancel">
                                <div onclick="PopupKeyUpActionProvider.CloseDialog();" class="cancelButton" title="<%= CRMCommonResource.Close %>"></div>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="infoPanel" style="display:none;">
                <div></div>
            </div>
            <div class="containerBodyBlock">
                {{if typeof(questionText) != "undefined" && questionText != ""}}
                <div>
                    <b>${questionText}</b>
                </div>
                {{/if}}

                {{html innerHtmlText}}

            {{if typeof(OKBtn) != 'undefined' && OKBtn != "" || typeof(OtherBtnHtml) != 'undefined' || typeof(CancelBtn) != 'undefined' && CancelBtn != ""}}
            <div class="crm-actionButtonsBlock">
                {{if typeof(OKBtn) != 'undefined' && OKBtn != ""}}
                <a class="button blue middle{{if typeof(OKBtnClass) != 'undefined'}} ${OKBtnClass}{{/if}}"
                    {{if typeof(OKBtnHref) != 'undefined' && OKBtnHref != ""}} href="${OKBtnHref}"{{/if}}>
                    ${OKBtn}
                </a>
                <span class="splitter-buttons"></span>
                {{/if}}
                {{if typeof(OtherBtnHtml) != 'undefined'}}
                    {{html OtherBtnHtml}}
                    <span class="splitter-buttons"></span>
                {{/if}}
                {{if typeof(CancelBtn) != 'undefined' && CancelBtn != ""}}
                <a class="button gray middle{{if typeof(CancelBtnClass) != 'undefined'}} ${CancelBtnClass}{{/if}}" onclick="PopupKeyUpActionProvider.EnableEsc = true; jq.unblockUI();">${CancelBtn}</a>
                {{/if}}
            </div>
            {{/if}}
            {{if typeof(progressText) != 'undefined' && progressText != "" }}
            <div class="crm-actionProcessInfoBlock">
                <span class="text-medium-describe">${progressText}</span>
                <br />
                <img alt="" src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>" />
            </div>
            {{/if}}
        </div>
        </div>
    </div>
</script>

<%--MakePublicPanel--%>
<script id="makePublicPanelTemplate" type="text/x-jquery-tmpl">
    <div>
        <div class="bold" style="margin: 16px 0 10px;">${Title}</div>
        <p>${Description}</p>

        <div>
            <table class="border-panel" cellpadding="5" cellspacing="0">
                <tr>
                    <td>
                        <input style="margin: 0" type="checkbox" id="isPublic" {{if IsPublicItem == true}}checked="checked"{{/if}} />
                    </td>
                    <td style="padding-left:0">
                        <label for="isPrivate">
                            ${CheckBoxLabel}
                        </label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</script>


<%--PrivatePanel--%>
<script id="privatePanelTemplate" type="text/x-jquery-tmpl">
    <div>
        <span class="header-base">${Title}</span>
        <p><${Description}</p>

        <div>
            <table class="border-panel" cellpadding="5" cellspacing="0">
                <tr>
                    <td>
                        <input style="margin: 0" type="checkbox" id="isPrivate" {{if isPrivateItem == true}}checked="checked"{{/if}} onclick="ASC.CRM.PrivatePanel.changeIsPrivateCheckBox();" />
                    </td>
                    <td style="padding-left:0">
                        <label for="isPrivate">
                            ${checkBoxLabel}
                        </label>
                    </td>
                </tr>
            </table>
        </div>

        <div id="privateSettingsBlock" {{if isPrivateItem == false}}style="display:none;"{{/if}}>
            <br />
            <b>${accessListLable}:</b>
        </div>
    </div>
</script>


<%--CategorySelector--%>
<script id="categorySelectorTemplate" type="text/x-jquery-tmpl">
    <div id="${jsObjName}">
        {{if typeof helpInfoText != "undefined" && helpInfoText != ""}}
        <div style="float: left;">
        {{/if}}

        {{if jq.browser.mobile === true}}
            {{tmpl "categorySelectorMobileTemplate"}}
        {{else}}
            {{tmpl "categorySelectorDesktopTemplate"}}
        {{/if}}

        {{if typeof helpInfoText != "undefined" && helpInfoText != ""}}
        </div>
        <div>
            <div style="height: 20px;margin: 0 0 0 4px;" class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: '${jsObjName}_helpInfo'});">
            </div>
            <div class="popup_helper" id="${jsObjName}_helpInfo">
                {{html helpInfoText}}
            </div>
        </div>
        {{/if}}
    </div>
</script>

<script id="categorySelectorMobileTemplate" type="text/x-jquery-tmpl">
    <select id="${jsObjName}_select" style="width:${maxWidth}px;" onchange="javascript:${jsObjName}.changeContact(jq(this).find('option:selected'));">
        {{each categories}}
            <option id="${jsObjName}_category_${$value.id}" value="${$value.id}">
                ${$value.title}
            </option>
        {{/each}}
    </select>
</script>

<script id="categorySelectorDesktopTemplate" type="text/x-jquery-tmpl">
    <div style="width:${maxWidth}px;" class="categorySelector-selector-container clearFix" id="${jsObjName}_selectorContainer">
        <input type="text" id="${jsObjName}_categoryTitle" value="${selectedCategory.title}" style="width:${maxWidth - 25}px; height:16px; border:none; padding:2px; float: left;" class="textEdit" readonly="readonly" />
        <div class="categorySelector-selector"></div>
        <input type="hidden" id="${jsObjName}_categoryID" value="${selectedCategory.id}" />
    </div>
    <div class="categorySelector-categoriesContainer" id="${jsObjName}_categoriesContainer">
        <div class="categorySelector-categories">

        {{each categories}}
            <div id="${jsObjName}_category_${$value.id}" class="categorySelector-category" onclick="javascript:${jsObjName}.changeContact(this);" >
                {{if typeof($value.cssClass) != "undefined"}}
                <label class="${$value.cssClass}"></label>
                {{/if}}    
                <div style="padding: 9px 0 0 35px;">${$value.title}</div>
            </div>
        {{/each}}

        </div>
    </div>
</script>



<%--History mail event--%>

<script id="historyMailTemplate" type="text/x-jquery-tmpl">
    <div class="headTitle clearFix">
        <div class="importance float-left">
            <i class="{{if important === true}}icon-important{{else}}icon-unimportant{{/if}}"></i>
        </div>
        <span class="header-base">${subject}</span>
    </div>
    <div class="head">
        <div class="row">
            <label><%= CRMCommonResource.From %>:</label>
            <div class="value">
                <a href="${fromHref}" target="_blank" class="from">${from}</a>
            </div>
        </div>
        <div class="row">
            <label><%= CRMCommonResource.To %>:</label>
            <div class="value to-addresses">
                <a href="${toHref}" target="_blank" class="to">${to}</a>
            </div>
        </div>

        {{if typeof cc_text != "undefined" && cc_text != ""}}
        <div class="row"> 
            <label><%= CRMCommonResource.MailCopyTo %>:</label>
            <div class="value cc-addresses">
                ${cc_text}
            </div> 
        </div>
        {{/if}}

        <div class="row">
            <label><%= CRMCommonResource.Date %>:</label>  
            <div class="value">
               <span>${date_created}</span>
            </div>
        </div>
    </div>
</script>


<%--UserSelectorListView--%>

<script id="userSelectorListViewTemplate" type="text/x-jquery-tmpl">
    <div>
        <div id="selectedUsers${objId}" class="clearFix" style="margin-top: 10px;">
            {{if typeof usersWhoHasAccess != "undefined" && usersWhoHasAccess != null && usersWhoHasAccess.length > 0 }}
                {{each(i, item) usersWhoHasAccess}}
                    <div class="selectedUser">
                        <img src="${peopleImgSrc}" alt="" />
                        ${item}
                    </div>
                {{/each}}
            {{/if}}
            {{if typeof selectedUsers != "undefined" && selectedUsers != null && selectedUsers.length > 0 }}
                {{each(i, obj) selectedUsers}}
                    <div class="selectedUser" id="selectedUser_${obj.id}${objId}"
                        onmouseover="${objName}.SelectedItem_mouseOver(this);"
                        onmouseout="${objName}.SelectedItem_mouseOut(this);">
                        <img src="${peopleImgSrc}" alt="" />
                        <img src="${deleteImgSrc}" id="deleteSelectedUserImg_${obj.id}${objId}"
                            onclick="${objName}.SelectedUser_deleteItem(this);"
                            title="<%=CRMCommonResource.DeleteUser%>" style="display: none;" alt="" />
                        ${obj.displayName}
                    </div>
                {{/each}}
            {{/if}}

            {{if typeof selectedGroups != "undefined" && selectedGroups != null && selectedGroups.length > 0 }}
                {{each(i, group) selectedGroups}}
                <div id="selectedGroup_${group.ID}${objId}" class="selectedGroup"
                    onmouseover="ASC.Settings.AccessRights.selectedItem_mouseOver(this);"
                    onmouseout="ASC.Settings.AccessRights.selectedItem_mouseOut(this);">
                            <img src="${groupImgSrc}" alt=""/>
                            <img src="${deleteImgSr}" id="deleteSelectedGroupImg_${group.ID}${objId}"
                                onclick="${objName}.DeleteGroupFromList(this);"
                                title="<%= CRMCommonResource.Delete %>" style="display: none;" alt="" />
                                ${group.Name}
                </div>
                {{/each}}
            {{/if}}

        </div>
        {{if showNotifyPanel === true}}
        <div style="float:right;" id="notifyPanel${objId}">
            <input type="checkbox" id="cbxNotify${objId}" style="float: left;">
            <label for="cbxNotify${objId}" style="float:left; padding: 2px 0 0 4px;">
                ${notifyLabel}
            </label>
        </div>
        {{/if}}
        <div class="clearFix">
            <div id="usrSrListViewAdvUsrSrContainer${objId}"></div>
        </div>
    </div>
</script>

<%-- EventLinkToPanel (HistoryView) --%>

<script id="eventLinkToPanelTmpl" type="text/x-jquery-tmpl">
    <div id="eventLinkToPanel" class="empty-select">
        {{if contactID != 0}}
            <span><%= CRMCommonResource.LinkTo %>:</span>
            <select id="typeSwitcherSelect" class="none-helper">
                <option class="default-field" value="-2" disabled="disabled"><%= CRMJSResource.Choose%></option>
                <option class="hidden" value="-1"><%= CRMCommonResource.ClearFilter%></option>
                <option value="${entityTypeOpportunity}"><%= CRMDealResource.Deal%></option>
                <option value="${entityTypeCase}"><%= CRMCasesResource.Case%></option>
            </select>

            <select id="dealsSwitcherSelect" style="display:none;" class="none-helper">
                <option class="default-field" value="-2" disabled="disabled"><%= CRMJSResource.Choose%></option>
                {{each deals}}
                <option value="${this.id}">${this.title}</option>
                {{/each}}
            </select>

            <select id="casesSwitcherSelect" style="display:none;" class="none-helper">
                <option class="default-field" value="-2" disabled="disabled"><%= CRMJSResource.Choose%></option>
                {{each cases}}
                <option value="${this.id}">${this.title}</option>
                {{/each}}
            </select>

            <input type="hidden" id="typeID" value=""/>
            <input type="hidden" id="itemID" value="0"/>
        {{else}}
            <span><%= CRMCommonResource.AttachThisNoteToContact %>:</span>
            <select id="contactSwitcherSelect" class="none-helper">
                <option class="default-field" value="-2" disabled="disabled"><%= CRMJSResource.Choose%></option>
                <option class="hidden" value="-1"><%= CRMCommonResource.ClearFilter%></option>
                {{each contacts}}
                <option value="${this.id}">${this.displayName}</option>
                {{/each}}
            </select>
            <input type="hidden" id="contactID" value="0"/>
        {{/if}}
    </div>
</script>