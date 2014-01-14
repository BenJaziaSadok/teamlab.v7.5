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
using ASC.Web.CRM.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Controls.Common;
using System.Text;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class CustomFieldsView : BaseUserControl
    {
        #region Property

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Settings/CustomFieldsView.ascx"); }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            EntityType entityType;

            var view = Request["view"];

            switch (view)
            {
                case "person":
                    entityType = EntityType.Person;
                    break;
                case "company":
                    entityType = EntityType.Company;
                    break;
                case "opportunity":
                    entityType = EntityType.Opportunity;
                    break;
                case "case":
                    entityType = EntityType.Case;
                    break;
                default:
                    entityType = EntityType.Contact;
                    break;
            }

            _manageFieldPopup.Options.IsPopup = true;

            _switcherEntityType.SortItemsHeader = CRMCommonResource.Show + ":";

            _switcherEntityType.SortItems[0].SortLabel = CRMSettingResource.BothPersonAndCompany;
            _switcherEntityType.SortItems[0].SortUrl = "settings.aspx?type=custom_field";
            _switcherEntityType.SortItems[0].IsSelected = entityType == EntityType.Contact;

            _switcherEntityType.SortItems[1].SortLabel = CRMSettingResource.JustForPerson;
            _switcherEntityType.SortItems[1].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}",
                                                                     EntityType.Person.ToString().ToLower());
            _switcherEntityType.SortItems[1].IsSelected = entityType == EntityType.Person;

            _switcherEntityType.SortItems[2].SortLabel = CRMSettingResource.JustForCompany;
            _switcherEntityType.SortItems[2].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}",
                                                                     EntityType.Company.ToString().ToLower());
            _switcherEntityType.SortItems[2].IsSelected = entityType == EntityType.Company;

            _switcherEntityType.SortItems[3].SortLabel = CRMCommonResource.DealModuleName;
            _switcherEntityType.SortItems[3].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}", EntityType.Opportunity.ToString().ToLower());
            _switcherEntityType.SortItems[3].IsSelected = entityType == EntityType.Opportunity;

            _switcherEntityType.SortItems[4].SortLabel = CRMCommonResource.CasesModuleName;
            _switcherEntityType.SortItems[4].SortUrl = String.Format("settings.aspx?type=custom_field&view={0}", EntityType.Case.ToString().ToLower());
            _switcherEntityType.SortItems[4].IsSelected = entityType == EntityType.Case;

            RegisterClientScriptHelper.DataCustomFieldsView(Page, entityType);
            RegisterScript();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.SettingsPage.initEmptyScreen('{6}','{7}','{8}','{9}');

                    ASC.CRM.SettingsPage.AddCustomFieldHeaderText = '{10}';
                    ASC.CRM.SettingsPage.AddCustomFieldButtonText = '{11}';
                    ASC.CRM.SettingsPage.AddCustomFieldProcessText = '{12}';
                    ASC.CRM.SettingsPage.EditCustomFieldProcessText = '{13}';
                    ASC.CRM.SettingsPage.init({0},{1},{2},{3},{4},{5});",
                Global.DefaultCustomFieldSize,
                Global.DefaultCustomFieldRows,
                Global.DefaultCustomFieldCols,
                Global.MaxCustomFieldSize,
                Global.MaxCustomFieldRows,
                Global.MaxCustomFieldCols,
                WebImageSupplier.GetAbsoluteWebPath("empty_screen_userfields.png", ProductEntryPoint.ID),
                CRMSettingResource.EmptyContentCustomFields.ReplaceSingleQuote(),
                CRMSettingResource.EmptyContentCustomFieldsDescript.ReplaceSingleQuote(),
                CRMSettingResource.CreateCustomField.ReplaceSingleQuote(),
                CRMSettingResource.CreateNewField.ReplaceSingleQuote(),
                CRMSettingResource.AddThisField.ReplaceSingleQuote(),
                CRMSettingResource.CreateFieldInProgressing.ReplaceSingleQuote(),
                CRMCommonResource.SaveChangesProggress.ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}