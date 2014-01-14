<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="false" MasterPageFile="~/Masters/basetemplate.master" CodeBehind="Wizard.aspx.cs" Inherits="ASC.Web.Studio.Wizard" Title="TeamLab" %>
<%@ MasterType TypeName="ASC.Web.Studio.Masters.BaseTemplate" %>
<asp:Content ContentPlaceHolderID="PageContent" runat="server">

    <div class="wizardContent">
        <div class="wizardTitle">
            <%= Resources.Resource.WelcomeTitle %>
        </div>
        <div class="wizardDesc"><%= Resources.Resource.WelcomeDescription %></div>
        <asp:PlaceHolder ID="content" runat="server"></asp:PlaceHolder>
    </div>
    <% if (!ASC.Core.CoreContext.Configuration.Standalone)
       { %>
    <!-- Google Code for Registered Users -->
    <!-- Remarketing tags may not be associated with personally identifiable information or placed on pages related to sensitive categories. For instructions on adding this tag and more information on the above requirements, read the setup guide: google.com/ads/remarketingsetup -->
    <script type="text/javascript">
        /* <![CDATA[ */
        var google_conversion_id = 1025072253;
        var google_conversion_label = "f_-wCOPEuQMQ_bjl6AM";
        var google_custom_params = window.google_tag_params;
        var google_remarketing_only = true;
        /* ]]> */
    </script>
    <script type="text/javascript" src="//www.googleadservices.com/pagead/conversion.js">
    </script>
    <noscript>
        <div style="display: inline;">
            <img height="1" width="1" style="border-style: none;" alt="" src="//googleads.g.doubleclick.net/pagead/viewthroughconversion/1025072253/?value=0&amp;label=f_-wCOPEuQMQ_bjl6AM&amp;guid=ON&amp;script=0" />
        </div>
    </noscript>
    <% } %>
</asp:Content>
