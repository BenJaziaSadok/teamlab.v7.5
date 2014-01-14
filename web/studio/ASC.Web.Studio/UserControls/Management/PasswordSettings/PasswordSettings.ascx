<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PasswordSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.PasswordSettings" %>
 <div class="clearFix">
     <div class="settings-block">
     <div class="header-base clearFix" id="mailDomainSettingsTitle">
		    <%=Resources.Resource.StudioPasswordSettings%>
    </div>

    <div id="studio_passwordSettingsInfo">
    </div>

    <div id="studio_passwordSettings">
    <%-- <div style="margin-bottom:10px;">
            <%=Resources.Resource.PasswordDescription%>
        </div>--%>
        
        <div class="clearFix slider">
            <div class="header-base-small">
                <%= Resources.Resource.PasswordMinLength %></div>
            <div class="clearFix passwordLengthBox">
                <div class="sliderPassword">
                    <div id="slider">
                    </div>
                </div>
                <div class="float-left" id="count">
                </div>
                <div class="countLabel float-left">
                <%= Resources.Resource.PasswordSymbolsCountLabel %>
                </div>
            </div>
        </div>
    <div class="clearFix fieldsBox">
        <div class="clearFix">
            <input type="checkbox" id="chkUpperCase" />
            <label for="chkUpperCase"><%= Resources.Resource.PasswordUseUpperCase %></label>
        </div>
        <div class="clearFix">
            <input type="checkbox" id="chkDigits" />
            <label for="chkDigits"><%= Resources.Resource.PasswordUseDigits %></label>
        </div>
        <div class="clearFix">
            <input type="checkbox" id="chkSpecSymbols" />
            <label for="chkSpecSymbols"><%= Resources.Resource.PasswordUseSpecialSymbols %></label>
        </div>
    </div>

    <div class="clearFix">
            <a class="button blue" id="savePasswordSettingsBtn" href="javascript:void(0);">
                <%=Resources.Resource.SaveButton %></a>
        </div>
    </div>
    </div>
    <div class="settings-help-block">
             <p><%=String.Format(Resources.Resource.HelpAnswerPasswordSettings, "<br />","<b>","</b>")%></p>
             <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
    </div>
</div>