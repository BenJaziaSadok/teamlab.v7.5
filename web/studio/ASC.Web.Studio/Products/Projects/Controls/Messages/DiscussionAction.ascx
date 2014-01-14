<%@ Assembly Name="ASC.Web.Studio" %>
<%@ Assembly Name="ASC.Projects.Core" %>
<%@ Assembly Name="ASC.Web.Projects" %>
<%@ Assembly Name="ASC.Projects.Engine" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DiscussionAction.ascx.cs"
    Inherits="ASC.Web.Projects.Controls.Messages.DiscussionAction" %>
<%@ Import Namespace="ASC.Projects.Core.Domain" %>
<%@ Import Namespace="ASC.Web.Projects.Classes" %>
<%@ Import Namespace="ASC.Web.Projects.Resources" %>
<%@ Register Assembly="FredCK.FCKeditorV2" Namespace="FredCK.FCKeditorV2" TagPrefix="FCKeditorV2" %>

<div id="pageHeader">
    <div class="pageTitle"><%= GetPageTitle() %></div>
    <div style="clear: both"></div>
</div>

<% if (Discussion == null && Project == null) { %>

<div id="discussionProjectContainer" class="requiredField">
    <span class="requiredErrorText">
        <%= MessageResource.SelectProject %></span>
    <div class="headerPanel">
        <%= ProjectResource.Project %>
    </div>
    <select id="discussionProject" class="comboBox">
        <option value="-1">
            <%= ProjectsCommonResource.Select %></option>
    </select>
</div>

<% } %>

<table width="100%">
    <tr>
        <td>
<div id="discussionTitleContainer" class="requiredField">
    <span class="requiredErrorText">
        <%= ProjectsJSResource.EmptyMessageTitle %></span>
    <div class="headerPanelSmall">
        <%= MessageResource.MessageTitle %>
    </div>
    <asp:TextBox ID="discussionTitle" Width="100%" runat="server" CssClass="textEdit"
        MaxLength="250" />
</div>
<div id="discussionTextContainer" class="requiredField" data-fckId="<%= fckEditor.ClientID %>">
    <span class="requiredErrorText">
        <%= MessageResource.EmptyMessageText %></span>
    <div class="headerPanelSmall">
        <%= MessageResource.MessageContent %>
    </div>
    <% if (IsMobile)
       { %>
    <asp:TextBox ID="discussionContent" Width="100%" runat="server" CssClass="nonstretch"
        TextMode="MultiLine" Rows="12"></asp:TextBox>
    <% }
       else
       { %>
    <FCKeditorV2:FCKeditor ID="fckEditor" Width="100%" Height="275px" runat="server">
    </FCKeditorV2:FCKeditor>
    <% } %>
</div>
        </td>
        <% if (!IsMobile) { %>
        <td class="teamlab-cut">
            <div class="title-teamlab-cut"><%= ProjectsCommonResource.TeamlabCutTitle %></div>
            <div class="text-teamlab-cut"><%= String.Format(ProjectsCommonResource.TeamlabCutText, "<span class=\"teamlab-cut-button\"></span>") %></div>
        </td>
        <% } %>
    </tr>
</table>

<div id="discussionTabs">
<%--    <% if (Project != null)
       { %>
        <div>
        <div class="tabs-section" container="discussionFilesContainer">
            
            <span class="header-base"><%= ProjectsCommonResource.DocsModuleTitle %></span>
            <span id="switcherFilesButton" class="toggle-button" data-switcher="0" 
                data-showtext="<%= ProjectsCommonResource.Show %>" data-hidetext="<%= ProjectsCommonResource.Hide %>">
                <%= ProjectsCommonResource.Hide %>
            </span>
        </div>
        </div>
        <div id="discussionFilesContainer" data-projectFolderId = "<%=ProjectFolderId %>" data-projectName="<%=Project.Title %>">
            <asp:PlaceHolder runat="server" ID="discussionFilesPlaceHolder" />
        </div>

    <% } %>--%>
    <div>
        <div class="tabs-section" container="discussionParticipantsContainer">
            <span class="header-base"><%= MessageResource.DiscussionParticipants %></span>
            <span id="switcherParticipantsButton" class="toggle-button" data-switcher="0" 
                data-showtext="<%= ProjectsCommonResource.Show %>" data-hidetext="<%= ProjectsCommonResource.Hide %>">
                <%= ProjectsCommonResource.Hide %>
            </span>
        </div>
    </div>
        <div id="discussionParticipantsContainer" class="participantsContainer" <%if(Project != null) {%> data-private = "<%=Project.Private %>"<%} %>>
        <div class="inviteMessage">
            <%= MessageResource.SubscribePeopleInfoComment %></div>
        <table id="discussionParticipants">
        <%if (Discussion != null){ %>
            <asp:Repeater ID="discussionParticipantRepeater" ItemType="ASC.Projects.Core.Domain.Participant" runat="server">
                
                <ItemTemplate>
                    <tr class="discussionParticipant <%# CanReadDiscussion(Item.ID)? "" : "gray" %>" guid="<%# Item.ID %>">
                       <td class="name">
                            <span>
                                <%# HttpUtility.HtmlEncode(Item.UserInfo.FirstName) + " " + HttpUtility.HtmlEncode(Item.UserInfo.LastName)%>
                            </span>
                        </td>
                        <td class="department">
                            <span><%#HttpUtility.HtmlEncode(Item.UserInfo.Department)%></span>
                        </td>
                         <td class="title">
                            <span><%#HttpUtility.HtmlEncode(Item.UserInfo.Title)%></span>
                        </td>
                        <td class="delMember">
                            <span title="<%=ProjectsCommonResource.Delete %>"></span>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
        <% } %>
        </table>
        <div style="clear: both;">
        </div>
        <span id="addDiscussionParticipantButton" class="manage-participants-button"><span class="dottedLink">
            <%= ProjectsCommonResource.AddParticipants %></span> </span>
         <%if (!RequestContext.IsInConcreteProject){%>
            <a id="addDiscussionParticipantOfProject" class="button blue <% if (Discussion == null && Project == null){%>disable <% } %>"><%= MessageResource.AddProjectsParticipant %></a> 
         <% } %>
        <asp:PlaceHolder ID="discussionParticipantsSelectorHolder" runat="server"></asp:PlaceHolder>
    </div>    

</div>

<div id="discussionButtonsContainer" class="big-button-container">
    <%= GetDiscussionAction()%>
</div>
<div id='discussionActionsInfoContainer' class='pm-ajax-info-block'>
    <span class="text-medium-describe">
        <%= ProjectResource.LoadingWait %></span><br />
    <img src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"
        alt="<%= ProjectResource.ProjectAdded %>" />
</div>
<div id="discussionPreviewContainer">
    <div class="middle-button-container">
        <a id="hideDiscussionPreviewButton" class="button blue">
            <%= ProjectsCommonResource.Collapse %></a>
    </div>
</div>

<div class="popup_helper" id="hintSubscribersPrivateProject">
      <p><%=ProjectsCommonResource.hintSubscribersPrivateProject %></p>
</div>

<script type="text/javascript">
    function FCKConfig_OnLoad(config){
        config.RedirectUrlToUpload("<%=RenderRedirectUpload()%>");
    }
    function FCKeditor_OnComplete(instance) {
        instance.Focus();
        jq('[id$=discussionTitle]').focus();
    }
</script>