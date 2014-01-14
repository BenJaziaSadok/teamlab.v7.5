<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserProfileEditControl.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.UserProfile.UserProfileEditControl" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.Studio.Core.SMS" %>
<%@ Import Namespace="Resources" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Studio.Core.Users" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div class="containerBodyBlock">
    <div class="clearFix profile-title header-with-menu">
        <span id="titleEditProfile" class="header text-overflow"><%= GetTitle()%></span>
    </div>
    <div class="profile-action-content clearFix">
        <div class="profile-photo-block">
            <div class="profile-user-photo">
                <div id="userProfilePhoto" class="profile-action-usericon">
                    <img src="<%=GetPhotoPath() %>" />
                </div>
                <div id="loadPhotoImage" class="action-block grey-phone">
                    <span class="bold">
                        <%= ( !IsPageEditProfileFlag ? Resource.AddImage : Resource.EditImage)%>
                    </span>
                </div>
                <%if (IsAdmin()) {%>
                    <div class="profile-role <%= RoleIconName %>" 
                    title="<%= RoleIconName == "admin" ? Resource.Administrator : CustomNamingPeople.Substitute<Resource>("Guest").HtmlEncode() %>">
                    </div>
                <%}%>
            </div>
            <div id="userProfilePhotoError">
            </div>
        </div>
        <table class="profile-action-userdata">
            <%--Type--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=Resource.UserType%>:</td>
                <td class="userdata-value user-type">
                <%if (CanAddUser)
                  {%>
                      <%if (IsVisitor && !CanEditType)
                        {%>
                        <span id="userTypeField" data-type="collaborator" class="link dotline nochange"><%= CustomNamingPeople.Substitute<Resource>("Guest").HtmlEncode()%></span> 
                        <%}
                        else
                        {
                            if (CanEditType)
                            {%> 
                            <select id="userType" class="user-type-selector float-left" <% if(!CanEditType) {%> disabled = 'disabled' <%} %>>
                                 <option class="optionItem" value="collaborator" <%if (IsVisitor) { %> selected="selected"<%} %>><%= CustomNamingPeople.Substitute<Resource>("Guest").HtmlEncode()%></option>
                                 <option class="optionItem" value="user" <%if (!IsVisitor) { %> selected="selected"<%} %>><%=CustomNamingPeople.Substitute<Resource>("User").HtmlEncode()%></option>                                                    
                            </select> 
                        <%}
                            else
                            {%>
                            <span id="userTypeField" data-type="user" class="link dotline nochange"><%= CustomNamingPeople.Substitute<Resource>("User").HtmlEncode() %></span> 
                        <%} %>                 
                    <%}
                  }
                  else
                  {
                      if (IsVisitor || !IsPageEditProfileFlag)
                      {%> 
                      <span id="userTypeField" data-type="collaborator" class="link dotline nochange"><%= CustomNamingPeople.Substitute<Resource>("Guest").HtmlEncode()%></span>
                    <%}
                      else
                      {
                        if (CanEditType)
                            {%> 
                            <select id="userType" class="user-type-selector" <% if(!CanEditType) {%> disabled = 'disabled' <%} %>>
                                 <option class="optionItem" value="collaborator" <%if (IsVisitor) { %> selected="selected"<%} %>><%= CustomNamingPeople.Substitute<Resource>("Guest").HtmlEncode()%></option>
                                 <option class="optionItem" value="user" <%if (!IsVisitor) { %> selected="selected"<%} %>><%= CustomNamingPeople.Substitute<Resource>("User").HtmlEncode() %></option>                                                    
                            </select> 
                        <%}
                            else
                            {%>
                            <span id="userTypeField" data-type="user" class="link dotline nochange"><%= CustomNamingPeople.Substitute<Resource>("User").HtmlEncode() %></span> 
                        <%} %>  
                    <%}
                  }%>
                    <div class="HelpCenterSwitcher" onclick="jq(this).helper({ BlockHelperID: 'AnswerForProjectTeam'});">
                    </div>
                    <div class="popup_helper" id="AnswerForProjectTeam">

                            <div id="collaboratorCanBlock" class="can-description-block <%if (!IsVisitor){%>display-none<% }%>">
                                <%=CustomNamingPeople.Substitute(String.Format(Resource.CollaboratorCanDescribe, "<p>", "</p><ul><li>", "</li><li>", "</li><li>", "</li></ul>"))%>                            </div>
                            <div id="userCanBlock" class="can-description-block <%if (IsVisitor){%>display-none<%} %>">
                                <%=CustomNamingPeople.Substitute(String.Format(Resource.UserCanDescribe, "<p>", "</p><ul><li>", "</li><li>", "</li><li>", "</li><li>", "</li></ul>"))%>
                            </div>
                      
                    </div>
                </td>
            </tr>
             <%--FirstName--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text requiredTitle"><%=Resources.Resource.FirstName%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profileFirstName" class="textEdit" value="<%=GetFirstName() %>" autocomplete="off" />
                    <span class="requiredErrorText"><%=Resource.ErrorEmptyUserFirstName%></span>
                 </td>
            </tr>
            <%--LastName--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text requiredTitle"><%=Resource.LastName%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profileSecondName" class="textEdit" value="<%=GetLastName() %>" autocomplete="off" />
                    <span class="requiredErrorText"><%=Resource.ErrorEmptyUserLastName%></span>
                </td>
            </tr>
            <%--Email--%>  
            <tr class="userdata-field">
                <td class="userdata-title describe-text requiredTitle"><%=Resource.Email%>:</td>
                <td class="userdata-value requiredField">
                    <input type="email" id="profileEmail" value="<%=GetEmail() %>" autocomplete="off" class="textEdit" <%= IsPageEditProfileFlag? "disabled":"" %> />
                    <span class="requiredErrorText"><%=Resource.ErrorNotCorrectEmail %></span>
                </td>
            </tr>
            <%--Department--%>
             <% if (IsAdmin() || Departments.Length != 0)
                          {%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text">
                    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resource>("Department").HtmlEncode()%>:
                </td>
                <td id="departmentsField" class="userdata-value">
                    <% if (IsAdmin())
                          {%>
                    <div class="field-with-actions default">
                        <select class="group-field">
                            <option class="optionItem" value="<%= Guid.Empty %>"><%= UserControlsCommonResource.LblSelect %></option>
                            <%= RenderDepartOptions() %>
                        </select>

                        <a class="delete-field link dotline">
                            <%= Resources.Resource.DeleteButton %></a>
                    </div>
                    <a class="add-new-field link dotline"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("BindDepartmentButton").HtmlEncode() %></a>
                    <% } else
                         {
                           foreach (GroupInfo department in Departments)
                              {%>
                        <div class="field-value"><%=department.Name%></div>
                            <% }%>
                    <% } %>
                        
                </td>
            </tr>
             <% } %>
            <%--Position--%>  
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resource>("UserPost").HtmlEncode()%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profilePosition" class="textEdit" value="<%=GetPosition() %>" autocomplete="off"/>
                    <span class="requiredErrorText"><%=Resource.ErrorMessageLongField64%></span>
                </td>
            </tr>           
            <%--Phone--%>   
            <% if (StudioSmsNotificationSettings.IsVisibleSettings && IsPageEditProfileFlag && !String.IsNullOrEmpty(Phone))
               {%>           
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%= Resources.Resource.MobilePhone%>:</td>
                <td class="userdata-value">
                    <div>+<%= Phone %></div>
                </td>
            </tr>
            <% } %>
            <%--Sex--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=Resources.Resource.Sex%>:</td>
                <td class="userdata-value">
                    <select id="userdataSex" class="comboBox" data-value="<%= IsPageEditProfileFlag ? ProfileGender : "-1" %>">
                        <option class="optionItem" value="-1"><%=UserControlsCommonResource.LblSelect %></option>
                        <option class="optionItem" value="1"><%=Resource.MaleSexStatus%></option>
                        <option class="optionItem" value="0"><%=Resource.FemaleSexStatus%></option>
                    </select>
                </td>
            </tr>
            <%--Registration Date--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resource>("WorkFromDate").HtmlEncode()%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profileRegistrationDate" class="textCalendar textEditCalendar" value="<%=GetWorkFromDate() %>" data-value="<%=GetWorkFromDate() %>" />
                    <span class="requiredErrorText"><%=Resource.ErrorNotCorrectDate%></span>
                </td>
            </tr>
            <%--Birth Date--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=Resource.Birthdate%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profileBirthDate" class="textCalendar textEditCalendar" value="<%=GetBirthDate() %>" data-value="<%=GetBirthDate() %>" />
                    <span class="requiredErrorText"><%=Resource.ErrorNotCorrectDate%></span>
                </td>
            </tr>
             <%--Location--%>
            <tr class="userdata-field">
                <td class="userdata-title describe-text"><%=Resource.Location%>:</td>
                <td class="userdata-value requiredField">
                    <input type="text" id="profilePlace" class="textEdit" value="<%=GetPlace() %>" autocomplete="off" />
                    <span class="requiredErrorText"><%=Resource.ErrorMessageLongField255%></span>
                </td>
            </tr>
        </table>            
    </div>
    <%--Comment--%>
    <div id="commentTab" class="tabs-section">
        <span class="header-base"><%=Resource.Comments%></span>
        <span id="switcherCommentButton" class="toggle-button" data-switcher="0" 
            data-showtext="<%=Resource.Show%>" data-hidetext="<%=Resource.Hide%>">
            <%=Resource.Hide%>
        </span>
    </div>
    <div id="commentContainer" class="tabs-content">
        <textarea id="profileComment" class="textEdit" rows="4"><%=GetComment() %></textarea>
    </div>
    <%--Contacts--%>
    <div id="contactInfoTab" class="tabs-section">
        <span class="header-base"><%=Resource.ContactInformation%></span>
        <span id="switcherContactInfoButton" class="toggle-button" data-switcher="0" 
            data-showtext="<%=Resource.Show%>" data-hidetext="<%=Resource.Hide%>">
            <%=Resources.Resource.Hide%>
        </span>
    </div>
    <div id="contactInfoContainer" class="tabs-content contacts-group">
        
        <div class="field-with-actions default">
            <select class="group-field">
                <option class="optionItem mail" value="mail" selected="selected"><%= Resource.TitleEmail %></option>
                <option class="optionItem phone" value="phone"><%= Resource.TitlePhone %></option>
                <option class="optionItem mobphone" value="mobphone"><%= Resource.TitleMobphone %></option>
                <option class="optionItem gmail" value="gmail"><%= Resource.TitleGooglemail %></option>
                <option class="optionItem skype" value="skype"><%= Resource.TitleSkype %></option>
                <option class="optionItem msn" value="msn"><%= Resource.TitleMsn %></option>
                <option class="optionItem icq" value="icq"><%= Resource.TitleIcq %></option>
                <option class="optionItem jabber" value="jabber"><%= Resource.TitleJabber %></option>
                <option class="optionItem aim" value="aim"><%= Resource.TitleAim %></option>
            </select>
            <a class="delete-field link dotline">
                <%= Resource.DeleteButton %></a>
            <input type="text" class="textEdit" value="" autocomplete="off" />
        </div>
        <a class="add-new-field link dotline">
            <%=Resource.BtnAddNewContact%></a>
    </div>
    <%--Social Nets--%>
    <div id="SocialTab" class="tabs-section">
        <span class="header-base"><%=Resource.SocialProfiles%></span>
        <span id="switcherSocialButton" class="toggle-button" data-switcher="0" 
            data-showtext="<%=Resource.Show%>" data-hidetext="<%=Resource.Hide%>">
            <%=Resource.Hide%>
        </span>
    </div>
    <div id="socialContainer" class="tabs-content contacts-group">
       <div class="field-with-actions default">
           <select class="group-field">
                <option class="optionItem facebook" value="facebook"><%= Resources.Resource.TitleFacebook %></option>
                <option class="optionItem livejournal" value="livejournal"><%= Resources.Resource.TitleLiveJournal %></option>
                <option class="optionItem myspace" value="myspace"><%= Resources.Resource.TitleMyspace %></option>
                <option class="optionItem twitter" value="twitter"><%= Resources.Resource.TitleTwitter %></option>
                <option class="optionItem blogger" value="blogger"><%= Resources.Resource.TitleBlogger %></option>
                <option class="optionItem yahoo" value="yahoo"><%= Resources.Resource.TitleYahoo %></option>
           </select>
            <a class="delete-field link dotline">
                <%= Resource.DeleteButton %></a>
            <input type="text" class="textEdit" value="" placeholder="<%=Resources.Resource.HintForSocialAccounts %>" autocomplete="off" />
        </div>
        <a class="add-new-field link dotline">
            <%= UserControlsCommonResource.AddNewSocialProfile%></a>
    </div>
    <div id="profileActionsContainer" class="big-button-container">
        <a id="profileActionButton" class="button blue big"><%=GetTextButton()%></a>
        <span class="splitter-buttons"></span>
        <a id="cancelProfileAction" class="button gray big"><%=UserControlsCommonResource.CancelButton%></a>
        <span id="serverError"></span>
    </div>
    
    <div id="profileActionsInfoContainer" class="pm-ajax-info-block">
        <span class="text-medium-describe">
            <%= Resource.PleaseWaitMessage %></span><br />
        <img src="<%= ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif") %>"
            alt="<%= Resource.PleaseWaitMessage %>" />
    </div>
</div>
<asp:PlaceHolder ID="loadPhotoWindow" runat="server"></asp:PlaceHolder>
