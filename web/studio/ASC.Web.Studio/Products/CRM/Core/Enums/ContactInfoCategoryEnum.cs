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
using ASC.Common.Security;
using ASC.Core.Users;
using ASC.Web.Core.Helpers;
using ASC.Web.CRM.Classes;
using ASC.Web.CRM.Resources;
using ASC.SocialMedia.LinkedIn;

#endregion

namespace ASC.CRM.Core
{
    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum ContactInfoBaseCategory
    {
        Home,
        Work,
        Other
    }

    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum PhoneCategory
    {
        Home,
        Work,
        Mobile,
        Fax,
        Direct,
        Other
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum AddressPart
    {
        Street,
        City,
        State,
        Zip,
        Country
    }

    [TypeConverter(typeof (LocalizedEnumConverter))]
    public enum AddressCategory
    {
        Home,
        Postal,
        Office,
        Billing,
        Other, 
        Work
    }
}