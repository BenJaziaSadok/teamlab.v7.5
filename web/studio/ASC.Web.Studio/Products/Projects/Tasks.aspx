<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Page Title="" Language="C#" MasterPageFile="~/Products/Projects/Masters/BasicTemplate.Master"
    AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="ASC.Web.Projects.Tasks" %>
<%@ MasterType TypeName="ASC.Web.Projects.Masters.BasicTemplate" %>

<asp:Content ID="PageContent" ContentPlaceHolderID="BTPageContent" runat="server">
    <div id="filterContainer">
        <div id="ProjectsAdvansedFilter"></div>
    </div>
    <asp:PlaceHolder runat="server" ID="_filter"></asp:PlaceHolder>
    <asp:PlaceHolder ID="_content" runat="server"></asp:PlaceHolder>
</asp:Content>

<asp:Content ID="projectsClientTemplatesResourcesPlaceHolder" ContentPlaceHolderID="projectsClientTemplatesResourcesPlaceHolder" runat="server">
</asp:Content>