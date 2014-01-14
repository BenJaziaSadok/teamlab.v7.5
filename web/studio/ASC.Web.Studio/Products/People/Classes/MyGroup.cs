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
using System.Web;

using ASC.Core.Users;

namespace ASC.Web.People.Classes
{
    public class MyGroup
    {
        public Guid Id { get; set; }
        public string Title { get; set; }

        public MyGroup(GroupInfo group)
        {
            Id = group.ID;
            Title = HttpUtility.HtmlEncode(group.Name);
        }
    }
}
