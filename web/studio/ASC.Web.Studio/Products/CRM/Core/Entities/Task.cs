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

using System;
using ASC.Common.Security;

#endregion

namespace ASC.CRM.Core.Entities
{
    public class Task : DomainObject, ISecurityObjectId
    {

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public Guid? LastModifedBy { get; set; }

        public DateTime? LastModifedOn { get; set; }

        public int ContactID { get; set; }

        public Contact Contact { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DeadLine { get; set; }

        public Guid ResponsibleID { get; set; }
      
        public bool IsClosed { get; set; }

        public int CategoryID { get; set; }

        public EntityType EntityType { get; set; }

        public int EntityID { get; set; }

        public int AlertValue { get; set; }

        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }
    }
}
