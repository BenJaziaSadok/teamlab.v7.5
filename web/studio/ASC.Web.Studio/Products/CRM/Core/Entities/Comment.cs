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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using ASC.Core.Users;

#endregion

namespace ASC.CRM.Core.Entities
{
    public class Comment
    {
        public Guid Parent { get; set; }

        public string Content { get; set; }

        public bool Inactive { get; set; }

        public String TargetUniqID { get; set; }

        public Guid CreateBy { get; set; }

        public DateTime CreateOn { get; set; }

        public override int GetHashCode()
        {
            return (GetType().FullName + "|" + Content + "|" + CreateBy.GetHashCode() + "|" + Parent.GetHashCode()).GetHashCode();
        }

    }
}