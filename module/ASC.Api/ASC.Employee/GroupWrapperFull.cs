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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "group", Namespace = "")]
    public class GroupWrapperFull
    {
        ///<summary>
        ///</summary>
        ///<param name="group"></param>
        ///<param name="includeMembers"></param>
        public GroupWrapperFull(GroupInfo group, bool includeMembers)
        {
            Id = group.ID;
            Category = group.CategoryID;
            Parent = group.Parent!=null?group.Parent.ID:Guid.Empty;
            Name = group.Name;
            Manager = EmployeeWraper.Get(Core.CoreContext.UserManager.GetUsers(Core.CoreContext.UserManager.GetDepartmentManager(group.ID)));
            if (includeMembers)
            {
                Members = new List<EmployeeWraper>(Core.CoreContext.UserManager.GetUsersByGroup(group.ID).Select(x=>EmployeeWraper.Get(x)));
            }
        }

        private GroupWrapperFull()
        {
        }

        ///<summary>
        ///</summary>
        [DataMember(Order = 5)]
        public string Description { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 4,EmitDefaultValue = true)]
        public Guid? Parent { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 3)]
        public Guid Category { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 9, EmitDefaultValue = true)]
        public EmployeeWraper Manager { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 10,EmitDefaultValue = false)]
        public List<EmployeeWraper> Members { get; set; }

        public static GroupWrapperFull GetSample()
        {
            return new GroupWrapperFull() { Id = Guid.NewGuid(), Manager = EmployeeWraper.GetSample(),
                Category = Guid.NewGuid(),Name = "Sample group",Parent = Guid.NewGuid(),Members = new List<EmployeeWraper>(){EmployeeWraper.GetSample()}};
        }
    }
}