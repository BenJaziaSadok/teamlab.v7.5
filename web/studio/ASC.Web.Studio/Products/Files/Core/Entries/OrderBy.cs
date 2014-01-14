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
using System.Diagnostics;

namespace ASC.Files.Core
{
    [DataContract(Name = "sorted_by_type", Namespace = "")]
    public enum SortedByType
    {
        [EnumMember] DateAndTime,

        [EnumMember] AZ,

        [EnumMember] Size,

        [EnumMember] Author,

        [EnumMember] Type,

        [EnumMember] New

    }

    [DataContract(Name = "orderBy", Namespace = "")]
    [DebuggerDisplay("{SortedBy} {IsAsc}")]
    public class OrderBy
    {
        [DataMember(Name = "is_asc")]
        public bool IsAsc { get; set; }

        [DataMember(Name = "property")]
        public SortedByType SortedBy { get; set; }

        public OrderBy(SortedByType sortedByType, bool isAsc)
        {
            SortedBy = sortedByType;
            IsAsc = isAsc;
        }
    }
}