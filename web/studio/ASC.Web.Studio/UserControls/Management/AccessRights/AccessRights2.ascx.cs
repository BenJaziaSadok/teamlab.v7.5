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
using System.Web.UI;
using ASC.Web.Studio.Core.Users;
using AjaxPro;
using ASC.Core;
using ASC.Core.Common.Logging;
using ASC.Core.Users;
using ASC.Data.Storage;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Core.Notify;
using ASC.Web.Studio.Utility;
using System.Web;
using System.Text;
using Resources;

namespace ASC.Web.Studio.UserControls.Management
{
    public class Item
    {
        public bool Disabled { get; set; }
        public bool DisplayedAlways { get; set; }
        public bool HasPermissionSettings { get; set; }
        public bool CanNotBeDisabled { get; set; }
        public string Name { get; set; }
        public string ItemName { get; set; }
        public string IconUrl { get; set; }
        public string DisabledIconUrl { get; set; }
        public string AccessSwitcherLabel { get; set; }
        public string UserOpportunitiesLabel { get; set; }
        public List<string> UserOpportunities { get; set; }
        public Guid ID { get; set; }
        public List<Item> SubItems { get; set; }
        public List<UserInfo> SelectedUsers { get; set; }
        public List<GroupInfo> SelectedGroups { get; set; }
    }

    [AjaxNamespace("AccessRightsController")]
    public partial class AccessRights : UserControl
    {
        #region Properies

        public static string Location
        {
            get { return "~/UserControls/Management/AccessRights/AccessRights.ascx"; }
        }

        protected bool CanOwnerEdit;
        protected bool AdvancedRightsEnabled = false;

        protected List<IProduct> Products;
        protected List<IProduct> ProductsForAccessSettings;

        protected string[] FullAccessOpportunities { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            InitControl();
            RegisterClientScript();
        }

        #endregion

        #region Methods

        private void InitControl()
        {
            AjaxPro.Utility.RegisterTypeForAjax(GetType());
            Products = new List<IProduct>();
            ProductsForAccessSettings = new List<IProduct>();

            //owner settings
            var curTenant = CoreContext.TenantManager.GetCurrentTenant();
            var currentOwner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
            CanOwnerEdit = currentOwner.ID.Equals(SecurityContext.CurrentAccount.ID);
            var disabledUsers = new List<Guid> { currentOwner.ID };
            ownerSelector.DisabledUsers = disabledUsers;
            ownerSelector.EmployeeType = EmployeeType.User;
            ownerSelector.AdditionalFunction = "ASC.Settings.AccessRights.selectOwner";

            _phOwnerCard.Controls.Add(new EmployeeUserCard
                {
                    EmployeeInfo = currentOwner,
                    EmployeeUrl = currentOwner.GetUserProfilePageURL(),
                });

            //admin settings
            adminSelector.IsLinkView = true;
            adminSelector.LinkText = Resource.AccessRightsAddAdministrator;
            adminSelector.AdditionalFunction = "ASC.Settings.AccessRights.addAdmin";
            adminSelector.DisabledUsers = disabledUsers;
            adminSelector.EmployeeType = EmployeeType.User;

            FullAccessOpportunities = Resource.AccessRightsFullAccessOpportunities.Split('|');
        }

        private void RegisterClientScript()
        {
            Page.RegisterBodyScripts(WebPath.GetPath("usercontrols/management/accessrights/js/accessrights.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/management/accessrights/css/accessrights.less"));

            var curTenant = CoreContext.TenantManager.GetCurrentTenant();
            var currentOwner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);
            var admins = WebItemSecurity.GetProductAdministrators(Guid.Empty).Where(admin => admin.ID != currentOwner.ID).SortByUserName();

            var sb = new StringBuilder();

