<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StepContainer.ascx.cs" Inherits="ASC.Web.Studio.UserControls.FirstTime.StepContainer" %>
<div class="step">

        <div class="stepBody">
            <asp:PlaceHolder ID="content1" runat="server"></asp:PlaceHolder>
        </div>
        <div class="footer clearFix">
                <div class="btnBox">
                    <a class="button blue big" id="saveSettingsBtn" href="javascript:void(0)" onclick="<%= this.SaveButtonEvent %>">
                        <%= Resources.Resource.SaveButton%></a>
                </div>                
            <div id="wizard_OperationInfo"></div>
        </div>
</div>
