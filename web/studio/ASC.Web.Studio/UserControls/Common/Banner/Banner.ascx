<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Banner.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.Banner.Banner" %>
<%@ Import Namespace="Resources" %>

<a id="joinAffilliateBanner" href="javascript:void(0);" title="<%= Resources.Resource.WeUseTeamLabOnlineOffice%>"
    class="banner-registration">
    <img alt="<%= Resources.Resource.WeUseTeamLabOnlineOffice%>" src="<%= GetImgUrlBanner() %>" />
</a>
<span id="errorAffilliateBanner" class="errorText"></span>