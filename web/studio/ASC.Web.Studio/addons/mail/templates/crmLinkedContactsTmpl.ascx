<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="crmContactItemTmpl" type="text/x-jquery-tmpl">
    <tr data-entity_id="${entity_id}" entity_type="${entity_type}" class='linked_entity_row'>
        <td class="linked_entity_row_avatar_column">
        {{if entity_type == 1}}
            <img src="${avatarLink}" class="crm_avatar_img"/>
        {{/if}}
        {{if entity_type == 2}}
            <div class="crm_avatar_img case"/>
        {{/if}}
        {{if entity_type == 3}}
            <div class="crm_avatar_img opportunities"/>
        {{/if}}
        </td>
        <td class="linked_entity_row_title_column">
            <span>
                ${title}
            </span>
        </td>
        <td class="linked_entity_row_button_column">
            <div class="unlink_entity"></div>
        </td>
    </tr>
</script>