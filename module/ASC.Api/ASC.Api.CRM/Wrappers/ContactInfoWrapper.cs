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
using System.Text;
using ASC.CRM.Core;
using ASC.Web.CRM.Classes;
using Newtonsoft.Json.Linq;

#endregion

namespace ASC.Api.CRM.Wrappers
{

    /// <summary>
    ///   Address
    /// </summary>
    [DataContract(Name = "address", Namespace = "")]
    public class Address
    {

        public Address()
        {
            
        }

        public Address(ContactInfo contactInfo)
        {
            if (contactInfo.InfoType != ContactInfoType.Address)
                throw new ArgumentException();

            City = JObject.Parse(contactInfo.Data)["city"].Value<String>();
            Country = JObject.Parse(contactInfo.Data)["country"].Value<String>();
            State = JObject.Parse(contactInfo.Data)["state"].Value<String>();
            Street = JObject.Parse(contactInfo.Data)["street"].Value<String>();
            Zip = JObject.Parse(contactInfo.Data)["zip"].Value<String>();
            Category = contactInfo.Category;
            CategoryName = contactInfo.CategoryToString();
            IsPrimary = contactInfo.IsPrimary;

        }

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public String Street { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public String City { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public String State { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public String Zip { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public String Country { get; set; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public int Category { get; set; }

        [DataMember(Order = 7, IsRequired = false, EmitDefaultValue = false)]
        public String CategoryName { get; set; }

        [DataMember(Order = 8, IsRequired = false, EmitDefaultValue = false)]
        public Boolean IsPrimary { get; set; }


        public static Address GetSample()
        {
            return new Address
                       {
                           Country = "Latvia",
                           Zip = "LV-1021",
                           Street = "Lubanas st. 125a-25",
                           State = "",
                           City = "Riga",
                           IsPrimary = true,
                           Category = (int)ContactInfoBaseCategory.Work,
                           CategoryName = ((AddressCategory)ContactInfoBaseCategory.Work).ToLocalizedString()
                       };
        }
    }

    /// <summary>
    ///   Contact information
    /// </summary>
    [DataContract(Name = "commonDataItem", Namespace = "")]
    public class ContactInfoWrapper : ObjectWrapperBase
    {
        public ContactInfoWrapper()
            : base(0)
        {

        }

        public ContactInfoWrapper(int id)
            : base(id)
        {

        }

        public ContactInfoWrapper(ContactInfo contactInfo)
            : base(contactInfo.ID)
        {
            InfoType = contactInfo.InfoType;
            Category = contactInfo.Category;
            CategoryName = contactInfo.CategoryToString();
            Data = contactInfo.Data;
            IsPrimary = contactInfo.IsPrimary;
            ID = contactInfo.ID;
        }


        [DataMember(Order = 1)]
        public ContactInfoType InfoType { get; set; }

        [DataMember(Order = 2)]
        public int Category { get; set; }

        [DataMember(Order = 3)]
        public String Data { get; set; }

        [DataMember(Order = 4)]
        public String CategoryName { get; set; }

        [DataMember(Order = 5)]
        public bool IsPrimary { get; set; }

        public static ContactInfoWrapper GetSample()
        {
            return new ContactInfoWrapper(0)
            {
                IsPrimary = true,
                Category = (int)ContactInfoBaseCategory.Home,
                CategoryName = ((ContactInfoBaseCategory)ContactInfoBaseCategory.Home).ToLocalizedString(),
                Data = "support@teamlab.com",
                InfoType = ContactInfoType.Email
            };
        }

    }
}
