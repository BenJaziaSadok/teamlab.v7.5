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
using ASC.Core;
using ASC.Web.Core;
using ASC.Web.Core.WebZones;
using ASC.Web.Calendar.Notification;
using System.Text;
using System.Web.UI;
using System.Web;

namespace ASC.Web.Calendar
{
    [WebZoneAttribute(WebZoneType.CustomProductList)]
    public class CalendarAddon : IAddon, IRenderCustomNavigation
    {
        public static Guid AddonID
        {
            get { return WebItemManager.CalendarProductID; }
        }

        public static string BaseVirtualPath
        {
            get { return "~/addons/calendar/"; }
        }

        private AddonContext _context;

        public AddonContext Context
        {
            get { return _context; }
        }

        WebItemContext IWebItem.Context
        {
            get { return _context; }
        }

        public string Description
        {
            get { return Resources.CalendarAddonResource.AddonDescription; }
        }

        public Guid ID
        {
            get { return AddonID; }
        }

        public void Init()
        {
            _context = new AddonContext
                           {
                               DefaultSortOrder = 80,
                               DisabledIconFileName = "disabledlogo.png",
                               IconFileName = "logo.png",
                               LargeIconFileName = "biglogo.png",
                               SubscriptionManager = new SubscriptionManager(),
                           };
        }

        public string Name
        {
            get { return Resources.CalendarAddonResource.AddonName; }
        }

        public void Shutdown()
        {

        }

        public string StartURL
        {
            get { return "~/addons/calendar/"; }
        }
        public string ProductClassName
        {
            get { return "calendar"; }
        }

        #region IRenderCustomNavigation Members

        public Control LoadCustomNavigationControl(Page page)
        {
            return null;
        }

        public string RenderCustomNavigation(Page page)
        {
            if (CoreContext.Configuration.YourDocs) return string.Empty;

            var sb = new StringBuilder();
            //sb.AppendFormat(@"<style type=""text/css"">
            //                .studioTopNavigationPanel .systemSection .calendar a{{background:url(""{0}"") left 1px no-repeat;}}
            //                </style>", WebImageSupplier.GetAbsoluteWebPath("minilogo.png", AddonID));

            //sb.AppendFormat(@"<li class=""itemBox calendar"" style=""float: right;"">
            //        <a href=""{0}""><span>{1}</span>
            //        </a></li>", VirtualPathUtility.ToAbsolute(this.StartURL), this.Name);

            sb.AppendFormat(@"<li class=""top-item-box calendar"">
                                  <a class=""inner-text"" href=""{0}"" title=""{1}"">
                                      <span class=""inner-label""></span>
                                  </a>
                              </li>", VirtualPathUtility.ToAbsolute(StartURL), Name);

            return sb.ToString();
        }

        #endregion
    }
}