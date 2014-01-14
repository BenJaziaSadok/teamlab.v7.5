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
    public enum ListType
    {
        ContactStatus = 1,
        TaskCategory = 2,
        HistoryCategory = 3,
        ContactType = 4
    }

    [TypeConverter(typeof(LocalizedEnumConverter))]
    public enum HistoryCategorySystem
    {
        TaskClosed = -1,
        FilesUpload = -2,
        MailMessage = -3
    }

}
