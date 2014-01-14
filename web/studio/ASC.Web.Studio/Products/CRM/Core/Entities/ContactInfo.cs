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
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Common.Security;
using ASC.Core.Users;
using ASC.Web.Core.Helpers;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.SocialMedia.LinkedIn;

#endregion

namespace ASC.CRM.Core
{
    [DataContract]
    public class ContactInfo : DomainObject
    {
        [DataMember(Name = "contactID")]
        public int ContactID { get; set; }

        [DataMember(Name = "infoType")]
        public ContactInfoType InfoType { get; set; }

        [DataMember(Name = "category")]
        public int Category { get; set; }

        [DataMember(Name = "data")]
        public String Data { get; set; }

        [DataMember(Name = "isPrimary")]
        public bool IsPrimary { get; set; }

        #region Methods


        public static int GetDefaultCategory(ContactInfoType infoTypeEnum)
        {
            switch (infoTypeEnum)
            {
                case ContactInfoType.Phone:
                    return (int)PhoneCategory.Work;
                case ContactInfoType.Address:
                    return (int)AddressCategory.Work;
                default:
                    return (int)ContactInfoBaseCategory.Work;
            }
        }

        public String CategoryToString()
        {
            switch (InfoType)
            {
                case ContactInfoType.Phone:
                    return ((PhoneCategory)Category).ToLocalizedString();
                case ContactInfoType.Address:
                    return ((AddressCategory)Category).ToLocalizedString();
                default:
                    return ((ContactInfoBaseCategory)Category).ToLocalizedString();
            }
        }

        public static Type GetCategory(ContactInfoType infoType)
        {
            switch (infoType)
            {
                case ContactInfoType.Phone:
                    return typeof(PhoneCategory);
                case ContactInfoType.Address:
                    return typeof(AddressCategory);
                default:
                    return typeof(ContactInfoBaseCategory);
            }
        }

        #endregion
    }
}