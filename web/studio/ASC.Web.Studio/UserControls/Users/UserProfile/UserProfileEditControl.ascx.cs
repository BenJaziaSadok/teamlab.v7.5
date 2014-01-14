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
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;

using ASC.Core;
using ASC.Core.Users;
using ASC.Core.Tenants;

using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

using Resources;

using Newtonsoft.Json;

namespace ASC.Web.Studio.UserControls.Users.UserProfile
{
    public partial class UserProfileEditControl : UserControl
    {
        #region Properies

        public ProfileHelper ProfileHelper { get; private set; }

        protected MyUserProfile UserProfile { get; set; }

        protected bool IsVisitor
        {
            get
            {
                if (ProfileHelper.isMe && !IsPageEditProfileFlag)
                {
                    return Request["type"] == "guest";
                }

                return ProfileHelper.isVisitor;
            }
        }

        public string RoleIconName
        {
            get
            {
                if (!IsPageEditProfileFlag) return "";

                if (ProfileHelper.UserInfo.IsAdmin() || ProfileHelper.UserInfo.GetListAdminModules().Any())
                {
                    return "admin";
                }
                if (IsVisitor)
                {
                    return "guest";
                }
                return "";
            }
        }

        protected bool CanEdit
        {
            get { return ProfileHelper.CanEdit; }
        }

        protected string Phone { get; set; }
        protected string ProfileGender { get; set; }
        protected List<MyContact> SocContacts { get; set; }
        protected List<MyContact> OtherContacts { get; set; }
        protected GroupInfo[] Departments { get; set; }
        protected bool CanAddUser { get; set; }
        protected bool CanEditType { get; private set; }

        protected bool IsPageEditProfileFlag { get; private set; }

        protected bool IsAdmin()
        {
            return CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin();
        }

        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserProfileEditControl.ascx"; }
        }

        #endregion

        #region events

        protected void Page_Load(object sender, EventArgs e)
        {
            IsPageEditProfileFlag = (Request["action"] == "edit");

            ProfileHelper = new ProfileHelper(Request["user"]);
            UserProfile = ProfileHelper.userProfile;

            if ((IsPageEditProfileFlag && !CanEdit) || (!IsPageEditProfileFlag && !IsAdmin()))
            {
                Response.Redirect("~/products/people/", true);
            }

            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/users/userprofile/js/userprofileeditcontrol.js"));
            Page.RegisterStyleControl(ResolveUrl("~/usercontrols/users/userprofile/css/profileeditcontrol_style.less"));

            CanAddUser = TenantStatisticsProvider.GetUsersCount() < TenantExtra.GetTenantQuota().ActiveUsers;

            CanEditType = SecurityContext.CheckPermissions(Constants.Action_AddRemoveUser) &&
                          (!(ProfileHelper.UserInfo.IsAdmin() || IsModuleAdmin()) || !IsPageEditProfileFlag);

            if (IsPageEditProfileFlag)
            {
                Phone = UserProfile.Phone;
                ProfileGender = UserProfile.Sex.HasValue ? UserProfile.Sex.Value ? "1" : "0" : "-1";
                Departments = CoreContext.UserManager.GetUserGroups(UserProfile.Id);
                SocContacts = UserProfile.Contacts;
                OtherContacts = new List<MyContact>();
                OtherContacts.AddRange(UserProfile.Emails);
                OtherContacts.AddRange(UserProfile.Messengers);
                OtherContacts.AddRange(UserProfile.Phones);
                var deps = Departments.ToList();

                var script =
                    String.Format(
                        @"<script type='text/javascript'>
                                    var departmentsList = {0};
                                    var socContacts = {1};
                                    var otherContacts = {2};
                                    var userId= {3};
                                  
                </script>",
                        JsonConvert.SerializeObject(deps.ConvertAll(item => new
                            {
                                id = item.ID
                            })),
                        JsonConvert.SerializeObject(SocContacts),
                        JsonConvert.SerializeObject(OtherContacts),
                        JsonConvert.SerializeObject(UserProfile.Id));
                Page.ClientScript.RegisterStartupScript(GetType(), Guid.NewGuid().ToString(), script);
            }

            var photoControl = (LoadPhotoControl)LoadControl(LoadPhotoControl.Location);
            loadPhotoWindow.Controls.Add(photoControl);

            Page.Title = HeaderStringHelper.GetPageTitle(GetTitle());
        }

        #endregion

        #region Methods

        public bool IsModuleAdmin()
        {
            return ProfileHelper.UserInfo.GetListAdminModules().Any();
        }

        public string GetTitle()
        {
            return IsPageEditProfileFlag
                       ? UserProfile.DisplayName + " - " + Resource.EditUserDialogTitle
                       : Resource.CreateNewProfile;
        }

        public string GetFirstName()
        {
            return IsPageEditProfileFlag ? UserProfile.FirstName.HtmlEncode() : String.Empty;
        }

        public string GetLastName()
        {
            return IsPageEditProfileFlag ? UserProfile.LastName.HtmlEncode() : String.Empty;
        }

        public string GetPosition()
        {
            return IsPageEditProfileFlag ? UserProfile.Title : String.Empty;
        }

        public string GetEmail()
        {
            return IsPageEditProfileFlag ? UserProfile.Email : String.Empty;
        }

        public string GetPlace()
        {
            return IsPageEditProfileFlag ? UserProfile.Place : String.Empty;
        }

        public string GetComment()
        {
            return IsPageEditProfileFlag ? UserProfile.Comment : String.Empty;
        }

        public string GetTextButton()
        {
            return IsPageEditProfileFlag ? Resource.SaveButton : Resource.AddButton;
        }

        public string GetPhotoPath()
        {
            return IsPageEditProfileFlag ? UserPhotoManager.GetPhotoAbsoluteWebPath(UserProfile.Id) : UserPhotoManager.GetDefaultPhotoAbsoluteWebPath();
        }

        public string GetWorkFromDate()
        {
            return IsPageEditProfileFlag ? UserProfile.RegDateValue : TenantUtil.DateTimeNow().ToString(DateTimeExtension.DateFormatPattern);
        }

        public string GetBirthDate()
        {
            return IsPageEditProfileFlag ? UserProfile.BirthDateValue : String.Empty;
        }

        protected string RenderDepartOptions()
        {
            var deps = new List<GroupInfo>();
            foreach (var department in CoreContext.UserManager.GetDepartments())
            {
                deps.Add(department);
                deps.AddRange(GetChildDepartments(department));
            }

            deps.Sort((d1, d2) => String.Compare(d1.Name, d2.Name, StringComparison.InvariantCultureIgnoreCase));
            var sb = new StringBuilder();
            foreach (var dep in deps)
            {
                sb.Append("<option value=\"" + dep.ID + "\">" + dep.Name.HtmlEncode() + "</option>");
            }
            return sb.ToString();
        }

        private static IEnumerable<GroupInfo> GetChildDepartments(GroupInfo dep)
        {
            return Enumerable.Empty<GroupInfo>();
        }

        #endregion

    }
}