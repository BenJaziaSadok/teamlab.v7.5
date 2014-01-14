<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.MailSettings" %>

<div id="studio_setInfSmtpSettingsInfo"></div>

<div id="studio_smtpSettingsBox" class="clearFix">
    <div class="smtpModeBox clearFix">
        <div class="float-left">
			<div class="clearFix">
				<input id="studio_corporateSMTPButton" name="studio_settingsSMTPSwitch" <%=!_isPesonalSMTP?"checked=\"checked\"":"" %> type="radio" />                
				<label for="studio_corporateSMTPButton" class="header-base-small"><%=Resources.Resource.CorporateSMTP %></label>
            </div>
        </div>
        
        <div class="personalSMTPButton">
			<div class="clearFix">
				<input id="studio_personalSMTPButton" name="studio_settingsSMTPSwitch" <%=_isPesonalSMTP?"checked=\"checked\"":"" %> type="radio" />                
				<label for="studio_personalSMTPButton" class="header-base-small"><%=Resources.Resource.PersonalSMTP %></label>
            </div>
        </div>
        
        <div class="float-right"><a target="_blank" class="linkDescribe" href="<%=ASC.Web.Studio.UserControls.Management.StudioSettings.ModifyHowToAdress("howto.aspx#Admin_ConfigureMailSettings")%>"><%=Resources.Resource.HelpCenter%></a></div>
        
    </div>
  
    <%--Host and Port--%>
    <div class="hostPortSecureBox clearFix">
        <div class="hostHeader header-base-small">
            <%=Resources.Resource.Host%>:                 
        </div>
         <div class="float-left">
            <input class="textEdit inputHost" type="text" id="studio_smtpAddress"
                value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.Host??""%>" />
        </div>
        <div class="portHeader header-base-small">
            <%=Resources.Resource.Port%>:
       </div>
       <div class="float-left">
            <input id="studio_smtpPort" class="textEdit inputPort" type="text" value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.Port??25%>" />
       </div>
       <%--Secure section--%>
       <div class="secureHeader header-base-small">
            <%=Resources.Resource.EnableSSL%>:
       </div>
       <div class="float-left">
            <input type="checkbox" id="studio_smtpEnableSSL" <%=ASC.Core.CoreContext.Configuration.SmtpSettings.EnableSSL ? "checked='checked'": ""%> />
       </div>
    </div>
    
    <%--SenderAddress--%>
    <div class="senderAddressBox clearFix">
        <div class="senderAddressHeader header-base-small">
            <%=Resources.Resource.SenderAddress%>:                
        </div>
        <div class="float-left">
            <input class="textEdit inputSenderAddress" type="text" id="studio_smtpSenderEmail"
                value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.SenderAddress??""%>" />
        </div>
    </div>
    
    <%--SenderDisplayName--%>
    <div class="senderNameBox clearFix">   
     <div class="header-base-small senderNameHeader">            
            <%=Resources.Resource.SenderDisplayName%>:
        </div>
        <div class="float-left">
            <input class="textEdit inputSenderNameBox" type="text" id="studio_smtpSenderDisplayName"
                value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.SenderDisplayName??""%>" />
        </div>
    </div>
    
    <%--personal smtp--%>
    <div id="studio_smtp_personal" style="<%=_isPesonalSMTP?"":"display:none;"%>">
    
        <%--CredentialsDomain--%>
        <div class="clearFix credentialsDomainBox">                  
         <div class="header-base-small credentialsDomainHeader">                
                <%=Resources.Resource.CredentialsDomain%>:
          </div>
            <div class="float-left">
                <input class="textEdit inputCredentialsDomain" type="text" id="studio_smtpCredentialsDomain"
                    value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.CredentialsDomain??""%>" />
            </div>
        </div>
        
        <%--CredentialsUserName--%>
        <div class="clearFix credentialsUserNameBox">
            <div class="header-base-small credentialsUserNameHeader">                                
                 <%=Resources.Resource.CredentialsUserName%>:                
            </div>     
            <div class="float-left">
                    <input class="textEdit inputCredentialsUserName" type="text" id="studio_smtpCredentialsUserName"
                        value="<%=ASC.Core.CoreContext.Configuration.SmtpSettings.CredentialsUserName??""%>" />
             </div>                     
         </div>
            
         <%--CredentialsUserPassword--%>
         <div class="clearFix credentialsUserPasswordBox">
            <div class="header-base-small credentialsUserPasswordHeader"> 
                <%=Resources.Resource.CredentialsUserPassword%>:   
            </div>            
           <div class="float-left">
                    <input class="textEdit inputCredentialsUserPassword"type="password" id="studio_smtpCredentialsUserPwd"
                        value="" />
            </div>
         </div>  
        
    </div>
                          
     <div class="clearFix saveMailSettingsBox">
        <a id="saveMailSettingsBtn" class="float-left button" href="javascript:void(0);" ><%=Resources.Resource.SaveButton %></a>                
    </div>
    
    
     <div class="recipientAddressBox">
         <div class="header-base-small recipientAddressHeader"> 
                <%=Resources.Resource.RecipientAddress%>:
         </div>
            <div class="float-left">
                <input id="studio_smtpRecipientAddress" type="text" class="textEdit inputRecipientAddress" />
            </div>
    
            <a id="sendTestMailBtn" class="sendTestMailLink button" href="javascript:void(0);">
                <%=Resources.Resource.SendTestMailButton %></a>
     </div>
</div>
