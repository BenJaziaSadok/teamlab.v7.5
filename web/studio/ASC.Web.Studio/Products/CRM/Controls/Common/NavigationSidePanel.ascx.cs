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
using System.Web;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.CRM.Core.Entities;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;

namespace ASC.Web.CRM.Controls.Common
{
    public partial class NavigationSidePanel : BaseUserControl
    {
        public static string Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/NavigationSidePanel.ascx");
            }
        }

        protected string CurrentPage { get; set; }
        protected bool MobileVer = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            InitCurrentPage();
            MobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);
            var help = (HelpCenter)LoadControl(HelpCenter.Location);
            help.IsSideBar = true;
            HelpHolder.Controls.Add(help);
            SupportHolder.Controls.Add(LoadControl(Support.Location));
        }

        private void InitCurrentPage()
        {
            var currentPath = HttpContext.Current.Request.Path;
            if(currentPath.IndexOf("settings.aspx", StringComparison.Ordinal)>0)
            {
                var typeValue = (HttpContext.Current.Request["type"] ?? "common").ToLower();
                CurrentPage = "settings_" + typeValue;
            }
            else if (currentPath.IndexOf("cases.aspx", StringComparison.Ordinal) > 0)
            {
                CurrentPage = "cases";
            }
            else if (currentPath.IndexOf("deals.aspx", StringComparison.Ordinal) > 0)
            {
                CurrentPage = "deals";
            }
            else if (currentPath.IndexOf("tasks.aspx", StringComparison.Ordinal) > 0)
            {
                CurrentPage = "tasks";
            }
            else if (currentPath.IndexOf("help.aspx", StringComparison.Ordinal) > 0)
            {
                CurrentPage = "help";
            }
            else
            {
                CurrentPage = "contacts";
                int contactID;
                if (int.TryParse(UrlParameters.ID, out contactID))
                {
                    var targetContact = Global.DaoFactory.GetContactDao().GetByID(contactID);
                    if (targetContact == null || !CRMSecurity.CanAccessTo(targetContact))
                        Response.Redirect(PathProvider.StartURL());
                    CurrentPage = targetContact is Company ? "companies" : "persons";
                }
            }

        }
    }
}