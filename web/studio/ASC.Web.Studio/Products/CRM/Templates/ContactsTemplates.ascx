<%@ Control Language="C#" AutoEventWireup="false" EnableViewState="false" %>
<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Configuration" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<%--Contacts List--%>

<script id="contactListTmpl" type="text/x-jquery-tmpl">
    <tbody>
        {{tmpl(contacts) "contactTmpl"}}
    </tbody>
</script>

<script id="contactTmpl" type="text/x-jquery-tmpl">
    <tr id="contactItem_${id}" class="with-entity-menu">
        <td class="borderBase" style="padding-left: 6px;">
            <input type="checkbox" id="check_contact_${id}" onclick="ASC.CRM.ListContactView.selectItem(this);" style="margin-left: 2px;" {{if isChecked == true}}checked="checked"{{/if}} />
            <img id="loaderImg_${id}" style="display:none;" src="<%=WebImageSupplier.GetAbsoluteWebPath("ajax_loader_small.gif", ProductEntryPoint.ID)%>" alt=""/>
        </td>


        <td class="borderBase">
            <div class="contactItemPhotoImgContainer{{if isShared === true}} sharedContact{{/if}}">
                {{if isCompany == true}}
                <img class="contactItemPhotoImg" src="<%=ContactPhotoManager.GetSmallSizePhoto(0, true) %>" alt="${displayName}" title="${displayName}" onload="ASC.CRM.Common.loadContactFoto(jq(this), jq(this).next(), '${smallFotoUrl}');" />
                {{else}}
                <img class="contactItemPhotoImg" src="<%=ContactPhotoManager.GetSmallSizePhoto(0, false) %>" alt="${displayName}" title="${displayName}" onload="ASC.CRM.Common.loadContactFoto(jq(this), jq(this).next(), '${smallFotoUrl}');" />
                {{/if}}
                <img class="contactItemPhotoImg" style="display:none;" alt="${displayName}" title="${displayName}"/>
            </div>
        </td>

        <td class="borderBase">
            <div class="contactTitle">
                <a class="linkHeaderMedium" href="default.aspx?id=${id}">
                    ${displayName}
                </a>
            </div>
            {{if isCompany == false && company != null}}
                <div class="contactTitle">
                    <%=CRMContactResource.Company%>:
                    <a href="default.aspx?id=${company.id}" data-id="${company.id}" id="contact_${id}_company_${company.id}" class="linkMedium crm-companyInfoCardLink">
                        ${company.displayName}
                    </a>
                </div>
            {{/if}}
        </td>

        <td class="borderBase">
            <div class="primaryDataContainer">
                <input type="text" id="addPrimaryPhone_${id}" class="textEdit addPrimaryDataInput" autocomplete="off" maxlength="100"/>
            {{if primaryPhone != null}}
                <span class="primaryPhone" title="${primaryPhone.data}">${primaryPhone.data}</span>
            {{/if}}
            </div>
        </td>

        <td class="borderBase">
            <div class="primaryDataContainer">
                <input type="text" id="addPrimaryEmail_${id}" class="textEdit addPrimaryDataInput" autocomplete="off" maxlength="100"/>
            {{if primaryEmail != null}}
                <a class="primaryEmail linkMedium" title="${primaryEmail.data}" href="${primaryEmail.emailHref}" target="_blank">
                    ${primaryEmail.data}
                </a>
            {{/if}}
            </div>
        </td>

        <td class="borderBase">
            {{if nearTask != null}}
                <span id="taskTitle_${nearTask.id}" class="header-base-small nearestTask"
                    ttl_label="<%=CRMCommonResource.Title%>" ttl_value="${nearTask.title}"
                    dscr_label="<%=CRMCommonResource.Description%>" dscr_value="${nearTask.description}"
                    resp_label="<%=CRMCommonResource.Responsible%>" resp_value="${nearTask.responsible.displayName}">
                        ${nearTask.category.title} ${nearTask.deadLineString}
                </span>
            {{/if}}
        </td>
        <td class="borderBase">
            <div id="contactMenu_${id}" class="entity-menu" title="<%= CRMCommonResource.Actions %>"></div>
        </td>
    </tr>
