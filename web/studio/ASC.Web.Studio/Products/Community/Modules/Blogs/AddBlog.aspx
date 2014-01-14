<%@ Assembly Name="ASC.Web.Community.Blogs" %>
<%@ Page Language="C#" MasterPageFile="~/Products/Community/Community.master" AutoEventWireup="true" CodeBehind="AddBlog.aspx.cs" Inherits="ASC.Web.Community.Blogs.AddBlog" Title="Untitled Page" %>

<%@ Import Namespace="ASC.Blogs.Core.Resources" %>
<%@ Import Namespace="ASC.Data.Storage" %>

<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CommunityPageHeader" runat="server">

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="CommunityPageContent" runat="server">
    <sc:Container id="mainContainer" runat="server">
        <header></header>
        <body>
          <table width="100%">
            <tr>
              <td>
            <div class="headerPanel-splitter requiredField">
                <span class="requiredErrorText"><%=BlogsResource.BlogTitleEmptyMessage %></span>
                <asp:Panel ID="pnlHeader" runat="server">
                    <div id="postHeader" class="headerPanelSmall-splitter headerPanelSmall">
                        <b><%=BlogsResource.BlogTitleLabel%>:</b>
                    </div>
                    <div>
                        <asp:TextBox ID="txtTitle" MaxLength="255"  CssClass="textEdit" runat="server" Width="100%"></asp:TextBox>
                    </div>
                </asp:Panel>
            </div>
            <div class="headerPanel-splitter">
                <div class="headerPanelSmall-splitter">
                    <b><%=BlogsResource.ContentTitle %>:</b>
                </div>                 
                <% if (_mobileVer){%>    
                    <textarea ID="mobiletextEdit" style="width:100%; height:200px;"></textarea>
                    <textarea id="mobiletext" name="mobiletext" style="display:none;"><%=_text%></textarea>
                <%}else{ %>
                    <FCKeditorV2:FCKeditor ID="FCKeditor" Height="400px" runat="server">
                    </FCKeditorV2:FCKeditor>
                <%} %>
            </div>
            <div class="headerPanel-splitter">
                <div class="headerPanelSmall-splitter">
                    <b><%=BlogsResource.TagsTitle%>:</b>
                </div>
                <div>
                    <asp:TextBox CssClass="textEdit" ID="txtTags" runat="server" Width="100%" 
                        autocomplete="off" onkeydown="return blogTagsAutocompleteInputOnKeyDown(event);"></asp:TextBox>
                    <div class="text-medium-describe" style="text-align: left;">
                        <%=BlogsResource.EnterTagsMessage%>
                    </div>
                </div>
            </div>
            <div>
                <input type="checkbox" id="notify_comments" name="notify_comments" checked /><label
                    for="notify_comments"><%=BlogsResource.SubscribeOnNewCommentsAction%></label>
            </div>
               </td>
               <%if (!_mobileVer){%>
               <td class="teamlab-cut">
                    <div class="title-teamlab-cut"><%= BlogsResource.TeamlabCutTitle %></div>
                    <div class="text-teamlab-cut"><%= String.Format(BlogsResource.TeamlabCutText, "<span class=\"teamlab-cut-button\"></span>") %></div>
                </td>
                <% } %>
                </tr>
                </table>

            <div id="panel_buttons" style="margin-top: 30px;">
                <%--<asp:LinkButton ID="lbtnPost" OnClientClick="javascript:BlogsManager.PerformMobilePost(); BlogsManager.BlockButtons();"
                    CssClass="button blue" Width="80" runat="server"></asp:LinkButton>--%>                    
                    <a class="button blue big" href="javascript:void(0);"
                        onclick="BlogsManager.PerformMobilePost(); BlogsManager.BlockButtons(); BlogsManager.CheckData();"><%=BlogsResource.PostButton%></a>
                    <span class="splitter"></span>
                    <a class="button blue big" href="javascript:void(0);"
                        onclick="BlogsManager.ShowPreview('<%=FCKeditor.ClientID%>', '<%=txtTitle.ClientID%>'); return false;"><%=BlogsResource.PreviewButton%></a>
                    <span class="splitter"></span>                    
                    <asp:LinkButton ID="lbCancel" OnClientClick="javascript:BlogsManager.BlockButtons();"
                        CssClass="button gray big cancelFckEditorChangesButtonMarker" runat="server"
                        OnClick="lbCancel_Click"><%=BlogsResource.CancelButton %></asp:LinkButton>
                    
            </div>
            <div style="margin-top: 20px; display: none;" id="action_loader" class="clearFix">
				<div class="text-medium-describe">
                    <%=BlogsResource.PleaseWaitMessage%>
                </div>                
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>">  
            </div>
            <div id="previewHolder" style="display: none;">
                <asp:PlaceHolder ID="PlaceHolderPreview" runat="server"></asp:PlaceHolder>
            </div>
        </body>
    </sc:Container>
</asp:Content>
<asp:Content ID="SidePanel" ContentPlaceHolderID="CommunitySidePanel" runat="server">
</asp:Content>