            sb.AppendFormat("adminList = {0};",
                            JavaScriptSerializer.Serialize(admins.ConvertAll(u => new
                                {
                                    id = u.ID,
                                    smallFotoUrl = u.GetSmallPhotoURL(),
                                    displayName = u.DisplayUserName(),
                                    title = u.Title.HtmlEncode(),
                                    userUrl = CommonLinkUtility.GetUserProfile(u.ID),
                                    accessList = GetAccessList(u.ID)
                                }))
                );
            sb.AppendFormat("ASC.Settings.AccessRights.init({0},\"{1}\");",
                            JavaScriptSerializer.Serialize(new List<Product>()),
                            CustomNamingPeople.Substitute<Resource>("AccessRightsAddGroup").HtmlEncode()
                );

            Page.RegisterInlineScript(sb.ToString());
        }

        private static string GetConfirmLink(Guid newOwnerId, string email)
        {
            var validationKey = EmailValidationKeyProvider.GetEmailKey(email + ConfirmType.PortalOwnerChange.ToString() + newOwnerId);

            return CommonLinkUtility.GetFullAbsolutePath("~/confirm.aspx") +
                   string.Format("?type={0}&email={1}&key={2}&uid={3}", ConfirmType.PortalOwnerChange.ToString(), email, validationKey, newOwnerId);
        }

        private object GetAccessList(Guid uId)
        {
            var fullAccess = WebItemSecurity.IsProductAdministrator(Guid.Empty, uId);
            var res = new List<object>
                {
                    new
                        {
                            pId = Guid.Empty,
                            pName = "full",
                            pAccess = fullAccess,
                            disabled = uId == SecurityContext.CurrentAccount.ID
                        }
                };

            if (ProductsForAccessSettings == null)
            {
                Products = new List<IProduct>();
                ProductsForAccessSettings = new List<IProduct>();
            }

            return res;
        }

        #endregion

        #region AjaxMethods

        [AjaxMethod]
        public object ChangeOwner(Guid ownerId)
        {
            try
            {
                SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

                var curTenant = CoreContext.TenantManager.GetCurrentTenant();
                var owner = CoreContext.UserManager.GetUsers(curTenant.OwnerId);

                if (owner.IsVisitor())
                    throw new System.Security.SecurityException("Collaborator can not be an owner");

                if (curTenant.OwnerId.Equals(SecurityContext.CurrentAccount.ID) && !Guid.Empty.Equals(ownerId))
                {
                    StudioNotifyService.Instance.SendMsgConfirmChangeOwner(curTenant,
                                                                           CoreContext.UserManager.GetUsers(ownerId).DisplayUserName(),
                                                                           GetConfirmLink(ownerId, owner.Email));

                    var emailLink = string.Format("<a href=\"mailto:{0}\">{0}</a>", owner.Email);
                    return new { Status = 1, Message = Resource.ChangePortalOwnerMsg.Replace(":email", emailLink) };
                }

                AdminLog.PostAction("Settings: changed portal owner to ID=\"{0}\"", ownerId);

                return new { Status = 0, Message = Resource.ErrorAccessDenied };
            }
            catch (Exception e)
            {
                return new { Status = 0, Message = e.Message.HtmlEncode() };
            }
        }

        [AjaxMethod]
        public object AddAdmin(Guid id)
        {
            SecurityContext.DemandPermissions(SecutiryConstants.EditPortalSettings);

            var user = CoreContext.UserManager.GetUsers(id);

            if (user.IsVisitor())
                throw new System.Security.SecurityException("Collaborator can not be an administrator");

            WebItemSecurity.SetProductAdministrator(Guid.Empty, id, true);

            var result = new
                {
                    id = user.ID,
                    smallFotoUrl = user.GetSmallPhotoURL(),
                    displayName = user.DisplayUserName(),
                    title = user.Title.HtmlEncode(),
                    userUrl = CommonLinkUtility.GetUserProfile(user.ID),
                    accessList = GetAccessList(user.ID)
                };

            AdminLog.PostAction("Settings: added portal administrator ID=\"{0}\"", id);

            return result;
        }

        #endregion
    }
}