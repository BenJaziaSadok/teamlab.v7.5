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
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Controls.Contacts;
using ASC.Web.CRM.Resources;
using ASC.Web.Studio.Utility;
using ASC.CRM.Core;
using ASC.Web.CRM.Controls.Common;
using ASC.Web.CRM.Configuration;

#endregion

namespace ASC.Web.CRM
{
    public partial class Contacts : BasePage
    {
        #region Property

        public static String Location
        {
            get { return "~/products/crm/default.aspx"; }
        }

        #endregion

        #region Events

        protected override void PageLoad()
        {
            InitControls();
            ProductEntryPoint.ConfigurePortal();
        }

        #endregion

        #region Methods

        protected void InitControls()
        {
            int contactID;

            if (int.TryParse(UrlParameters.ID, out contactID))
            {

                var targetContact = Global.DaoFactory.GetContactDao().GetByID(contactID);

                if (targetContact == null || !CRMSecurity.CanAccessTo(targetContact))
                    Response.Redirect(PathProvider.StartURL());

                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecContactActionView(targetContact);
                else
                    ExecContactDetailsView(targetContact);

                _ctrlContactID.Value = targetContact.ID.ToString();
            }
            else
            {
                if (String.Compare(UrlParameters.Action, "manage", true) == 0)
                    ExecContactActionView(null);
                else if (String.Compare(UrlParameters.Action, "import", true) == 0)
                    ExecImportView();
                else
                    ExecListContactView();
            }
        }

        static public string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes = System.Text.UTF8Encoding.UTF8.GetBytes(toEncode);

            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }

        static public string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            string returnValue = System.Text.UTF8Encoding.UTF8.GetString(encodedDataAsBytes);

            return returnValue;
        }

        protected void ExecImportView()
        {
            var importViewControl = (ImportFromCSVView)LoadControl(ImportFromCSVView.Location);
            importViewControl.EntityType = EntityType.Contact;

            CommonContainerHolder.Controls.Add(importViewControl);

            Master.CurrentPageCaption = CRMContactResource.ImportContacts;
            Title = HeaderStringHelper.GetPageTitle(CRMContactResource.ImportContacts);
        }

        protected void ExecContactDetailsView(Contact targetContact)
        {
            var contactDetailsViewControl = (ContactDetailsView)LoadControl(ContactDetailsView.Location);
            contactDetailsViewControl.TargetContact = targetContact;

            CommonContainerHolder.Controls.Add(contactDetailsViewControl);

            var title = targetContact.GetTitle().HtmlEncode();

            Master.CurrentPageCaption = title;

            Master.CommonContainerHeader = Global.RenderItemHeaderWithMenu(title, targetContact is Company ? EntityType.Company : EntityType.Person, false);

            ((BasicTemplate)Master).ShowChangeButton = false;
            Title = HeaderStringHelper.GetPageTitle(title);
        }

        protected void ExecListContactView()
        {
            CommonContainerHolder.Controls.Add(LoadControl(ListContactView.Location));
            Title = HeaderStringHelper.GetPageTitle(Master.CurrentPageCaption ?? CRMContactResource.AllContacts);
        }

        protected void ExecContactActionView(Contact targetContact)
        {
            var contactActionViewControl = (ContactActionView)LoadControl(ContactActionView.Location);
            contactActionViewControl.TargetContact = targetContact;

            if (targetContact == null)
            {

                if (String.Compare(UrlParameters.Type, "people", true) != 0)
                {
                    contactActionViewControl.TypeAddedContact = "company";
                    contactActionViewControl.SaveContactButtonText = CRMContactResource.AddThisCompanyButton;
                    contactActionViewControl.SaveAndCreateContactButtonText = CRMContactResource.AddThisAndCreateCompanyButton;

                    contactActionViewControl.AjaxProgressText = CRMContactResource.AddingCompany;
                    Master.CurrentPageCaption = CRMContactResource.BreadCrumbsAddCompany;
                    Title = HeaderStringHelper.GetPageTitle(CRMContactResource.BreadCrumbsAddCompany);
                }
                else
                {
                    contactActionViewControl.TypeAddedContact = "people";
                    contactActionViewControl.SaveContactButtonText = CRMContactResource.AddThisPersonButton;
                    contactActionViewControl.SaveAndCreateContactButtonText = CRMContactResource.AddThisAndCreatePeopleButton;

                    contactActionViewControl.AjaxProgressText = CRMContactResource.AddingPersonProgress;
                    Master.CurrentPageCaption = CRMContactResource.BreadCrumbsAddPerson;
                    Title = HeaderStringHelper.GetPageTitle(CRMContactResource.BreadCrumbsAddPerson);
                }
            }
            else
            {
                var contactTitle = targetContact.GetTitle();

                contactActionViewControl.SaveAndCreateContactButtonText = String.Compare(UrlParameters.Type, "people", true) != 0 ? CRMContactResource.SaveThisAndCreateCompanyButton : CRMContactResource.SaveThisAndCreatePeopleButton;

                contactActionViewControl.SaveContactButtonText = CRMContactResource.SaveChanges;
                contactActionViewControl.AjaxProgressText = CRMContactResource.SavingChangesProgress;

                if (targetContact is Company)
                {
                    contactActionViewControl.TypeAddedContact = "company";
                    var headerTitle = String.Format(CRMContactResource.EditCompany, contactTitle);
                    Master.CurrentPageCaption = headerTitle;
                    Title = HeaderStringHelper.GetPageTitle(headerTitle);
                }
                else
                {
                    contactActionViewControl.TypeAddedContact = "people";
                    var headerTitle = String.Format(CRMContactResource.EditPerson, contactTitle);
                    Master.CurrentPageCaption = headerTitle;
                    Title = HeaderStringHelper.GetPageTitle(headerTitle);
                }
            }

            CommonContainerHolder.Controls.Add(contactActionViewControl);
        }

        #endregion
    }
}