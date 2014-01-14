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
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.UserControls.Users;
using ASC.Core;

namespace ASC.Web.Studio.Personal
{
    public partial class profile : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Title = HeaderStringHelper.GetPageTitle(Resources.Resource.Profile);
            Master.DisabledSidePanel = true;

            PersonalHelper.AdjustTopNavigator(Master.TopStudioPanel, PersonalPart.Profile);

            _myStaffContainer.CurrentPageCaption = Resources.Resource.Profile;

            var userProfileControl = (UserProfileControl)LoadControl(UserProfileControl.Location);
            userProfileControl.UserProfileHelper = new ProfileHelper(SecurityContext.CurrentAccount.ID.ToString());

            _myStaffContainer.Body.Controls.Add(userProfileControl);
        }
    }
}