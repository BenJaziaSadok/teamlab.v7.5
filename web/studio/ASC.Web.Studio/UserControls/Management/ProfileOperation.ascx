<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProfileOperation.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ProfileOperation" %>

<div id="operationBlock" runat="server">
<div style="font-weight:bold;font-size:16px;">
<%= Resources.Resource.DeleteProfileConfirm%>
</div>

<div style="margin-top:60px;">
    <asp:LinkButton ID="lblDelete" runat="server" CssClass="button blue" OnClick="DeleteProfile"><%= Resources.Resource.DeleteProfileButton%></asp:LinkButton>
</div>
</div>
<div style="font-weight:bold;font-size:16px;" id="result" runat="server"></div>