<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StudioSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.StudioSettings" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="ASC.Web.Studio.Utility" %>

<%--transfer portal--%>
<asp:PlaceHolder ID="_transferPortalSettings" runat="server"></asp:PlaceHolder>

<%--timezone & language--%>
<div class="clearFix">
<div class="settings-block">
    <div class="header-base clearFix">
        <%= Resource.StudioTimeLanguageSettings %>
    </div>
    <div id="studio_lngTimeSettingsInfo"></div>
    <div id="studio_lngTimeSettingsBox">
        <asp:PlaceHolder ID="_timelngHolder" runat="server"></asp:PlaceHolder>
        <div class="clearFix">
            <a class="float-left button blue" onclick="StudioManagement.SaveLanguageTimeSettings();"
                href="javascript:void(0);">
                <%= Resource.SaveButton %></a>
        </div>
     </div>
</div>
<div class="settings-help-block">
  <p><%= String.Format(Resource.HelpAnswerLngTimeSettings, "<br />", "<b>", "</b>") %></p>
  <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%= Resource.LearnMore %></a>
</div>
</div>

<%-- Promo code --%>
<asp:PlaceHolder ID="promoCodeSettings" runat="server" />

<%--DNS settings--%>
<div id="dnsSettings" class="clearFix <%=EnableDomain ? "" : "disable" %>">
<div class="settings-block">
<asp:PlaceHolder ID="_dnsSettingsHolder" runat="server">
    <div class="header-base clearFix">
        <%= Resource.DnsSettings %>
    </div>

    <div id="studio_enterDnsBox">

        <div class="clearFix">
            <div class="clearFix dns-settings-title">
                <input type="checkbox" id="studio_enableDnsName" onclick="jq('#studio_dnsName').attr('disabled', !this.checked);" <%=EnableDnsChange ? "checked='checked'" : "" %> <%=EnableDomain ? "" : "disabled='disabled'" %> />
                <label for="studio_enableDnsName" onselectstart="return false;" onmousedown="return false;" ondblclick="return false;">
                    <%= Resource.CustomDomainName %>
                </label>
            </div>
            <div class="clearFix">
                <input type="text" id="studio_dnsName" class="textEdit" maxlength="150" value="<%=ASC.Core.CoreContext.TenantManager.GetCurrentTenant().MappedDomain??string.Empty%>" <%=(EnableDnsChange && EnableDomain) ? "" : "disabled='disabled'" %> />
            </div>
        </div>
        <div class="clearFix">
            <a class="float-left button blue" onclick="<%=EnableDomain ? "StudioManagement.SaveDnsSettings();" : "" %>" href="javascript:void(0);">
                <%= Resource.SaveButton %></a>
        </div>
        <p id="dnsChange_sent" class="display-none"></p>
    </div>
</asp:PlaceHolder>
</div>
    <div class="settings-help-block">
        <% if(!EnableDomain) {%>
        <p>
            <%= Resource.ErrorNotAllowedOption %></p>
        <a href="<%= TenantExtra.GetTariffPageLink() %>" target="_blank">
            <%= Resource.ViewTariffPlans %></a>
        <% }else{%>
        <p><%= String.Format(Resource.HelpAnswerDNSSettings, "<br />", "<b>", "</b>") %></p>
        <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%= Resource.LearnMore %></a>
        <% }%>
    </div>
</div>

<%--version settings--%>
<asp:PlaceHolder ID="_portalVersionSettings" runat="server"></asp:PlaceHolder>

<%--trusted mail domain--%>
<asp:PlaceHolder ID="_mailDomainSettings" runat="server"></asp:PlaceHolder>

<%--strong security password--%>
<asp:PlaceHolder ID="_strongPasswordSettings" runat="server"></asp:PlaceHolder>

<%--invitational link--%>
<asp:PlaceHolder ID="invLink" runat="server"></asp:PlaceHolder>

<%--sms settings--%>
<asp:PlaceHolder ID="_smsValidationSettings" runat="server"></asp:PlaceHolder>

<%--admin message settings--%>
<asp:PlaceHolder ID="_admMessSettings" runat="server"></asp:PlaceHolder>

<%--default page settings--%>
<asp:PlaceHolder ID="_defaultPageSeettings" runat="server"></asp:PlaceHolder>

