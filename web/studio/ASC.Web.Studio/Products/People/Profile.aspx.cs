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
using System.Text;
using ASC.Web.Studio;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.People
{
    public partial class Profile : MainPage
    {
        public ProfileHelper ProfileHelper;

        protected MyUserProfile UserInfo
        {
            get
            {
                return ProfileHelper.userProfile;
            }
        }

        protected bool IsMe
        {
            get
            {
                return ProfileHelper.isMe;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            ProfileHelper = new ProfileHelper(Request["user"]);

            Title = HeaderStringHelper.GetPageTitle(UserInfo.DisplayName);

            var control = (UserProfileControl)LoadControl(UserProfileControl.Location);
            control.UserProfileHelper = ProfileHelper;

            CommonContainerHolder.Controls.Add(control);

            var actions = (UserProfileActions)LoadControl(UserProfileActions.Location);
            actions.MyStaff = ProfileHelper.isMe;
            actions.profileHelper = ProfileHelper;
            actions.Actions = new AllowedActions(ProfileHelper.UserInfo);
            actionsHolder.Controls.Add(actions);

            if (IsMe)
            {
                InitSubscriptionView();

                var script = new StringBuilder();
                script.Append("jq('#switcherSubscriptionButton').one('click',");
                script.Append("function() {");
                script.Append("if (!jq('#subscriptionBlockContainer').hasClass('subsLoaded') &&");
                script.Append("typeof (window.CommonSubscriptionManager) != 'undefined' &&");
                script.Append("typeof (window.CommonSubscriptionManager.LoadSubscriptions) === 'function') {");
                script.Append("window.CommonSubscriptionManager.LoadSubscriptions();");
                script.Append("jq('#subscriptionBlockContainer').addClass('subsLoaded');");
                script.Append("}});");

                Page.RegisterInlineScript(script.ToString());
            }
        }

        private void InitSubscriptionView()
        {
            var control = (UserSubscriptions)LoadControl(UserSubscriptions.Location);
            _phSubscriptionView.Controls.Add(control);
        }
    }
}
