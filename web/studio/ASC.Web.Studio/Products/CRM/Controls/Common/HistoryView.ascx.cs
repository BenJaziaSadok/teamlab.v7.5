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
using System.Collections.Generic;
using System.Linq;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using ASC.Core;
using Newtonsoft.Json;
using Constants = ASC.Core.Users.Constants;
using System.Text;
using ASC.Core.Tenants;
using ASC.Web.Core;


#endregion

namespace ASC.Web.CRM.Controls.Common
{
    public partial class HistoryView : BaseUserControl
    {
        #region Property

        public static String Location { get { return PathProvider.GetFileStaticRelativePath("Common/HistoryView.ascx"); } }

        public int TargetEntityID { get; set; }

        public EntityType TargetEntityType { get; set; }

        public int TargetContactID { get; set; }

        protected bool MobileVer = false;

        #endregion

        #region Events

        private void Page_Load(object sender, EventArgs e)
        {
            MobileVer = ASC.Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            Page.RegisterClientScript(typeof(Masters.ClientScripts.HistoryViewData));

            var userSelector = (Studio.UserControls.Users.UserSelector) LoadControl(Studio.UserControls.Users.UserSelector.Location);
            userSelector.BehaviorID = "UserSelector";

            if (!MobileVer)
            {
                //init uploader
                var uploader = (FileUploader) LoadControl(FileUploader.Location);
                _phfileUploader.Controls.Add(uploader);
            }

            initUserSelectorListView();
            RegisterScript();
        }

        #endregion

        #region Methods

        private void initUserSelectorListView(){

            List<Guid> users = null;

            switch (TargetEntityType)
            {
                case EntityType.Contact:
                    var contact = Global.DaoFactory.GetContactDao().GetByID(TargetContactID);
                    if (contact.IsShared == false)
                    {
                        users = CRMSecurity.GetAccessSubjectGuidsTo(contact);
                    }
                    break;
                case EntityType.Opportunity:
                    var deal = Global.DaoFactory.GetDealDao().GetByID(TargetEntityID);
                    if (CRMSecurity.IsPrivate(deal))
                    {
                        users = CRMSecurity.GetAccessSubjectGuidsTo(deal);
                    }
                    break;
                case EntityType.Case:
                    var caseItem = Global.DaoFactory.GetCasesDao().GetByID(TargetEntityID);
                    if (CRMSecurity.IsPrivate(caseItem))
                    {
                        users = CRMSecurity.GetAccessSubjectGuidsTo(caseItem);
                    }
                    break;
            }




            //init userSelectorListView
            if (users == null)
            {
                RegisterClientScriptHelper.DataHistoryView(Page, null);
            }
            else
            {
                List<UserInfo> UserList = null;
                List<Guid> UserListGuid = new List<Guid>();

                //with admins
                var admins = CoreContext.UserManager.GetUsersByGroup(Constants.GroupAdmin.ID).ToList();
                admins.AddRange(WebItemSecurity.GetProductAdministrators(ProductEntryPoint.ID).ToList());
                admins = admins.Distinct().ToList();

                admins.AddRange(from u in users
                                where !CoreContext.UserManager.IsUserInGroup(u, Constants.GroupAdmin.ID)&& !WebItemSecurity.IsProductAdministrator(ProductEntryPoint.ID, u)
                                select CoreContext.UserManager.GetUsers(u));

                UserList = admins.SortByUserName();
                UserListGuid = UserList.ConvertAll(n => n.ID).Where(g => g != SecurityContext.CurrentAccount.ID).ToList();

                RegisterClientScriptHelper.DataHistoryView(Page, UserListGuid);
            }
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.HistoryView.init({0},""{1}"",{2},{3},""{4}"",""{5}"",""{6}"", {7});",
                    TargetContactID,
                    TargetEntityType.ToString().ToLower(),
                    TargetEntityID,
                    Global.EntryCountOnPage,
                    TenantUtil.DateTimeNow().ToShortDateString(),
                    DateTimeExtension.DateMaskForJQuery,
                    WebImageSupplier.GetAbsoluteWebPath("empty_screen_filter.png"),
                    ASC.CRM.Core.CRMSecurity.IsAdmin.ToString().ToLower());

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}