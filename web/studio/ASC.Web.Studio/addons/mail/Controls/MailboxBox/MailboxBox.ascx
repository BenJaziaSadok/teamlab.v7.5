<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailboxBox.ascx.cs" Inherits="ASC.Web.Mail.Controls.MailboxBox" %>
<%@ Register TagPrefix="ascwc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div class="panel">
  <div class="LeftSideHeader">
    <b class="link"><%=ASC.Web.Mail.Resources.MailScriptResource.AccountsLabel%></b>
    <a id="manageAccounts" manage="<%=ASC.Web.Mail.Resources.MailResource.Manage%>" ready="<%=ASC.Web.Mail.Resources.MailResource.Ready%>"><%=ASC.Web.Mail.Resources.MailResource.Manage%></a>
  </div>
  <div class="studioLeftSidePanel_content" id="customAccountPanelContent"></div>
  <a class="addNew account">Add new</a>
</div>
