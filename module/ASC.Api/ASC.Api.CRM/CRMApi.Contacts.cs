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

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Web.CRM.Classes;
using Contact = ASC.CRM.Core.Entities.Contact;
using System.Net;
using System.IO;
using ASC.Specific;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///    Returns the detailed information about the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <returns>Contact</returns>
        /// <short>Get contact by ID</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Read(@"contact/{contactid:[0-9]+}")]
        public ContactWrapper GetContactByID(int contactid)
        {
            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            return ToContactWrapper(contact);
        }

        /// <summary>
        ///  Returns the contact list for the project with the ID specified in the request
        /// </summary>
        /// <short>
        ///  Get contacts by project ID
        /// </short>
        /// <param name="projectid">Project ID</param>
        /// <category>Contacts</category>
        /// <returns>
        ///     Contact list
        /// </returns>
        ///<exception cref="ArgumentException"></exception>
        [Read("contact/project/{projectid:[0-9]+}")]
        public IEnumerable<ContactWrapper> GetContactsByProjectID(int projectid)
        {
            if (projectid <= 0)
                throw new ArgumentException();

            var contacts = DaoFactory.GetContactDao().GetContactsByProjectID(projectid);


            return ToListContactWrapper(contacts.ToList());
        }

        /// <summary>
        ///  Links the selected contact to the project with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="projectid">Project ID</param>
        /// <category>Contacts</category>
        /// <short>Link contact with project</short> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>Contact Info</returns>
        [Create("contact/{contactid:[0-9]+}/project/{projectid:[0-9]+}")]
        public ContactBaseWrapper SetRelativeContactToProject(int contactid, int projectid)
        {
            if (contactid <= 0 || projectid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().SetRelativeContactProject(new List<int> { contactid }, projectid);

            return ToContactBaseWrapper(contact);

        }

        /// <summary>
        ///  Links the selected contacts to the project with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact IDs array</param>
        /// <param name="projectid">Project ID</param>
        /// <category>Contacts</category>
        /// <short>Link contact list with project</short> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Contact list
        /// </returns>        
        [Create("contact/project/{projectid:[0-9]+}")]
        public IEnumerable<ContactBaseWrapper> SetRelativeContactListToProject(IEnumerable<int> contactid, int projectid)
        {
            if (!contactid.Any() || projectid <= 0)
                throw new ArgumentException();

            var contacts = DaoFactory.GetContactDao().GetContacts(contactid.ToArray());

            DaoFactory.GetContactDao().SetRelativeContactProject(contactid, projectid);

            return contacts.ConvertAll(x => ToContactBaseWrapper(x));

        }

        /// <summary>
        ///  Removes the link with the selected project from the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="projectid">Project ID</param>
        /// <category>Contacts</category>
        /// <short>Remove contact from project</short> 
        /// <returns>
        ///    Contact info
        /// </returns>
        [Delete("contact/{contactid:[0-9]+}/project/{projectid:[0-9]+}")]
        public ContactBaseWrapper RemoveRelativeContactToProject(int contactid, int projectid)
        {
            if (contactid <= 0 || projectid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().RemoveRelativeContactProject(contactid, projectid);

            return ToContactBaseWrapper(contact);
        }

        /// <summary>
        ///   Adds the selected opportunity to the contact with the ID specified in the request. The same as AddMemberToDeal
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <short>Add contact opportunity</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Opportunity
        /// </returns>
        [Create(@"contact/{contactid:[0-9]+}/opportunity/{opportunityid:[0-9]+}")]
        public OpportunityWrapper AddDealToContact(int contactid, int opportunityid)
        {
            if ((opportunityid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var opportunity = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (opportunity == null)
                throw new ItemNotFoundException();

            var result = ToOpportunityWrapper(opportunity);

            DaoFactory.GetDealDao().AddMember(opportunityid, contactid);

            return result;
        }

        /// <summary>
        ///   Deletes the selected opportunity from the contact with the ID specified in the request
        /// </summary>
        /// <param name="opportunityid">Opportunity ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <short>Add contact opportunity</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Opportunity
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}/opportunity/{opportunityid:[0-9]+}")]
        public OpportunityWrapper DeleteDealFromContact(int contactid, int opportunityid)
        {
            if ((opportunityid <= 0) || (contactid <= 0))
                throw new ArgumentException();

            var opportunity = DaoFactory.GetDealDao().GetByID(opportunityid);

            if (opportunity == null)
                throw new ItemNotFoundException();

            var result = ToOpportunityWrapper(opportunity);

            DaoFactory.GetDealDao().RemoveMember(opportunityid, contactid);

            return result;
        }



        /// <summary>
        ///    Returns the list of all contacts in the CRM module matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactStage">Contact stage ID (warmth)</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <short>Get contact list</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact list
        /// </returns>
        [Read(@"contact/filter")]
        public IEnumerable<ContactWrapper> GetContacts(IEnumerable<String> tags,
                                                      int contactStage,
                                                      int contactType,
                                                      ContactListViewType contactListView,
                                                      ApiDateTime fromDate,
                                                      ApiDateTime toDate)
        {
            IEnumerable<ContactWrapper> result;

            OrderBy contactsOrderBy;

            ContactSortedByType sortBy;

            var searchString = _context.FilterValue;

            if (Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out sortBy))
            {
                contactsOrderBy = new OrderBy(sortBy, !_context.SortDescending);
            }
            else if (String.IsNullOrEmpty(_context.SortBy))
            {
                contactsOrderBy = new OrderBy(ContactSortedByType.Created, false);
            }
            else
            {
                contactsOrderBy = null;
            }


            var fromIndex = (int)_context.StartIndex;
            var count = (int)_context.Count;


            if (contactsOrderBy != null)
            {
                result = ToListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactStage,
                                                               contactType,
                                                               contactListView,
                                                               fromDate,
                                                               toDate,
                                                               fromIndex,
                                                               count,
                                                               contactsOrderBy));
                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();
            }
            else
            {
                result = ToListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactStage,
                                                               contactType,
                                                               contactListView,
                                                               fromDate,
                                                               toDate,
                                                               0,
                                                               0,
                                                               null));
            }


            int totalCount;

            if (result.Count() < count)
            {
                totalCount = fromIndex + result.Count();
            }
            else
            {
                totalCount = DaoFactory.GetContactDao().GetContactsCount(searchString,
                                                                           tags,
                                                                           contactStage,
                                                                           contactType,
                                                                           contactListView,
                                                                           fromDate,
                                                                           toDate);
            }

            _context.SetTotalCount(totalCount);

            return result;

        }

        /// <summary>
        ///    Returns the list of all contacts in the CRM module matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactStage">Contact stage ID (warmth)</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <param optional="true" name="responsibleid">Responsible ID</param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <short>Get contact list</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact list
        /// </returns>
        /// <visible>false</visible>
        [Read(@"contact/simple/filter")]
        public IEnumerable<ContactWithTaskWrapper> GetSimpleContacts(
            IEnumerable<String> tags,
            int contactStage,
            int contactType,
            ContactListViewType contactListView,
            Guid responsibleid,
            ApiDateTime fromDate,
            ApiDateTime toDate)
        {
            IEnumerable<ContactWithTaskWrapper> result;

            OrderBy contactsOrderBy;

            ContactSortedByType sortBy;

            var searchString = _context.FilterValue;

            if (Web.CRM.Classes.EnumExtension.TryParse(_context.SortBy, true, out sortBy))
            {
                contactsOrderBy = new OrderBy(sortBy, !_context.SortDescending);
            }
            else if (String.IsNullOrEmpty(_context.SortBy))
            {
                contactsOrderBy = new OrderBy(ContactSortedByType.DisplayName, true);
            }
            else
            {
                contactsOrderBy = null;
            }

            var fromIndex = (int)_context.StartIndex;
            var count = (int)_context.Count;


            if (contactsOrderBy != null)
            {
                result = ToSimpleListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactStage,
                                                               contactType,
                                                               contactListView,
                                                               fromDate,
                                                               toDate,
                                                               fromIndex,
                                                               count,
                                                               contactsOrderBy,
                                                               responsibleid));
                _context.SetDataPaginated();
                _context.SetDataFiltered();
                _context.SetDataSorted();

            }
            else
            {
                result = ToSimpleListContactWrapper(DaoFactory.GetContactDao()
                                                  .GetContacts(searchString,
                                                               tags,
                                                               contactStage,
                                                               contactType,
                                                               contactListView,
                                                               fromDate,
                                                               toDate,
                                                               0,
                                                               0,
                                                               null));
            }

            int totalCount;

            if (result.Count() < count)
            {
                totalCount = fromIndex + result.Count();
            }
            else
            {
                totalCount = DaoFactory.GetContactDao().GetContactsCount(searchString,
                                                                          tags,
                                                                          contactStage,
                                                                          contactType,
                                                                          contactListView,
                                                                          fromDate,
                                                                          toDate,
                                                                          responsibleid);
            }

            _context.SetTotalCount(totalCount);

            return result;

        }


        /// <summary>
        ///   Get the group of contacts with the IDs specified in the request
        /// </summary>
        /// <param name="contactids">Contact ID list</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Get contact group</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact list
        /// </returns>
        /// <visible>false</visible>
        [Read(@"contact/mail")]
        public IEnumerable<ContactBaseWithEmailWrapper> GetContactsForMail(IEnumerable<int> contactids)
        {
            if (contactids == null)
                throw new ArgumentException();

            var contacts = DaoFactory.GetContactDao().GetContacts(contactids.ToArray());

            var result = contacts.Select(contact => ToContactBaseWithEmailWrapper(contact));

            return result;
        }


        /// <summary>
        ///   Deletes the list of all contacts in the CRM module matching the parameters specified in the request
        /// </summary>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactStage">Contact stage ID (warmth)</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete the list of all contacts </short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Delete(@"contact/filter")]
        public IEnumerable<ContactBaseWrapper> DeleteBatchContacts(IEnumerable<String> tags,
                                                                    int contactStage,
                                                                    int contactType,
                                                                    ContactListViewType contactListView,
                                                                    ApiDateTime fromDate,
                                                                    ApiDateTime toDate)
        {
            var contacts = DaoFactory.GetContactDao().GetContacts(_context.FilterValue,
                                                                  tags,
                                                                  contactStage,
                                                                  contactType,
                                                                  contactListView,
                                                                  fromDate,
                                                                  toDate,
                                                                  0,
                                                                  0,
                                                                  null);

            var result = contacts.Select(x => ToContactBaseWrapper(x));

            DaoFactory.GetContactDao().DeleteBatchContact(contacts);

            return result;
        }



        /// <summary>
        ///    Returns the list of all the persons linked to the company with the ID specified in the request
        /// </summary>
        /// <param name="companyid">Company ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <short>Get company linked persons list</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Linked persons
        /// </returns>
        [Read(@"contact/company/{companyid:[0-9]+}/person")]
        public IEnumerable<ContactWrapper> GetPeopleFromCompany(int companyid)
        {

            if (companyid <= 0)
                throw new ArgumentException();

            return ToListContactWrapper(DaoFactory.GetContactDao().GetMembers(companyid));
        }


        /// <summary>
        ///   Adds the selected person to the company with the ID specified in the request
        /// </summary>
        /// <param optional="true"  name="companyid">Company ID</param>
        /// <param optional="true" name="personid">Person ID</param>
        /// <short>Add person to company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Person
        /// </returns>
        [Create(@"contact/company/{companyid:[0-9]+}/person")]
        public PersonWrapper AddPeopleToCompany(
            int companyid,
            int personid)
        {
            if ((companyid <= 0) || (personid <= 0))
                throw new ArgumentException();

            var company = DaoFactory.GetContactDao().GetByID(companyid);
            var person = DaoFactory.GetContactDao().GetByID(personid);

            if (person == null || company == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().AddMember(personid, companyid);

            return (PersonWrapper)ToContactWrapper(person);
        }

        /// <summary>
        ///   Deletes the selected person from the company with the ID specified in the request
        /// </summary>
        /// <param optional="true"  name="companyid">Company ID</param>
        /// <param optional="true" name="personid">Person ID</param>
        /// <short>Delete person from company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///    Person
        /// </returns>
        [Delete(@"contact/company/{companyid:[0-9]+}/person")]
        public PersonWrapper DeletePeopleFromCompany(
            int companyid,
            int personid)
        {
            if ((companyid <= 0) || (personid <= 0))
                throw new ArgumentException();


            var company = DaoFactory.GetContactDao().GetByID(companyid);
            var person = DaoFactory.GetContactDao().GetByID(personid);

            if (person == null || company == null)
                throw new ItemNotFoundException();

            DaoFactory.GetContactDao().RemoveMember(personid);


            return (PersonWrapper)ToContactWrapper(person);
        }


        /// <summary>
        ///    Creates the person with the parameters (first name, last name, description, etc.) specified in the request
        /// </summary>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param optional="true"  name="jobTitle">Post</param>
        /// <param optional="true" name="companyId">Company ID</param>
        /// <param optional="true" name="about">Person description text</param>
        /// <param name="isShared">Person privacy: shared or not</param>
        /// <param optional="true" name="managerList">List of managers for the person</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Create person</short> 
        /// <category>Contacts</category>
        /// <return>Person</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/person")]
        public PersonWrapper CreatePerson(String firstName,
                                          String lastName,
                                          String jobTitle,
                                          int companyId,
                                          String about,
                                          bool isShared,
                                          IEnumerable<Guid> managerList,
                                          IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                          HttpPostedFileBase photo)
        {
            var peopleInst = new Person
                                 {
                                     FirstName = firstName,
                                     LastName = lastName,
                                     JobTitle = jobTitle,
                                     CompanyID = companyId,
                                     About = about,
                                     IsShared = isShared
                                 };

            peopleInst.ID = DaoFactory.GetContactDao().SaveContact(peopleInst);
            peopleInst.CreateBy = Core.SecurityContext.CurrentAccount.ID;
            peopleInst.CreateOn = DateTime.UtcNow;

            var managerListLocal = managerList.ToList();
            if (managerListLocal.Count != 0)
            {
                CRMSecurity.SetAccessTo(peopleInst, managerListLocal);
            }

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, peopleInst.ID, field.Key, field.Value);
            }

            var result = (PersonWrapper)ToContactWrapper(peopleInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(peopleInst.ID, photo);

            return result;

        }

        /// <summary>
        ///    Changes the photo for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short> Change contact photo</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Path to contact photo
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/changephoto")]
        public String ChangeContactPhoto(int contactid, HttpPostedFileBase photo)
        {
            if (contactid == 0)
                throw new ArgumentException();

            if (!(photo.ContentType.StartsWith("image/") && photo.ContentLength > 0)) return String.Empty;

            if (!photo.InputStream.CanRead) return String.Empty;

            var buffer = new byte[photo.ContentLength];

            photo.InputStream.Read(buffer, 0, buffer.Length);

            return ContactPhotoManager.UploadPhoto(buffer, contactid);
        }

        /// <summary>
        ///    Changes the photo for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="photourl">contact photo url</param>
        /// <short> Change contact photo</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Path to contact photo
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/changephotobyurl")]
        public String ChangeContactPhoto(int contactid, String photourl)
        {
            if (contactid == 0)
                throw new ArgumentException();

            if (String.IsNullOrEmpty(photourl)) return String.Empty;

            var photoData = getImageFromUrl(photourl);

            return ContactPhotoManager.UploadPhoto(photoData, contactid);
        }

        private byte[] getImageFromUrl(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (var stream = response.GetResponseStream())
                    {
                        var buffer = new byte[response.ContentLength];
                        stream.Read(buffer, 0, buffer.Length);
                        return buffer;
                    }
                }
            }
            return new byte[0];
        }



        /// <summary>
        ///    Merge two selected contacts
        /// </summary>
        /// <param name="fromcontactid">the first contact ID for merge</param>
        /// <param name="tocontactid">the second contact ID for merge</param>
        /// <short>Merge contacts</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///    Contact
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        [Update(@"contact/merge")]
        public ContactWrapper MergeContacts(int fromcontactid, int tocontactid)
        {
            if (fromcontactid <= 0 || tocontactid <= 0)
                throw new ArgumentException();

            DaoFactory.GetContactDao().MergeDublicate(fromcontactid, tocontactid);
            var resultContact = DaoFactory.GetContactDao().GetByID(tocontactid);
            return ToContactWrapper(resultContact);
        }


        /// <summary>
        ///    Updates the selected person with the parameters (first name, last name, description, etc.) specified in the request
        /// </summary>
        /// <param name="personid">Person ID</param>
        /// <param name="firstName">First name</param>
        /// <param name="lastName">Last name</param>
        /// <param optional="true"  name="jobTitle">Post</param>
        /// <param optional="true" name="companyId">Company ID</param>
        /// <param optional="true" name="about">Person description text</param>
        /// <param name="isShared">Person privacy: shared or not</param>
        /// <param optional="true" name="managerList">List of persons managers</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Update person</short> 
        /// <category>Contacts</category>
        /// <return>Person</return>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update(@"contact/person/{personid:[0-9]+}")]
        public PersonWrapper UpdatePerson(
                                          int personid,
                                          String firstName,
                                          String lastName,
                                          String jobTitle,
                                          int companyId,
                                          String about,
                                          bool isShared,
                                          IEnumerable<Guid> managerList,
                                          IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                          HttpPostedFileBase photo)
        {

            if (personid == 0 || String.IsNullOrEmpty(firstName) || String.IsNullOrEmpty(lastName))
                throw new ArgumentException();

            var peopleInst = new Person
            {
                ID = personid,
                FirstName = firstName,
                LastName = lastName,
                JobTitle = jobTitle,
                CompanyID = companyId,
                About = about,
                IsShared = isShared
            };

            DaoFactory.GetContactDao().UpdateContact(peopleInst);

            peopleInst = (Person)DaoFactory.GetContactDao().GetByID(peopleInst.ID);

            var managerListLocal = managerList.ToList();
            if (managerListLocal.Count > 0)
                CRMSecurity.SetAccessTo(peopleInst, managerListLocal);

            foreach (var field in customFieldList)
                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Person, peopleInst.ID, field.Key, field.Value);

            var result = (PersonWrapper)ToContactWrapper(peopleInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(peopleInst.ID, photo);

            return result;
        }

        /// <summary>
        ///    Creates the company with the parameters specified in the request
        /// </summary>
        /// <param  name="companyName">Company name</param>
        /// <param optional="true" name="about">Company description text</param>
        /// <param optional="true" name="personList">Linked person list</param>
        /// <param name="isShared">Company privacy: shared or not</param>
        /// <param optional="true" name="managerList">List of managers for the company</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <param optional="true" name="photo">Contact photo (upload using multipart/form-data)</param>
        /// <short>Create company</short> 
        /// <category>Contacts</category>
        /// <return>Company</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/company")]
        public CompanyWrapper CreateCompany(
                                           String companyName,
                                           String about,
                                           IEnumerable<int> personList,
                                           bool isShared,
                                           IEnumerable<Guid> managerList,
                                           IEnumerable<ItemKeyValuePair<int, String>> customFieldList,
                                           HttpPostedFileBase photo)
        {
            var companyInst = new Company
                                  {
                                      CompanyName = companyName,
                                      About = about,
                                      IsShared = isShared
                                  };


            companyInst.ID = DaoFactory.GetContactDao().SaveContact(companyInst);
            companyInst.CreateBy = Core.SecurityContext.CurrentAccount.ID;
            companyInst.CreateOn = DateTime.UtcNow;

            var personListLocal = personList.ToList();

            foreach (var personID in personListLocal)
                AddPeopleToCompany(companyInst.ID, personID);

            var managerListLocal = managerList.ToList();
            if (managerListLocal.Count > 0)
                CRMSecurity.SetAccessTo(companyInst, managerListLocal);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;
                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, companyInst.ID, field.Key, field.Value);
            }

            var result = (CompanyWrapper)ToContactWrapper(companyInst);

            if (photo != null)
                result.SmallFotoUrl = ChangeContactPhoto(companyInst.ID, photo);

            return result;
        }


        /// <summary>
        ///    Quickly creates the list of companies
        /// </summary>
        /// <short>
        ///    Quick company list creation
        /// </short>
        /// <param name="companyName">Company name</param>
        /// <category>Contacts</category>
        /// <return>Contact list</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/company/quick")]
        public IEnumerable<ContactBaseWrapper> CreateCompany(IEnumerable<String> companyName)
        {
            var contacts = new List<Contact>();

            int recordIndex = 0;

            if (companyName == null)
                throw new ArgumentException();

            foreach (var item in companyName)
            {
                if (String.IsNullOrEmpty(item)) continue;

                contacts.Add(new Company
                                 {
                                     ID = recordIndex++,
                                     CompanyName = item,
                                     IsShared = false
                                 });
            }

            if (contacts.Count == 0) return null;

            DaoFactory.GetContactDao().SaveContactList(contacts);

            var selectedManagers = new List<Guid>();
            selectedManagers.Add(ASC.Core.SecurityContext.CurrentAccount.ID);

            foreach (var ct in contacts)
            {
                CRMSecurity.SetAccessTo(ct, selectedManagers);
            }

            return contacts.ConvertAll(ToContactBaseWrapper);
        }

        /// <summary>
        ///    Quickly creates the list of persons with the first and last names specified in the request
        /// </summary>
        /// <short>
        ///    Quick person list creation
        /// </short>
        /// <param name="data">Pairs: user first name, user last name</param>
        /// <category>Contacts</category>
        /// <return>Contact list</return>
        /// <exception cref="ArgumentException"></exception>
        [Create(@"contact/person/quick")]
        public IEnumerable<ContactBaseWrapper> CreatePerson(IEnumerable<ItemKeyValuePair<String, String>> data)
        {
            var contacts = new List<Contact>();

            if (data == null) return null;

            int recordIndex = 0;

            foreach (var item in data)
            {
                if (String.IsNullOrEmpty(item.Key) || String.IsNullOrEmpty(item.Value)) continue;

                contacts.Add(new Person
                {
                    ID = recordIndex++,
                    FirstName = item.Key,
                    LastName = item.Value,
                    IsShared = false
                });
            }

            if (contacts.Count == 0) return null;

            DaoFactory.GetContactDao().SaveContactList(contacts);

            var selectedManagers = new List<Guid>();
            selectedManagers.Add(ASC.Core.SecurityContext.CurrentAccount.ID);

            foreach (var ct in contacts)
            {
                CRMSecurity.SetAccessTo(ct, selectedManagers);
            }

            return contacts.ConvertAll(ToContactBaseWrapper);
        }


        /// <summary>
        ///    Updates the selected company with the parameters specified in the request
        /// </summary>
        /// <param name="companyid">Company ID</param>
        /// <param  name="companyName">Company name</param>
        /// <param optional="true" name="about">Company description text</param>
        /// <param name="isShared">Company privacy: shared or not</param>
        /// <param optional="true" name="managerList">List of company managers</param>
        /// <param optional="true" name="customFieldList">User field list</param>
        /// <short>Update company</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Company
        /// </returns>
        [Update(@"contact/company/{companyid:[0-9]+}")]
        public CompanyWrapper UpdateCompany(
                                           int companyid,
                                           String companyName,
                                           String about,
                                           bool isShared,
                                           IEnumerable<Guid> managerList,
                                           IEnumerable<ItemKeyValuePair<int, String>> customFieldList)
        {
            var companyInst = new Company
            {
                ID = companyid,
                CompanyName = companyName,
                About = about,
                IsShared = isShared
            };

            DaoFactory.GetContactDao().UpdateContact(companyInst);

            companyInst = (Company)DaoFactory.GetContactDao().GetByID(companyInst.ID);

            var managerListLocal = managerList.ToList();

            if (managerListLocal.Count > 0)
                CRMSecurity.SetAccessTo(companyInst, managerListLocal);

            foreach (var field in customFieldList)
            {
                if (String.IsNullOrEmpty(field.Value)) continue;

                DaoFactory.GetCustomFieldDao().SetFieldValue(EntityType.Company, companyInst.ID, field.Key, field.Value);
            }

            return (CompanyWrapper)ToContactWrapper(companyInst);

        }


        /// <summary>
        ///    Updates the selected contact status
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param  name="contactStatusid">Contact status ID</param>
        /// <short>Update contact status</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Company
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/status")]
        public ContactWrapper UpdateContactStatus(int contactid, int contactStatusid)
        {
            if (contactid <= 0 || contactStatusid < 0)
                throw new ArgumentException();

            if (contactStatusid > 0)
            {
                var curListItem = DaoFactory.GetListItemDao().GetByID(contactStatusid);
                if (curListItem == null)
                    throw new ItemNotFoundException();
            }

            var companyInst = DaoFactory.GetContactDao().GetByID(contactid);
            if (companyInst == null)
                throw new ItemNotFoundException();

            companyInst.StatusID = contactStatusid;

            DaoFactory.GetContactDao().UpdateContact(companyInst);

            companyInst = DaoFactory.GetContactDao().GetByID(companyInst.ID);

            return ToContactWrapper(companyInst);
        }


        [Update(@"contact/company/{companyid:[0-9]+}/status")]
        public ContactWrapper UpdateCompanyAndParticipantsStatus(int companyid, int contactStatusid)
        {
            if (companyid <= 0 || contactStatusid < 0)
                throw new ArgumentException();

            if (contactStatusid > 0)
            {
                var curListItem = DaoFactory.GetListItemDao().GetByID(contactStatusid);
                if (curListItem == null)
                    throw new ItemNotFoundException();
            }

            var companyInst = DaoFactory.GetContactDao().GetByID(companyid);
            if (companyInst == null)
                throw new ItemNotFoundException();

            if (companyInst is Person)
                throw new Exception("It is not a company");

            DaoFactory.GetContactDao().UpdateCompanyAndParticipantsStatus(companyid, contactStatusid);

            return ToContactWrapper(companyInst);
        }

        [Update(@"contact/person/{personid:[0-9]+}/status")]
        public ContactWrapper UpdatePersonAndItsCompanyStatus(int personid, int contactStatusid)
        {
            if (personid <= 0 || contactStatusid < 0)
                throw new ArgumentException();

            if (contactStatusid > 0)
            {
                var curListItem = DaoFactory.GetListItemDao().GetByID(contactStatusid);
                if (curListItem == null)
                    throw new ItemNotFoundException();
            }

            var personInst = DaoFactory.GetContactDao().GetByID(personid);
            if (personInst == null)
                throw new ItemNotFoundException();

            if (personInst is Company)
                throw new Exception("It is not a person");

            var companyID = ((Person)personInst).CompanyID;
            if (companyID != 0)
            {
                DaoFactory.GetContactDao().UpdateCompanyAndParticipantsStatus(companyID, contactStatusid);
            }
            else
            {
                personInst.StatusID = contactStatusid;
                DaoFactory.GetContactDao().UpdateContact(personInst);
            }

            personInst = DaoFactory.GetContactDao().GetByID(personInst.ID);
            return ToContactWrapper(personInst);
        }

        /// <summary>
        ///   Sets access rights for other users to the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="isShared">Contact privacy: private or not</param>
        /// <param name="managerList">List of managers</param>
        /// <short>Set contact access rights</short> 
        /// <category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        ///<exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact
        /// </returns>
        [Update("contact/{contactid:[0-9]+}/access")]
        public ContactWrapper SetAccessToContact(
            int contactid,
            bool isShared,
            IEnumerable<Guid> managerList)
        {

            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            if (!(CRMSecurity.IsAdmin || contact.CreateBy == Core.SecurityContext.CurrentAccount.ID))
                throw new SecurityException(""); ;

            SetAccessToContact(contact, isShared, managerList);

            return ToContactWrapper(contact);
        }

        private void SetAccessToContact(
            Contact contact,
            bool isShared,
            IEnumerable<Guid> managerList)
        {

            var managerListLocal = managerList.ToList();
            
            if (managerListLocal.Count > 0)
                CRMSecurity.SetAccessTo(contact, managerListLocal);
            else
                CRMSecurity.MakePublic(contact);

            DaoFactory.GetContactDao().MakePublic(contact.ID, isShared);

        }


        /// <summary>
        ///   Sets access rights for other users to the list of contacts with the IDs specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID list</param>
        /// <param name="isShared">Company privacy: shared or not</param>
        /// <param name="managerList">List of managers</param>
        /// <short>Set contact access rights</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Update("contact/access")]
        public IEnumerable<ContactWrapper> SetAccessToBatchContact(
            IEnumerable<int> contactid,
            bool isShared,
            IEnumerable<Guid> managerList
            )
        {

            var result = new List<ContactWrapper>();

            foreach (var id in contactid)
            {
                try
                {

                    var contactWrapper = SetAccessToContact(id, isShared, managerList);

                    result.Add(contactWrapper);

                }
                catch (Exception)
                {

                }
            }

            return result;
        }


        /// <summary>
        ///   Sets access rights for the selected user to the list of contacts with the parameters specified in the request
        /// </summary>
        /// <param name="isPrivate">Contact privacy: private or not</param>
        /// <param name="managerList">List of managers</param>
        /// <param optional="true" name="tags">Tag</param>
        /// <param optional="true" name="contactStage">Contact stage ID (warmth)</param>
        /// <param optional="true" name="contactType">Contact type ID</param>
        /// <param optional="true" name="contactListView" remark="Allowed values: Company, Person, WithOpportunity"></param>
        /// <param optional="true" name="fromDate">Start date</param>
        /// <param optional="true" name="toDate">End date</param>
        /// <short>Set contact access rights</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Update("contact/filter/access")]
        public IEnumerable<ContactWrapper> SetAccessToBatchContact(
            IEnumerable<String> tags,
            int contactStage,
            int contactType,
            ContactListViewType contactListView,
            ApiDateTime fromDate,
            ApiDateTime toDate,
            bool isPrivate,
            IEnumerable<Guid> managerList
            )
        {

            var result = new List<Contact>();

            var contacts = DaoFactory.GetContactDao().GetContacts(_context.FilterValue, tags,
                                                                    contactStage, contactType, contactListView,
                                                                    fromDate, toDate,
                                                                    0, 0,
                                                                    null);

            if (!contacts.Any())
                return Enumerable.Empty<ContactWrapper>();

            foreach (var contact in contacts)
            {
                if (contact == null)
                    throw new ItemNotFoundException();

                if (!(CRMSecurity.IsAdmin || contact.CreateBy == Core.SecurityContext.CurrentAccount.ID)) continue;

                SetAccessToContact(contact, isPrivate, managerList);

                result.Add(contact);
            }
            return ToListContactWrapper(result);
        }


        /// <summary>
        ///     Deletes the contact with the ID specified in the request from the portal
        /// </summary>
        /// <short>Delete contact</short> 
        /// <category>Contacts</category>
        /// <param name="contactid">Contact ID</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}")]
        public ContactWrapper DeleteContact(int contactid)
        {
            if (contactid <= 0)
                throw new ArgumentException();

            var contact = DaoFactory.GetContactDao().GetByID(contactid);

            if (contact == null)
                throw new ItemNotFoundException();

            var contactWrapper = ToContactWrapper(contact);

            DaoFactory.GetContactDao().DeleteContact(contactid);

            return contactWrapper;

        }

        /// <summary>
        ///   Deletes the group of contacts with the IDs specified in the request
        /// </summary>
        /// <param name="contactids">Contact ID list</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <short>Delete contact group</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact list
        /// </returns>
        [Update(@"contact")]
        public IEnumerable<ContactBaseWrapper> DeleteBatchContacts(IEnumerable<int> contactids)
        {
            if (contactids == null)
                throw new ArgumentException();

            var contacts = DaoFactory.GetContactDao().GetContacts(contactids.ToArray());

            var result = contacts.Select(contact => ToContactBaseWrapper(contact));

            DaoFactory.GetContactDao().DeleteBatchContact(contacts);

            return result;

        }

        /// <summary>
        ///    Returns the list of 30 contacts in the CRM module with prefix
        /// </summary>
        /// <param optional="true" name="prefix"></param>
        /// <param optional="false" name="searchType" remark="Allowed values: -1 (Any), 0 (Company), 1 (Persons), 2 (PersonsWithoutCompany), 3 (CompaniesAndPersonsWithoutCompany)">searchType</param>
        /// <param optional="true" name="entityType"></param>
        /// <param optional="true" name="entityID"></param>
        /// <category>Contacts</category>
        /// <returns>
        ///    Contact list
        /// </returns>
        /// <visible>false</visible>
        [Read(@"contact/byprefix")]
        public IEnumerable<ContactBaseWrapper> GetContactsByPrefix(string prefix, int searchType, EntityType entityType, int entityID)
        {
            List<ContactBaseWrapper> result = new List<ContactBaseWrapper>();
            var allContacts = new List<Contact>();

            if (entityID > 0)
            {
                var findedContacts = new List<Contact>();
                switch (entityType)
                {
                    case EntityType.Opportunity:
                        allContacts = DaoFactory.GetContactDao().GetContacts(DaoFactory.GetDealDao().GetMembers(entityID));
                        break;
                    case EntityType.Case:
                        allContacts = DaoFactory.GetContactDao().GetContacts(DaoFactory.GetCasesDao().GetMembers(entityID));
                        break;
                }

                foreach (var c in allContacts)
                {
                    if (c is Person)
                    {
                        var people = (Person)c;
                        if (ASC.Core.Users.UserFormatter.GetUserName(people.FirstName, people.LastName).IndexOf(prefix) != -1)
                            findedContacts.Add(c);
                    }
                    else
                    {
                        var company = (Company)c;
                        if (company.CompanyName.IndexOf(prefix) != -1)
                            findedContacts.Add(c);

                    }
                }
                foreach (var cont in findedContacts)
                {
                    result.Add(ToContactBaseWrapper(cont));
                }
                _context.SetTotalCount(findedContacts.Count);

            }
            else
            {
                var maxItemCount = 30;
                if (searchType < -1 || searchType > 3) throw new ArgumentException();
                allContacts = DaoFactory.GetContactDao().GetContactsByPrefix(prefix, searchType, 0, maxItemCount);
                foreach (var cont in allContacts)
                {
                    result.Add(ToContactBaseWrapper(cont));
                }
            }

            return result;

        }


        private IEnumerable<ContactWithTaskWrapper> ToSimpleListContactWrapper(IReadOnlyList<Contact> itemList)
        {
            if (itemList.Count == 0) return new List<ContactWithTaskWrapper>();

            var result = new List<ContactWithTaskWrapper>();

            var personsIDs = new List<int>();
            var companyIDs = new List<int>();
            var contactIDs = new int[itemList.Count];

            var peopleCompanyIDs = new List<int>();

            var peopleCompanyList = new Dictionary<int, ContactBaseWrapper>();

            for (int index = 0; index < itemList.Count; index++)
            {
                var contact = itemList[index];

                if (contact is Company)
                    companyIDs.Add(contact.ID);
                else if (contact is Person)
                {
                    var person = (Person)contact;

                    personsIDs.Add(contact.ID);

                    if (person.CompanyID > 0)
                        peopleCompanyIDs.Add(person.CompanyID);
                }

                contactIDs[index] = itemList[index].ID;
            }

            if (peopleCompanyIDs.Count > 0)
                peopleCompanyList = DaoFactory.GetContactDao().GetContacts(peopleCompanyIDs.ToArray())
                                   .ToDictionary(item => item.ID,
                                                         ToContactBaseWrapper);


            var contactInfos = new Dictionary<int, List<ContactInfoWrapper>>();

            var addresses = new Dictionary<int, List<Address>>();

            DaoFactory.GetContactInfoDao().GetAll(contactIDs).ForEach(
                item =>
                {
                    if (item.InfoType == ContactInfoType.Address)
                    {
                        if (!addresses.ContainsKey(item.ContactID))
                            addresses.Add(item.ContactID, new List<Address>
                                                               {
                                                                   new Address(item)
                                                               });
                        else
                            addresses[item.ContactID].Add(new Address(item));
                    }
                    else
                    {
                        if (!contactInfos.ContainsKey(item.ContactID))
                            contactInfos.Add(item.ContactID, new List<ContactInfoWrapper> { new ContactInfoWrapper(item) });
                        else
                            contactInfos[item.ContactID].Add(new ContactInfoWrapper(item));
                    }
                }
           );

            var nearestTasks = DaoFactory.GetTaskDao().GetNearestTask(contactIDs.ToArray());

            IEnumerable<TaskCategoryBaseWrapper> taskCategories = new List<TaskCategoryBaseWrapper>();

            if (nearestTasks.Any())
            {
                taskCategories = DaoFactory.GetListItemDao().GetItems(ListType.TaskCategory).ConvertAll(item => new TaskCategoryBaseWrapper(item));
            }

            foreach (var contact in itemList)
            {
                ContactWrapper contactWrapper;

                if (contact is Person)
                {
                    var people = (Person)contact;

                    var peopleWrapper = new PersonWrapper(people);

                    if (people.CompanyID > 0 && peopleCompanyList.ContainsKey(people.CompanyID))
                    {
                        peopleWrapper.Company = peopleCompanyList[people.CompanyID];
                    }

                    contactWrapper = peopleWrapper;
                }
                else if (contact is Company)
                {
                    contactWrapper = new CompanyWrapper((Company)contact);
                }
                else
                {
                    throw new ArgumentException();
                }


                contactWrapper.CommonData = contactInfos.ContainsKey(contact.ID) ? contactInfos[contact.ID] : new List<ContactInfoWrapper>();

                TaskBaseWrapper taskWrapper = null;

                if (nearestTasks.ContainsKey(contactWrapper.ID))
                {
                    var task = nearestTasks[contactWrapper.ID];
                    taskWrapper = new TaskBaseWrapper(task);

                    if (task.CategoryID > 0)
                        taskWrapper.Category = taskCategories.First(x => x.ID == task.CategoryID);
                }

                result.Add(new ContactWithTaskWrapper
                    {
                        Contact = contactWrapper,
                        Task = taskWrapper
                    });
            }

            return result;
        }

        private IEnumerable<ContactWrapper> ToListContactWrapper(List<Contact> itemList)
        {

            if (itemList.Count == 0) return new List<ContactWrapper>();

            var result = new List<ContactWrapper>();

            var personsIDs = new List<int>();
            var companyIDs = new List<int>();
            var contactIDs = new int[itemList.Count];

            var peopleCompanyIDs = new List<int>();

            var peopleCompanyList = new Dictionary<int, ContactBaseWrapper>();

            for (int index = 0; index < itemList.Count; index++)
            {
                var contact = itemList[index];

                if (contact is Company)
                    companyIDs.Add(contact.ID);
                else if (contact is Person)
                {
                    var person = (Person)contact;

                    personsIDs.Add(contact.ID);

                    if (person.CompanyID > 0)
                        peopleCompanyIDs.Add(person.CompanyID);
                }

                contactIDs[index] = itemList[index].ID;
            }

            if (peopleCompanyIDs.Count > 0)
                peopleCompanyList = DaoFactory.GetContactDao().GetContacts(peopleCompanyIDs.ToArray())
                                   .ToDictionary(item => item.ID,
                                                         ToContactBaseWrapper);

            var companiesMembersCount = DaoFactory.GetContactDao().GetMembersCount(companyIDs.Distinct().ToArray());

            var contactStatusIDs = itemList.Select(item => item.StatusID).Distinct().ToArray();
            var contactInfos = new Dictionary<int, List<ContactInfoWrapper>>();

            var haveLateTask = DaoFactory.GetTaskDao().HaveLateTask(contactIDs);
            var contactStatus = DaoFactory.GetListItemDao().GetItems(contactStatusIDs).ToDictionary(item => item.ID,
                                                                                    item => new ContactStatusBaseWrapper(item));

            var personsCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person,
                                                                                    personsIDs.ToArray());

            var companyCustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company,
                                                                                    companyIDs.ToArray());


            var customFields = personsCustomFields.Union(companyCustomFields).GroupBy(item => item.EntityID).ToDictionary(
                           item => item.Key, item => item.Select(ToCustomFieldBaseWrapper));


            var addresses = new Dictionary<int, List<Address>>();
            var taskCount = DaoFactory.GetTaskDao().GetTasksCount(contactIDs);

            var contactTags = DaoFactory.GetTagDao().GetEntitiesTags(EntityType.Contact);

            DaoFactory.GetContactInfoDao().GetAll(contactIDs).ForEach(
                 item =>
                 {
                     if (item.InfoType == ContactInfoType.Address)
                     {
                         if (!addresses.ContainsKey(item.ContactID))
                             addresses.Add(item.ContactID, new List<Address>
                                                               {
                                                                   new Address(item)
                                                               });
                         else
                             addresses[item.ContactID].Add(new Address(item));
                     }
                     else
                     {
                         if (!contactInfos.ContainsKey(item.ContactID))
                             contactInfos.Add(item.ContactID, new List<ContactInfoWrapper> { new ContactInfoWrapper(item) });
                         else
                             contactInfos[item.ContactID].Add(new ContactInfoWrapper(item));
                     }
                 }
            );

            foreach (var contact in itemList)
            {

                ContactWrapper contactWrapper;

                if (contact is Person)
                {
                    var people = (Person)contact;

                    var peopleWrapper = new PersonWrapper(people);

                    if (people.CompanyID > 0 && peopleCompanyList.ContainsKey(people.CompanyID))
                    {
                        peopleWrapper.Company = peopleCompanyList[people.CompanyID];
                    }

                    contactWrapper = peopleWrapper;

                }
                else if (contact is Company)
                {
                    contactWrapper = new CompanyWrapper((Company)contact);

                    if (companiesMembersCount.ContainsKey(contactWrapper.ID))
                        ((CompanyWrapper)contactWrapper).PersonsCount = companiesMembersCount[contactWrapper.ID];
                }
                else
                    throw new ArgumentException();

                if (contactTags.ContainsKey(contact.ID))
                    contactWrapper.Tags = contactTags[contact.ID];

                if (addresses.ContainsKey(contact.ID))
                    contactWrapper.Addresses = addresses[contact.ID];

                contactWrapper.CommonData = contactInfos.ContainsKey(contact.ID) ? contactInfos[contact.ID] : new List<ContactInfoWrapper>();

                if (contactStatus.ContainsKey(contact.StatusID))
                    contactWrapper.ContactStatus = contactStatus[contact.StatusID];

                contactWrapper.HaveLateTasks = haveLateTask.ContainsKey(contact.ID) && haveLateTask[contact.ID];

                contactWrapper.CustomFields = customFields.ContainsKey(contact.ID) ? customFields[contact.ID] : new List<CustomFieldBaseWrapper>();

                contactWrapper.TaskCount = taskCount.ContainsKey(contact.ID) ? taskCount[contact.ID] : 0;

                result.Add(contactWrapper);
            }

            return result;

        }

        private static ContactBaseWrapper ToContactBaseWrapper(Contact contact)
        {
            return contact == null ? null : new ContactBaseWrapper(contact);
        }

        private ContactWrapper ToContactWrapper(Contact contact)
        {
            ContactWrapper result;

            if (contact is Person)
            {
                var people = (Person)contact;

                var peopleWrapper = new PersonWrapper(people);

                if (people.CompanyID > 0)
                    peopleWrapper.Company = ToContactBaseWrapper(DaoFactory.GetContactDao().GetByID(people.CompanyID));

                result = peopleWrapper;

            }
            else if (contact is Company)
            {
                result = new CompanyWrapper((Company)contact);

                ((CompanyWrapper)result).PersonsCount = DaoFactory.GetContactDao().GetMembersCount(result.ID);

            }
            else
                throw new ArgumentException();

            if (contact.StatusID > 0)
            {
                var listItem = DaoFactory.GetListItemDao().GetByID(contact.StatusID);
                if (listItem == null)
                    throw new ItemNotFoundException();
                result.ContactStatus = new ContactStatusBaseWrapper(listItem);
            }

            result.TaskCount = DaoFactory.GetTaskDao().GetTasksCount(contact.ID);
            result.HaveLateTasks = DaoFactory.GetTaskDao().HaveLateTask(contact.ID);

            var contactInfos = new List<ContactInfoWrapper>();
            var addresses = new List<Address>();

            var data = DaoFactory.GetContactInfoDao().GetList(contact.ID, null, null, null);

            foreach (var contactInfo in data)
                if (contactInfo.InfoType == ContactInfoType.Address)
                    addresses.Add(new Address(contactInfo));
                else
                    contactInfos.Add(new ContactInfoWrapper(contactInfo));

            result.Addresses = addresses;
            result.CommonData = contactInfos;

            if (contact is Person)
                result.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Person, contact.ID, false).ConvertAll(item => new CustomFieldBaseWrapper(item)).ToSmartList();
            else
                result.CustomFields = DaoFactory.GetCustomFieldDao().GetEnityFields(EntityType.Company, contact.ID, false).ConvertAll(item => new CustomFieldBaseWrapper(item)).ToSmartList();

            return result;
        }

        private ContactBaseWithEmailWrapper ToContactBaseWithEmailWrapper(Contact contact)
        {
            if (contact == null) return null;

            ContactBaseWithEmailWrapper result = new ContactBaseWithEmailWrapper(contact);
            var primaryEmail = DaoFactory.GetContactInfoDao().GetList(contact.ID, ContactInfoType.Email, null, true);
            if (primaryEmail == null || primaryEmail.Count == 0)
            {
                result.Email = null;
            }
            else
            {
                result.Email = new ContactInfoWrapper(primaryEmail.FirstOrDefault());
            }
            return result;
        }


    }
}
