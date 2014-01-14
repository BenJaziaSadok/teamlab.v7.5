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

using System.Runtime.Serialization;

namespace ASC.Files.Core
{
    [DataContract]
    public enum FilterType
    {
        [EnumMember] None = 0,

        [EnumMember] FilesOnly = 1,

        [EnumMember] FoldersOnly = 2,

        [EnumMember] DocumentsOnly = 3,

        [EnumMember] PresentationsOnly = 4,

        [EnumMember] SpreadsheetsOnly = 5,

        [EnumMember] ImagesOnly = 7,

        [EnumMember] ByUser = 8,

        [EnumMember] ByDepartment = 9
    }
}