</script>


<%--ContactActionView and ContactDetailsView: social networks --%>

<script id="FacebookProfileTmpl" type="text/x-jquery-tmpl">
    <tr>
        <td class="sm_tbl_UserList_clmnBtRelate">
            <a class="button gray plus" onclick="ASC.CRM.SocialMedia.AddFacebookProfileToContact('${UserID}'); return false;"><%=CRMCommonResource.Add%></a>
        </td>
        <td class="sm_tbl_UserList_clmnAvatar">
            <div style="min-height: 40px;">
                <img src="${SmallImageUrl}" alt="${UserName}" width="40"/>
            </div>
        </td>
        <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
            <span class="header-base-small sn_userName" style="color: Black !important;">${UserName}</span>
        </td>
    </tr>
</script>

<script id="LinkedInProfileTmpl" type="text/x-jquery-tmpl">
    <tr>
        <td class="sm_tbl_UserList_clmnBtRelate">
            <a class="button gray plus" onclick="ASC.CRM.SocialMedia.AddLinkedInProfileToContact('${UserID}', '${CompanyName}', '${Position}', '${PublicProfileUrl}', '${UserName}'); return false;"><%=CRMCommonResource.Add%></a>
        </td>
        <td class="sm_tbl_UserList_clmnAvatar">
            <div style="min-height: 40px;">
                <img src="${ImageUrl}" alt="${UserName}" width="40"/>
            </div>
        </td>
        <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
            <span class="header-base-small sn_userName" style="color: Black !important;">${UserName}</span>
        </td>
    </tr>
</script>

<script id="TwitterProfileTmpl" type="text/x-jquery-tmpl">
    <tr>
        <td class="sm_tbl_UserList_clmnBtRelate">
            <a class="button gray plus"
                onclick="ASC.CRM.SocialMedia.AddTwitterProfileToContact('${ScreenName}'); return false;">
                <%= CRMCommonResource.Add %>
            </a>
        </td>
        <td class="sm_tbl_UserList_clmnAvatar">
            <div style="min-height: 40px;">
                <img src="${SmallImageUrl}" alt="${UserName}" width="40"/>
            </div>
        </td>
        <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
            <span class="header-base-small sn_userName" style="color: Black !important;">${UserName}</span>
            <br />
            ${Description}
        </td>
    </tr>
</script>

<script id="TwitterProfileTabTmpl" type="text/x-jquery-tmpl">
    <tr>
        <td class="sm_tbl_UserList_clmnBtRelate">
            <a class="button gray plus"
                onclick="ASC.CRM.SocialMedia.AddAndSaveTwitterProfileToContact('${ScreenName}', jq.getURLParam('id')); return false;">
                <%= CRMCommonResource.Choose %>
            </a>
        </td>
        <td class="sm_tbl_UserList_clmnAvatar">
            <div style="min-height: 40px;">
                <img src="${SmallImageUrl}" width="40" alt=""/>
            </div>
        </td>
        <td class="sm_tbl_UserList_clmnUserName" style="padding:5px;">
            <span class="header-base-small sn_userName" style="color: Black !important;">${UserName}</span>
            <br />
            ${Description}
        </td>
    </tr>
</script>

<script id="socialMediaAvatarTmpl" type="text/x-jquery-tmpl">
    <div class="ImageHolderOuter" onclick="ASC.CRM.SocialMedia.UploadUserAvatar(event,'${SocialNetwork}','${Identity}');">
        <img src="${ImageUrl}" alt="Avatar" class="AvatarImage" />
    </div>
</script>


<script id="personTmpl" type="text/x-jquery-tmpl">
    <div>
        <input type="checkbox" id="${person.permalink}" checked="checked"/> ${person.first_name} ${person.last_name}
    </div>
