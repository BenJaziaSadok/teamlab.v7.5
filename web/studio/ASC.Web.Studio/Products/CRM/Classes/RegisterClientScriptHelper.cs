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
using System.IO;
using System.Web;
using System.Web.UI;
using System.Runtime.Serialization.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Thrdparty.Configuration;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM;
using ASC.Web.CRM.Configuration;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using Newtonsoft.Json;

#endregion

namespace ASC.Web.CRM.Classes
{
    public static class RegisterClientScriptHelper
    {
        private static bool IsFacebookSearchEnabled
        {
            get { return !string.IsNullOrEmpty(KeyStorage.Get("facebookAppID")) && !string.IsNullOrEmpty(KeyStorage.Get("facebookAppSecret")); }
        }

        private static bool IsLinkedInSearchEnabled
        {
            get { return !string.IsNullOrEmpty(KeyStorage.Get("linkedInKey")) && !string.IsNullOrEmpty(KeyStorage.Get("linkedInSecret")); }
        }

        private static bool IsTwitterSearchEnabled
        {
            get { return !string.IsNullOrEmpty(KeyStorage.Get("twitterKey")) && !string.IsNullOrEmpty(KeyStorage.Get("twitterSecret")); }
        }


        public static void DataCommon(Page page)
        {
        }

        public static void DataUserSelectorListView(BasePage page, String ObjId, Dictionary<Guid, String> SelectedUsers, List<GroupInfo> SelectedGroups, bool DisabledGroupSelector)
        {
            var ids = SelectedUsers != null && SelectedUsers.Count > 0 ? SelectedUsers.Select(i => i.Key).ToArray() : new List<Guid>().ToArray();
            var names = SelectedUsers != null && SelectedUsers.Count > 0 ? SelectedUsers.Select(i => i.Value).ToArray() : new List<string>().ToArray();

            page.RegisterInlineScript(String.Format(" SelectedUsers{0} = {1}; ",
                                                        ObjId,
                                                        JsonConvert.SerializeObject(
                                                            new
                                                            {
                                                                IDs = ids,
                                                                Names = names,
                                                                PeopleImgSrc = WebImageSupplier.GetAbsoluteWebPath("people_icon.png", ProductEntryPoint.ID),
                                                                DeleteImgSrc = WebImageSupplier.GetAbsoluteWebPath("trash_12.png"),
                                                                DeleteImgTitle = CRMCommonResource.DeleteUser,
                                                                CurrentUserID = SecurityContext.CurrentAccount.ID
                                                            })), onReady: false);
            if (!DisabledGroupSelector)
            {
                ids = SelectedGroups != null && SelectedGroups.Count > 0 ? SelectedGroups.Select(i => i.ID).ToArray() : new List<Guid>().ToArray();
                names = SelectedGroups != null && SelectedGroups.Count > 0 ? SelectedGroups.Select(i => i.Name.HtmlEncode()).ToArray() : new List<string>().ToArray();

                page.RegisterInlineScript(String.Format(" SelectedGroups{0} = {1}; ",
                                                            ObjId,
                                                            JsonConvert.SerializeObject(
                                                                    new
                                                                    {
                                                                        IDs = ids,
                                                                        Names = names,
                                                                        GroupImgSrc = WebImageSupplier.GetAbsoluteWebPath("group_12.png"),
                                                                        TrashImgSrc = WebImageSupplier.GetAbsoluteWebPath("trash_12.png"),
                                                                        TrashImgTitle = CRMCommonResource.Delete
                                                                    })), onReady: false);
            }
        }

        #region Data for History View

        public static void DataHistoryView(BasePage page, List<Guid> UserList)
        {
            if (UserList != null)
            {
                page.RegisterInlineScript(String.Format(" UserList_HistoryUserSelector = {0}; ", JsonConvert.SerializeObject(UserList)), onReady: false);
            }

            RegisterClientScriptHelper.DataUserSelectorListView(page, "_HistoryUserSelector", null, null, true);
        }

