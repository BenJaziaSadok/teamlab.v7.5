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
using ASC.Web.CRM.Resources;
using ASC.CRM.Core;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;
using System.Text;
using ASC.Web.Studio.Controls.Users;
using ASC.Web.Studio.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Users;

namespace ASC.Web.CRM.Controls.Settings
{
    public partial class TaskTemplateView : BaseUserControl
    {
        #region Properties

        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Settings/TaskTemplateView.ascx"); }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            _templateConatainerPanel.Options.IsPopup = true;
            _templatePanel.Options.IsPopup = true;

            var entityType = StringToEntityType(Request["view"]);

            InitViewSwitcher(entityType);

            RegisterClientScript(entityType);

            //var cntrlCategorySelector = (CategorySelector)LoadControl(CategorySelector.Location);
            //cntrlCategorySelector.Categories = Global.DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory);
            //cntrlCategorySelector.ID = "taskTemplateCategorySelector";
            //phCategorySelector.Controls.Add(cntrlCategorySelector);

            var emptyScreenControl = new EmptyScreenControl
                {
                    ImgSrc = WebImageSupplier.GetAbsoluteWebPath("empty_screen_tasks.png", ProductEntryPoint.ID),
                    Header = CRMSettingResource.EmptyContentTaskTemplates,
                    Describe = CRMSettingResource.EmptyContentTaskTemplatesDescript,
                    ButtonHTML = String.Format("<a id='addTag' class='link dotline blue plus' onclick='ASC.CRM.TaskTemplateView.showTemplateConatainerPanel();'>{0}</a>",
                                               CRMSettingResource.AddTaskTemplateContainer)
                };

            _phEmptyContent.Controls.Add(emptyScreenControl);

            //init task responsible
            var taskTemplateResponsibleSelector = new AdvancedUserSelector
                {
                    ID = "taskTemplateResponsibleSelector",
                    IsLinkView = false,
                    EmployeeType = EmployeeType.User,
                    LinkText = CustomNamingPeople.Substitute<CRMCommonResource>("AddUser"),
                    ParentContainerHtmlSelector = "#taskTemplateViewAdvUsrSrContainer"
                };
            AdvancedUserSelector.RegisterStartupScripts(Page, taskTemplateResponsibleSelector);

            RegisterScript();
        }

        #endregion

        #region Methods

        private static EntityType StringToEntityType(string type)
        {
            switch (type)
            {
                case "person":
                    return EntityType.Person;
                case "company":
                    return EntityType.Company;
                case "opportunity":
                    return EntityType.Opportunity;
                case "case":
                    return EntityType.Case;
                default:
                    return EntityType.Contact;
            }
        }

        private void InitViewSwitcher(EntityType entityType)
        {
            _switcherEntityType.SortItemsHeader = CRMCommonResource.Show + ":";

            _switcherItemAllContacts.SortLabel = CRMSettingResource.BothPersonAndCompany;
            _switcherItemAllContacts.SortUrl = "settings.aspx?type=task_template";
            _switcherItemAllContacts.IsSelected = entityType == EntityType.Contact;

            _switcherItemPersons.SortLabel = CRMSettingResource.JustForPerson;
            _switcherItemPersons.SortUrl = String.Format("settings.aspx?type=task_template&view={0}", EntityType.Person.ToString().ToLower());
            _switcherItemPersons.IsSelected = entityType == EntityType.Person;

            _switcherItemCompanies.SortLabel = CRMSettingResource.JustForCompany;
            _switcherItemCompanies.SortUrl = String.Format("settings.aspx?type=task_template&view={0}", EntityType.Company.ToString().ToLower());
            _switcherItemCompanies.IsSelected = entityType == EntityType.Company;

            _switcherItemDeals.SortLabel = CRMCommonResource.DealModuleName;
            _switcherItemDeals.SortUrl = String.Format("settings.aspx?type=task_template&view={0}", EntityType.Opportunity.ToString().ToLower());
            _switcherItemDeals.IsSelected = entityType == EntityType.Opportunity;

            _switcherItemrCases.SortLabel = CRMCommonResource.CasesModuleName;
            _switcherItemrCases.SortUrl = String.Format("settings.aspx?type=task_template&view={0}", EntityType.Case.ToString().ToLower());
            _switcherItemrCases.IsSelected = entityType == EntityType.Case;
        }

        private void RegisterClientScript(EntityType entityType)
        {
            var apiServer = new Api.ApiServer();
            var templateConatainerList = apiServer.GetApiResponse(
                String.Format("{0}crm/{1}/tasktemplatecontainer.json", SetupInfo.WebApiBaseUrl, entityType.ToString().ToLower()), "GET");

            Page.JsonPublisher(templateConatainerList, "templateConatainerList");

        }

        protected string InitHoursSelect()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < 24; i++)
            {
                sb.AppendFormat("<option value='{0}' id='optDeadlineHours_{0}'>{1}</option>", i, i < 10 ? "0" + i.ToString() : i.ToString());
            }

            return sb.ToString();
        }

        protected string InitMinutesSelect()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < 60; i++)
            {
                sb.AppendFormat("<option value='{0}' id='optDeadlineMinutes_{0}'>{1}</option>", i, i < 10 ? "0" + i.ToString() : i.ToString());
            }

            return sb.ToString();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"
                    ASC.CRM.TaskTemplateView.init();

                    ASC.CRM.TaskTemplateView.ConatainerPanel_AddHeaderText = '{0}';
                    ASC.CRM.TaskTemplateView.ConatainerPanel_AddButtonText = '{1}';
                    ASC.CRM.TaskTemplateView.ConatainerPanel_EditHeaderText = '{2}';

                    ASC.CRM.TaskTemplateView.TemplatePanel_AddHeaderText = '{3}';
                    ASC.CRM.TaskTemplateView.TemplatePanel_AddButtonText = '{4}';
                    ASC.CRM.TaskTemplateView.TemplatePanel_EditHeaderText = '{5}';",
                CRMSettingResource.AddNewTaskTemplateContainer.ReplaceSingleQuote(),
                CRMSettingResource.AddThisTaskTemplateContainer.ReplaceSingleQuote(),
                CRMSettingResource.EditSelectedTaskTemplateContainer.ReplaceSingleQuote(),
                CRMSettingResource.AddNewTaskTemplate.ReplaceSingleQuote(),
                CRMSettingResource.AddThisTaskTemplate.ReplaceSingleQuote(),
                CRMSettingResource.EditSelectedTaskTemplate.ReplaceSingleQuote()
            );

            Page.RegisterInlineScript(sb.ToString());
        }

        #endregion
    }
}