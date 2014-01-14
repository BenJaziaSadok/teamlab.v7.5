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
using System.Linq;
using System.Text;
using System.Web;
using ASC.Core.Common.Logging;
using ASC.Web.Studio.Controls.FileUploader;
using AjaxPro;
using ASC.CRM.Core;
using ASC.Web.Core.Utility;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.Common.Threading.Progress;
using System.Collections.Generic;
using ASC.Web.Studio.Core.Users;
using ASC.Core;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

namespace ASC.Web.CRM.Controls.Common
{
    public class ImportFileHandler : IFileUploadHandler
    {
        public FileUploadResult ProcessUpload(HttpContext context)
        {
            var fileUploadResult = new FileUploadResult();

            if (!ProgressFileUploader.HasFilesToUpload(context)) return fileUploadResult;

            var file = new ProgressFileUploader.FileToUpload(context);

            String assignedPath;

            Global.GetStore().SaveTemp("temp", out assignedPath, file.InputStream);

            file.InputStream.Position = 0;

            var jObject = ImportFromCSV.GetInfo(file.InputStream, context.Request["importSettings"]);

            jObject.Add("assignedPath", assignedPath);

            fileUploadResult.Success = true;
            fileUploadResult.Data = Convert.ToBase64String(Encoding.UTF8.GetBytes(jObject.ToString()));

            return fileUploadResult;
        }
    }

    [AjaxNamespace("AjaxPro.Utils.ImportFromCSV")]
    public partial class ImportFromCSVView : BaseUserControl
    {
        #region Property

        public static String Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Common/ImportFromCSVView.ascx"); }
        }

        public EntityType EntityType;

        protected String ImportFromCSVStepOneHeaderLabel;
        protected String ImportFromCSVStepTwoHeaderLabel;

        protected String ImportFromCSVStepOneDescriptionLabel;
        protected String ImportFromCSVStepTwoDescriptionLabel;

        protected String StartImportLabel;

        protected String ImportStartingPanelHeaderLabel;
        protected String ImportStartingPanelDescriptionLabel;
        protected String ImportStartingPanelButtonLabel;

        protected String GoToRedirectURL;

        protected String ImportImgSrc;

        #endregion


        #region Ajax Methods

        [AjaxMethod]
        public void StartImport(EntityType entityType, String CSVFileURI, String importSettingsJSON)
        {
            ImportFromCSV.Start(entityType, CSVFileURI, importSettingsJSON);
            AdminLog.PostAction("CRM: started import operation of type \"{0}\" with settings {1}>", entityType, importSettingsJSON);
        }

        [AjaxMethod]
        public IProgressItem GetStatus(EntityType entityType)
        {
            return ImportFromCSV.GetStatus(entityType);
        }

        [AjaxMethod]
        public String GetSampleRow(String CSVFileURI, int indexRow, String jsonSettings)
        {
            if (String.IsNullOrEmpty(CSVFileURI) || indexRow < 0)
                throw new ArgumentException();

            if (!Global.GetStore().IsFile("temp", CSVFileURI))
                throw new ArgumentException();

            var CSVFileStream = Global.GetStore().GetReadStream("temp", CSVFileURI);

            return ImportFromCSV.GetRow(CSVFileStream, indexRow, jsonSettings);
        }

        public EncodingInfo[] GetEncodings()
        {
            return Encoding.GetEncodings().OrderBy(x => x.DisplayName).ToArray();
        }

        #endregion

        #region Events

