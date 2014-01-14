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
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using ASC.Web.Calendar.UserControls;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Personal;

namespace ASC.Web.Calendar
{
    public partial class _default : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = HeaderStringHelper.GetPageTitle(Resources.CalendarAddonResource.AddonName);
            Master.DisabledSidePanel = true;

            CalendarPageContent.Controls.Add(LoadControl(CalendarControl.Location));

            //for personal
            if (SetupInfo.IsPersonal)
            {
                PersonalHelper.AdjustTopNavigator(Master.TopStudioPanel, PersonalPart.WebItem, CalendarAddon.AddonID);
            }
        }
    }
}