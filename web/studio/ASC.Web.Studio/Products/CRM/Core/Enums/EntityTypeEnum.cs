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

using System.ComponentModel;
using ASC.Web.CRM.Classes;

#endregion

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum EntityType
    {
        Any  = -1,
        Contact = 0,
        Opportunity = 1,
        RelationshipEvent = 2,
        Task = 3,
        Company = 4,
        Person = 5,
        File = 6,
        Case = 7
    }
}