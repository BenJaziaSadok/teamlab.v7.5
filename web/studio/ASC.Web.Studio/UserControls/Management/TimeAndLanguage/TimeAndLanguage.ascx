<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TimeAndLanguage.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.TimeAndLanguage" %>

<div class="clearFix langTimeZoneBlock">
    <div class="header-base-small headertitle">
        <%=Resources.Resource.Language%>: <span class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'NotFoundLanguage'});">
        </span>
    </div>
    <div class="timeandlangText">
        <%=RenderLanguageSelector()%></div>
    <div class="header-base-small headertitle two">
        <%=Resources.Resource.TimeZone%>:</div>
    <div class="timeandlangText">
        <%=RenderTimeZoneSelector()%></div>
    <div class="popup_helper" id="NotFoundLanguage">
        <p>
            <%=string.Format(Resources.Resource.NotFoundLanguage, "<a href=\"mailto:documentation@teamlab.com\">", "</a>")%>
            <a href="http://helpcenter.teamlab.com/guides/become-translator.aspx" target="_blank">
                <%=Resources.Resource.LearnMore%></a></p>
    </div>
</div>
