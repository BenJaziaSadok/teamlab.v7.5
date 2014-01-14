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

#region Usings

using System.Runtime.Serialization;
using ASC.Api.Employee;

#endregion

namespace ASC.Api.Projects.Wrappers
{
    [DataContract(Namespace = "")]
    public class ObjectWrapperBase
    {
        [DataMember(Order = 1)]
        public int Id { get; set; }

        [DataMember(Order = 10)]
        public string Title { get; set; }

        [DataMember(Order = 11)]
        public string Description { get; set; }

        [DataMember(Order = 20)]
        public int Status { get; set; }

        [DataMember(Order = 30)]
        public EmployeeWraper Responsible { get; set; }
    }
}
