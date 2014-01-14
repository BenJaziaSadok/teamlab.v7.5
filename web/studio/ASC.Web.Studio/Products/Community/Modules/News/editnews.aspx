<%@ Page Language="C#" MasterPageFile="~/Products/Community/Modules/News/news.Master" AutoEventWireup="true" CodeBehind="editnews.aspx.cs" Inherits="ASC.Web.Community.News.EditNews" %>

<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>

<%@ Import Namespace="ASC.Data.Storage" %>
<%@ Import Namespace="ASC.Web.Community.News.Resources" %>

<asp:Content ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="PageContent" ContentPlaceHolderID="NewsContents" runat="server">
    <div style="margin-top: 15px;">
    <div class="headerPanel-splitter requiredField">
        <span class="requiredErrorText"><%=NewsResource.RequaredFieldValidatorCaption%></span>
        <div class="headerPanelSmall-splitter headerPanelSmall" id="newsCaption">
            <b><%=NewsResource.NewsCaption%>:</b>
        </div>
        <asp:TextBox runat="server" ID="feedName" class="textEdit" Style="width: 100%" />
    </div>
    <div class="headerPanel-splitter">
        <div style="float: left; margin-right: 8px;">
            <b><%=NewsResource.NewsType%>:</b>
        </div>
        <asp:DropDownList runat="server" ID="feedType" class="comboBox" DataTextField="TypeName" DataValueField="id" CssClass="display-none" />
    </div>
    <div class="headerPanel-splitter">
        <div class="headerPanelSmall-splitter">
            <b><%=NewsResource.NewsBody%>:</b>
        </div>
        <% if (_mobileVer) { %>
        <asp:TextBox runat="server" ID="SimpleText" TextMode="MultiLine" style="width:100%; height:200px;"></asp:TextBox>
        <textarea id="mobiletext" name="mobiletext" style="display:none;"><%=_text%></textarea>
        <% } else { %>
        <FCKeditorV2:FCKeditor runat="server" ID="HTML_FCKEditor" Width="100%" Height="400px" />
        <% } %>
    </div>
    
    <div class="big-button-container" id="panel_buttons">
        <%--<asp:LinkButton ID="lbSave" OnClientClick="javascript:PreSaveMobile(); NewsBlockButtons();" CssClass="button blue"
            OnClick="SaveFeed" CausesValidation="true" runat="server" Style="margin-right: 8px;"><%=NewsResource.PostButton%></asp:LinkButton>--%>
        <a href="javascript:void(0);" id="lbSave" class="button blue big" onclick="PreSaveMobile(); NewsBlockButtons(); CheckDataNews();"
         style="margin-right: 8px;"><%=NewsResource.PostButton%></a>
        <a href="javascript:void(0);" onclick="FeedPrevShow(); return false;"
            class="button blue big" style="margin-right: 8px;">
            <%=NewsResource.Preview%></a>
            
            <asp:LinkButton ID="lbCancel" CssClass="button gray big cancelFckEditorChangesButtonMarker"
            OnClick="CancelFeed" CausesValidation="true" OnClientClick="javascript:NewsBlockButtons();" runat="server"><%=NewsResource.CancelButton%></asp:LinkButton>
             
    </div>
    <div style="padding-top: 15px; display: none;" id="action_loader" class="clearFix">        
        <div class="text-medium-describe">
            <%=NewsResource.PleaseWaitMessage%>
		</div>
		<img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
    </div>
    <div id="feedPrevDiv" style="display: none; padding-top: 20px">
        <div class="headerPanel">
            <%=NewsResource.FeedPrevCaption%>
        </div>
        <input id="feedPrevDiv_Caption" class="feedPrevCaption" />
        <div id="feedPrevDiv_Body" class="feedPrevBody clearFix longWordsBreak">
        </div>
        <div style='margin-top:25px;'><a class="button blue big" href='javascript:void(0);' onclick='HidePreview(); return false;'><%= NewsResource.HideButton%></a></div>
    </div>
    </div>
</asp:Content>
