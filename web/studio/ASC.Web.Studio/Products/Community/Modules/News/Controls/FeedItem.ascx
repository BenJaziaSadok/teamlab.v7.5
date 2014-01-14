<%@ Assembly Name="ASC.Web.Community.News" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FeedItem.ascx.cs" Inherits="ASC.Web.Community.News.Controls.FeedItem" %>

<td class="borderBase gray-text">
    <asp:Label runat="server" ID="Date"></asp:Label>
</td>
<td class="borderBase">
    <b><asp:Literal runat="server" ID="Type"></asp:Literal></b>
</td>
<td class="borderBase">
    <div class="news-link-container">
        <asp:HyperLink runat="server" ID="NewsLink" class="linkMedium longWordsBreak"></asp:HyperLink>
    </div>
</td>
<td class="borderBase" style="text-align: right">
    <asp:Literal runat="server" ID="profileLink"></asp:Literal>
</td>
