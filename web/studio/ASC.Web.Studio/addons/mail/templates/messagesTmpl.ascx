<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="messagesTmpl" type="text/x-jquery-tmpl">
<table class="messages" anchor="new"{{if has_next}} has_next="true"{{/if}}{{if has_prev}} has_prev="true"{{/if}}>
  <tbody>
    {{tmpl(messages, { max_displayed_tags_count: max_displayed_tags_count }) "messageItemTmpl"}}
  </tbody>
</table>
</script>

<script id="messageItemTmpl" type="text/x-jquery-tmpl">
<tr messageid="${id}" date="${date}" chain_date="${chainDate}" fromCRM="${isFromCRM}" fromTL="${isFromTL}" class="message{{if isNew}} new{{/if}}"{{if restoreFolderId}} PrevFolderId="${restoreFolderId}"{{/if}}>
  <td class="checkbox"><input type="checkbox" messageid="${id}" title="<%: MailResource.Select %>"></input></td>
  <td class="importance"><i class="icon-{{if important!=true}}un{{/if}}important"></i></td>
  <td class="from">
    <a href="${anchor}">
      <span class="author" title="${author!='' ? author : sender}" email="${sender}">
        {{if author=='' && sender==''}}<%: MailResource.NoAddress %>{{else}}${author!='' ? author : sender}{{/if}}
      </span>
      {{if chainLength > 1}}<span class="chain-counter">(${chainLength})</span>{{/if}}
    </a>
  </td>
  <td class="subject">
    <a href="${anchor}" _tags="{{each tagIds}}{{if $index>0}},{{/if}}${$value}{{/each}}">
      {{tmpl({ tags: tags, max_displayed_tags_count: $item.max_displayed_tags_count }) "messageItemTagsTmpl"}}
      {{if subject==''}}<%: MailResource.NoSubject %>{{else}}${subject}{{/if}}
    </a>
  </td>
  <td class="attachment"><a href="${anchor}">{{if hasAttachments==true}}<i class="icon-attachment"></i>{{/if}}</a></td>
  <td class="date"><a href="${anchor}">{{if isToday}}<%: MailResource.TodayLabel %>{{else isYesterday}}<%: MailResource.YesterdayLabel %>{{else}}${displayDate}{{/if}}</a></td>
  <td class="time"><a href="${anchor}">${displayTime}</a></td>
</tr>
</script>

<script id="messageItemTagsTmpl" type="text/x-jquery-tmpl">
    {{each tags}}
      {{if $index<max_displayed_tags_count}}<span labelid="${$value.id}" class="tag tagArrow tag${$value.style}" title="${$value.name}">${$value.short_name}</span>{{/if}}
    {{/each}}
    {{if tags.length>max_displayed_tags_count}}<span class="more-tags">${"<%: MailScriptResource.More %>".replace('%1', tags.length-max_displayed_tags_count)}</span>{{/if}}
</script>