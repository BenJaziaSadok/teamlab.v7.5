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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Api.Employee;

namespace ASC.Api.Settings
{
    [DataContract(Name = "security", Namespace = "")]
    public class SecurityWrapper
    {
        [DataMember]
        public string WebItemId { get; set; }

        [DataMember]
        public IEnumerable<EmployeeWraper> Users { get; set; }

        [DataMember]
        public IEnumerable<GroupWrapperSummary> Groups { get; set; }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public bool IsSubItem { get; set; }
    }
}