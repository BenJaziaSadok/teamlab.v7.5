<%@ Assembly Name="ASC.Web.Mail" %>
<%@ Control Language="C#" AutoEventWireup="True" CodeBehind="TagBox.ascx.cs" Inherits="ASC.Web.Mail.Controls.TagBox" %>
<%@ Import Namespace="ASC.Web.Mail.Resources" %>

<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="tags_panel" class="expandable top-margin-menu left-margin hidden">
  <div class="content">
    <div id="id_tags_panel_content">
    </div>
  </div>
  <div class="more hidden">
    <div class="shadow">
    </div>
    <div class="text">
      <%=MailResource.ShowMoreTags%>
    </div>
  </div>
</div>


<div id="tagWnd" style="display: none"   savetag="<%=MailResource.TagEdit%>"
                                         newtag="<%=MailResource.NewTag%>"
                                         deletetag="<%=MailResource.DeleteTag%>">
    <sc:Container ID="tagFieldPopup" runat="server">
        <header>
        </header>
        <body>
            <div id="mail_CreateTag_Name" class="save requiredField">
                <span class="requiredErrorText required-hint"><%= MailScriptResource.ErrorEmptyField %></span>
                <table style="display:inline-table;">
                    <tr>
                        <td>
                            <div class="tag color save" style="margin: 0;">
                                <div class="outer">
                                    <div class="inner tag1" colorstyle="1" />
                                </div>
                            </div>
                        </td>
                        <td>
                            <input id="mail_tag_name" type="text" class="textEdit" maxlength="20" placeholder="<%=MailResource.TagNamePlaceholder%>" style="display:inline-block; width: 247px">
                        </td>
                    </tr>
                </table>
            </div>
            <div class="linked_addresses save">
                <span class="markAllLabel"><%=MailResource.MarkFrom%>:</span>
                <table id="mail_EmailsContainer"></table>
                <div id="mail_CreateTag_Email" class="requiredField">
                   <span class="requiredErrorText required-hint" style="padding-bottom: 6px;"><%= MailScriptResource.ErrorEmptyField %></span>
                   <input id="mail_tag_email" type="email" value="" class="textEdit addemail" placeholder="<%=MailResource.AddEmailPlaceholder%>"/>
                   <a class="plus plusmail" title="<%=MailResource.AddEmailAddressHint%>"></a>
                </div>
                <div class="addemail_error" style="width:275px;"></div>
                <div class="clearFix"></div>
                <div class="progressContainer" style="padding-top: 7px;">
                    <div class="loader" style="display: none;"></div>
                </div>
            </div>
            <div class="del">
                <p><%=MailResource.DeleteTagAttention%></p>
                <p id="deleteTagShure"></p>
            </div>
            <div class="buttons new-tag">
                <button class="button blue save" type="button"><%=MailResource.SaveBtnLabel%></button>
                <button class="button blue del" type="button"><%=MailResource.DeleteBtnLabel%></button>
                <button class="button gray cancel" type="button"><%=MailScriptResource.CancelBtnLabel%></button>
            </div>
        </body>
    </sc:Container>
</div>

<div id="addTagsPanel" class="actionPanel stick-over">
    <div class="popup-corner"></div>
    <div id="tagsPanelContent" style="display: block;">
        <div class="actionPanelSection">
            <label for="markallrecipients" class="mark_all_checkbox">
                <input type="checkbox" id="markallrecipients"/>
                <span  id="markallrecipientsLabel"><%=MailScriptResource.MarkAllSendersLabel%></span>
            </label>
        </div>
        <div class="existsTags">
        </div>
        <div class="h_line"></div>
        <div class="actionPanelSection">
            <div><%=MailResource.CreateNewTagLabel%></div>
            <div class="entertagname">
                <a title="Select color" class="square"></a>
                <input type="text" class="tagname" placeholder="<%=MailResource.TagNamePlaceholder%>" maxlength="20" id="createnewtag"/>
            </div>
            <input type="button" class="entertag_button button blue" value="<%=MailScriptResource.OkBtnLabel%>"/>
        </div>

        <div id="error_message" class="addemail_error"></div>
    </div>
</div>