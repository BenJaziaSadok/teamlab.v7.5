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

using System.Web;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Settings;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;

#endregion

namespace ASC.Web.CRM
{
    public partial class Settings : BasePage
    {
        protected override void PageLoad()
        {
            if (!CRMSecurity.IsAdmin)
                Response.Redirect(PathProvider.StartURL());

            this.Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/crm/masters/SettingsBodyScripts.ascx")));

            var typeValue = (HttpContext.Current.Request["type"] ?? "common").ToLower();
            ListItemView listItemViewControl;

            string titlePage;
            switch (typeValue)
            {
                case "common":
                    CommonContainerHolder.Controls.Add(LoadControl(CommonSettingsView.Location));

                    titlePage = CRMSettingResource.CommonSettings;
                    break;

                case "deal_milestone":
                    var dealMilestoneViewControl = (DealMilestoneView)LoadControl(DealMilestoneView.Location);
                    CommonContainerHolder.Controls.Add(dealMilestoneViewControl);

                    titlePage = CRMDealResource.DealMilestone;
                    break;

                case "task_category":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.TaskCategory;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisCategory;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.CreateNewCategory;
                    listItemViewControl.AddListButtonText = CRMSettingResource.CreateNewCategoryListButton;

                    listItemViewControl.AjaxProgressText = CRMSettingResource.CreateCategoryInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteCategory;
                    listItemViewControl.EditText = CRMSettingResource.EditCategory;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedCategory;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextTaskCategory;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextTaskCategoryEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    titlePage = CRMTaskResource.TaskCategories;
                    break;

                case "history_category":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.HistoryCategory;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisCategory;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.CreateNewCategory;
                    listItemViewControl.AddListButtonText = CRMSettingResource.CreateNewCategoryListButton;
                    listItemViewControl.AjaxProgressText = CRMSettingResource.CreateCategoryInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteCategory;
                    listItemViewControl.EditText = CRMSettingResource.EditCategory;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedCategory;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextHistoryCategory;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextHistoryCategoryEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    titlePage = CRMSettingResource.HistoryCategories;
                    break;

                case "contact_stage":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.ContactStatus;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisStage;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.CreateNewStage;
                    listItemViewControl.AddListButtonText = CRMSettingResource.CreateNewStageListButton;

                    listItemViewControl.AjaxProgressText = CRMSettingResource.CreateContactStageInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteContactStage;
                    listItemViewControl.EditText = CRMSettingResource.EditContactStage;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedContactStage;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextContactStage;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextContactStageEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    titlePage = CRMContactResource.ContactStages;
                    break;

                case "contact_type":
                    listItemViewControl = (ListItemView)LoadControl(ListItemView.Location);
                    listItemViewControl.CurrentTypeValue = ListType.ContactType;
                    listItemViewControl.AddButtonText = CRMSettingResource.AddThisContactType;
                    listItemViewControl.AddPopupWindowText = CRMSettingResource.CreateNewContactType;
                    listItemViewControl.AddListButtonText = CRMSettingResource.CreateNewContactTypeListButton;

                    listItemViewControl.AjaxProgressText = CRMSettingResource.CreateContactTypeInProgressing;
                    listItemViewControl.DeleteText = CRMSettingResource.DeleteContactType;
                    listItemViewControl.EditText = CRMSettingResource.EditContactType;
                    listItemViewControl.EditPopupWindowText = CRMSettingResource.EditSelectedContactType;
                    listItemViewControl.DescriptionText = CRMSettingResource.DescriptionTextContactType;
                    listItemViewControl.DescriptionTextEditDelete = CRMSettingResource.DescriptionTextContactTypeEditDelete;
                    CommonContainerHolder.Controls.Add(listItemViewControl);
                    titlePage = CRMSettingResource.ContactTypes;
                    break;

                case "tag":
                    var tagSettingsViewControl = (TagSettingsView)LoadControl(TagSettingsView.Location);
                    CommonContainerHolder.Controls.Add(tagSettingsViewControl);

                    titlePage = CRMCommonResource.Tags;
                    break;

                case "web_to_lead_form":
                    CommonContainerHolder.Controls.Add(LoadControl(WebToLeadFormView.Location));
                    titlePage = CRMSettingResource.WebToLeadsForm;
                    break;
                case "task_template":
                    CommonContainerHolder.Controls.Add(LoadControl(TaskTemplateView.Location));

                    titlePage = CRMSettingResource.TaskTemplates;
                    break;
                default:
                    typeValue = "custom_field";
                    CommonContainerHolder.Controls.Add(LoadControl(CustomFieldsView.Location));

                    titlePage = CRMSettingResource.CustomFields;
                    break;
            }

            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? titlePage);
        }
    }
}