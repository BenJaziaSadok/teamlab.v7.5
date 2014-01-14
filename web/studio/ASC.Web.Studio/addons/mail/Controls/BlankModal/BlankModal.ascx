<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BlankModal.ascx.cs" Inherits="ASC.Web.Mail.Controls.BlankModal" %>

<div class="backdrop" blank-page=""></div>

<div class="centerBox" blank-page="">
    <div class="header">
        <span class="close" onclick="blankModal.close();"></span><%=ASC.Web.Mail.Resources.MailResource.BlankModalHeader%>
    </div>
    <div class="content">
        <div class="column">
            <div class="img accounts"></div>
           <div class="title"><%=ASC.Web.Mail.Resources.MailResource.BlankModalAccountsTitle%></div>
           <ul>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalAccountsTip1%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalAccountsTip2%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalAccountsTip3%></li>
           </ul>
       </div>
       <div class="column">
           <div class="img tags"></div>
           <div class="title"><%=ASC.Web.Mail.Resources.MailResource.BlankModalTagsTitle%></div>
           <ul>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalTagsTip1%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalTagsTip2%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalTagsTip3%></li>
           </ul>
       </div>
       <div class="column">
           <div class="img crm"></div>
           <div class="title"><%=ASC.Web.Mail.Resources.MailResource.BlankModalCRMTitle%></div>
           <ul>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalCRMTip1%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalCRMTip2%></li>
               <li><%=ASC.Web.Mail.Resources.MailResource.BlankModalCRMTip3%></li>
           </ul>
       </div>
    </div>
    <div class="footer">
        <div class="line">
            <table class="btn" cellpadding=0 cellspacing=0 onclick="blankModal.addAccount();">
                <tr>
                    <td class="left-corner"></td>
                    <td class="body">
                        <%=ASC.Web.Mail.Resources.MailResource.BlankModalCreateBtn%>
                    </td>
                    <td class="right-corner"></td>
                </tr>
            </table>
        </div>
        <div class="line">
            <span class="link" onclick="blankModal.close();"><%=ASC.Web.Mail.Resources.MailResource.BlankModalGoNowLnk%></span>
        </div>
    </div>
</div>