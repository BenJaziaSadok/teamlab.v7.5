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
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.CRM.Core;
using Newtonsoft.Json;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using System.Text;

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class TagSettingsView : BaseUserControl
    {
        #region Members

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Settings/TagSettingsView.ascx"); }
        }

        public EntityType entityType { get; set; }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _manageTagPopup.Options.IsPopup = true;

            entityType = StringToEntityType(Request["view"]);

            _switcherEntityType.SortItemsHeader = CRMCommonResource.Show + ":";

            _forContacts.SortLabel = CRMSettingResource.BothPersonAndCompany;
            _forContacts.SortUrl = "settings.aspx?type=tag";
            _forContacts.IsSelected = entityType == EntityType.Contact;


            _forDeals.SortLabel = CRMCommonResource.DealModuleName;
            _forDeals.SortUrl = String.Format("settings.aspx?type=tag&view={0}", EntityType.Opportunity.ToString().ToLower());
            _forDeals.IsSelected = entityType == EntityType.Opportunity;


            _forCases.SortLabel = CRMCommonResource.CasesModuleName;
            _forCases.SortUrl = String.Format("settings.aspx?type=tag&view={0}", EntityType.Case.ToString().ToLower());
            _forCases.IsSelected = entityType == EntityType.Case;

            RegisterClientScriptHelper.DataTagSettingsView(Page, entityType);
            RegisterScript();
        }

        #endregion

        #region Methods

        private static EntityType StringToEntityType(string type)
        {
            switch (type)
            {
                case "opportunity":
                    return EntityType.Opportunity;
                case "case":
                    return EntityType.Case;
                default:
                    return EntityType.Contact;
            }
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.TagSettingsView.initEmptyScreen('{2}','{3}','{4}','{5}');

                    ASC.CRM.TagSettingsView.init('{0}','{1}');",
                CRMSettingResource.EmptyLabelError.ReplaceSingleQuote(),
                CRMSettingResource.TagAlreadyExistsError.ReplaceSingleQuote(),
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_tags.png", ProductEntryPoint.ID),
                CRMSettingResource.EmptyContentTags.ReplaceSingleQuote(),
                CRMSettingResource.EmptyContentTagsDescript.ReplaceSingleQuote(),
                CRMSettingResource.CreateNewTag.ReplaceSingleQuote() 
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}