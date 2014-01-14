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
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Api.Employee
{
    ///<summary>
    ///</summary>
    [DataContract(Name = "group", Namespace = "")]
    public class GroupWrapperSummary
    {
        ///<summary>
        ///</summary>
        ///<param name="group"></param>
        public GroupWrapperSummary(GroupInfo group)
        {
            Id = group.ID;
            Name = group.Name;
            Manager = CoreContext.UserManager.GetUsers(CoreContext.UserManager.GetDepartmentManager(group.ID)).UserName;
        }

        protected GroupWrapperSummary()
        {
            
        }
        ///<summary>
        ///</summary>
        [DataMember(Order = 2)]
        public string Name { get; set; }


        ///<summary>
        ///</summary>
        [DataMember(Order = 1)]
        public Guid Id { get; set; }

        ///<summary>
        ///</summary>
        [DataMember(Order = 9, EmitDefaultValue = true)]
        public string Manager { get; set; }

        public static GroupWrapperSummary GetSample()
        {
            return new GroupWrapperSummary() { Id = Guid.NewGuid(), Manager = "Jake.Zazhitski", Name="Group Name" };
        }
    }
}