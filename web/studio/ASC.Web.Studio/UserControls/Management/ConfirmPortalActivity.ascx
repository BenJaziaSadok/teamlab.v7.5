<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPortalActivity.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmPortalActivity" %>

<div class="tintMedium borderBase" style="border-right:none; border-left:none; border-bottom:none; margin:20px 0px; width: 600px; text-align:center; <%if (IsChangeDnsMode) { %> width: auto; <%}%>">
        <div class="header-base" style="padding:10px 0px;"><%=_title%></div>        
</div>

<asp:PlaceHolder ID="_confirmContentHolder" runat="server">
    <div style="margin-top:50px;">
        <a class="button blue" onclick="document.forms[0].submit(); return false;" href="javascript:void(0);"><%=_buttonTitle%></a>
        <a class="button gray" href="./" style="margin-left:10px;"><%=Resources.Resource.CancelButton %></a>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_messageHolder" runat="server">
    <script type="text/javascript" >
        var link = jq("#successMessageCnt").find("a").attr("href");
        setTimeout("window.open(link)", 10000);
    </script>        
    <div id="successMessageCnt" style="margin-top:50px; width:400px;">
        <%=_successMessage%>
    </div>
</asp:PlaceHolder>