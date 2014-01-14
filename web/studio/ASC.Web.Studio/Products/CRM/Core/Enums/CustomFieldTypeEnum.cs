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

using System.ComponentModel;
using ASC.Web.CRM.Classes;

namespace ASC.CRM.Core
{
    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum CustomFieldType
    {
        TextField = 0,
        TextArea = 1,
        SelectBox = 2,
        CheckBox = 3,
        Heading = 4,
        Date = 5
    }
}