        protected void InitForContacts()
        {
            StartImportLabel = CRMContactResource.StartImport;

            ImportFromCSVStepOneHeaderLabel = CRMContactResource.ImportFromCSVStepOneHeader;
            ImportFromCSVStepTwoHeaderLabel = CRMContactResource.ImportFromCSVStepTwoHeader;

            ImportFromCSVStepOneDescriptionLabel = CRMContactResource.ImportFromCSVStepOneDescription;
            ImportFromCSVStepTwoDescriptionLabel = CRMContactResource.ImportFromCSVStepTwoDescription;


            ImportStartingPanelHeaderLabel = CRMContactResource.ImportStartingPanelHeader;
            ImportStartingPanelDescriptionLabel = CRMContactResource.ImportStartingPanelDescription;
            ImportStartingPanelButtonLabel = CRMContactResource.ImportStartingPanelButton;

            ImportImgSrc = WebImageSupplier.GetAbsoluteWebPath("import_contacts.png", ProductEntryPoint.ID);

            var columnSelectorData = new[]
                {
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.NoMatchSelect,
                            isHeader = false
                        },
                    new
                        {
                            name = "-1",
                            title = CRMContactResource.DoNotImportThisField,
                            isHeader = false
                        },
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.GeneralInformation,
                            isHeader = true
                        },
                    new
                        {
                            name = "firstName",
                            title = CRMContactResource.FirstName,
                            isHeader = false
                        },
                    new
                        {
                            name = "lastName",
                            title = CRMContactResource.LastName,
                            isHeader = false
                        },
                    new
                        {
                            name = "title",
                            title = CRMContactResource.JobTitle,
                            isHeader = false
                        },
                    new
                        {
                            name = "companyName",
                            title = CRMContactResource.CompanyName,
                            isHeader = false
                        },
                    new
                        {
                            name = "contactStage",
                            title = CRMContactResource.ContactStage,
                            isHeader = false
                        },
                    new
                        {
                            name = "contactType",
                            title = CRMContactResource.ContactType,
                            isHeader = false
                        },
                    new
                        {
                            name = "notes",
                            title = CRMContactResource.About,
                            isHeader = false
                        }
                }.ToList();

            foreach (ContactInfoType infoTypeEnum in Enum.GetValues(typeof(ContactInfoType)))
                foreach (Enum categoryEnum in Enum.GetValues(ContactInfo.GetCategory(infoTypeEnum)))
                {

                    var localName = String.Format("contactInfo_{0}_{1}", infoTypeEnum, Convert.ToInt32(categoryEnum));
                    var localTitle = String.Format("{1} ({0})", categoryEnum.ToLocalizedString().ToLower(), infoTypeEnum.ToLocalizedString());

                    if (infoTypeEnum == ContactInfoType.Address)
                        foreach (AddressPart addressPartEnum in Enum.GetValues(typeof(AddressPart)))
                            columnSelectorData.Add(new
                                {
                                    name = String.Format(localName + "_{0}", addressPartEnum),
                                    title = String.Format(localTitle + " {0}", addressPartEnum.ToLocalizedString().ToLower()),
                                    isHeader = false
                                });
                    else
                        columnSelectorData.Add(new
                            {
                                name = localName,
                                title = localTitle,
                                isHeader = false
                            });
                }

            var fieldsDescription = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Company);

            Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Person).ForEach(item =>
                                                                                                      {
                                                                                                          var alreadyContains = fieldsDescription.Any(field => field.ID == item.ID);

                                                                                                          if (!alreadyContains)
                                                                                                              fieldsDescription.Add(item);
                                                                                                      });

            columnSelectorData.AddRange(fieldsDescription.FindAll(customField => customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea || customField.FieldType == CustomFieldType.Heading)
                                                         .ConvertAll(customField => new
                                                             {
                                                                 name = "customField_" + customField.ID,
                                                                 title = customField.Label.HtmlEncode(),
                                                                 isHeader = customField.FieldType == CustomFieldType.Heading
                                                             }));

            columnSelectorData.AddRange(
                new[]
                    {
                        new
                            {
                                name = String.Empty,
                                title = CRMContactResource.ContactTags,
                                isHeader = true
                            },
                        new
                            {
                                name = "tag",
                                title = CRMContactResource.ContactTagList,
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 1),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 2),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 3),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 4),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 5),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 6),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 7),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 8),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 9),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMContactResource.ContactTag, 10),
                                isHeader = false
                            },

                    }.ToList()
                );

            Page.RegisterInlineScript(String.Format(" var columnSelectorData = {0}; ", JavaScriptSerializer.Serialize(columnSelectorData)), onReady: false);

            RegisterClientScriptHelper.DataUserSelectorListView(Page, "_ImportContactsManager", null, null, true);
        }

        protected void InitForOpportunity()
        {
            StartImportLabel = CRMDealResource.StartImport;

            ImportFromCSVStepOneHeaderLabel = CRMDealResource.ImportFromCSVStepOneHeader;
            ImportFromCSVStepTwoHeaderLabel = CRMDealResource.ImportFromCSVStepTwoHeader;

            ImportFromCSVStepOneDescriptionLabel = CRMDealResource.ImportFromCSVStepOneDescription;
            ImportFromCSVStepTwoDescriptionLabel = CRMDealResource.ImportFromCSVStepTwoDescription;

            // 
            // ImportFromCSVStepTwoDescription


            ImportStartingPanelHeaderLabel = CRMDealResource.ImportStartingPanelHeader;
            ImportStartingPanelDescriptionLabel = CRMDealResource.ImportStartingPanelDescription;
            ImportStartingPanelButtonLabel = CRMDealResource.ImportStartingPanelButton;

            ImportImgSrc = WebImageSupplier.GetAbsoluteWebPath("import-opportunities.png", ProductEntryPoint.ID);

            var columnSelectorData = new[]
                {
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.NoMatchSelect,
                            isHeader = false
                        },
                    new
                        {
                            name = "-1",
                            title = CRMContactResource.DoNotImportThisField,
                            isHeader = false
                        },
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.GeneralInformation,
                            isHeader = true
                        },
                    new
                        {
                            name = "title",
                            title = CRMDealResource.NameDeal,
                            isHeader = false
                        },
                    new
                        {
                            name = "client",
                            title = CRMDealResource.ClientDeal,
                            isHeader = false
                        },
                    new
                        {
                            name = "description",
                            title = CRMDealResource.DescriptionDeal,
                            isHeader = false
                        },

                    new
                        {
                            name = "bid_currency",
                            title = CRMCommonResource.Currency,
                            isHeader = false
                        },

                    new
                        {
                            name = "bid_amount",
                            title = CRMDealResource.DealAmount,
                            isHeader = false
                        },
                    new
                        {
                            name = "bid_type",
                            title = CRMDealResource.BidType,
                            isHeader = false
                        },
                    new
                        {
                            name = "per_period_value",
                            title = CRMDealResource.BidTypePeriod,
                            isHeader = false
                        },
                    new
                        {
                            name = "responsible",
                            title = CRMDealResource.ResponsibleDeal,
                            isHeader = false
                        },
                    new
                        {
                            name = "expected_close_date",
                            title = CRMJSResource.ExpectedCloseDate,
                            isHeader = false
                        },
                    new
                        {
                            name = "actual_close_date",
                            title = CRMJSResource.ActualCloseDate,
                            isHeader = false
                        },
                    new
                        {
                            name = "deal_milestone",
                            title = CRMDealResource.CurrentDealMilestone,
                            isHeader = false
                        },
                    new
                        {
                            name = "probability_of_winning",
                            title = CRMDealResource.ProbabilityOfWinning + " %",
                            isHeader = false
                        }
                }.ToList();


            columnSelectorData.AddRange(Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Opportunity).FindAll(customField => customField.FieldType == CustomFieldType.Date || customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea || customField.FieldType == CustomFieldType.Heading)
                                              .ConvertAll(customField => new
                                                  {
                                                      name = "customField_" + customField.ID,
                                                      title = customField.Label.HtmlEncode(),
                                                      isHeader = customField.FieldType == CustomFieldType.Heading
                                                  }));
            columnSelectorData.AddRange(
                new[]
                    {
                        new
                            {
                                name = String.Empty,
                                title = CRMDealResource.DealParticipants,
                                isHeader = true
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 1),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 2),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 3),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 4),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 5),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 6),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 7),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 8),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 9),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMDealResource.DealParticipant, 10),
                                isHeader = false
                            }
                    });

            columnSelectorData.AddRange(
                new[]
                    {
                        new
                            {
                                name = String.Empty,
                                title = CRMDealResource.DealTags,
                                isHeader = true
                            },
                        new
                            {
                                name = "tag",
                                title = CRMDealResource.DealTagList,
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 1),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 2),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 3),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 4),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 5),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 6),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 7),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 8),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 9),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMDealResource.DealTag, 10),
                                isHeader = false
                            },

                    }
                );

            Page.RegisterInlineScript(String.Format(" var columnSelectorData = {0}; ", JavaScriptSerializer.Serialize(columnSelectorData)), onReady: false);

            var privatePanel = (PrivatePanel)Page.LoadControl(PrivatePanel.Location);
            privatePanel.CheckBoxLabel = CRMDealResource.PrivatePanelCheckBoxLabel;
            privatePanel.IsPrivateItem = false;

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser") };
            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);

        }

        protected void InitForTask()
        {
            StartImportLabel = CRMTaskResource.StartImport;

            ImportFromCSVStepOneHeaderLabel = CRMTaskResource.ImportFromCSVStepOneHeader;
            ImportFromCSVStepTwoHeaderLabel = CRMTaskResource.ImportFromCSVStepTwoHeader;

            ImportFromCSVStepOneDescriptionLabel = CRMTaskResource.ImportFromCSVStepOneDescription;
            ImportFromCSVStepTwoDescriptionLabel = CRMTaskResource.ImportFromCSVStepTwoDescription;

            ImportStartingPanelHeaderLabel = CRMTaskResource.ImportStartingPanelHeader;
            ImportStartingPanelDescriptionLabel = CRMTaskResource.ImportStartingPanelDescription;
            ImportStartingPanelButtonLabel = CRMTaskResource.ImportStartingPanelButton;

            ImportImgSrc = WebImageSupplier.GetAbsoluteWebPath("import-tasks.png", ProductEntryPoint.ID);

            var columnSelectorData = new[]
                {
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.NoMatchSelect,
                            isHeader = false
                        },
                    new
                        {
                            name = "-1",
                            title = CRMContactResource.DoNotImportThisField,
                            isHeader = false
                        },
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.GeneralInformation,
                            isHeader = true
                        },
                    new
                        {
                            name = "title",
                            title = CRMTaskResource.TaskTitle,
                            isHeader = false
                        },
                    new
                        {
                            name = "description",
                            title = CRMTaskResource.Description,
                            isHeader = false
                        },
                    new
                        {
                            name = "due_date",
                            title = CRMTaskResource.DueDate,
                            isHeader = false
                        },
                    new
                        {
                            name = "responsible",
                            title = CRMTaskResource.Responsible,
                            isHeader = false
                        },
                    new
                        {
                            name = "contact",
                            title = CRMContactResource.ContactTitle,
                            isHeader = false
                        },
                    new
                        {
                            name = "status",
                            title = CRMTaskResource.TaskStatus,
                            isHeader = false
                        },
                    new
                        {
                            name = "taskCategory",
                            title = CRMTaskResource.TaskCategory,
                            isHeader = false
                        },
                    new
                        {
                            name = "alertValue",
                            title = CRMCommonResource.Alert,
                            isHeader = false
                        }

                }.ToList();

            Page.RegisterInlineScript(String.Format(" var columnSelectorData = {0}; ", JavaScriptSerializer.Serialize(columnSelectorData)), onReady: false);
        }

        protected void InitForCase()
        {
            StartImportLabel = CRMCasesResource.StartImport;

            ImportFromCSVStepOneHeaderLabel = CRMCasesResource.ImportFromCSVStepOneHeader;
            ImportFromCSVStepTwoHeaderLabel = CRMCasesResource.ImportFromCSVStepTwoHeader;

            ImportFromCSVStepOneDescriptionLabel = CRMCasesResource.ImportFromCSVStepOneDescription;
            ImportFromCSVStepTwoDescriptionLabel = CRMCasesResource.ImportFromCSVStepTwoDescription;

            ImportStartingPanelHeaderLabel = CRMCasesResource.ImportStartingPanelHeader;
            ImportStartingPanelDescriptionLabel = CRMCasesResource.ImportStartingPanelDescription;
            ImportStartingPanelButtonLabel = CRMCasesResource.ImportStartingPanelButton;

            ImportImgSrc = WebImageSupplier.GetAbsoluteWebPath("import-cases.png", ProductEntryPoint.ID);

            var columnSelectorData = new[]
                {
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.NoMatchSelect,
                            isHeader = false
                        },
                    new
                        {
                            name = "-1",
                            title = CRMContactResource.DoNotImportThisField,
                            isHeader = false
                        },
                    new
                        {
                            name = String.Empty,
                            title = CRMContactResource.GeneralInformation,
                            isHeader = true
                        },
                    new
                        {
                            name = "title",
                            title = CRMCasesResource.CaseTitle,
                            isHeader = false
                        }
                }.ToList();

            columnSelectorData.AddRange(Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Case).FindAll(customField => customField.FieldType == CustomFieldType.Date || customField.FieldType == CustomFieldType.TextField || customField.FieldType == CustomFieldType.TextArea || customField.FieldType == CustomFieldType.Heading)
                                              .ConvertAll(customField => new
                                                  {
                                                      name = "customField_" + customField.ID,
                                                      title = customField.Label.HtmlEncode(),
                                                      isHeader = customField.FieldType == CustomFieldType.Heading
                                                  }));

            columnSelectorData.AddRange(
                new[]
                    {
                        new
                            {
                                name = String.Empty,
                                title = CRMCasesResource.CasesParticipants,
                                isHeader = true
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 1),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 2),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 3),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 4),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 5),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 6),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 7),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 8),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 9),
                                isHeader = false
                            },
                        new
                            {
                                name = "member",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesParticipant, 10),
                                isHeader = false
                            }
                    });

            columnSelectorData.AddRange(
                new[]
                    {
                        new
                            {
                                name = String.Empty,
                                title = CRMCasesResource.CasesTag,
                                isHeader = true
                            },
                        new
                            {
                                name = "tag",
                                title = CRMCasesResource.CasesTagList,
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 1),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 2),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 3),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 4),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 5),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 6),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 7),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 8),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 9),
                                isHeader = false
                            },
                        new
                            {
                                name = "tag",
                                title = String.Format("{0} {1}", CRMCasesResource.CasesTag, 10),
                                isHeader = false
                            }
                    }
                );

            Page.RegisterInlineScript(String.Format(" var columnSelectorData = {0}; ", JavaScriptSerializer.Serialize(columnSelectorData)), onReady: false);

            var privatePanel = (PrivatePanel)Page.LoadControl(PrivatePanel.Location);
            privatePanel.CheckBoxLabel = CRMCasesResource.PrivatePanelCheckBoxLabel;
            privatePanel.IsPrivateItem = false;

            var usersWhoHasAccess = new List<string> { CustomNamingPeople.Substitute<CRMCommonResource>("CurrentUser") };

            privatePanel.UsersWhoHasAccess = usersWhoHasAccess;
            privatePanel.DisabledUsers = new List<Guid> { SecurityContext.CurrentAccount.ID };
            privatePanel.HideNotifyPanel = true;
            _phPrivatePanel.Controls.Add(privatePanel);
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Utility.RegisterTypeForAjax(typeof(ImportFromCSVView));

            switch (EntityType)
            {

                case EntityType.Contact:
                    GoToRedirectURL = "default.aspx";
                    InitForContacts();
                    break;
                case EntityType.Opportunity:
                    GoToRedirectURL = "deals.aspx";
                    InitForOpportunity();
                    break;
                case EntityType.Task:
                    GoToRedirectURL = "tasks.aspx";
                    InitForTask();
                    break;
                case EntityType.Case:
                    GoToRedirectURL = "cases.aspx";
                    InitForCase();
                    break;

            }

            RegisterScript();
        }

        private void RegisterScript()
        {
            var script = @"ASC.CRM.ImportFromCSVView.init(" + (int)EntityType + @");";

            Page.RegisterInlineScript(script);
        }

        #endregion

    }
}