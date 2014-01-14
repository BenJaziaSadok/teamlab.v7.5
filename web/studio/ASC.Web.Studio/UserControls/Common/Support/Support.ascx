<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Support.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Common.Support.Support" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>

<% if (MakeUserVoice)
   { %>
<script type="text/javascript" src="<%= SetupInfo.UserVoiceURL %>"></script>
<% } %>

<li class="menu-item none-sub-list support add-block">
    <div class="category-wrapper">
        <a class="menu-item-label outer-text text-overflow support-link" <%= MakeUserVoice ? " href=\"javascript:UserVoice.showPopupWidget();\" " : " href=\"http://feedback.teamlab.com\" target=\"_blank\" " %>>
            <span class="menu-item-icon support"></span>
            <span class="menu-item-label inner-text">
                <%= Resources.Resource.FeedbackSupport%>
            </span>
        </a>
    </div>
</li>
