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
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Core.Tenants;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Core.SMS;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.UserControls.Management;
using ASC.Web.Studio.UserControls.Users.UserProfile;
using ASC.Web.Studio.Utility;
using Resources;

using AjaxPro;

namespace ASC.Web.Studio.UserControls.Users
{

    public class AllowedActions
    {
        public bool AllowEdit { get; private set; }
        public bool AllowAddOrDelete { get; private set; }

        public AllowedActions()
        {
        }

        public AllowedActions(UserInfo userInfo)
        {
            var isOwner = userInfo.IsOwner();
            var isMe = userInfo.IsMe();
            AllowAddOrDelete = SecurityContext.CheckPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser) && (!isOwner || isMe);
            AllowEdit = SecurityContext.CheckPermissions(new UserSecurityProvider(userInfo.ID), ASC.Core.Users.Constants.Action_EditUser) && (!isOwner || isMe);
        }
    }

    [AjaxNamespace("AjaxPro.UserProfileControl")]
    public partial class UserProfileControl : UserControl
    {
        #region SavePhotoThumbnails

        public class SavePhotoThumbnails : IThumbnailsData
        {
            #region User ID

            private Guid userID;

            public Guid UserID
            {
                get
                {
                    if (Guid.Empty.Equals(userID))
                    {
                        userID = SecurityContext.CurrentAccount.ID;
                    }
                    return userID;
                }
                set { userID = value; }
            }

            #endregion

            public Bitmap MainImgBitmap
            {
                get { return UserPhotoManager.GetPhotoBitmap(UserID); }
            }

            public string MainImgUrl
            {
                get { return UserPhotoManager.GetPhotoAbsoluteWebPath(UserID); }
            }

            public List<ThumbnailItem> ThumbnailList
            {
                get
                {
                    var tmp = new List<ThumbnailItem>
                        {
                            new ThumbnailItem
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.BigFotoSize,
                                    imgUrl = UserPhotoManager.GetBigPhotoURL(UserID)
                                },
                            new ThumbnailItem
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.MediumFotoSize,
                                    imgUrl = UserPhotoManager.GetMediumPhotoURL(UserID)
                                },
                            new ThumbnailItem
                                {
                                    id = UserPhotoManager.BigFotoSize.ToString(),
                                    size = UserPhotoManager.SmallFotoSize,
                                    imgUrl = UserPhotoManager.GetSmallPhotoURL(UserID)
                                }
                        };
                    return tmp;
                }
            }

            public void Save(List<ThumbnailItem> bitmaps)
            {
                foreach (var item in bitmaps)
                    UserPhotoManager.SaveThumbnail(UserID, item.bitmap, MainImgBitmap.RawFormat);
            }
        }

        #endregion

        public ProfileHelper UserProfileHelper { get; set; }

        protected UserInfo UserInfo { get; set; }

        protected MyUserProfile UserProfile { get; set; }

        protected bool ShowSocialLogins { get; set; }

        protected bool ShowPrimaryMobile;

        protected string BirthDayText { get; set; }

        protected AllowedActions Actions { get; set; }

        protected bool IsAdmin { get; set; }

        protected int HappyBirthday { get; set; }

        public string RoleIconName
        {
            get
            {
                if (UserInfo.IsAdmin() || UserInfo.GetListAdminModules().Any())
                {
                    return "admin";
                }
                if (UserInfo.IsVisitor())
                {
                    return "guest";
                }
                return "";
            }
        }

        public string MainImgUrl
        {
            get { return UserPhotoManager.GetPhotoAbsoluteWebPath(UserInfo.ID); }
        }

        protected bool UserHasAvatar
        {
            get { return !MainImgUrl.Contains("default/images/"); }
        }

        protected bool ShowUserLocation
        {
            get { return UserInfo != null && !string.IsNullOrEmpty(UserInfo.Location) && !string.IsNullOrEmpty(UserInfo.Location.Trim()) && !"null".Equals(UserInfo.Location.Trim(), StringComparison.InvariantCultureIgnoreCase); }
        }

        protected string JoinAffilliateLink
        {
            get { return WebConfigurationManager.AppSettings["web.affiliates.link"]; }
        }

        public static string Location
        {
            get { return "~/UserControls/Users/UserProfile/UserProfileControl.ascx"; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (UserProfileHelper == null)
            {
                UserProfileHelper = new ProfileHelper(SecurityContext.CurrentAccount.ID.ToString());
            }

            UserProfile = UserProfileHelper.userProfile;
            ShowSocialLogins = UserProfileHelper.isMe;
            UserInfo = UserProfileHelper.UserInfo;

            IsAdmin = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsAdmin();

            if (!IsAdmin && (UserInfo.Status != EmployeeStatus.Active))
            {
                Response.Redirect(CommonLinkUtility.GetFullAbsolutePath("~/products/people/"), true);
            }

            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Actions = new AllowedActions(UserInfo);
            HappyBirthday = CheckHappyBirthday();

            ContactPhones.DataSource = UserProfile.Phones;
            ContactPhones.DataBind();

            ContactEmails.DataSource = UserProfile.Emails;
            ContactEmails.DataBind();

            ContactMessengers.DataSource = UserProfile.Messengers;
            ContactMessengers.DataBind();

            ContactSoccontacts.DataSource = UserProfile.Contacts;
            ContactSoccontacts.DataBind();

            _deleteProfileContainer.Options.IsPopup = true;

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/users/userprofile/css/userprofilecontrol_style.less"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/usercontrols/users/userprofile/js/userprofilecontrol.js"));

            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/skins/default/toastr.css"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/third-party/jquery/toastr.js"));

            if (Actions.AllowEdit)
            {
                _editControlsHolder.Controls.Add(LoadControl(PwdTool.Location));
            }
            if (Actions.AllowEdit || (UserInfo.IsOwner() && IsAdmin))
            {
                var control = (UserEmailChange)LoadControl(UserEmailChange.Location);
                control.UserInfo = UserInfo;
                userEmailChange.Controls.Add(control);
            }

            if (!Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context))
            {
                var thumbnailEditorControl = (ThumbnailEditor)LoadControl(ThumbnailEditor.Location);
                thumbnailEditorControl.Title = Resource.TitleThumbnailPhoto;
                thumbnailEditorControl.BehaviorID = "UserPhotoThumbnail";
                thumbnailEditorControl.JcropMinSize = UserPhotoManager.SmallFotoSize;
                thumbnailEditorControl.JcropAspectRatio = 1;
                thumbnailEditorControl.SaveFunctionType = typeof(SavePhotoThumbnails);
                _editControlsHolder.Controls.Add(thumbnailEditorControl);
            }

            if (ShowSocialLogins && AccountLinkControl.IsNotEmpty)
            {
                var accountLink = (AccountLinkControl)LoadControl(AccountLinkControl.Location);
                accountLink.ClientCallback = "loginCallback";
                accountLink.SettingsView = true;
                _accountPlaceholder.Controls.Add(accountLink);
            }

            var emailControl = (UserEmailControl)LoadControl(UserEmailControl.Location);
            emailControl.User = UserInfo;
            emailControl.Viewer = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            _phEmailControlsHolder.Controls.Add(emailControl);

            var photoControl = (LoadPhotoControl)LoadControl(LoadPhotoControl.Location);
            loadPhotoWindow.Controls.Add(photoControl);

            if (UserInfo.IsMe())
            {
                _phLanguage.Controls.Add(LoadControl(UserLanguage.Location));
            }

            if (StudioSmsNotificationSettings.IsVisibleSettings && (Actions.AllowEdit && !String.IsNullOrEmpty(UserInfo.MobilePhone) || UserInfo.IsMe()))
            {
                ShowPrimaryMobile = true;
                var changeMobile = (ChangeMobileNumber)LoadControl(ChangeMobileNumber.Location);
                changeMobile.User = UserInfo;
                ChangeMobileHolder.Controls.Add(changeMobile);
            }

            if (UserInfo.BirthDate.HasValue)
            {
                switch (HappyBirthday)
                {
                    case 0:
                        BirthDayText = Resource.DrnToday;
                        break;
                    case 1:
                        BirthDayText = Resource.DrnTomorrow;
                        break;
                    case 2:
                        BirthDayText = Resource.In + " " + DateTimeExtension.Yet(2);
                        break;
                    case 3:
                        BirthDayText = Resource.In + " " + DateTimeExtension.Yet(3);
                        break;
                    default:
                        BirthDayText = String.Empty;
                        break;
                }
            }
        }

        protected string RenderDepartment()
        {
            if (UserInfo.Status == EmployeeStatus.Terminated)
            {
                return HttpUtility.HtmlEncode(UserInfo.Department ?? "");
            }
            var result = string.Empty;
            foreach (var dep in CoreContext.UserManager.GetUserGroups(UserInfo.ID))
            {
                result += string.Format("<a href=\"./#group={0}\">{1}</a>, ", dep.ID, HttpUtility.HtmlEncode(dep.Name));
            }

            return !string.IsNullOrEmpty(result) ? result.Substring(0, result.Length - 2) : HttpUtility.HtmlEncode(UserInfo.Department ?? "");
        }

        private int CheckHappyBirthday()
        {
            if (!UserInfo.BirthDate.HasValue) return -1;

            var now = TenantUtil.DateTimeNow();
            var birthday = UserInfo.BirthDate;
            var today = new DateTime(now.Year, now.Month, now.Day);

            var daysInMonth = DateTime.DaysInMonth(today.Year, birthday.Value.Month);
            if (daysInMonth < birthday.Value.Day)
            {
                return -1;
            }

            var fest = new DateTime(today.Year, birthday.Value.Month, birthday.Value.Day);

            if ((fest - today).Days < 0)
            {
                fest = new DateTime(today.Year + 1, birthday.Value.Month, birthday.Value.Day);
            }
            return (fest - today).Days;
        }

        #region Ajax

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse RemoveUser(Guid userID)
        {
            var resp = new AjaxResponse();
            try
            {
                SecurityContext.DemandPermissions(ASC.Core.Users.Constants.Action_AddRemoveUser);
                UserPhotoManager.RemovePhoto(Guid.Empty, userID);
                CoreContext.UserManager.DeleteUser(userID);

                resp.rs1 = "1";
                resp.rs2 = Resource.SuccessfullyDeleteUserInfoMessage;
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public object SendInstructionsToDelete()
        {
            try
            {
                var user = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
                StudioNotifyService.Instance.SendMsgProfileDeletion(user.Email);

                return new { Status = 1, Message = Resource.SuccessfullySentNotificationDeleteUserInfoMessage };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message };
            }
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse ResendActivation(Guid userID)
        {
            var resp = new AjaxResponse();
            try
            {
                if (CoreContext.UserManager.UserExists(userID))
                {
                    var user = CoreContext.UserManager.GetUsers(userID);
                    if (!user.IsActive)
                    {
                        if (user.IsVisitor())
                        {
                            StudioNotifyService.Instance.GuestInfoActivation(user);
                        }
                        else
                        {
                            StudioNotifyService.Instance.UserInfoActivation(user);
                        }
                    }
                    resp.rs1 = "1";
                }
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse JoinToAffiliateProgram()
        {
            var resp = new AjaxResponse();
            try
            {
                resp.rs1 = "1";
                resp.rs2 = AffiliateHelper.Join();
            }
            catch (Exception e)
            {
                resp.rs1 = "0";
                resp.rs2 = HttpUtility.HtmlEncode(e.Message);
            }
            return resp;

        }

        #endregion
    }
}