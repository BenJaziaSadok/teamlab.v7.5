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
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Web.Core;
using ASC.Web.Core.Client.Bundling;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.HelpCenter;
using ASC.Web.Studio.Masters;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Studio
{
    public partial class Welcome : MainPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            if (CoreContext.Configuration.YourDocs)
                Context.Response.Redirect(CommonLinkUtility.FilesBaseAbsolutePath);

            var fileUri = Request[CommonLinkUtility.FileUri];
            if (!string.IsNullOrEmpty(fileUri))
            {
                UserHelpTourHelper.IsNewUser = true;
                var fileExt = FileUtility.GetInternalExtension(Path.GetFileName(HttpUtility.UrlDecode(fileUri)));
                var createUrl = CommonLinkUtility.FileHandlerPath
                                + "?action=create"
                                + "&" + CommonLinkUtility.FileUri + "=" + HttpUtility.UrlEncode(fileUri)
                                + "&" + CommonLinkUtility.FileTitle + "=Demo" + fileExt
                                + "&openfolder=true";
                Response.Redirect(createUrl, true);
            }
        }

        protected String docsScript { get; set; }
        protected bool showProjects { get; set; }
        protected bool showCRM { get; set; }
        protected string buttonWait { get; set; }
        protected string buttonContinue { get; set; }
        protected string videoTitle { get; set; }

        protected string culture {
            get { return CoreContext.TenantManager.GetCurrentTenant().GetCulture().Name; }
        }

        protected bool IsVideoPage()
        {
            return (Request["module"] != null);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/firsttime/js/start.js"));
            AjaxPro.Utility.RegisterTypeForAjax(typeof(UserVideoSettings));

            UserHelpTourHelper.IsNewUser = true;

            var studioMaster = (BaseTemplate)Master;
            studioMaster.DisabledSidePanel = true;

            if (studioMaster != null)
            {
                //top panel
                studioMaster.TopStudioPanel.DisableProductNavigation = true;
                studioMaster.TopStudioPanel.DisableUserInfo = true;
                studioMaster.TopStudioPanel.DisableSearch = true;
                studioMaster.TopStudioPanel.DisableSettings = true;
                studioMaster.TopStudioPanel.DisableTariff = true;
                studioMaster.TopStudioPanel.DisableTariffNotify = true;
                studioMaster.TopStudioPanel.DisableVideo = true;
            }

            bool isOwner = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsOwner();
            buttonContinue = isOwner ? Resources.Resource.CreatingPortalContinue : Resources.Resource.CreatingPortalContinueUser;
            buttonWait = isOwner ? Resources.Resource.CreatingPortalWaiting : Resources.Resource.CreatingPortalWaitingUser;
            videoTitle = isOwner ? 
                String.Format(Resources.Resource.WizardVideoTitle, "<b>", "</b>", "<span class='gray-text'>", "</span>") :
                String.Format(Resources.Resource.WizardVideoTitleUser, "<b>", "</b>", "<span class='gray-text'>", "</span>");

            var items = WebItemManager.Instance.GetItems(Web.Core.WebZones.WebZoneType.StartProductList);
            var projects = (Product) items.Find(r => r.ProductClassName == "projects");
            var crm = (Product) items.Find(r => r.ProductClassName == "crm");

            if ((!items.Contains(projects) && !items.Contains(crm)) || CoreContext.Configuration.YourDocs)
            {
                if (string.IsNullOrEmpty(Request["module"]))
                {
                    Response.Redirect(Request.RawUrl + "?module=documents");
                }
            }
           
            showProjects = items.Contains(projects);
            showCRM = items.Contains(crm);
            docsScript = WebConfigurationManager.AppSettings["files.docservice.url.preloader"];
        }
    }
}