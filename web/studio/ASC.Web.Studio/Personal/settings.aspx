<%@ Page Title="" Language="C#" MasterPageFile="~/Masters/basetemplate.master" AutoEventWireup="true" CodeBehind="settings.aspx.cs" Inherits="ASC.Web.Studio.Personal.settings" %>
<%@ MasterType TypeName="ASC.Web.Studio.Masters.BaseTemplate" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <sc:Container ID="_settingsContainer" runat="server">
        <Header></Header>
        <Body>
            <%--timezone & language--%>
            <div class="header-base borderBase" style="margin-top: 20px; padding-left: 15px; padding-bottom: 5px; border-top: none; border-right: none; border-left: none;">
                <%=Resources.Resource.StudioTimeLanguageSettings%>
            </div>
            <div id="studio_lngTimeSettingsInfo"></div>

            <div id="studio_lngTimeSettingsBox" style="padding: 0 20px 15px 20px;">
                <asp:PlaceHolder ID="_timelngHolder" runat="server"></asp:PlaceHolder>

                <div class="clearFix" style="margin-top: 20px;">
                    <a class="button blue" style="float: left;" onclick="StudioManagement.SaveLanguageTimeSettings();" href="javascript:void(0);">
                        <%=Resources.Resource.SaveButton %>
                    </a>
                </div>
            </div>

            <%--themes--%>
            <asp:PlaceHolder runat="server" ID="_themesHolder"></asp:PlaceHolder>
        </Body>
    </sc:Container>
</asp:Content>
