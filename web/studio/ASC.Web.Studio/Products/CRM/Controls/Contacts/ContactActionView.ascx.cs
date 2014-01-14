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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using Newtonsoft.Json.Linq;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.CRM.Configuration;

#endregion

namespace ASC.Web.CRM.Controls.Contacts
{

    public partial class ContactActionView : BaseUserControl
    {
        #region Properies

        public static string Location { get { return PathProvider.GetFileStaticRelativePath("Contacts/ContactActionView.ascx"); } }
        public Contact TargetContact { get; set; }
        public String TypeAddedContact { get; set; }

        public String SaveContactButtonText { get; set; }
        public String SaveAndCreateContactButtonText { get; set; }

        public String AjaxProgressText { get; set; }

        protected List<Int32> OtherCompaniesID { get; set; }

        protected List<ContactInfoType> ContactInfoTypes { get; set; }
        protected bool MobileVer = false;

        protected bool IsCrunchBaseSearchEnabled
        {
            get { return !string.IsNullOrEmpty(KeyStorage.Get("crunchBaseKey")); }
        }

        

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            MobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(this.Context);

            var country = new List<string> {CRMJSResource.ChooseCountry};
            
            var enUs = CultureInfo.GetCultureInfo("en-US");
            var additionalCountries = new List<string>
                                        {
                                            CRMCommonResource.ResourceManager.GetString("Country_Gambia", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_Ghana", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_RepublicOfCyprus", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_SierraLeone", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_Tanzania", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_Zambia", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_RepublicOfMadagascar", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_SolomonIslands", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_RepublicOfMoldova", enUs),
                                            CRMCommonResource.ResourceManager.GetString("Country_RepublicOfMauritius", enUs)
                                        };

            var standardCountries = Global.GetCountryList();
            standardCountries.AddRange(additionalCountries);
            country.AddRange(standardCountries.OrderBy(c => c));

            contactCountry.DataSource = country;

            contactCountry.Name = "contactInfo_Address_" + (int)ContactInfoBaseCategory.Work + "_"  + AddressPart.Country;
            contactCountry.DataBind();

            ContactInfoTypes = (from ContactInfoType item in Enum.GetValues(typeof(ContactInfoType))
                                where (item != ContactInfoType.Address && item != ContactInfoType.Email &&
                                item != ContactInfoType.Phone)
                                select item).ToList();


            saveContactButton.Text = SaveContactButtonText;
            saveContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.submitForm('{0}');", saveContactButton.UniqueID);

            saveAndCreateContactButton.Text = SaveAndCreateContactButtonText;
            saveAndCreateContactButton.OnClientClick = String.Format("return ASC.CRM.ContactActionView.submitForm('{0}');", saveAndCreateContactButton.UniqueID);


            List<CustomField> data;
            var networks = new List<ContactInfo>();
            if (TargetContact == null)
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetFieldsDescription(UrlParameters.Type != "people" ? EntityType.Company : EntityType.Person);

                var URLEmail = UrlParameters.Email;
                if (!String.IsNullOrEmpty(URLEmail))
                    networks.Add(new ContactInfo()
                            {
                               Category = (int) ContactInfoBaseCategory.Work,
                               ContactID = 0,
                               Data = URLEmail.HtmlEncode(),
                               ID = 0,
                               InfoType = ContactInfoType.Email,
                               IsPrimary = true
                            });
                if (UrlParameters.Type != "people") {
                    //init ListContactView
                    RegisterClientScriptHelper.DataListContactTab(Page, 0, EntityType.Company);
                }
            }
            else
            {
                data = Global.DaoFactory.GetCustomFieldDao().GetEnityFields(
                    TargetContact is Person ? EntityType.Person : EntityType.Company,
                    TargetContact.ID, true);

                networks = Global.DaoFactory.GetContactInfoDao().GetList(TargetContact.ID, null, null, null).ConvertAll(
                n => new ContactInfo()
                                    {
                                        Category = n.Category,
                                        ContactID = n.ContactID,
                                        Data = n.Data.HtmlEncode(),
                                        ID = n.ID,
                                        InfoType = n.InfoType,
                                        IsPrimary = n.IsPrimary
                                    });
                if (TargetContact is Company) {
                    //init ListContactView
                    RegisterClientScriptHelper.DataListContactTab(Page, TargetContact.ID, EntityType.Company);
                }
            }

            RegisterClientScriptHelper.DataContactActionView(Page, TargetContact, data, networks);


