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
using System.Text;
using System.Web;
using ASC.Core;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using Resources;

namespace ASC.Web.Studio
{
    public partial class MyStaff : MainPage
    {
        public MyUserProfile UserInfo { get; private set; }

        public bool EditProfileFlag { get; private set; }

        private ProfileHelper helper;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CoreContext.Configuration.YourDocs)
            {
                InitScripts();
            }
            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/ajaxupload.js"));

            Master.DisabledSidePanel = true;

            helper = new ProfileHelper(Request["user"]);

            UserInfo = helper.userProfile;

            if (Request.Params["action"] == "edit")
            {
                InitEditControl();
                EditProfileFlag = true;
            }
            else
            {
                InitProfileControl();
                EditProfileFlag = false;
            }

            Title = HeaderStringHelper.GetPageTitle(Resource.MyProfile);
        }

        private void InitScripts()
        {
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

        private void InitProfileControl()
        {
            var actions = (UserProfileActions)LoadControl(UserProfileActions.Location);
            actions.MyStaff = helper.isMe;
            actions.profileHelper = helper;
            actions.Actions = new AllowedActions(helper.UserInfo);
            actionsHolder.Controls.Add(actions);

            var userProfileControl = (UserProfileControl)LoadControl(UserProfileControl.Location);
            userProfileControl.UserProfileHelper = helper;

            _contentHolderForProfile.Controls.Add(userProfileControl);

            var userSubscriptions = (UserSubscriptions)LoadControl(UserSubscriptions.Location);
            _phSubscriptionView.Controls.Add(userSubscriptions);
        }

        private void InitEditControl()
        {
            var userProfileEditControl = LoadControl(UserProfileEditControl.Location) as UserProfileEditControl;

            _contentHolderForEditForm.Controls.Add(userProfileEditControl);
        }
    }
}