</script>

<script id="crunchbaseContactFullTmpl" type="text/x-jquery-tmpl">
<div style="height:24px; vertical-align:middle;">
    {{if namespace == "company"}}
        <a class="crunhbaseLink" href="${crunchbase_url}" target="_blank">${name}</a><br>
    {{else}}
        <a class="crunhbaseLink" href="${crunchbase_url}" target="_blank">${first_name} ${last_name}</a><br>
    {{/if}}
</div>
<table id="tblCompanyFields">
    {{if homepage_url }}
        <tr>
            <td style="width:120px;" class="describe-text"><input type="checkbox" id="chbWebsite" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Website.ToLocalizedString() %>:</td>
            <td><a href="${homepage_url}" target="_blank">${homepage_url}</a></td>
        </tr>
    {{/if}}
    {{if email_address }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbEmail" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Email.ToLocalizedString() %>:</td>
            <td><a href="mailto:${email_address}">${email_address}</a></td>
        </tr>
    {{/if}}
    {{if phone_number }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbPhoneNumber" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Phone.ToLocalizedString() %>:</td>
            <td>${phone_number}</td>
        </tr>
    {{/if}}
    {{if overview }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbDescription" checked="checked"/>
                <%= CRMContactResource.Overview %>:</td>
            <td>${jq(overview).text().substring(0,60)}...</td>
        </tr>
    {{/if}}
    {{if twitter_username }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbTwitter" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Twitter.ToLocalizedString() %>:</td>
            <td><a href="http://twitter.com/#!/${twitter_username}">http://twitter.com/#!/${twitter_username}</a></td>
        </tr>
    {{/if}}
    {{if blog_url }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbBlog" checked="checked"/>
                <%= ASC.CRM.Core.ContactInfoType.Blogger.ToLocalizedString() %>:</td>
            <td><a href="${blog_url}">${blog_url}</a></td>
        </tr>
    {{/if}}
    {{if blog_feed_url }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbBlogFeed" checked="checked"/>
                <%= CRMSocialMediaResource.BlogFeed %>:</td>
            <td><a href="${blog_feed_url}">${blog_feed_url}</a></td>
        </tr>
    {{/if}}
    {{if tag_list && false}}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbTag" checked="checked"/>
                <%= CRMContactResource.Tags %>:</td>
            <td>${tag_list}</td>
        </tr>
    {{/if}}
    {{if image }}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbImage" checked="checked"/>
                <%= CRMSocialMediaResource.Image %>:</td>
            <td><img height="25" src="http://www.crunchbase.com/${image.available_sizes[0][1]}"></td>
        </tr>
    {{/if}}
    {{if namespace == "company" && relationships && relationships.length != 0}}
        <tr>
            <td class="describe-text"><input type="checkbox" id="chbRelationship" checked="checked"
                onclick="ASC.CRM.SocialMedia.switchCheckedPersonsInCompany(jq(this).is(':checked'));"/>
                <%= CRMContactResource.Persons %> (${relationships.length}):</td>
            <td id="chbPersonsRelationship">
                <div style="max-height: 100px; overflow: auto;" class="borderBase">
                    {{each relationships}}
                        {{tmpl($value) "personTmpl"}}
                    {{/each}}
                </div>
                <div class="text-medium-describe">
                    *<%= CRMContactResource.CrunchBaseWatermarkText %>
                </div>
            </td>
        </tr>
    {{/if}}
</table>

<input id="crbsWebSite" type="hidden" value="${homepage_url}" />
<input id="crbsEmail" type="hidden" value="${email_address}" />
<input id="crbsPhoneNumber" type="hidden" value="${phone_number}">
<input id="crbsOverview" type="hidden" value="${jq(overview).text()}" />
<input id="crbsTwitterUserName" type="hidden" value="${twitter_username}" />
<input id="crbsBlogUrl" type="hidden" value="${blog_url}" />
<input id="crbsBlogFeedUrl" type="hidden" value="${blog_feed_url}" />
<input id="crbsTagList" type="hidden" value="${tag_list}" />
<input id="crbsPeopleJSON" type="hidden" value="${jq.toJSON(relationships)}" />
<input id="crbsImageJSON" type="hidden" value="${jq.toJSON(image)}" />

</script>


<%--ContactFullCardView: contactInfo list --%>

<script id="collectionContainerTmpl" type="text/x-jquery-tmpl">
    <tr>
        <td class="describe-text" style="white-space:nowrap;">${Type}:</td>
        <td></td>
        <td class="collectionItemsTD"></td>
    </tr>
</script>

<script id="collectionTmpl" type="text/x-jquery-tmpl">
    {{if infoType == 0 }}
        <div class="collectionItem">
            {{if isPrimary == true}}
            <a href="callto:${data}?call" class="linkMedium">${data}</a>
            {{else}}
            <span>${data}</span>
            {{/if}}
            <span class="text-medium-describe"> (${categoryName})</span>
        </div>

    {{else infoType == 1}}
        <div class="collectionItem">
            <a href="mailto:${data}" class="linkMedium">${data}</a>
            <span class="text-medium-describe"> (${categoryName})</span>
            {{if isPrimary == true}}<a class="linkDescribe baseLinkAction writeEmail" target="_blank" data-email="${data}"><%= CRMContactResource.WriteEmail %></a>{{/if}}
        </div>

    {{else infoType == 3 || infoType == 15 || infoType == 16 }}
        <div class="collectionItem">
            <span>${data}</span>
            <span class="text-medium-describe"> (${categoryName})</span>
        </div>

    {{else infoType == 10  || infoType == 12 || infoType == 13}}
        <div class="collectionItem">
            <a href="mailto:${data}" class="linkMedium">${data}</a>
            <span class="text-medium-describe"> (${categoryName})</span>
        </div>
    {{else infoType == 2 || infoType == 4 || infoType == 5 || infoType == 6 ||infoType == 8 ||infoType == 9 || infoType == 11 || infoType == 14}}
        <div class="collectionItem">
            <a href="${href}" target="_blank" class="linkMedium">${data}</a>
            <span class="text-medium-describe"> (${categoryName})</span>
        </div>

    {{else infoType == 7}}
        <div class="collectionItem">
            {{html data}}
            <span class="text-medium-describe"> (${categoryName})</span><br/>
            <a style="text-decoration: underline;" href="${href}" target="_blank" class="linkMedium">
                <%= CRMContactResource.ShowOnMap %>
            </a>
        </div>
    {{/if}}
</script>

<%--ContactDetailsView: projects tab --%>

<script id="projectSelectorOptionTmpl" type="text/x-jquery-tmpl">
    <option value="${id}">${title}</option>
</script>

<script id="projectSelectorItemTmpl" type="text/x-jquery-tmpl">
   <li data-id="${id}"><div class="dropdown-item">${title}</div></li>
</script>

<%--ContactDetailsView: deals tab --%>

<script id="dealSelectorOptionTmpl" type="text/x-jquery-tmpl">
    <option value="${id}">${title}</option>
</script>

<script id="dealSelectorItemTmpl" type="text/x-jquery-tmpl">
   <li data-id="${id}"><div class="dropdown-item">${title}</div></li>
</script>

<%--ContactDetailsView: merge --%>

<script id="listContactsToMergeTmpl" type="text/x-jquery-tmpl">
    {{each contacts}}
    <li>
        <input type="radio" name="contactToMerge" value="${id}" />
        <span>${displayName}</span>
    </li>
    {{/each}}
    <li>
        <input type="radio" name="contactToMerge" value="0"{{if count == 0}} style="display: none;"{{/if}} />
        <div class="contactToMergeSelectorContainer"{{if count > 0}} style="margin-left: 0;"{{/if}}>
        </div>
        <input type="hidden" name="selectedContactToMergeID" value="0" />
    </li>
</script>