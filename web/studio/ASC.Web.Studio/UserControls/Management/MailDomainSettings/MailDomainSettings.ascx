<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MailDomainSettings.ascx.cs" Inherits="ASC.Web.Studio.UserControls.Management.MailDomainSettings" %>

<%@ Import Namespace="ASC.Web.Studio.Core" %>
<%@ Import Namespace="ASC.Web.Core.Users" %>
 <div class="clearFix">
     <div class="settings-block">
      <div class="header-base clearFix" id="mailDomainSettingsTitle">
  		    <%=Resources.Resource.StudioDomainSettings%>
      </div>
      <div id="studio_domainSettingsInfo">    
      </div>
      <div id="studio_domainSettingsBox">
      
          <div class="clearFix">
              <div  class="clearFix">
                  <input id="offMailDomains" type="radio" value="<%=(int)ASC.Core.Tenants.TenantTrustedDomainsType.None %>" name="signInType" <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.None?"checked=\"checked\"":""%>/>     
                  <label class="header-base-small" for="offMailDomains"><%=Resources.Resource.OffMailDomains%></label>
              </div>
              <div  class="clearFix">
                   <input id="trustedMailDomains" type="radio" value="<%=(int)ASC.Core.Tenants.TenantTrustedDomainsType.Custom %>" name="signInType" <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.Custom?"checked=\"checked\"":""%>/>     
                  <label class="header-base-small" for="trustedMailDomains"><%=Resources.Resource.TrustedDomainSignInTitle%></label>
              </div>
          </div>
          
          <div id="trustedMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.Custom?"":"style=\"display:none;\""%>>                
              <div class="clearFix" id="studio_domainListBox">
              <%for (int i = 0; i < _currentTenant.TrustedDomains.Count; i++)
                {var domain = _currentTenant.TrustedDomains[i];%>
                
                       <div name="<%=i%>" id="studio_domain_box_<%=i%>" class="domainSettingsBlock clearFix">
                             <input id="studio_domain_<%=i%>" type="text" maxlength="60" class="textEdit" value="<%=HttpUtility.HtmlEncode(domain)%>"/>
                             <a class="removeDomain" onclick="MailDomainSettingsManager.RemoveTrustedDomain('<%=i%>');" href="javascript:void(0);">
                               <img align="absmiddle" border="0" alt="<%=Resources.Resource.DeleteButton%>" src="<%=ASC.Web.Core.Utility.Skins.WebImageSupplier.GetAbsoluteWebPath("trash_16.png")%>"/>
                             </a>
                       </div>
              <%}%>
              </div>        
              
              <a href="javascript:void(0);" id="addTrustDomainBtn"><%=Resources.Resource.AddTrustedDomainButton%></a>
            <div class="clearFix domain-settings-cbx-container">
                <input type="checkbox" id="cbxInviteUsersAsVisitors" <%= _studioTrustedDomainSettings.InviteUsersAsVisitors ? "checked=\"checked\"" : "" %> <%= _enableInviteUsers ? "" : "disabled=\"disabled\"" %>>
                <label for="cbxInviteUsersAsVisitors"><%=Resources.Resource.InviteUsersAsCollaborators%></label>
            </div>
          </div>
          
          <div id="allMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.All?"":"style=\"display:none;\""%>>        
          </div>
          
          <div id="offMailDomainsDescription" class="description"  <%=_currentTenant.TrustedDomainsType == ASC.Core.Tenants.TenantTrustedDomainsType.None?"":"style=\"display:none;\""%>>        
          </div>
          
          <div class="clearFix">
              <a class="button blue float-left" id="saveMailDomainSettingsBtn" href="javascript:void(0);">
                  <%=Resources.Resource.SaveButton %></a>
          </div>
      </div>    
    </div>
    <div class="settings-help-block">
              <p><%=String.Format(Resources.Resource.HelpAnswerMailDomainSettings, "<br />","<b>","</b>")%></p>
              <a href="http://helpcenter.teamlab.com/gettingstarted/configuration.aspx#ChangingGeneralSettings_block" target="_blank"><%=Resources.Resource.LearnMore%></a>
    </div>
</div>