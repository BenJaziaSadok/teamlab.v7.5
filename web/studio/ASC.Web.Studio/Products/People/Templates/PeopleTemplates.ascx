<%@ Assembly Name="ASC.Web.People" %>
<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Import Namespace="ASC.Core" %>
<%@ Import Namespace="ASC.Core.Users" %>
<%@ Import Namespace="ASC.Web.People.Resources" %>


<script id="userListTemplate" type="text/x-jquery-tmpl">
{{each(i, user) users}}
  <tr id="user_${user.id}" class="item profile {{if ($data.isAdmin === true || user.isMe === true) && (user.isMe === true || user.isPortalOwner !== true)}} with-entity-menu {{/if}} {{if user.isTerminated}} blocked{{else user.isActivated === false}} waited{{/if}} {{if user.isChecked == true}} selectedRow{{/if}}"
        data-id="${user.id}"
        data-username="${user.userName}"
        data-email="${user.email}"
        data-displayname="${user.displayName}"
        data-status="{{if user.isTerminated}}blocked{{else user.isActivated === false}}waited{{/if}}"
        data-isAdmin="${user.isAdmin}" >
    {{if ($data.isAdmin === true)}}
    <td class="check-list" id="checkRow_${user.id}">
        <input type="checkbox" id="checkUser_${user.id}" class="checkbox-user"
            {{if user.isChecked == true}}checked="checked"{{/if}} />
    </td>
    {{/if}}
    <td class="icon-list">
      <a class="ava" href="${user.link}">
        <img src="${user.avatarSmall}" alt="${user.displayName}" /> 
        {{if ($data.isAdmin === true)}}          
            {{if (user.isVisitor)}}<span class="role collaborator" title="<%= ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Guest").HtmlEncode() %>"></span>{{/if}}
            {{if (user.isAdmin || listAdminModules.length)}}<span class="role admin" title="<%= Resources.Resource.Administrator %>"></span>{{/if}}      
        {{/if}} 
        {{if (user.isTerminated || user.isActivated === false)}}<span class="status"></span> {{/if}}
      </a>
    </td>
    <td class="name">
        <a class="link bold" href="${user.link}">${user.displayName}</a>
      {{if user.bithdayDaysLeft != null}}
        <div class="birthday">
        {{if user.bithdayDaysLeft == 0}}<%= Resources.Resource.DrnToday %>
        {{else user.bithdayDaysLeft==1}}<%= Resources.Resource.DrnTomorrow %>
        {{else user.bithdayDaysLeft==2}}<%= Resources.Resource.In %> <% = DateTimeExtension.Yet(2) %>
        {{else user.bithdayDaysLeft==3}}<%= Resources.Resource.In %> <% = DateTimeExtension.Yet(3) %>
        {{/if}}
        </div>
      {{/if}}
        <div class="title">
            <span class="inner-text">${user.title}</span>
        </div>
    </td>
    <td class="group">
        {{if user.groups.length === 1 && user.group != null}}
        <a class="link text-overflow" title="${user.group.name}" href="#group=${user.group.id}">
            ${user.group.name}
        </a>
        {{else user.groups.length > 1}}
            <span id="peopleGroupsSwitcher_${user.id}" class="withHoverArrowDown">
                <span class="link dotted">${user.groups.length} <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("Departments").HtmlEncode()%></span>
            </span>
            <div id="peopleGroups_${user.id}" class="studio-action-panel">
                <div class="corner-top left"></div>
                <ul class="dropdown-content">
                    {{each(i, gr) user.groups}}
                    <li><a class="dropdown-item" href="#group=${gr.id}">${gr.name}</a></li>
                    {{/each}}
                </ul>
            </div>
        {{/if}}
    </td>
    <td class="location">
        <span class="inner-text">${user.location}</span>
    </td>
    <td class="info">
    {{if user.isTerminated}}
        <span class="red-text">
           <%= PeopleResource.BlockedMessage %>
        </span>
     {{else}}
          {{if user.isActivated === false && user.email}}
            <a class="link underlined email gray-text" target="_blank" href="<%=VirtualPathUtility.ToAbsolute("~/addons/mail/#composeto/email=${user.email}")%>">
               <%= PeopleResource.WaitingForConfirmation %>
            </a>
          {{/if}}
       {{/if}}
      {{if user.isActivated && !user.isTerminated && user.email}}
        <span class="btn email">
            <span class="link dotted">${user.email}</span>
        </span>
        <button class="ui-btn send-email"><span class="inner-text"><%= PeopleResource.LblSendEmail %></span></button>
        {{if !user.isMe && !user.isTerminated}}
          <button class="ui-btn open-dialog"><span class="inner-text"><%= PeopleResource.LblSendMessage %></span></button>
        {{/if}}
      {{else}}
        &nbsp;
      {{/if}}
    </td>
    <td class="menu-list">
        {{if ($data.isAdmin === true || user.isMe === true) && (user.isMe === true || user.isPortalOwner !== true)}}
        <div id="peopleMenu_${id}" class="entity-menu" title="<%= PeopleResource.Actions %>"></div>
        {{/if}}
    </td>
  </tr>
{{/each}}
</script>