            if (TargetContact != null)
            {
                cancelButton.Attributes.Add("href", String.Format("default.aspx?{0}={1}{2}", UrlConstant.ID, TargetContact.ID,
                    !String.IsNullOrEmpty(UrlParameters.Type) ?
                    String.Format("&{0}={1}", UrlConstant.Type, UrlParameters.Type) :
                    String.Empty));
            }
            else
            {
                cancelButton.Attributes.Add("href",
                             Request.UrlReferrer != null && Request.Url != null && String.Compare(Request.UrlReferrer.PathAndQuery, Request.Url.PathAndQuery) != 0
                                 ? Request.UrlReferrer.OriginalString
                                 : "default.aspx");
            }

            InitContactManagerSelector();

            RegisterScript();
        }

        #endregion

        #region Methods
        public String GetTitle()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person) TargetContact).JobTitle.HtmlEncode();
            return String.Empty;
        }

        public String GetFirstName()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person) TargetContact).FirstName.HtmlEncode();

            var URLFullName = UrlParameters.FullName;
            if (!String.IsNullOrEmpty(URLFullName))
            {
                var parts = URLFullName.Split(' ');
                return parts.Length < 2 ? String.Empty : parts[0];
            }

            return String.Empty;
        }

        public String GetLastName()
        {
            if (TargetContact != null && TargetContact is Person)
                return ((Person)TargetContact).LastName.HtmlEncode();

            var URLFullName = UrlParameters.FullName;
            if (!String.IsNullOrEmpty(URLFullName))
            {
                var parts = URLFullName.Split(' ');
                return parts.Length < 2 ? URLFullName : URLFullName.Remove(0, parts[0].Length);
            }
            return String.Empty;
        }

        public String GetCompanyName()
        {
            if (TargetContact != null && TargetContact is Company)
                return ((Company)TargetContact).CompanyName.HtmlEncode();
            return UrlParameters.FullName;
        }

        public String GetCompanyIDforPerson()
        {
            if ((TargetContact != null && TargetContact is Person))
                return ((Person)TargetContact).CompanyID.ToString();
            return String.Empty;
        }


        protected void InitContactManagerSelector()
        {
            Dictionary<Guid,String> SelectedUsers = null;
            if (TargetContact != null)
            {
                var AccessSubjectTo = CRMSecurity.GetAccessSubjectTo(TargetContact, EmployeeStatus.Active).ToList();
                SelectedUsers = new Dictionary<Guid, String>();
                if (AccessSubjectTo.Count != 0)
                {
                    foreach (var item in AccessSubjectTo)
                    {
                        SelectedUsers.Add(item.Key, item.Value);
                    }
                }
            }
            else
            {
                SelectedUsers = new Dictionary<Guid,String>();
                SelectedUsers.Add(SecurityContext.CurrentAccount.ID, SecurityContext.CurrentAccount.Name.HtmlEncode());
            }
            RegisterClientScriptHelper.DataUserSelectorListView(Page, "_ContactManager", SelectedUsers, null, true);
        }

        protected void SaveOrUpdateContact(object sender, CommandEventArgs e)
        {
            Contact contact;

            var typeAddedContact = Request["typeAddedContact"];

            var companyID = 0;

            if (!String.IsNullOrEmpty(Request["baseInfo_compID"]))
            {
                companyID = Convert.ToInt32(Request["baseInfo_compID"]);
            }
            else if (!String.IsNullOrEmpty(Request["baseInfo_compName"]))
            {
                var peopleCompany = new Company
                {
                    CompanyName = Request["baseInfo_compName"].Trim()
                };

                peopleCompany.ID = Global.DaoFactory.GetContactDao().SaveContact(peopleCompany);

                CRMSecurity.MakePublic(peopleCompany);

                companyID = peopleCompany.ID;
            }

            if (typeAddedContact.Equals("people"))
            {
                contact = new Person
                              {
                                  FirstName = Request["baseInfo_firstName"].Trim(),
                                  LastName = Request["baseInfo_lastName"].Trim(),
                                  JobTitle = Request["baseInfo_personPosition"].Trim(),
                                  CompanyID = companyID
                              };
            }
            else
            {
                contact = new Company
                              {
                                  CompanyName = Request["baseInfo_companyName"].Trim()
                              };
            }

            if (!String.IsNullOrEmpty(Request["baseInfo_contactOverview"]))
            {
                contact.About = Request["baseInfo_contactOverview"].Trim();
            }

            Boolean isSharedContact;
            if(bool.TryParse(Request["isPublicContact"], out isSharedContact))
            {
                contact.IsShared = isSharedContact;
            }

            contact.ContactTypeID = Convert.ToInt32(Request["baseInfo_contactType"]);

            if (TargetContact != null)
            {
                contact.ID = TargetContact.ID;
                contact.StatusID = TargetContact.StatusID;
                Global.DaoFactory.GetContactDao().UpdateContact(contact);
                contact = Global.DaoFactory.GetContactDao().GetByID(contact.ID);
            }
            else
            {
                contact.ID = Global.DaoFactory.GetContactDao().SaveContact(contact);
                contact = Global.DaoFactory.GetContactDao().GetByID(contact.ID);
            }

            SetContactManager(contact);

            if (contact is Company)
            {
                int[] assignedContactsIDs = null;

                if (!String.IsNullOrEmpty(Request["baseInfo_assignedContactsIDs"]))
                    assignedContactsIDs = Request["baseInfo_assignedContactsIDs"].Split(',').Select(item => Convert.ToInt32(item)).ToArray();

                Global.DaoFactory.GetContactDao().SetMembers(contact.ID, assignedContactsIDs);

            }

            var assignedTags = Request["baseInfo_assignedTags"];
            if (assignedTags != null)
            {
                var oldTagList = Global.DaoFactory.GetTagDao().GetEntityTags(EntityType.Contact, contact.ID);
                foreach (var tag in oldTagList)
                {
                    Global.DaoFactory.GetTagDao().DeleteTagFromEntity(EntityType.Contact, contact.ID, tag);
                }
                if (assignedTags != string.Empty)
                {
                    var tagListInfo = JObject.Parse(assignedTags)["tagListInfo"].ToArray();
                    var newTagList = tagListInfo.Select(t => t.ToString()).ToArray();
                    Global.DaoFactory.GetTagDao().SetTagToEntity(EntityType.Contact, contact.ID, newTagList);
                }
            }

            var contactInfos = new List<ContactInfo>();
            var addressList = new Dictionary<int, ContactInfo>();
            var addressTemplate = new JObject();

            foreach (String addressPartName in Enum.GetNames(typeof(AddressPart)))
            {
                addressTemplate.Add(addressPartName.ToLower(), "");
            }

            var addressTemplateStr = addressTemplate.ToString();

            foreach (var item in Request.Form.AllKeys)
            {
                if (item.StartsWith("customField_"))
                {
                    int fieldID = Convert.ToInt32(item.Split('_')[1]);
                    String fieldValue = Request.Form[item].Trim();

                    if (contact is Person)
                    {
                        Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, contact.ID, fieldID, fieldValue);
                    }
                    else
                    {
                        Global.DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, contact.ID, fieldID, fieldValue);
                    }

                }
                else if (item.StartsWith("contactInfo_"))
                {
                    var nameParts = item.Split('_').Skip(1).ToList();
                    var contactInfoType = (ContactInfoType) Enum.Parse(typeof (ContactInfoType), nameParts[0]);
                    var category = Convert.ToInt32(nameParts[2]);

                    if (contactInfoType == ContactInfoType.Address)
                    {
                        var index = Convert.ToInt32(nameParts[1]);
                        var addressPart = (AddressPart)Enum.Parse(typeof(AddressPart), nameParts[3]);
                        var isPrimaryAddress = Convert.ToInt32(nameParts[4]) == 1;
                        var dataValues = Request.Form.GetValues(item).Select(n => n.Trim()).ToList();

                        if (!addressList.ContainsKey(index))
                        {
                            var newAddress = new ContactInfo
                                                 {
                                                     Category = category,
                                                     InfoType = contactInfoType,
                                                     Data = addressTemplateStr,
                                                     IsPrimary = isPrimaryAddress,
                                                     ContactID = contact.ID
                                                 };
                            addressList.Add(index, newAddress);
                        }

                        foreach (var data in dataValues)
                        {
                            var addressParts = JObject.Parse(addressList[index].Data);
                            addressParts[addressPart.ToString().ToLower()] = data;
                            addressList[index].Data = addressParts.ToString();
                        }
                        continue;
                    }

                    var isPrimary = Convert.ToInt32(nameParts[3]) == 1;
                    if (Request.Form.GetValues(item) != null)
                    {
                        var dataValues = Request.Form.GetValues(item).Where(n => !string.IsNullOrEmpty(n.Trim())).ToList();

                        contactInfos.AddRange(dataValues.Select(dataValue => new ContactInfo
                                                                                 {
                                                                                     Category = category,
                                                                                     InfoType = contactInfoType,
                                                                                     Data = dataValue.Trim(),
                                                                                     IsPrimary = isPrimary,
                                                                                     ContactID = contact.ID
                                                                                 }));
                    }
                }
            }

            if (addressList.Count>0)
                contactInfos.AddRange(addressList.Values.ToList());

            Global.DaoFactory.GetContactInfoDao().DeleteByContact(contact.ID);
            Global.DaoFactory.GetContactInfoDao().SaveList(contactInfos);

            var emails = contactInfos
                .Where(item => item.InfoType == ContactInfoType.Email)
                .Select(item => item.Data)
                .ToList();
            if (emails.Count > 0)
            {
                var userIds = CRMSecurity.GetAccessSubjectTo(contact).Keys.ToList();
                Global.DaoFactory.GetContactInfoDao().UpdateMailAggregator(emails, userIds);
            }

            var photoPath = Request["uploadPhotoPath"];

            if (!String.IsNullOrEmpty(photoPath))
            {
                if (photoPath != "null")
                {
                    if (photoPath.StartsWith(PathProvider.BaseAbsolutePath))
                    {
                        var tmpDirName = photoPath.Substring(0, photoPath.LastIndexOf('/'));
                        ContactPhotoManager.TryUploadPhotoFromTmp(contact.ID, TargetContact == null, tmpDirName);
                    }
                    else
                    {
                        ContactPhotoManager.UploadPhoto(photoPath, contact.ID);
                    }
                }
            }
            else if (TargetContact != null)
            {
                ContactPhotoManager.DeletePhoto(TargetContact.ID);
            }

            Response.Redirect(String.Compare(e.CommandArgument.ToString(), "0", true) == 0
                                  ? String.Format("default.aspx?id={0}{1}", contact.ID,
                                                  contact is Company
                                                      ? ""
                                                      : String.Format("&{0}=people", UrlConstant.Type))
                                  : String.Format("default.aspx?action=manage{0}",
                                                  contact is Company
                                                      ? ""
                                                      : String.Format("&{0}=people", UrlConstant.Type)));
        }

        protected void SetContactManager(Contact contact)
        {
            var notifyContactManagers = false;
            bool value;

            if(bool.TryParse(Request.Form["notifyContactManagers"], out value))
            {
                notifyContactManagers = value;
            }

            var selectedManagers = Request.Form["selectedContactManagers"]
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(item => new Guid(item)).ToList();

            if (notifyContactManagers)
            {
                var notifyUsers = selectedManagers.Where(n => n != SecurityContext.CurrentAccount.ID).ToArray();
                if (contact is Person)
                    Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Person, contact.ID, notifyUsers);
                else
                    Services.NotifyService.NotifyClient.Instance.SendAboutSetAccess(EntityType.Company, contact.ID, notifyUsers);
            }

            CRMSecurity.SetAccessTo(contact, selectedManagers);
        }

        protected string GetContactPhone(int contactID)
        {
            var phones = Global.DaoFactory.GetContactInfoDao().GetList(contactID, ContactInfoType.Phone, null, true);
            return phones.Count == 0 ? String.Empty : phones[0].Data.HtmlEncode();
        }

        protected string GetContactEmail(int contactID)
        {
            var emails = Global.DaoFactory.GetContactInfoDao().GetList(contactID, ContactInfoType.Email, null, true);
            return emails.Count == 0 ? String.Empty : emails[0].Data.HtmlEncode();
        }

        private void RegisterScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(@"ASC.CRM.ContactActionView.init(""{0}"",{1},{2},{3},{4},{5},""{6}"",""{7}"");
                    ASC.CRM.SocialMedia.init(""{8}"");
                    ASC.CRM.SocialMedia.emptyPeopleLogo = '{9}';",
                DateTimeExtension.DateMaskForJQuery,
                TargetContact != null ? TargetContact.ID : 0,
                TargetContact != null ? TargetContact.ContactTypeID : 0,
                TargetContact != null ? TargetContact.IsShared.ToString().ToLower() : false.ToString().ToLower(),
                TargetContact != null && TargetContact is Person || TargetContact == null && UrlParameters.Type == "people" ?
                    (int)ContactSelectorTypeEnum.Companies :
                    (int)ContactSelectorTypeEnum.PersonsWithoutCompany,
                CRMSecurity.IsAdmin.ToString().ToLower(),
                Studio.Core.FileSizeComment.GetFileImageSizeNote(CRMContactResource.ContactPhotoInfo, true),
                ContactPhotoManager.GetMediumSizePhoto(0, UrlParameters.Type != "people"),
                ContactPhotoManager.GetBigSizePhoto(0, UrlParameters.Type != "people"),
                WebImageSupplier.GetAbsoluteWebPath("empty_people_logo_40_40.png",ProductEntryPoint.ID)
            );

            Page.RegisterInlineScript(sb.ToString());
        }
        #endregion
    }
}