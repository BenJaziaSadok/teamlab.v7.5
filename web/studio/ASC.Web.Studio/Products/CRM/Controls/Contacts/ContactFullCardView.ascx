<%@ Assembly Name="ASC.Web.CRM" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactFullCardView.ascx.cs"
    Inherits="ASC.Web.CRM.Controls.Contacts.ContactFullCardView" %>
<%@ Assembly Name="ASC.Web.Core" %>
<%@ Assembly Name="ASC.Common" %>
<%@ Assembly Name="ASC.Core.Common" %>
<%@ Import Namespace="ASC.CRM.Core" %>
<%@ Import Namespace="ASC.CRM.Core.Entities" %>
<%@ Import Namespace="ASC.Web.Core.Utility.Skins" %>
<%@ Import Namespace="ASC.Web.CRM" %>
<%@ Import Namespace="ASC.Web.CRM.Classes" %>
<%@ Import Namespace="ASC.Web.CRM.Resources" %>

<% if (TargetContact is Person) %>
<% { %>
    <input type="hidden" name="baseInfo_firstName" value="<%= ((Person)TargetContact).FirstName.HtmlEncode() %>" />
    <input type="hidden" name="baseInfo_lastName" value="<%= ((Person)TargetContact).LastName.HtmlEncode() %>" />
<% } else {%>
    <input type="hidden" name="baseInfo_companyName" value="<%= ((Company)TargetContact).CompanyName.HtmlEncode() %>" />
<% } %>

