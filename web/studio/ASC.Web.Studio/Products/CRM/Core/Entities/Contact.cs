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
using ASC.Common.Security;
using ASC.Web.Core.Helpers;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;

namespace ASC.CRM.Core.Entities
{
    [Serializable]
    public class Person : Contact
    {
        public Person()
        {
            FirstName = String.Empty;
            LastName = String.Empty;
            CompanyID = 0;
            JobTitle = String.Empty;
        }

        public String FirstName { get; set; }

        public String LastName { get; set; }

        public int CompanyID { get; set; }

        public String JobTitle { get; set; }
    }

    [Serializable]
    public class Company : Contact
    {
        public Company()
        {
            CompanyName = String.Empty;
        }

        public String CompanyName { get; set; }
    }

    public static class ContactExtension
    {
        public static String GetTitle(this Contact contact)
        {
            if (contact == null)
                return String.Empty;

            if (contact is Company)
            {
                var company = (Company)contact;

                return company.CompanyName;
            }

            var people = (Person)contact;

            return String.Format("{0} {1}", people.FirstName, people.LastName);
        }

        public static String RenderLinkForCard(this Contact contact)
        {
            var isCompany = contact is Company;
            var popupID = Guid.NewGuid();

            return String.Format(@"
                <a class='linkMedium {0}' id='{5}' data-id='{2}'
                            href='default.aspx?{1}={2}{3}'>
                     {4}
                </a>",
                                 isCompany ? "crm-companyInfoCardLink" : "crm-peopleInfoCardLink",
                                 UrlConstant.ID, contact != null ? contact.ID : 0,
                                 isCompany ? String.Empty : String.Format("&{0}=people", UrlConstant.Type),
                                 GetTitle(contact).HtmlEncode(), popupID);
        }

        public static String GetEmployeesCountString(this Contact contact)
        {
            if (contact is Person) return String.Empty;
            var count = Global.DaoFactory.GetContactDao().GetMembersCount(contact.ID);
            return count + " " + GrammaticalHelper.ChooseNumeralCase(count,
                                                                     CRMContactResource.MembersNominative,
                                                                     CRMContactResource.MembersGenitiveSingular,
                                                                     CRMContactResource.MembersGenitivePlural);
        }
    }

    [Serializable]
    public abstract class Contact : DomainObject, ISecurityObjectId
    {
        protected Contact()
        {
            About = String.Empty;
            Industry = String.Empty;
            StatusID = 0;
            ContactTypeID = 0;
            IsShared = false;
        }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid? LastModifedBy { get; set; }

        public DateTime? LastModifedOn { get; set; }

        public String About { get; set; }

        public String Industry { get; set; }

        public int StatusID { get; set; }

        public int ContactTypeID { get; set; }

        public bool IsShared { get; set; }

        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}