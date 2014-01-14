<%@ Control Language="C#" AutoEventWireup="true" EnableViewState="false" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<script id="replyMessageHtmlBodyTmpl" type="text/x-jquery-tmpl">
  <div style="padding: 10px;"><br><br>
    <p>${date}, ${originalFrom}:</p>
    <div style="margin-left:20px; padding-left:20px; border-left:1px solid #D1D1D1;">{{html htmlBody}}</div>
  </div>
</script>

<script id="forwardMessageHtmlBodyTmpl" type="text/x-jquery-tmpl">
  <div style="padding: 10px;"><br><br>
    <p>-------- <%: MailScriptResource.ForwardTitle %> --------<p/>
    <p><%: MailScriptResource.FromLabel %>: ${to}</p>
    <p><%: MailScriptResource.ToLabel %>: ${from}</p>
    <p><%: MailScriptResource.DateLabel %>: ${date}</p>
    <p><%: MailScriptResource.SubjectLabel %>: ${subject}</p>
    <br/>
    <div>{{html htmlBody}}</div>
  </div>
</script>