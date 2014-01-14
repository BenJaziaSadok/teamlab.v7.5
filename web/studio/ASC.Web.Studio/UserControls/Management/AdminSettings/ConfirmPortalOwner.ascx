<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ConfirmPortalOwner.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.ConfirmPortalOwner" %> 

<div class="tintMedium borderBase confirmTitle">    
    <div class="header-base"><%=String.Format(Resources.Resource.ConfirmOwnerPortalTitle, _newOwnerName)%></div>
</div>

<asp:PlaceHolder ID="_confirmContentHolder" runat="server">
    <div class="confirmContent clearFix">
        <a class="button blue" onclick="document.forms[0].submit(); return false;" href="javascript:void(0);"><%=Resources.Resource.SaveButton%></a>
        <a class="button gray" href="./"><%=Resources.Resource.CancelButton %></a>
    </div>
</asp:PlaceHolder>

<asp:PlaceHolder ID="_messageHolder" runat="server">
    <script type="text/javascript" language="javascript">
        setTimeout("window.open('./','_self');",10000);
    </script>        
    <div class="confirmSuccess">
        <%=string.Format(Resources.Resource.ConfirmOwnerPortalSuccessMessage, "<br/>", "<a href=\"" + ASC.Web.Studio.Utility.CommonLinkUtility.GetDefault() + "\">", "</a>")%>
    </div>
</asp:PlaceHolder>