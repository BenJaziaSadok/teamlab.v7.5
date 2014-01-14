/* 
 * 
 * (c) Copyright Ascensio System Limited 2010-2014
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * http://www.gnu.org/licenses/agpl.html 
 * 
 */

using System;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Mail.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;
using ASC.Web.Studio.Utility;
using ASC.Web.Core;
using ASC.Web.CRM.Configuration;

namespace ASC.Web.Mail
{
    public partial class MailPage : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor()) // Redirect to home page if user hasn't permissions or not authenticated.
            {
                Response.Redirect("/");
            }

            _manageFieldPopup.Options.IsPopup = true;
            _commonPopup.Options.IsPopup = true;

            Page.Title = HeaderStringHelper.GetPageTitle(Resources.MailResource.MailTitle);

            ProductEntryPoint.ConfigurePortal();

            MailSidePanelContainer.Controls.Add(LoadControl(TagBox.Location));

            MailControlContainer.Controls.Add(LoadControl(MailBox.Location));

            var helpCenter = (HelpCenter)LoadControl(HelpCenter.Location);
            helpCenter.IsSideBar = true;
            sideHelpCenter.Controls.Add(helpCenter);

            SupportHolder.Controls.Add(LoadControl(Support.Location));

            PeopleGroupLocalize.Text = CustomNamingPeople.Substitute<Resources.MailResource>("FilterByGroup");

            // If user doesn't have any mailboxes this will showed.
            var mailBoxManager = new ASC.Mail.Aggregator.MailBoxManager(ConfigurationManager.ConnectionStrings["mail"], 0);
            var mailBoxes = mailBoxManager.GetMailBoxes(TenantProvider.CurrentTenantID, SecurityContext.CurrentAccount.ID.ToString());
            if (mailBoxes.Count < 1)
                BlankModalPH.Controls.Add(LoadControl(BlankModal.Location));

            if (!IsCrmAvailable())
            {
                crmContactsContainer.Visible = false;
            }

            if (!IsPeopleAvailable())
            {
                tlContactsContainer.Visible = false;
            }

            Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/addons/mail/masters/BodyScripts.ascx")));
            Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/addons/mail/masters/Styles.ascx")));
            Page.RegisterClientLocalizationScript(typeof(Masters.ClientScripts.ClientLocalizationResources));
            Page.RegisterClientLocalizationScript(typeof(Masters.ClientScripts.ClientTemplateResources));

            Master.DisabledHelpTour = true;
        }

        public String GetServiceCheckTimeout()
        {
            return WebConfigurationManager.AppSettings["ServiceCheckTimeout"] ?? "30";
        }

        public String GetMailServicePath()
        {
            return VirtualPathUtility.ToAbsolute(WebConfigurationManager.AppSettings["ASCMailService"] ?? "~/addons/mail/Service.svc/");
        }

        public String GetApiBaseUrl()
        {
            return SetupInfo.WebApiBaseUrl;
        }

        protected bool IsAdministrator
        {
            get { return CoreContext.UserManager.IsUserInGroup(SecurityContext.CurrentAccount.ID, Constants.GroupAdmin.ID); }
        }

        public String GetMailFaqUri()
        {
            return WebConfigurationManager.AppSettings["mail.faq-url"] ?? "http://helpcenter.teamlab.com/troubleshooting/mail.aspx";
        }

        public static String GetMailSupportUri()
        {
            return WebConfigurationManager.AppSettings["mail.support-url"] ?? "mailto:support@teamlab.com";
        }

        public static String GetImportOAuthAccessUrl()
        {
            return WebConfigurationManager.AppSettings["mail.import-oauth-url"];
        }

        public static bool IsTurnOnOAuth()
        {
            var config_value = WebConfigurationManager.AppSettings["mail.googleOAuth"];
            return (string.IsNullOrEmpty(config_value) || Convert.ToBoolean(config_value)) // default is true
                && !string.IsNullOrEmpty(GetImportOAuthAccessUrl()); 
        }

        public static bool IsTurnOnAttachmentsGroupOperations()
        {
            var config_value = WebConfigurationManager.AppSettings["mail.attachments-group-operations"];
            return !string.IsNullOrEmpty(config_value) || Convert.ToBoolean(config_value); // default is false
        }

        public bool IsCrmAvailable()
        {
            return WebItemSecurity.IsAvailableForUser(WebItemManager.CRMProductID.ToString(), SecurityContext.CurrentAccount.ID);
        }

        public bool IsPeopleAvailable()
        {
            return WebItemSecurity.IsAvailableForUser(WebItemManager.PeopleProductID.ToString(), SecurityContext.CurrentAccount.ID);
        }

        public String GetMailDownloadHandlerUri()
        {
            return WebConfigurationManager.AppSettings["mail.download-handler-url"] ?? "/addons/mail/httphandlers/download.ashx?attachid={0}";
        }

        public String GetMailDownloadAllHandlerUri()
        {
            return WebConfigurationManager.AppSettings["mail.download-all-handler-url"] ?? "/addons/mail/httphandlers/downloadall.ashx?messageid={0}";
        }

        public String GetMailViewDocumentHandlerUri()
        {
            return WebConfigurationManager.AppSettings["mail.view-document-handler-url"] ?? "/addons/mail/httphandlers/viewdocument.ashx?attachid={0}";
        }

        public String GetMailEditDocumentHandlerUri()
        {
            return WebConfigurationManager.AppSettings["mail.edit-document-handler-url"] ?? "/addons/mail/httphandlers/editdocument.ashx?attachid={0}";
        }
    }
}