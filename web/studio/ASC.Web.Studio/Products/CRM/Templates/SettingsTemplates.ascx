<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>

<%--Settings: Custom fields --%>

<script id="customFieldSettingsRowTmpl" type="text/x-jquery-tmpl">
{{if fieldType ==  3}}
    <li class="with-entity-menu">
        <table class="field_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="borderBase" style="width:200px;"></td>
                    <td class="borderBase">
                        <label>
                            {{tmpl "customFieldSettingsTmpl"}}
                            <span class="customFieldTitle">${label}</span>
                        </label>
                    </td>
                    <td class="borderBase count_link_contacts gray-text" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    <td class="borderBase" style="width:30px;">
                        <div id="fieldMenu_${id}" fieldid="${id}" class="entity-menu" data-relativeitemscount="${relativeItemsCount}" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
{{else fieldType ==  4}}
      <li class="expand_collapce_element with-entity-menu">
         <table class="field_row" cellspacing="0" cellpadding="0" border="0" width="100%">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="borderBase" style="padding-left: 10px">
                        {{tmpl "customFieldSettingsTmpl"}}
                    </td>
                    <td class="borderBase count_link_contacts" style="width:100px;"></td>
                    <td class="borderBase" style="width:30px;">
                        <div id="fieldMenu_${id}" fieldid="${id}" class="entity-menu" data-relativeitemscount="${relativeItemsCount}" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    </td>
                </tr>
             </tbody>
          </table>
       </li>
{{else}}
       <li class="with-entity-menu">
          <table class="field_row" cellspacing="0" cellpadding="0" border="0" width="100%">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_row_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="header-base-small borderBase customFieldTitle" style="width:200px; padding-left:10px;">
                        <div class="text-overflow" style="width:200px;overflow:hidden;" title="${label}">${label}</div>
                    </td>
                    <td class="borderBase">
                       {{tmpl "customFieldSettingsTmpl"}}
                    </td>
                    <td class="borderBase count_link_contacts gray-text" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    <td class="borderBase" style="width:30px;">
                        <div id="fieldMenu_${id}" fieldid="${id}" class="entity-menu" data-relativeitemscount="${relativeItemsCount}" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
{{/if}}
</script>

<script id="customFieldSettingsTmpl" type="text/x-jquery-tmpl">
    {{if fieldType ==  0}}
        <input id="custom_field_${id}" name="custom_field_${id}" size="${(maskObj.size > 120 ? 120 : maskObj.size)}" type="text" class="textEdit">
    {{else fieldType ==  1}}
        <textarea rows="${(maskObj.rows > 25 ? 25 : maskObj.rows)}" cols="${(maskObj.cols > 120 ? 120 : maskObj.cols)}" name="custom_field_${id}" id="custom_field_${id}"></textarea>
    {{else fieldType ==  2}}
        <select class="comboBox" name="custom_field_${id}" id="custom_field_${id}" disabled="disabled">
          {{each maskObj}}
             <option value="${$value}">${$value}</option>
          {{/each}}
        </select>
    {{else fieldType ==  3}}
              <input name="custom_field_${id}" id="custom_field_${id}" type="checkbox" style="vertical-align: middle;" disabled="disabled"/>
    {{else fieldType ==  4}}
       <span id="custom_field_${id}" class="header-base headerExpand customFieldTitle" onclick="ASC.CRM.SettingsPage.toggleCollapceExpand(this)">${label}</span>
    {{else fieldType ==  5}}
      <input type="text" id="custom_field_${id}"  name="custom_field_${id}" class="textEdit textEditCalendar" />
    {{/if}}
</script>


<%--Settings: DealMilestoneView--%>

