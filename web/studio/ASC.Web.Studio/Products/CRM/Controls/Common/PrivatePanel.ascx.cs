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
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Resources;
using System.Collections.Generic;
using ASC.Web.CRM.Classes;
using System.Text;
using Newtonsoft.Json;


namespace ASC.Web.CRM.Controls.Common
{
    public partial class PrivatePanel : BaseUserControl
    {
        #region Properties

        public static String Location
        {
            get
            {
                return PathProvider.GetFileStaticRelativePath("Common/PrivatePanel.ascx");
            }
        }

        public String Title { get; set; }

        public String Description { get; set; }

        public String CheckBoxLabel { get; set; }

        public String AccessListLable { get; set; }

        public bool IsPrivateItem { get; set; }

        public List<String> UsersWhoHasAccess { get; set; }

        public Dictionary<Guid, String> SelectedUsers { get; set; }

        public List<Guid> DisabledUsers { get; set; }

        public bool HideNotifyPanel { get; set; }

        #endregion

        #region Events

        protected void Page_Init(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(Title))
                Title = CRMCommonResource.PrivatePanelHeader;

            if (String.IsNullOrEmpty(Description))
                Description = CustomNamingPeople.Substitute<CRMCommonResource>("PrivatePanelDescription");

            if (String.IsNullOrEmpty(CheckBoxLabel))
                CheckBoxLabel = CRMCommonResource.PrivatePanelCheckBoxLabel;

            if (String.IsNullOrEmpty(AccessListLable))
                AccessListLable = CustomNamingPeople.Substitute<CRMCommonResource>("PrivatePanelAccessListLable");  

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            RegisterClientScriptHelper.DataUserSelectorListView(Page, "", SelectedUsers, null, true);

            RegisterScript();
        }

        #endregion

        #region Methods

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.PrivatePanel.init({0},ASC.CRM.Resources.CRMCommonResource.Notify,{1},null,{2});",
                (!HideNotifyPanel).ToString().ToLower(),
                JsonConvert.SerializeObject(UsersWhoHasAccess),
                JsonConvert.SerializeObject(DisabledUsers)
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}