        #endregion

        #region Data for Contact Views

        public static void DataListContactTab(BasePage page, Int32 entityID, EntityType entityType)
        {
            page.RegisterInlineScript(String.Format(" var entityData = {0}; ",
                                            JsonConvert.SerializeObject(new
                                            {
                                                id = entityID,
                                                type = entityType.ToString().ToLower()
                                            })), onReady: false);
        }

        public static void DataContactFullCardView(BasePage page, Contact targetContact)
        {
            List<CustomField> data;

            if (targetContact is Company)
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, targetContact.ID, false);
            else
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, targetContact.ID, false);

            var networks =
                Global.DaoFactory.GetContactInfoDao().GetList(targetContact.ID, null, null, null).ConvertAll(
                    n => new
                    {
                        data = n.Data.HtmlEncode(),
                        infoType = n.InfoType,
                        isPrimary = n.IsPrimary,
                        categoryName = n.CategoryToString(),
                        infoTypeLocalName = n.InfoType.ToLocalizedString()
                    });

            String json;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(data.GetType());
                serializer.WriteObject(stream, data);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            var listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactStatus);

            var tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Contact, targetContact.ID);
            var availableTags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact).Where(item => !tags.Contains(item));
            var responsibleIDs = CRMSecurity.GetAccessSubjectGuidsTo(targetContact);

            var script = String.Format(@"
                            var customFieldList = {0};
                            var contactNetworks = {1};
                            var sliderListItems = {2};
                            var imgExst = {3};
                            var contactTags = {4};
                            var contactAvailableTags = {5};
                            var contactResponsibleIDs = {6}; ",
                        json,
                        JsonConvert.SerializeObject(networks),
                        JsonConvert.SerializeObject(new
                        {
                            id = targetContact.ID,
                            status = targetContact.StatusID,
                            positionsCount = listItems.Count,
                            items = listItems.ConvertAll(n => new
                            {
                                id = n.ID,
                                color = n.Color,
                                title = n.Title.HtmlEncode()
                            })
                        }),
                        JsonConvert.SerializeObject(FileUtility.ExtsImage),
                        JsonConvert.SerializeObject(tags.ToList().ConvertAll(t => t.HtmlEncode())),
                        JsonConvert.SerializeObject(availableTags.ToList().ConvertAll(t => t.HtmlEncode())),
                        JsonConvert.SerializeObject(responsibleIDs)
                        );

            page.RegisterInlineScript(script, onReady: false);
        }

        public static void DataContactActionView(BasePage page, Contact targetContact, List<CustomField> data, List<ContactInfo> networks)
        {
            var tags = targetContact != null ? Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Contact, targetContact.ID) : new string[] { };
            var availableTags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Contact).Where(item => !tags.Contains(item));

            String json;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(data.GetType());
                serializer.WriteObject(stream, data);
                json = Encoding.UTF8.GetString(stream.ToArray());
            }

            var listItems = Global.DaoFactory.GetListItemDao().GetItems(ListType.ContactType);

            var presetCompanyForPersonJson = "";
            if (targetContact != null && targetContact is Person && ((Person)targetContact).CompanyID > 0)
            {
                var company = Global.DaoFactory.GetContactDao().GetByID(((Person)targetContact).CompanyID);
                if (company == null)
                {
                    throw new Exception("Can't find parent company for person with ID = " + targetContact.ID);
                }
                else
                {
                    presetCompanyForPersonJson = JsonConvert.SerializeObject(new
                    {
                        id = company.ID,
                        displayName = company.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                        smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(company.ID, true)
                    });
                }
            }

            var presetPersonsForCompanyJson = "";
            if (targetContact != null && targetContact is Company)
            {
                var people = Global.DaoFactory.GetContactDao().GetMembers(targetContact.ID);
                if (people.Count != 0) {
                    presetPersonsForCompanyJson = JsonConvert.SerializeObject(people.ConvertAll(item => new
                        {
                            id = item.ID,
                            displayName = item.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                            smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(item.ID, false)
                        }));
                }
            }

            var script = String.Format(@"
                                var customFieldList = {0};
                                var contactNetworks = {1};
                                var contactActionTags = {2};
                                var contactActionAvailableTags = {3};
                                var imgExst = {4};
                                var contactAvailableTypes = {5};
                                var presetCompanyForPersonJson = '{6}';
                                var presetPersonsForCompanyJson = '{7}';
                                var facebokSearchEnabled = {8};
                                var linkedinSearchEnabled = {9};
                                var twitterSearchEnabled = {10};",
                              json,
                              JsonConvert.SerializeObject(networks),
                              JsonConvert.SerializeObject(tags.ToList().ConvertAll(t => t.HtmlEncode())),
                              JsonConvert.SerializeObject(availableTags.ToList().ConvertAll(t => t.HtmlEncode())),
                              JsonConvert.SerializeObject(FileUtility.ExtsImage),
                              JsonConvert.SerializeObject(
                                    listItems.ConvertAll(n => new
                                    {
                                        id = n.ID,
                                        title = n.Title.HtmlEncode()
                                    })),
                              presetCompanyForPersonJson,
                              presetPersonsForCompanyJson,
                              IsFacebookSearchEnabled.ToString().ToLower(),
                              IsLinkedInSearchEnabled.ToString().ToLower(),
                              IsTwitterSearchEnabled.ToString().ToLower()
                              );

            page.RegisterInlineScript(script, onReady: false);
        }

        #endregion

        #region Data for Task Views

        public static void DataContactDetailsViewForTaskAction(BasePage page, Contact TargetContact)
        {
            var isPrivate = !CRMSecurity.CanAccessTo(TargetContact);
            var contactAccessList = new List<Guid>();
            if (isPrivate)
            {
                contactAccessList = CRMSecurity.GetAccessSubjectTo(TargetContact).Keys.ToList<Guid>();
            }

            page.RegisterInlineScript(String.Format(" var contactForInitTaskActionPanel = {0}; ",
                                                    JsonConvert.SerializeObject(new
                                                    {
                                                        id = TargetContact.ID,
                                                        displayName = TargetContact.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                                                        smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(TargetContact.ID, TargetContact is Company),
                                                        isPrivate = isPrivate,
                                                        accessList = contactAccessList.ConvertAll(n => new { id = n })
                                                    })), onReady: false);
        }

        #endregion

        #region Data for Cases Views

        public static void DataCasesFullCardView(BasePage page, ASC.CRM.Core.Entities.Cases targetCase)
        {
            var customFieldList = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Case, targetCase.ID, false);
            var tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Case, targetCase.ID);
            var availableTags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Case).Where(item => !tags.Contains(item));
            var responsibleIDs = new List<Guid>();
            if (CRMSecurity.IsPrivate(targetCase)) {
                responsibleIDs = CRMSecurity.GetAccessSubjectGuidsTo(targetCase);
            }
            var script = String.Format(@"
                                        var caseTags = {0};
                                        var caseAvailableTags = {1};
                                        var caseResponsibleIDs = {2}; ",
                                    JsonConvert.SerializeObject(tags),
                                    JsonConvert.SerializeObject(availableTags),
                                    JsonConvert.SerializeObject(responsibleIDs));

            page.RegisterInlineScript(script, onReady: false);
            page.JsonPublisher(customFieldList, "casesCustomFieldList");
        }

        public static void DataCasesActionView(BasePage page, ASC.CRM.Core.Entities.Cases targetCase)
        {
            var customFieldList = targetCase != null
                ? Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Case, targetCase.ID, true)
                : Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Case);
            var tags = targetCase != null ? Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Case, targetCase.ID) : new string[] { };
            var availableTags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Case).Where(item => !tags.Contains(item));


            var presetContactsJson = "";
            var selectedContacts = new List<Contact>();
            if (targetCase != null)
            {
                selectedContacts = Global.DaoFactory.GetContactDao().GetContacts(Global.DaoFactory.GetCasesDao().GetMembers(targetCase.ID));
            }
            else
            {
                var URLContactID = UrlParameters.ContactID;
                if (URLContactID != 0)
                {
                    var target = Global.DaoFactory.GetContactDao().GetByID(URLContactID);
                    if (target != null)
                    {
                        selectedContacts.Add(target);
                    }
                }
            }

            if (selectedContacts.Count > 0)
            {
                presetContactsJson = JsonConvert.SerializeObject(selectedContacts.ConvertAll(item => new
                                {
                                    id = item.ID,
                                    displayName = item.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                                    smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(item.ID, item is Company)
                                }));
            }

            var script = String.Format(@"
                                        var casesActionTags = {0};
                                        var casesActionAvailableTags = {1};
                                        var casesActionSelectedContacts = '{2}'; ",
                                    JsonConvert.SerializeObject(tags),
                                    JsonConvert.SerializeObject(availableTags),
                                    presetContactsJson
                                    );

            page.RegisterInlineScript(script, onReady: false);
            page.JsonPublisher(customFieldList, "casesEditCustomFieldList");
        }

        #endregion

        #region Data for Opportunity Views

        public static void DataDealFullCardView(BasePage page, Deal targetDeal)
        {
            var customFieldList = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, targetDeal.ID, false);
            var tags = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Opportunity, targetDeal.ID);
            var availableTags = Global.DaoFactory.GetTagDao().GetAllTags(EntityType.Opportunity).Where(item => !tags.Contains(item));

            var responsibleIDs = new List<Guid>();
            if (CRMSecurity.IsPrivate(targetDeal)) {
                responsibleIDs = CRMSecurity.GetAccessSubjectGuidsTo(targetDeal);
            }

            var script = String.Format(@"
                                            var dealTags = {0};
                                            var dealAvailableTags = {1};
                                            var dealResponsibleIDs = {2}; ",
                                        JsonConvert.SerializeObject(tags),
                                        JsonConvert.SerializeObject(availableTags),
                                        JsonConvert.SerializeObject(responsibleIDs)
                                        );

            page.RegisterInlineScript(script, onReady: false);
            page.JsonPublisher(customFieldList, "customFieldList");
        }

        public static void DataDealActionView(BasePage page, Deal targetDeal)
        {
             var customFieldList = targetDeal != null
                ? Global.DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Opportunity, targetDeal.ID, true)
                : Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(EntityType.Opportunity);
           
            var dealExcludedIDs = new List<Int32>();
            var dealClientIDs = new List<Int32>();
            var dealMembersIDs = new List<Int32>();
            
            if (targetDeal != null)
            {
                dealExcludedIDs = Global.DaoFactory.GetDealDao().GetMembers(targetDeal.ID).ToList();
                dealMembersIDs = new List<int>(dealExcludedIDs);
                if (targetDeal.ContactID != 0) {
                    dealMembersIDs.Remove(targetDeal.ContactID);
                    dealClientIDs.Add(targetDeal.ContactID);
                }
            }


            var presetClientContactsJson = "";
            var presetMemberContactsJson = "";
            var showMembersPanel = false;
            var selectedContacts = new List<Contact>();
            var hasTargetClient = false;

            if (targetDeal != null && targetDeal.ContactID != 0)
            {
                var contact = Global.DaoFactory.GetContactDao().GetByID(targetDeal.ContactID);
                if(contact != null)
                {
                    selectedContacts.Add(contact);
                }
            }
            else
            {
                var URLContactID = UrlParameters.ContactID;
                if (URLContactID != 0)
                {
                    var target = Global.DaoFactory.GetContactDao().GetByID(URLContactID);
                    if (target != null)
                    {
                        selectedContacts.Add(target);
                        hasTargetClient = true;
                    }
                }
            }
            if (selectedContacts.Count > 0)
            {
                presetClientContactsJson = JsonConvert.SerializeObject(selectedContacts.ConvertAll(item => new
                                {
                                    id = item.ID,
                                    displayName = item.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                                    smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(item.ID, item is Company)
                                }));
            }


            selectedContacts = new List<Contact>();
            selectedContacts.AddRange(Global.DaoFactory.GetContactDao().GetContacts(dealMembersIDs.ToArray()));
            if (selectedContacts.Count > 0)
            {
                showMembersPanel = true;
                presetMemberContactsJson = JsonConvert.SerializeObject(selectedContacts.ConvertAll(item => new
                                {
                                    id = item.ID,
                                    displayName = item.GetTitle().HtmlEncode().ReplaceSingleQuote(),
                                    smallFotoUrl = ContactPhotoManager.GetSmallSizePhoto(item.ID, item is Company)
                                }));
            }

            var script = String.Format(@"
                                            var presetClientContactsJson = '{0}';
                                            var presetMemberContactsJson = '{1}';
                                            var hasDealTargetClient = {2};
                                            var showMembersPanel = {3};
                                            var dealClientIDs = {4};
                                            var dealMembersIDs = {5}; ",
                                        presetClientContactsJson,
                                        presetMemberContactsJson,
                                        hasTargetClient.ToString().ToLower(),
                                        showMembersPanel.ToString().ToLower(),
                                        JsonConvert.SerializeObject(dealClientIDs),
                                        JsonConvert.SerializeObject(dealMembersIDs)
                                    );

            page.RegisterInlineScript(script, onReady: false);
            page.JsonPublisher(customFieldList, "customFieldList");
            page.JsonPublisher(Global.DaoFactory.GetDealMilestoneDao().GetAll(), "dealMilestones");

            if (targetDeal != null) {
                page.JsonPublisher(targetDeal, "targetDeal");
            }
        }

        #endregion

        #region Data for Settings Pages View

        public static void DataCustomFieldsView(BasePage page, EntityType entityType)
        {
            var customFieldList = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(entityType);
            page.JsonPublisher(customFieldList, "customFieldList");
            page.RegisterInlineScript(String.Format(" var relativeItems = {0}; ", Global.DaoFactory.GetCustomFieldDao().GetContactLinkCountJSON(entityType)), onReady: false);
        }

        public static void DataTagSettingsView(BasePage page, EntityType entityType)
        {
            var tagList = Global.DaoFactory.GetTagDao().GetAllTags(entityType).ToList();
            var relativeItemsCountArrayJSON = Global.DaoFactory.GetTagDao().GetTagsLinkCountJSON(entityType);

            var script = String.Format(@"
                                        var tagList = {0};
                                        var relativeItemsCountArray = {1}; ",
                                    JsonConvert.SerializeObject(tagList.ConvertAll(item => new { value = item.HtmlEncode() })),
                                    relativeItemsCountArrayJSON);

            page.RegisterInlineScript(script, onReady: false);
        }

        public static void DataDealMilestoneView(BasePage page)
        {
            var apiServer = new Api.ApiServer();
            var data = apiServer.GetApiResponse(String.Format("{0}crm/opportunity/stage.json", SetupInfo.WebApiBaseUrl), "GET");

            page.JsonPublisher(data, "dealMilestoneList");
        }


        public static void DataListItemView(BasePage page, ListType currentTypeValue)
        {
            var query = "history/category";
            switch (currentTypeValue)
            {
                case ListType.ContactStatus:
                    query = "contact/status";
                    break;
                case ListType.ContactType:
                    query = "contact/type";
                    break;
                case ListType.HistoryCategory:
                    query = "history/category";
                    break;
                case ListType.TaskCategory:
                    query = "task/category";
                    break;
            }

            var apiServer = new Api.ApiServer();
            var data = apiServer.GetApiResponse(String.Format("{0}crm/{1}.json", SetupInfo.WebApiBaseUrl, query), "GET");

            page.JsonPublisher(data, "itemList");
        }

        #endregion

    }
}