<script id="dealMilestoneTmpl" type="text/x-jquery-tmpl">
    <li id="deal_milestone_id_${id}" class="with-entity-menu">
        <table class="deal_milestone_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_drag_handle borderBase">&nbsp;</div>
                    </td>
                    <td class="borderBase" style="width:25px;">
                        <div class="currentColor" style="background:${color}" onclick="ASC.CRM.DealMilestoneView.showColorsPanel(this);"></div>
                    </td>
                    <td class="header-base-small borderBase deal_milestone_title" style="width:200px;">
                        ${title}
                    </td>
                    <td class="borderBase">
                        {{if stageType == 1}}
                        <%= CRMSettingResource.DealMilestoneStatusDescription_ClosedAndWon %>
                        {{else stageType == 2}}
                        <%= CRMSettingResource.DealMilestoneStatusDescription_ClosedAndLost %>
                        {{/if}}
                        {{html jq.htmlEncodeLight(description).replace(/&#10;/g, "<br/>") }}
                    <td class="borderBase" style="width:60px; text-align: center;">
                          ${successProbability}%
                    </td>
                    <td class="borderBase count_link_deals" style="width:100px;">
                        <a class="gray-text" href="${relativeItemsUrl}">${relativeItemsString}</a>
                    </td>
                    <td class="borderBase" style="width:30px;">
                    {{if relativeItemsCount == 0 }}
                        <div id="deal_milestone_menu_${id}" dealmilestoneid="${id}" class="entity-menu" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" alt="" title="" style="display: none;">&nbsp;</div>
                    {{/if}}
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
</script>


<%--Settings: ListItemView--%>

<script id="listItemsTmpl" type="text/x-jquery-tmpl">
    <li id="list_item_id_${id}" class="with-entity-menu">
        <table class="item_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="" style="width:6px; background: #FFFFFF">
                        <div class="sort_drag_handle borderBase">&nbsp;</div>
                    </td>

                    {{if ASC.CRM.ListItemView.CurrentType === 2 || ASC.CRM.ListItemView.CurrentType === 3}}
                        <td class="borderBase" style="width:36px;">
                            <label alt="${imageAlt}" img_name="${imageName}" title="${imageTitle}"
                                class="currentIcon {{if ASC.CRM.ListItemView.CurrentType == 2}}task_category{{else}}event_category{{/if}} ${cssClass}"
                                onclick="ASC.CRM.ListItemView.showIconsPanel(this);"></label>
                            <div class="ajax_change_icon" alt="" title="" style="display: none;">&nbsp;</div>
                        </td>
                    {{else ASC.CRM.ListItemView.CurrentType === 1}}
                        <td class="borderBase" style="width:25px;">
                            <div class="currentColor" style="background:${color}" onclick="ASC.CRM.ListItemView.showColorsPanel(this);"
                                title="<%= CRMSettingResource.ChangeColor %>"></div>
                        </td>
                    {{/if}}

                    <td class="header-base-small borderBase item_title" style="width:230px;">
                        ${title}
                    </td>

                    <td class="borderBase item_description">
                        {{if ASC.CRM.ListItemView.CurrentType !== 4}}
                            {{html jq.htmlEncodeLight(description).replace(/&#10;/g, "<br/>") }}
                        {{/if}}
                    </td>

                    {{if ASC.CRM.ListItemView.CurrentType === 3}}
                    <td class="borderBase count_link_items gray-text" style="width:100px;">
                        ${relativeItemsString}
                    </td>
                    {{else}}
                    <td class="borderBase count_link_items" style="width:100px;">
                        <a class="gray-text" href="${relativeItemsUrl}">${relativeItemsString}</a>
                    </td>
                    {{/if}}

                    <td class="borderBase" style="width:30px;">
                    {{if relativeItemsCount == 0 }}
                        <div id="list_item_menu_${id}" listitemid="${id}" class="entity-menu" title="<%= CRMCommonResource.Actions %>"></div>
                        <div class="ajax_loader" alt="" title="" style="display: none;">&nbsp;</div>
                    {{/if}}
                   </td>
                </tr>
            </tbody>
        </table>
    </li>
</script>


<%--Settings: TagSettingsView--%>

<script id="deleteUnusedTagsButtonTmpl" type="text/x-jquery-tmpl">
    <li>
        <a id="deleteUnusedTagsButton" class="dropdown-item" title="<%= CRMSettingResource.DeleteUnusedTags %>">
            <%= CRMSettingResource.DeleteUnusedTags %>
        </a>
    </li>
</script>

<script id="tagRowTemplate" type="text/x-jquery-tmpl">
    <li class="borderBase">
        <table class="tag_row tableBase" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="header-base-small">
                        <div class="title">${value}</div>
                    </td>
                    <td class="count_link_items" style="width:150px;">
                        <a class="gray-text" href="${relativeItemsUrl}">${relativeItemsString}</a>
                    </td>
                    <td style="width:40px;">
                    {{if relativeItemsCount == 0 }}
                        <label class="crm-deleteBtn" title="<%= CRMSettingResource.DeleteTag %>" alt="<%= CRMSettingResource.DeleteTag %>"
                            onclick='ASC.CRM.TagSettingsView.deleteTag(this);'></label>
                        <div class="ajax_loader" style="display: none;" title="" alt="">&nbsp;</div>
                    {{/if}}
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
</script>


<%--Settings: TaskTemplateView--%>

<script id="templateContainerRow" type="text/x-jquery-tmpl">
    <li id="templateContainerHeader_${id}">
        <table class="templateContainer_row" cellspacing="0" cellpadding="0">
            <tbody>
                <tr>
                    <td class="borderBase">
                        <span onclick="ASC.CRM.TaskTemplateView.toggleCollapceExpand(this)" class="header-base headerExpand">
                            ${title}
                        </span>
                    </td>
                    <td class="borderBase" style="width:70px;text-align: right;padding-right: 10px;">
                        <label class="crm-addBtn" align="absmiddle"
                            title="<%= CRMSettingResource.AddNewTaskTemplate %>"
                            onclick="ASC.CRM.TaskTemplateView.showTemplatePanel(${id})"></label>
                        <label class="crm-editLink" align="absmiddle"
                            title="<%= CRMSettingResource.EditTaskTemplateContainer %>"
                            onclick="ASC.CRM.TaskTemplateView.showTemplateConatainerPanel(${id})"></label>
                        <label class="crm-deleteBtn" align="absmiddle"
                            title="<%= CRMSettingResource.DeleteTaskTemplateContainer %>"
                            onclick="ASC.CRM.TaskTemplateView.deleteTemplateConatainer(${id})"></label>
                        <img class="loaderImg" align="absmiddle" style="display: none;"
                            src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>"/>
                    </td>
                </tr>
            </tbody>
        </table>
    </li>
    <li id="templateContainerBody_${id}" style="{{if typeof(items)=="undefined"}}display:none;{{/if}}">
        {{tmpl(items) "templateRow"}}
    </li>
</script>

<script id="templateRow" type="text/x-jquery-tmpl">
    <table cellspacing="0" cellpadding="0"  id="templateRow_${id}" class="templateContainer_row" style="margin-bottom: -1px;">
        <tbody>
            <tr>
                <td class="borderBase" style="width:30px;">
                    <img title="${category.title}" alt="${category.title}" src="${category.imagePath}" />
                </td>
                <td class="borderBase">
                    <div class="divForTemplateTitle">
                        <span id="templateTitle_${id}" class="templateTitle" title="${description}">${title}</span>
                    </div>
                    <div style="padding-top: 5px; display: inline-block;">
                        ${ASC.CRM.TaskTemplateView.getDeadlineDisplacement(offsetTicks, deadLineIsFixed)}
                    </div>
                </td>
                <td class="borderBase" style="width:200px;">
                    <span class="userLink">${responsible.displayName}</span>
                </td>
                <td class="borderBase" style="width:70px;text-align: right;padding-right: 10px;">
                    <label class="crm-editLink" align="absmiddle"
                            title="<%= CRMSettingResource.EditTaskTemplate %>"
                            onclick="ASC.CRM.TaskTemplateView.showTemplatePanel(${containerID}, ${id})"></label>
                    <label class="crm-deleteBtn" align="absmiddle"
                         title="<%= CRMSettingResource.DeleteTaskTemplate %>"
                         onclick="ASC.CRM.TaskTemplateView.deleteTemplate(${id})"></label>
                    <img class="loaderImg" align="absmiddle" style="display: none;"
                         src="<%= WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID) %>"/>
                </td>
            </tr>
        </tbody>
    </table>
</script>


<%--Settings: WebToLeadFormView--%>

<script id="sampleFormTmpl" type="text/x-jquery-tmpl">
  <form name='sampleForm' method='POST' action='${webtoleadfromhandlerPath}' accept-charset='UTF-8'>
    <meta content='text/html;charset=UTF-8' http-equiv='content-type'>
    <style type="text/css">
        #sampleFormPanel {
           padding: 10px;
        }
        #sampleFormPanel dt {
           float: left;
           text-align: right;
           width: 40%;
        }
        #sampleFormPanel dd:not(:first-child) {
             margin-bottom: 5px;
             margin-left: 40%;
             padding-left: 10px;
        }
        #sampleFormPanel input[type=text] {
             border: solid 1px #C7C7C7;
        }
    </style>

    <dl id="sampleFormPanel">
        <dt></dt><dd><input type="hidden" name="is_company" value="${isCompany}"/></dd>

         {{each fieldListInfo}}
           <dt>
            ${title}:
           </dt>
           <dd>
              <input name="${name}" type="text"/>
           </dd>
        {{/each}}
        {{each tagListInfo}}
           <input type="hidden" name="tag" value="${title}" />
        {{/each}}
        <dt>
        </dt>
        <dd>
            <input name="${name}" value="<%= CRMSettingResource.SubmitFormData %>" type="submit"
                onclick="javascript:
                            var isValid = true,
                                form = document.getElementById('sampleFormPanel'),
                                childs = form.getElementsByTagName('input'),
                                isCompany = true,                        
                                firstName = '',
                                lastName = '',
                                companyName = '';

                            for (var i = 0, n = childs.length; i < n; i++) {
                                var fieldName = childs[i].getAttribute('name');

                                switch (fieldName) {
                                    case 'is_company':
                                        isCompany = childs[i].value.trim();
                                        break;
                                    case 'firstName':
                                        firstName = childs[i].value.trim();
                                        break;
                                    case 'lastName':
                                        lastName = childs[i].value.trim();
                                        break;
                                    case 'companyName':
                                        companyName = childs[i].value.trim();
                                        break;
                                }
                            }

                            if (isCompany == 'false') {
                                if (firstName == ''){
                                    alert('<%= CRMContactResource.ErrorEmptyContactFirstName %>');
                                    isValid = false;
                                }
                                else if (lastName == ''){
                                    alert('<%= CRMContactResource.ErrorEmptyContactLastName %>');
                                    isValid = false;
                                }
                            }
                            else if (isCompany == 'true') {
                                if(companyName == '') {
                                    alert('<%= CRMContactResource.ErrorEmptyCompanyName %>');
                                    isValid = false;
                                }
                            } else {
                                isValid = false;
                            }
                            return isValid;"/>
        </dd>
    </dl>
    <input type="hidden" name="return_url" value="${returnURL}" />
    <input type="hidden" name="web_form_key"  value="${webFormKey}"/>
    <input type="hidden" name="notify_list" value="${notifyList}"/>
    <input type="hidden" name="managers_list" value="${managersList}"/>
    <input type="hidden" name="is_shared" value="${isShared}"/>
  </form>
</script>


<%--Settings: SMTPSettingsForm--%>

<script id="SMTPSettingsFormTemplate" type="text/x-jquery-tmpl">
    <table cellpadding="5" cellspacing="0">
        <tr>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.Host%>:</div>
                <input type="text" class="textEdit" id="tbxHost"/>
            </td>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.Port%>:</div>
                <div>
                    <input type="text" class="textEdit" id="tbxPort" maxlength="5"/>
                    <input id="cbxAuthentication" type="checkbox" />
                    <label for="cbxAuthentication" class="header-base-small" style="line-height: 21px;"><%=CRMSettingResource.Authentication%></label>
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.HostLogin%>:</div>
                <input type="text" class="textEdit" id="tbxHostLogin"/>
            </td>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.HostPassword%>:</div>
                <input type="password" class="textEdit" id="tbxHostPassword"/>
            </td>
        </tr>
        <tr>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.SenderDisplayName%>:</div>
                <input type="text" class="textEdit" id="tbxSenderDisplayName"/>
            </td>
            <td>
                <div class="header-base-small headerTitle"><%=CRMSettingResource.SenderEmailAddress%>:</div>
                <input type="text" class="textEdit" id="tbxSenderEmailAddress"/>
            </td>
        </tr>
        <tr>
            <td>
                <input id="cbxEnableSSL" type="checkbox" />
                <label for="cbxEnableSSL" class="header-base-small" style="float: left; line-height: 20px;">
                    <%=CRMSettingResource.EnableSSL%>
                </label>
            </td>
            <td></td>
        </tr>
    </table>
</script>