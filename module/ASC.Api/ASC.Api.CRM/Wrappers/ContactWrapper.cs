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
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Employee;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using ASC.Web.CRM.Classes;
using Contact = ASC.CRM.Core.Entities.Contact;
using ASC.Web.Studio.Utility;
using ASC.Web.CRM;

#endregion

namespace ASC.Api.CRM.Wrappers
{
    /// <summary>
    ///   Person
    /// </summary>
    [DataContract(Name = "person", Namespace = "")]
    public class PersonWrapper : ContactWrapper
    {
        public PersonWrapper(int id) :
            base(id)
        {
        }

        public PersonWrapper(Person person)
            : base(person)
        {
            FirstName = person.FirstName;
            LastName = person.LastName;
            Title = person.JobTitle;
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String FirstName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String LastName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactBaseWrapper Company { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Title { get; set; }

        public new static PersonWrapper GetSample()
        {
            return new PersonWrapper(0)
                       {
                           IsPrivate = false,
                           IsShared = false,
                           IsCompany = false,
                           FirstName = "Tadjeddine",
                           LastName = "Bachir",
                           Company = CompanyWrapper.GetSample(),
                           Title = "Programmer",
                           About = "",
                           Created = (ApiDateTime) DateTime.UtcNow,
                           CreateBy = EmployeeWraper.GetSample()
                       };
        }
    }

    /// <summary>
    ///  Company
    /// </summary>
    [DataContract(Name = "company", Namespace = "")]
    public class CompanyWrapper : ContactWrapper
    {
        public CompanyWrapper(int id) :
            base(id)
        {
        }

        public CompanyWrapper(Company company)
            : base(company)
        {
            CompanyName = company.CompanyName;
          //  PersonsCount = Global.DaoFactory.GetContactDao().GetMembersCount(company.ID);
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public String CompanyName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ContactBaseWrapper> Persons { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public int PersonsCount { get; set; }

        public new static CompanyWrapper GetSample()
        {
            return new CompanyWrapper(0)
                       {
                           IsPrivate = false,
                           IsCompany = true,
                           About = "",
                           CompanyName = "Food and Culture Project",
                           PersonsCount = 0
                       };
        }
    }

    [DataContract(Name = "contact", Namespace = "")]
    [KnownType(typeof (PersonWrapper))]
    [KnownType(typeof (CompanyWrapper))]
    public abstract class ContactWrapper : ContactBaseWrapper
    {
        protected ContactWrapper(int id)
            : base(id)
        {
        }

        protected ContactWrapper(Contact contact)
            : base(contact)
        {
            CreateBy = EmployeeWraper.Get(contact.CreateBy);
            Created = (ApiDateTime) contact.CreateOn;
            About = contact.About;
            Industry = contact.Industry;
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<Address> Addresses { get; set; }

    

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public EmployeeWraper CreateBy { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ApiDateTime Created { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String About { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String Industry { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactStatusBaseWrapper ContactStatus { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactTypeBaseWrapper ContactType { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<ContactInfoWrapper> CommonData { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<CustomFieldBaseWrapper> CustomFields { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<String> Tags { get; set; }
            
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int TaskCount { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool HaveLateTasks { get; set; }
    }

    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWithEmailWrapper : ContactBaseWrapper
    {
        protected ContactBaseWithEmailWrapper(int id)
            : base(id)
        {
        }

        public ContactBaseWithEmailWrapper(Contact contact)
            : base(contact)
        {
        }

        public ContactBaseWithEmailWrapper(ContactWrapper contactWrapper) : base(contactWrapper.ID)
        {
            this.AccessList = contactWrapper.AccessList;
            this.CanEdit = contactWrapper.CanEdit;
            this.DisplayName = contactWrapper.DisplayName;
            this.IsCompany = contactWrapper.IsCompany;
            this.IsPrivate = contactWrapper.IsPrivate;
            this.IsShared = contactWrapper.IsShared;
            this.MediumFotoUrl = contactWrapper.MediumFotoUrl;
            this.SmallFotoUrl = contactWrapper.SmallFotoUrl;

            if (contactWrapper.CommonData != null && contactWrapper.CommonData.Count() != 0)
            {
                this.Email = contactWrapper.CommonData.Where(item => item.InfoType == ContactInfoType.Email && item.IsPrimary == true).FirstOrDefault();
            }
            else
            {
                this.Email = null;
            }
        }


        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactInfoWrapper Email { get; set; }
    }

    /// <summary>
    ///  Contact base information
    /// </summary>
    [DataContract(Name = "contactBase", Namespace = "")]
    public class ContactBaseWrapper : ObjectWrapperBase
    {
        public ContactBaseWrapper(Contact contact)
            : base(contact.ID)
        {
            DisplayName = contact.GetTitle();
            IsPrivate = CRMSecurity.IsPrivate(contact);
            IsShared = contact.IsShared;

            if (IsPrivate)
                AccessList = CRMSecurity.GetAccessSubjectTo(contact)
                             .Select(item => EmployeeWraper.Get(item.Key));


            SmallFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=1", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower();
            MediumFotoUrl = String.Format("{0}HttpHandlers/filehandler.ashx?action=contactphotoulr&cid={1}&isc={2}&ps=2", PathProvider.BaseAbsolutePath, contact.ID, contact is Company).ToLower();
            IsCompany = contact is Company;
            CanEdit = CRMSecurity.CanEdit(contact);
        }


        protected ContactBaseWrapper(int contactID)
            : base(contactID)
        {
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String SmallFotoUrl { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public String MediumFotoUrl { get; set; }
        
        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public String DisplayName { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsCompany { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public IEnumerable<EmployeeWraper> AccessList { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsPrivate { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool IsShared { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = true)]
        public bool CanEdit { get; set; }

        public static ContactBaseWrapper GetSample()
        {
            return new ContactBaseWrapper(0)
                       {
                           IsPrivate = false,
                           IsShared = false,
                           IsCompany = false,
                           DisplayName = "Tadjeddine Bachir",
                           SmallFotoUrl = "url to foto"
                       };
        }
    }



    [DataContract(Name = "contact_task", Namespace = "")]
    public class ContactWithTaskWrapper
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TaskBaseWrapper Task { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ContactWrapper Contact { get; set; }
    }
}