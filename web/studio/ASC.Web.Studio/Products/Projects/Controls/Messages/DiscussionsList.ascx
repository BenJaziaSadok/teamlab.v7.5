<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionsList.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Messages.DiscussionsList" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>


<div id="filterContainer">
    <div id="ProjectsAdvansedFilter"></div>
</div>  

<div class="simplePageNavigator">
</div>
<div id="discussionsList" data-discussions-count="<%=AllDiscCount %>">
</div>
<table id="tableForNavigation" cellpadding="4" cellspacing="0">
    <tbody>
    <tr>
        <td>
            <div id="divForTaskPager" class="divPager">
            </div>
        </td>
        <td style="text-align:right;">
            <span class="gray-text"><%= ProjectsCommonResource.Total%> : </span>
            <span class="gray-text" style="margin-right: 20px;" id="totalCount"><%=AllDiscCount%></span>
            <span class="gray-text"><%= ProjectsCommonResource.ShowOnPage%> : </span>
            <select id="countOfRows" class="top-align">
                <option value="10">10</option>
                <option value="20">20</option>
                <option value="30">30</option>
                <option value="40">40</option>
            </select> 
        </td>
    </tr>
    </tbody>
</table>

<asp:PlaceHolder runat="server" ID="emptyScreenHolder"></asp:PlaceHolder>

