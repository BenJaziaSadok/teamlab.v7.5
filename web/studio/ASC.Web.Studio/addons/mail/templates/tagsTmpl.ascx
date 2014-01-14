<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="tagsTmpl" type="text/x-jquery-tmpl">
    <table class="tag_list">
        <tbody>
            {{tmpl(tags) "tagItemTmpl"}}
        </tbody>
    </table>
</script>

<script id="tagItemTmpl" type="text/x-jquery-tmpl">
    <tr data_id="${id}" class="tag_item row {{if id<0 }}inactive{{/if}}">
        <td class="label">
            <span class="tag tagArrow tag${style}" title="${name}" style="margin-top:0;">${name}</span>
        </td>
        <td class="addresses"></td>
        {{if id<0 }}
            <td class="notify_column">
                <span class="notification" title="<%: MailScriptResource.TagNotificationText %>"><%: MailScriptResource.TagNotificationText %></span>
            </td>
        {{else}}
            <td class="menu_column">
                <div class="menu" title="<%: MailScriptResource.Actions %>" data_id="${id}"></div>
            </td>
        {{/if}}
    </tr>
</script>

<script id="tagInPanelTmpl" type="text/x-jquery-tmpl">
    <div>
        <span class="tag {{if used == false}} inactive{{else}} tagArrow tag${style}{{/if}}" tag_id="${id}" title="${name}">
            <span class="square tag${style}"></span>
            <div class="name">${name}</div>
        </span>
    </div>
</script>