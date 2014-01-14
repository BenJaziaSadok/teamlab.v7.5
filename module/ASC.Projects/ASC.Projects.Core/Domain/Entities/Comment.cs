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

#endregion

namespace ASC.Projects.Core.Domain
{
    public class Comment : DomainObject<Guid>
    {
        public Guid Parent { get; set; }

        public string Content { get; set; }

        public bool Inactive { get; set; }


        public string TargetUniqID { get; set; }


        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }


        public string ProjectTitle { get; set; }

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + Content + "|" + CreateBy.GetHashCode() + "|" + Parent.GetHashCode()).GetHashCode();
        }
    }
}
