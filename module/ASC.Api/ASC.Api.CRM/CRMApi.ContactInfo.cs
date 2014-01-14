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
using System.Text;
using ASC.Api.Attributes;
using ASC.Api.CRM.Wrappers;
using ASC.Api.Collections;
using ASC.Api.Employee;
using ASC.Api.Exceptions;
using ASC.CRM.Core;
using ASC.CRM.Core.Entities;
using ASC.Specific;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Api.CRM
{
    public partial class CRMApi
    {

        /// <summary>
        ///   Returns the list of all available contact categories
        /// </summary>
        /// <param name="infoType">
        ///    Contact information type
        /// </param>
        /// <short>Get all categories</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   List of all available contact categories
        /// </returns>
        [Read(@"contact/data/{infoType}/category")]
        public IEnumerable<String> GetContactInfoCategory(ContactInfoType infoType)
        {
            return Enum.GetNames(ContactInfo.GetCategory(infoType)).ToItemList();
        }

        /// <summary>
        ///   Returns the list of all available contact information types
        /// </summary>
        /// <short>Get all contact info types</short> 
        /// <category>Contacts</category>
        /// <returns></returns>
        [Read(@"contact/data/infoType")]
        public IEnumerable<String> GetContactInfoType()
        {
            return Enum.GetNames(typeof(ContactInfoType)).ToItemList();
        }

        /// <summary>
        ///   Returns the detailed list of all information available for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact information ID</param>
        /// <param name="id">Contact ID</param>
        /// <short>Get contact info</short> 
        /// <category>Contacts</category>
        /// <returns>Contact information</returns>
        ///<exception cref="ArgumentException"></exception>
        [Read(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper GetContactInfoByID(int contactid, int id)
        {
            if (contactid <= 0 || id <= 0)
                throw new ArgumentException();

            return ToContactInfoWrapper(DaoFactory.GetContactInfoDao().GetByID(id));
        }

        /// <summary>
        ///    Adds the information with the parameters specified in the request to the contact with the selected ID
        /// </summary>
        ///<param name="contactid">Contact ID</param>
        ///<param name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param name="isPrimary">Contact importance: primary or not</param>
        ///<param   name="category">Category</param>
        ///<short> Add contact info</short> 
        ///<category>Contacts</category>
        /// <seealso cref="GetContactInfoType"/>
        /// <seealso cref="GetContactInfoCategory"/>
        /// <returns>
        ///    Contact information
        /// </returns> 
        ///<exception cref="ArgumentException"></exception>
        [Create(@"contact/{contactid:[0-9]+}/data")]
        public ContactInfoWrapper CreateContactInfo(int contactid,
                                                 ContactInfoType infoType,
                                                 String data,
                                                 bool isPrimary,
                                                 String category
                                             )
        {
            if (String.IsNullOrEmpty(data) || contactid == 0)
                throw new ArgumentException();

            var categoryType = ContactInfo.GetCategory(infoType);

            if (!Enum.IsDefined(categoryType, category))
                throw new ArgumentException();

            var contactInfo = new ContactInfo
                                  {
                                      Data = data,
                                      InfoType = infoType,
                                      ContactID = contactid,
                                      IsPrimary = isPrimary,
                                      Category = (int)Enum.Parse(categoryType, category)
                                  };

            var contactInfoID = DaoFactory.GetContactInfoDao().Save(contactInfo);

            if (contactInfo.InfoType == ContactInfoType.Email)
            {
                var contact = DaoFactory.GetContactDao().GetByID(contactInfo.ContactID);
                var userIds = CRMSecurity.GetAccessSubjectTo(contact).Keys.ToList();
                var emails = new[] { contactInfo.Data };
                DaoFactory.GetContactInfoDao().UpdateMailAggregator(emails, userIds);
            }

            var contactInfoWrapper = ToContactInfoWrapper(contactInfo);

            contactInfoWrapper.ID = contactInfoID;

            return contactInfoWrapper;
        }

        /// <summary>
        ///  Creates contact information with the parameters specified in the request
        /// </summary>
        ///<short>Group contact info</short> 
        ///<param name="items">Contact information</param>
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Create(@"contact/data")]
        public IEnumerable<ContactInfoWrapper> CreateBatchContactInfo(IEnumerable<ContactInfoWrapper> items)
        {

            var ids = DaoFactory.GetContactInfoDao().SaveList(items.Select(x=>FromContactInfoWrapper(x)).ToList());

            var result = items.ToList();

            for (int index = 0; index < result.Count; index++)
            {
                var infoWrapper = result[index];

                infoWrapper.ID = ids[index];

            }


            return result;
        }

		/// <summary>
		///  Creates contact information with the parameters specified in the request for the contact with the selected ID
		/// </summary>
		///<short>Group contact info</short> 
		/// <param name="contactid">Contact ID</param>
		/// <param name="items">Contact information</param>
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [Create(@"contact/{contactid:[0-9]+}/batch")]
        public IEnumerable<ContactInfoWrapper> CreateBatchContactInfo(int contactid, IEnumerable<ContactInfoWrapper> items)
        {
            var contactInfoList = items.Select(x => FromContactInfoWrapper(x)).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                contactInfo.ContactID = contactid;
            }

            var ids = DaoFactory.GetContactInfoDao().SaveList(contactInfoList);

            var result = items.ToList();

            for (int index = 0; index < result.Count; index++)
            {
                var infoWrapper = result[index];

                infoWrapper.ID = ids[index];

            }

            return result;
        }

        /// <summary>
        ///   Updates the information with the parameters specified in the request for the contact with the selected ID
        /// </summary>
        ///<param name="id">Contact information record ID</param>
        ///<param name="contactid">Contact ID</param>
        ///<param optional="true" name="infoType">Contact information type</param>
        ///<param name="data">Data</param>
        ///<param optional="true" name="isPrimary">Contact importance: primary or not</param>
        ///<param optional="true" name="category">Contact information category</param>
        ///<short>Update contact info</short> 
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper UpdateContactInfo(
                                                    int id,
                                                    int contactid,
                                                    ContactInfoType? infoType,
                                                    String data,
                                                    bool? isPrimary,
                                                    String category)
        {

            if (id == 0 || String.IsNullOrEmpty(data) || contactid <= 0)
                throw new ArgumentException();

            var contactInfo = DaoFactory.GetContactInfoDao().GetByID(id);

            if (infoType != null)
            {
                var categoryType = ContactInfo.GetCategory(infoType.Value);

                if (!String.IsNullOrEmpty(category) && Enum.IsDefined(categoryType, category))
                    contactInfo.Category = (int)Enum.ToObject(categoryType, category);

                contactInfo.InfoType = infoType.Value;

            }

            contactInfo.ContactID = contactid;

            if (isPrimary != null)
                contactInfo.IsPrimary = isPrimary.Value;

            contactInfo.Data = data;

            DaoFactory.GetContactInfoDao().Update(contactInfo);

            if (contactInfo.InfoType == ContactInfoType.Email)
            {
                var contact = DaoFactory.GetContactDao().GetByID(contactInfo.ContactID);
                var userIds = CRMSecurity.GetAccessSubjectTo(contact).Keys.ToList();
                var emails = new[] { contactInfo.Data };
                DaoFactory.GetContactInfoDao().UpdateMailAggregator(emails, userIds);
            }

            var contactInfoWrapper = ToContactInfoWrapper(contactInfo);

            return contactInfoWrapper;

        }


        /// <summary>
		///  Updates contact information with the parameters specified in the request
		/// </summary>
        ///<short>Group contact info update</short> 
        ///<param name="items">Contact information</param>
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Update(@"contact/data")]
        public IEnumerable<ContactInfoWrapper> UpdateBatchContactInfo(IEnumerable<ContactInfoWrapper> items)
        {

            var ids = DaoFactory.GetContactInfoDao().UpdateList(items.Select(x => FromContactInfoWrapper(x)).ToList());

            var result = items.ToList();

            for (int index = 0; index < result.Count; index++)
            {
                var infoWrapper = result[index];

                infoWrapper.ID = ids[index];
            }


            return result;

        }

        /// <summary>
		///  Updates contact information with the parameters specified in the request for the contact with the selected ID
		/// </summary>
		///<short>Group contact info update</short> 
		///<param name="contactid">Contact ID</param>
        ///<param name="items">Contact information</param>
        ///<category>Contacts</category>
        ///<exception cref="ArgumentException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        /// <visible>false</visible>
        [Update(@"contact/{contactid:[0-9]+}/batch")]
        public IEnumerable<ContactInfoWrapper> UpdateBatchContactInfo(int contactid, IEnumerable<ContactInfoWrapper> items)
        {
            items = items ?? new List<ContactInfoWrapper>();
            var contactInfoList = items.Select(x => FromContactInfoWrapper(x)).ToList();

            foreach (var contactInfo in contactInfoList)
            {
                contactInfo.ContactID = contactid;
            }

            DaoFactory.GetContactInfoDao().DeleteByContact(contactid);
            var ids = DaoFactory.GetContactInfoDao().SaveList(contactInfoList);

            var result = items.ToList();

            for (int index = 0; index < result.Count; index++)
            {
                var infoWrapper = result[index];

                infoWrapper.ID = ids[index];
            }


            return result;

        }

        /// <summary>
        ///    Returns the detailed information for the contact with the selected ID by the information type specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="infoType">Contact information type</param>
        /// <short>Get contact information by type</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Read(@"contact/{contactid:[0-9]+}/data/{infoType}")]
        public IEnumerable<String> GetContactInfo(int contactid, ContactInfoType infoType)
        {
            return DaoFactory.GetContactInfoDao().GetListData(contactid, infoType);
        }

        /// <summary>
        ///   Adds the address with the parameters specified in the request to the selected contact
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address</param>
        /// <param name="isPrimary">Address type: primary or not</param>
        /// <param name="category">Category</param>
        ///<short>Add contact address</short> 
        ///<category>Contacts</category>
        /// <returns>
        ///   Address
        /// </returns>
        [Create(@"contact/{contactid:[0-9]+}/data/address/{category}")]
        public int AddAddress(int contactid,
                                  Address address,
                                  bool isPrimary,
                                  String category)
        {

            return CreateContactInfo(contactid, ContactInfoType.Address, JsonConvert.SerializeObject(address), isPrimary,
                                category).ID;
        }


        /// <summary>
        ///   Updates the address with the parameters specified in the request for the selected contact
        /// </summary>
        ///<param name="id">Contact information record ID</param>
        /// <param name="contactid">Contact ID</param>
        /// <param name="address">Address</param>
        /// <param name="isPrimary">Address type: primary or not</param>
        /// <param name="category">Category</param>
        /// <short>Update contact address</short> 
        /// <category>Contacts</category>
        /// <returns>
        ///   Address
        /// </returns>
        [Update(@"contact/{contactid:[0-9]+}/data/address/{id:[0-9]+}")]
        public Address UpdateAddress(
                                  int id,
                                  int contactid,
                                  Address address,
                                  bool isPrimary,
                                  String category)
        {
            UpdateContactInfo(id,
                              contactid,
                              ContactInfoType.Address,
                              JsonConvert.SerializeObject(address),
                              isPrimary,
                              category);

            return address;
        }



        /// <summary>
        ///   Deletes the address for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information record ID</param>
        /// <short>Delete contact address</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Address
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}/data/address/{id:[0-9]+}")]
        public Address DeleteAddress(int contactid, int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var contactInfo = DaoFactory.GetContactInfoDao().GetByID(id);

            if (contactInfo == null)
                throw new ItemNotFoundException();

            if (contactInfo.InfoType != ContactInfoType.Address)
                throw new ArgumentException();

            DeleteContactInfo(contactid, id);

            return new Address(contactInfo);

        }

        /// <summary>
        ///   Deletes the contact information for the contact with the ID specified in the request
        /// </summary>
        /// <param name="contactid">Contact ID</param>
        /// <param name="id">Contact information record ID</param>
        /// <short>Delete contact info</short> 
        /// <category>Contacts</category>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ItemNotFoundException"></exception>
        /// <returns>
        ///   Contact information
        /// </returns>
        [Delete(@"contact/{contactid:[0-9]+}/data/{id:[0-9]+}")]
        public ContactInfoWrapper DeleteContactInfo(int contactid, int id)
        {
            if (id <= 0)
                throw new ArgumentException();

            var contactInfo = DaoFactory.GetContactInfoDao().GetByID(id);

            if (contactInfo == null)
                throw new ItemNotFoundException();


            var result = ToContactInfoWrapper(contactInfo);

            DaoFactory.GetContactInfoDao().Delete(id);

            return result;
        }


        private ContactInfoWrapper ToContactInfoWrapper(ContactInfo contactInfo)
        {
            return new ContactInfoWrapper(contactInfo);
        }

        private ContactInfo FromContactInfoWrapper(ContactInfoWrapper contactInfoWrapper)
        {
            return new ContactInfo
                       {
                           ID = contactInfoWrapper.ID,
                           Category = contactInfoWrapper.Category,
                           Data = contactInfoWrapper.Data,
                           InfoType = contactInfoWrapper.InfoType,
                           IsPrimary = contactInfoWrapper.IsPrimary

                       };

        }
    }
}
