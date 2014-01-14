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
using System.Linq;
using System.Collections.Generic;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.Utility;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core.Users;
using ASC.Web.CRM.Controls.Common;
using ASC.Core;
using System.Text;

#endregion

namespace ASC.Web.CRM.Controls.Settings
{
    [AjaxNamespace("AjaxPro.WebToLeadFormView")]
    public partial class WebToLeadFormView : BaseUserControl
    {
        #region Members

        protected String _webFormKey;

        #endregion

        #region Properties

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Settings/WebToLeadFormView.ascx"); } }

        protected string GetHandlerUrl { get { return CommonLinkUtility.ServerRootPath + PathProvider.BaseAbsolutePath + "HttpHandlers/WebToLeadFromHandler.ashx".ToLower(); } }

        protected List<String> TagList { get; set; }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof(WebToLeadFormView));

            _webFormKey = Global.TenantSettings.WebFormKey.ToString();

            RegisterContactFields();
            RegisterScript();


            RegisterClientScriptHelper.DataUserSelectorListView(Page, "_ContactManager", null, null, true);

            RegisterClientScriptHelper.DataUserSelectorListView(
                                        Page,
                                        "_Notify",
                                        new Dictionary<Guid, string>
                                        {
                                             {
                                                 SecurityContext.CurrentAccount.ID,
                                                 SecurityContext.CurrentAccount.Name.HtmlEncode()//CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser").HtmlEncode()
                                              }
                                        },
                                        null,
                                        true);
        }

        [AjaxMethod]
        public String ChangeWebFormKey()
        {
            
            var tenantSettings = Global.TenantSettings;

            tenantSettings.WebFormKey = Guid.NewGuid();

            SettingsManager.Instance.SaveSettings(tenantSettings, TenantProvider.CurrentTenantID);

            return tenantSettings.WebFormKey.ToString();
        }

        protected void RegisterContactFields()
        {
            var columnSelectorData = new[]
                                         {

                                             new
                                             {
                                                  name = "firstName",
                                                  title = CRMContactResource.FirstName
                                             },
                                             new
                                             {
                                                  name = "lastName",
                                                  title = CRMContactResource.LastName
                                                  
                                             },
                                             new
                                             {
                                                  name = "jobTitle",
                                                  title = CRMContactResource.JobTitle
                                                  
                                             },
                                             new
                                             {
                                                  name = "companyName",
                                                  title = CRMContactResource.CompanyName
                                                  
                                             },
                                             new
                                             {
                                                  name = "about",
                                                  title = CRMContactResource.About
                                                  
                                             }
                                         }.ToList();

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
            {

                var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, ContactInfo.GetDefaultCategory(infoTypeEnum));
                var localTitle = infoTypeEnum.ToLocalizedString();

                if (infoTypeEnum == ContactInfoType.Address)
                    foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                        columnSelectorData.Add(new
                        {
                            name = String.Format(localName + "_{0}_{1}", addressPartEnum, (int)AddressCategory.Work),
                            title = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower())
                        });
                else
                    columnSelectorData.Add(new
                    {
                        name = localName,
                        title = localTitle
                    });
            }

            columnSelectorData.AddRange(Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Contact)
            .FindAll(customField => customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea)
                                                          .ConvertAll(customField => new
                                                          {
                                                              name = "customField_" + customField.ID,
                                                              title = customField.Label.HtmlEncode()
                                                          }));

            var tagList = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact);

            if (tagList.Length > 0)
            {
                TagList = tagList.ToList();

                Page.RegisterInlineScript(String.Format(" var tagList = {0}; ",
                                                        JavaScriptSerializer.Serialize(TagList.Select(tagName =>
                                                        new
                                                        {
                                                            name = "tag_" + tagName.HtmlEncode(),
                                                            title = tagName.HtmlEncode()
                                                        }))), onReady: false);
            }

            Page.RegisterInlineScript(String.Format(" var columnSelectorData = {0}; ", JavaScriptSerializer.Serialize(columnSelectorData)), onReady: false);
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.SettingsPage.WebToLeadFormView.init(""{0}"");",
                CommonLinkUtility.GetFullAbsolutePath("~/products/crm/httphandlers/webtoleadfromhandler.ashx")
            );

            Page.RegisterInlineScript(sb.ToString());
        }
    }
}