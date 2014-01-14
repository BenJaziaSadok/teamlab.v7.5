<%@ Page MasterPageFile="~/Masters/basetemplate.master" Language="C#" AutoEventWireup="true" EnableViewState="false" CodeBehind="Management.aspx.cs" Inherits="ASC.Web.Studio.Management" Title="TeamLab" %>
<%@ MasterType TypeName="ASC.Web.Studio.Masters.BaseTemplate" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<asp:Content ContentPlaceHolderID="PageContent" runat="server">
    <sc:Container ID="_settingsContainer" runat="server">
        <Header>
        </Header>
        <Body>
        </Body>
    </sc:Container>
</asp:Content>

<asp:Content ContentPlaceHolderID="SidePanel" runat="server">
    <asp:PlaceHolder runat="server" ID="SideNavigation" />
</asp:Content>
