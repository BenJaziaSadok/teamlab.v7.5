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

using System.Web;
using ASC.Web.Projects.Classes;
using ASC.Web.Projects.Resources;
using ASC.Web.Studio.Utility;


namespace ASC.Web.Projects
{
    public partial class Import : BasePage
    {
        protected bool QuotaEndFlag { get; set; }

        protected override void PageLoad()
        {
            if (!Participant.IsFullAdmin || Participant.IsVisitor)
                HttpContext.Current.Response.Redirect(PathProvider.BaseVirtualPath, true);

            if (TenantExtra.GetRemainingCountUsers() <= 0)
                QuotaEndFlag = true;

            InitPage();
        }

        public void InitPage(){
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/tariffsettings/css/default/tarifflimitexceed.css"));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("import.js"));
            Title = HeaderStringHelper.GetPageTitle(ImportResource.ImportFromBasecamp);

            HiddenFieldForPermission.Value = ((BasePage)Page).Participant.IsFullAdmin ? "1" : "0";

            import_info_container.Options.IsPopup = true;
            import_projects_container.Options.IsPopup = true;
            import_popup_Error.Options.IsPopup = true;
            users_quota_ends.Options.IsPopup = true;
        }
    }
}