<div id="contactProfile" class="clearFix">
    <table border="0" cellpadding="0" cellspacing="0" class="contactInfo">
        <tbody>
        <tr>
            <td width="230px">
                <div class="contact-photo">
                    <div class="contact-photo-img<%= TargetContact.IsShared ? " sharedContact" : "" %>">
                        <img class="contact_photo" src="<%= String.Format("{0}?{1}",ContactPhotoManager.GetBigSizePhoto(0, TargetContact is Company), new DateTime().Ticks) %>"
                            data-avatarurl="<%= String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=3", PathProvider.BaseAbsolutePath, TargetContact.ID, TargetContact is Company).ToLower() %>"
                            title="<%= TargetContact.GetTitle().HtmlEncode() %>"
                            alt="<%= TargetContact.GetTitle().HtmlEncode() %>" />
                    </div>
                    <% if(!MobileVer) {%>
                    <div class="under_logo">
                        <a onclick="ASC.CRM.SocialMedia.OpenLoadPhotoWindow(); return false;" class="linkChangePhoto grey-phone">
                             <span class="bold"><%= CRMContactResource.ChangePhoto%></span>
                        </a>
                    </div>
                    <% } %>
                </div>
            </td>
            <td>
                <table border="0" cellpadding="0" cellspacing="0" class="crm-detailsTable" id="contactGeneralList">
                    <colgroup>
                        <col style="width: 50px;"/>
                        <col style="width: 22px;"/>
                        <col/>
                    </colgroup>
                    <tbody>
                    <% if (TargetContact is Person && !String.IsNullOrEmpty(((Person)TargetContact).JobTitle)) %>
                    <% { %>
                        <tr>
                            <td class="describe-text" style="white-space:nowrap;"><%= CRMContactResource.PersonPosition %>:</td>
                            <td></td>
                            <td><%=((Person)TargetContact).JobTitle.HtmlEncode()%></td>
                        </tr>
                    <% } %>

                    <% if (TargetContact is Person && ((Person)TargetContact).CompanyID != 0)%>
                    <% { %>
                        <tr>
                            <td class="describe-text" style="white-space:nowrap;"><%= CRMContactResource.CompanyName%>:</td>
                            <td></td>
                            <td><%= Global.DaoFactory.GetContactDao().GetByID(((Person)TargetContact).CompanyID).RenderLinkForCard()%></td>
                        </tr>
                    <% } %>

                    <% if (!String.IsNullOrEmpty(TargetContact.Industry)) %>
                    <% { %>
                        <tr>
                            <td class="describe-text" style="white-space:nowrap;"><%= CRMContactResource.Industry %>:</td>
                            <td></td>
                            <td><%= TargetContact.Industry.HtmlEncode()%></td>
                        </tr>
                    <% } %>

                    <tr>
                        <td class="describe-text" style="white-space:nowrap;"><%= CRMJSResource.Stage %>:</td>
                        <td></td>
                        <td>
                            <div id="loyaltySliderDetails"></div>
                            <div onclick="jq(this).helper({ BlockHelperID: 'contactStageSlider_helpInfo'});" class="HelpCenterSwitcher"></div>
                            <div class="popup_helper" id="contactStageSlider_helpInfo">
                                <%= String.Format(CRMCommonResource.ContactCategoriesHelpInfo,
                                    "<a class='linkAction' href='settings.aspx?type=contact_stage' target='blank'>",
                                    "</a>") %>
                            </div>
                        </td>
                    </tr>

                    <% if (TargetContact.ContactTypeID != 0) %>
                    <% { %>
                    <tr>
                        <td class="describe-text" style="white-space:nowrap;"><%= CRMContactResource.ContactType %>:</td>
                        <td></td>
                        <td><%= Global.DaoFactory.GetListItemDao().GetByID(TargetContact.ContactTypeID).Title.HtmlEncode()%></td>
                    </tr>
                    <% } %>

                    <tr id="contactTagsTR">
                        <td class="describe-text" style="white-space:nowrap;"><%= CRMCommonResource.Tags %>:</td>
                        <td></td>
                        <td></td>
                    </tr>

                    <tr>
                        <td class="describe-text" style="white-space:nowrap;"><%= CRMContactResource.ContactManager %>:</td>
                        <td></td>
                        <td id="contactManagerList"></td>
                    </tr>

                    <tr class="viewMailingHistory display-none">
                        <td colspan="3">
                            <a class="button gray middle" target="_blank"><%= CRMContactResource.ViewMailingHistory %></a>
                        </td>
                    </tr>

                    </tbody>
                </table>
            </td>
        </tr>
        </tbody>
    </table>

    <% if (!String.IsNullOrEmpty(TargetContact.About)) %>
    <% { %>
    <table border="0" cellpadding="0" cellspacing="0" class="crm-detailsTable">
        <colgroup>
            <col style="width: 50px;"/>
            <col style="width: 22px;"/>
            <col/>
        </colgroup>
        <tbody>
            <tr class="headerToggleBlock">
                <td colspan="3" style="white-space:nowrap;">
                    <span class="headerToggle header-base"><%= CRMContactResource.Overview %></span>
                    <span class="openBlockLink"><%= CRMCommonResource.Show %></span>
                    <span class="closeBlockLink"><%= CRMCommonResource.Hide %></span>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    <%= TargetContact.About.Trim().HtmlEncode().Replace("\n","<br/>") %>
                </td>
            </tr>
        </tbody>
    </table>
    <% } %>

    <table border="0" cellpadding="0" cellspacing="0" class="crm-detailsTable" id="contactAdditionalTable">
        <colgroup>
            <col style="width: 50px;"/>
            <col style="width: 22px;"/>
            <col/>
        </colgroup>
        <tbody>
            <tr class="headerToggleBlock">
                <td colspan="3" style="white-space:nowrap;">
                    <span class="headerToggle header-base"><%= CRMContactResource.AdditionalContactInformation %></span>
                    <span class="openBlockLink"><%= CRMCommonResource.Show %></span>
                    <span class="closeBlockLink"><%= CRMCommonResource.Hide %></span>
                </td>
            </tr>
        </tbody>
    </table>

     <table border="0" cellpadding="0" cellspacing="0" class="crm-detailsTable" id="contactHistoryTable">
        <colgroup>
            <col style="width: 50px;"/>
            <col style="width: 22px;"/>
            <col/>
        </colgroup>
        <tbody>
            <tr class="headerToggleBlock open">
                <td colspan="3" style="white-space:nowrap;">
                    <span class="headerToggle header-base"><%= CRMCommonResource.History %></span>
                    <span class="openBlockLink"><%= CRMCommonResource.Show %></span>
                    <span class="closeBlockLink"><%= CRMCommonResource.Hide %></span>
                </td>
            </tr>
            <tr>
                <td colspan="3" style="padding-top:10px;">
                    <asp:PlaceHolder runat="server" ID="_phHistoryView"></asp:PlaceHolder>
                </td>
            </tr>
        </tbody>
    </table>
</div>
<br />