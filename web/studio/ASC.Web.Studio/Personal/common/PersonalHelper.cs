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
using System.Web;
using ASC.Web.Studio.UserControls.Common;
using ASC.Web.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.Personal
{
    public enum PersonalPart
    {
        Default,
        WebItem,
        Profile,
        Settings,
        Backup
    }

    public class PersonalHelper
    {
        private static readonly List<Guid> PersonalItems =
            new List<Guid>
                {
                    WebItemManager.CalendarProductID,
                    WebItemManager.DocumentsProductID
                };


        public static void TransferRequest(MainPage page)
        {
            if (!SetupInfo.IsPersonal)
                return;

            if (page.TemplateSourceDirectory.IndexOf("/personal", StringComparison.InvariantCultureIgnoreCase) >= 0)
                return;

            //undo redirect for personal items
            var curItem = CommonLinkUtility.GetWebItemByUrl(HttpContext.Current.Request.GetUrlRewriter().ToString());
            if (curItem != null && PersonalItems.Contains(curItem.ID))
                return;

            var url = String.Empty;
            if (page is Management)
            {
                var managementType = ManagementType.General;
                if (!String.IsNullOrEmpty(HttpContext.Current.Request["type"]))
                {
                    try
                    {
                        managementType = (ManagementType)Convert.ToInt32(HttpContext.Current.Request["type"]);
                    }
                    catch
                    {
                        managementType = ManagementType.General;
                    }
                }

                url =
                    managementType == ManagementType.Account
                        ? "~/personal/backup.aspx"
                        : "~/personal/settings.aspx";
            }

            else if (page is Auth)
                url = "~/personal/auth.aspx";

            else if (page is MyStaff)
                url = "~/personal/my.aspx";

            if (url.Equals(String.Empty) && PersonalItems.Count > 0)
                url = WebItemManager.Instance[PersonalItems[0]].StartURL;


            HttpContext.Current.Server.TransferRequest(url);
        }

        public static string GetPartUrl(PersonalPart part)
        {
            switch (part)
            {
                case PersonalPart.Settings:
                    return CommonLinkUtility.GetAdministration(ManagementType.General);

                case PersonalPart.Backup:
                    return CommonLinkUtility.GetAdministration(ManagementType.Account);

                case PersonalPart.Profile:
                    return CommonLinkUtility.GetMyStaff();

            }

            return CommonLinkUtility.GetDefault();
        }

        public static void AdjustTopNavigator(TopStudioPanel topNavPanel, PersonalPart part)
        {
            AdjustTopNavigator(topNavPanel, part, Guid.Empty);
        }

        public static void AdjustTopNavigator(TopStudioPanel topNavPanel, PersonalPart part, Guid currentItem)
        {
            topNavPanel.DisableProductNavigation = true;
            topNavPanel.DisableSearch = true;
            topNavPanel.DisableUserInfo = true;
            topNavPanel.DisableVideo = true;

            //items
            //foreach (var itemId in PersonalItems)
            //{
            //    var webItem = WebItemManager.Instance[itemId];
            //topNavPanel.NavigationItems.Add(
            //    new NavigationItem
            //        {
            //            Name = webItem.Name,
            //            URL = VirtualPathUtility.ToAbsolute(webItem.StartURL),
            //            Selected = currentItem.Equals(webItem.ID) || (part == PersonalPart.Default && isFirst)
            //        });
            //}

            ////backup
            //topNavPanel.NavigationItems.Add(
            //    new NavigationItem
            //        {
            //            Name = Resources.Resource.Backup,
            //            URL = GetPartUrl(PersonalPart.Backup),
            //            Selected = (part == PersonalPart.Backup),
            //            RightAlign = true
            //        });

            ////settings
            //topNavPanel.NavigationItems.Add(
            //    new NavigationItem
            //        {
            //            Name = Resources.Resource.Administration,
            //            URL = GetPartUrl(PersonalPart.Settings),
            //            Selected = (part == PersonalPart.Settings),
            //            RightAlign = true
            //        });

            ////profile
            //topNavPanel.NavigationItems.Add(
            //    new NavigationItem
            //        {
            //            Name = Resources.Resource.Profile,
            //            URL = GetPartUrl(PersonalPart.Profile),
            //            Selected = (part == PersonalPart.Profile),
            //            RightAlign = true
            //        });
        }
    }
}