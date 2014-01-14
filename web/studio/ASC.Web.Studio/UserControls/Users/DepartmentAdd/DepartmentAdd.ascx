<%@ Assembly Name="ASC.Web.Core" %>

<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentAdd.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Users.DepartmentAdd" %>

<%@ Register TagPrefix="sa" Namespace="ASC.Web.Studio.Controls.Users" Assembly="ASC.Web.Studio" %>
<%@ Register TagPrefix="sc" Namespace="ASC.Web.Studio.Controls.Common" Assembly="ASC.Web.Studio" %>

<div id="studio_departmentAddDialog" class="display-none">
    <sc:Container runat="server" ID="_departmentAddContainer">
        <Header>
        <div id="grouptitle">
            <%= ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("AddDepartmentDlgTitle").HtmlEncode() %>
        </div>
        </Header>
        <Body>
            <asp:HiddenField runat="server" ID="_depProductID" />
            <input type="hidden" id="addDepartment_infoID" value="<%=_departmentAddContainer.ClientID%>_InfoPanel" />
            <div class="clearFix requiredField" >
                <span class="requiredErrorText"><%=Resources.Resource.ErrorEmptyName%></span>
                <div class="headerPanel" style="font-weight: normal; font-size:12px; margin-bottom: 4px;">
                    <%=Resources.Resource.Title%>:
                </div>
                <input type="text" id="studio_newDepName" class="textEdit" style="width: 99%;" maxlength="100" />
            </div>
            <div class="clearFix" style="margin-top: 10px;">
                <div class="headerPanel" style="font-weight: normal; font-size:12px; margin-bottom: 4px;">
                    <%=ASC.Web.Studio.Core.Users.CustomNamingPeople.Substitute<Resources.Resource>("DepartmentMaster").HtmlEncode()%>:
                </div>
                <div style="margin: 3px 0;">
                    <sa:AdvancedUserSelector runat="server" ID="headSelector"></sa:AdvancedUserSelector>                  
                </div>
            </div>
            <div id="depActionBtn" class="middle-button-container">
                <a class="button blue middle" onclick="StudioManagement.AddDepartmentCallback('<%=ProductID %>');">
                    <%=Resources.Resource.AddButton%>
                </a>
                <span class="splitter-buttons"></span>
                <a class="button gray middle" onclick="StudioManagement.CloseAddDepartmentDialog();">
                    <%=Resources.Resource.CancelButton%>
                </a>
            </div>
            <div id="dep_action_loader" class="middle-button-container display-none">
                <div class="text-medium-describe">
                    <%=Resources.Resource.PleaseWaitMessage%>
                </div>
                <img src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("ajax_progress_loader.gif")%>" />
            </div>
        </Body>
    </sc:Container>
</div>