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
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.People.Core;
using ASC.Web.People.Resources;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Users;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.People
{
    public partial class Default : MainPage
    {
        #region Properies

        protected bool IsAdmin { get; private set; }

        private UserInfo userInfo;

        #endregion

        #region Events

        public AllowedActions Actions;


        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AjaxPro.Utility.RegisterTypeForAjax(typeof(UserProfileControl));
            this.Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/people/masters/DefaultBodyScripts.ascx")));
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            userInfo = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            IsAdmin = userInfo.IsAdmin();
            Actions = new AllowedActions(userInfo);

            _confirmationDeleteDepartmentPanel.Options.IsPopup = true;
            _resendInviteDialog.Options.IsPopup = true;
            _changeStatusDialog.Options.IsPopup = true;
            _changeTypeDialog.Options.IsPopup = true;
            _deleteUsersDialog.Options.IsPopup = true;


            var emptyContentForPeopleFilter = new EmptyScreenControl
                {
                    ID = "emptyContentForPeopleFilter",
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png"),
                    Header = PeopleResource.NotFoundTitle,
                    Describe = PeopleResource.NotFoundDescription,
                    ButtonHTML = String.Format(@"<a class='clearFilterButton' href='javascript:void(0);' 
                                            onclick='ASC.People.PeopleController.resetAllFilters();'>{0}</a>",
                                               PeopleResource.ClearButton),
                    CssClass = "display-none"
                };

            emptyScreen.Controls.Add(emptyContentForPeopleFilter);


            var controlEmailChange = (UserEmailChange)LoadControl(UserEmailChange.Location);
            controlEmailChange.UserInfo = userInfo;
            userEmailChange.Controls.Add(controlEmailChange);

            userConfirmationDelete.Controls.Add(LoadControl(ConfirmationDeleteUser.Location));

            if (Actions.AllowEdit)
            {
                userPwdChange.Controls.Add(LoadControl(PwdTool.Location));
            }
            Title = HeaderStringHelper.GetPageTitle(PeopleResource.ProductName);
        }

        #endregion
    }
}