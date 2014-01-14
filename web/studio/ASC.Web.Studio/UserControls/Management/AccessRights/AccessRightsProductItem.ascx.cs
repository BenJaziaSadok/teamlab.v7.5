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
using System.Linq;
using System.Web.UI;
using System.Collections.Generic;
using ASC.Web.Core.Utility.Skins;
using ASC.Core;
using ASC.Web.Studio.Controls.Users;
using AjaxPro;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Users;
using System.Text;

namespace ASC.Web.Studio.UserControls.Management
{
    public partial class AccessRightsProductItem : UserControl
    {
        #region Properies

        public static string Location
        {
            get { return "~/UserControls/Management/AccessRights/AccessRightsProductItem.ascx"; }
        }

        public Item ProductItem { get; set; }

        protected string PeopleImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("user_12.png"); }
        }

        protected string GroupImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("group_12.png"); }
        }

        protected string TrashImgSrc
        {
            get { return WebImageSupplier.GetAbsoluteWebPath("trash_12.png"); }
        }

        protected bool PublicModule
        {
            get { return ProductItem.SelectedUsers.Count == 0 && ProductItem.SelectedGroups.Count == 0; }
        }

        protected Guid PortalOwnerId
        {
            get { return CoreContext.TenantManager.GetCurrentTenant().OwnerId; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(ProductItem.ItemName)) return;

            InitUserSelector();

            RegisterClientScript();
        }

        private void InitUserSelector()
        {
            var userSelector = new AdvancedUserSelector
            {
                ID = "userSelector_" + ProductItem.ItemName,
                LinkText = CustomNamingPeople.Substitute<Resources.Resource>("AccessRightsAddUser"),
                IsLinkView = true
            };

            if (ProductItem.ID == new Guid("6743007c-6f95-4d20-8c88-a8601ce5e76d") || ProductItem.ID == new Guid("f4d98afd-d336-4332-8778-3c6945c81ea0"))
            {
                // hack: crm and people products not visible for collaborators
                userSelector.EmployeeType = EmployeeType.User;
            }

            phUserSelector.Controls.Add(userSelector);
        }

        private void RegisterClientScript()
        {
            var sb = new StringBuilder();

            var ids = ProductItem.SelectedUsers.Select(i => i.ID).ToArray();
            var names = ProductItem.SelectedUsers.Select(i => i.DisplayUserName()).ToArray();

            sb.AppendFormat("SelectedUsers_{0} = {1};",
                ProductItem.ItemName,
                JavaScriptSerializer.Serialize(
                new
                {
                    IDs = ids,
                    Names = names,
                    PeopleImgSrc = PeopleImgSrc,
                    TrashImgSrc = TrashImgSrc,
                    TrashImgTitle = Resources.Resource.DeleteButton,
                    CurrentUserID = SecurityContext.CurrentAccount.ID
                })
            );

            ids = ProductItem.SelectedGroups.Select(i => i.ID).ToArray();
            names = ProductItem.SelectedGroups.Select(i => i.Name.HtmlEncode()).ToArray();

            sb.AppendFormat("SelectedGroups_{0} = {1};",
                ProductItem.ItemName,
                JavaScriptSerializer.Serialize(
                new
                {
                    IDs = ids,
                    Names = names,
                    GroupImgSrc = GroupImgSrc,
                    TrashImgSrc = TrashImgSrc,
                    TrashImgTitle = Resources.Resource.DeleteButton
                })
            );

            if (!ProductItem.CanNotBeDisabled)
            {
                sb.AppendFormat("ASC.Settings.AccessRights.initProduct(\"{0}\",\"{1}\",{2});",
                                ProductItem.ID,
                                ProductItem.ItemName,
                                PublicModule.ToString().ToLower()
                    );
            }


            Page.RegisterInlineScript(sb.ToString());
        }
    }
}