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

#region Import

using System;
using System.Text;
using System.Web;
using ASC.Core;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Thrdparty.Configuration;
using ASC.Web.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Controls.Deals;
using ASC.Web.CRM.Resources;
using ASC.Web.CRM.SocialMedia;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{
    public partial class ContactDetailsView : BaseUserControl
    {
        #region Properies

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactDetailsView.ascx"); }
        }

        public Contact TargetContact { get; set; }

        protected bool ShowEventLinkToPanel
        {
            get
            {
                var dealsCount = Global.DaoFactory.GetDealDao().GetDealsCount();
                var casesCount = Global.DaoFactory.GetCasesDao().GetCasesCount();

                return dealsCount + casesCount > 0;
            }
        }

        protected bool MobileVer = false;

        protected bool CanCreateProjects {get; set;}

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            CanCreateProjects = Global.CanCreateProjects();

            ExecFullCardView();
            ExecTasksView();
            ExecFilesView();
            ExecDealsView();

            if (TargetContact is Company)
                ExecPeopleContainerView();

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/products/projects/app_themes/default/css/allprojects.css"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/projects/js/projects.js"));
            RegisterScript();
        }

        #endregion

        #region Methods

        protected void ExecDealsView()
        {
            Page.RegisterClientScript(typeof(Masters.ClientScripts.ExchangeRateViewData));
        }

        protected void ExecPeopleContainerView()
        {
            RegisterClientScriptHelper.DataListContactTab(Page, TargetContact.ID, EntityType.Company);
        }

        protected void ExecFullCardView()
        {
            var contactFullCardControl = (ContactFullCardView)LoadControl(ContactFullCardView.Location);
            contactFullCardControl.TargetContact = TargetContact;

            _phProfileView.Controls.Add(contactFullCardControl);
        }

        protected void ExecFilesView()
        {
            var filesViewControl = (Studio.UserControls.Common.Attachments.Attachments)LoadControl(Studio.UserControls.Common.Attachments.Attachments.Location);
            filesViewControl.EntityType = "contact";
            filesViewControl.ModuleName = "crm";
            _phFilesView.Controls.Add(filesViewControl);
        }

        protected void ExecTasksView()
        {
            RegisterClientScriptHelper.DataContactDetailsViewForTaskAction(Page, TargetContact);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.Append(@"
                var hash = window.location.hash;
                hash = hash.charAt(0) == '#' ? hash.substring(1) : hash;
                if (!hash) {
                    window.location.hash = 'profile';
                }"
            );

            sb.AppendFormat(@"
                    ASC.CRM.DealTabView.initTab({0},{1},""{2}"",""{3}"");
                    ASC.CRM.ListTaskView.initTab({0},""{4}"",0,{1},{5},{6},""{7}"");
                    ASC.CRM.ContactDetailsView.init({8},{9},""{10}"",""{11}"", {13}, {14}, {15}, {17});
                    ASC.CRM.SocialMedia.init(""{16}"");
                    ASC.CRM.ContactDetailsView.checkSocialMediaError();
                    {12}",

                TargetContact.ID,
                Global.EntryCountOnPage,
                System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_deals.png", ProductEntryPoint.ID),

                EntityType.Contact.ToString().ToLower(),
                Global.VisiblePageCount,
                (int)HistoryCategorySystem.TaskClosed,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),

                (TargetContact is Company).ToString().ToLower(),
                CanCreateProjects.ToString().ToLower(),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_company_participants.png", ProductEntryPoint.ID),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_projects.png", ProductEntryPoint.ID),

                ShowEventLinkToPanel ? "" : "jq('#eventLinkToPanel').hide();",
                (TargetContact is Company).ToString().ToLower(),
                WebItemSecurity.IsAvailableForUser(WebItemManager.ProjectsProductID.ToString(), SecurityContext.CurrentAccount.ID).ToString().ToLower(),
                (!string.IsNullOrEmpty(KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterDefaultAccessToken))).ToString().ToLower(),
                ContactPhotoManager.GetBigSizePhoto(0, TargetContact is Company),
                TargetContact.IsShared.ToString().ToLower()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}