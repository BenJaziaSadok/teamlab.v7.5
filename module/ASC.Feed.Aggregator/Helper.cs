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
using ASC.CRM.Core.Dao;
using ASC.CRM.Core.Entities;
using ASC.Common.Utils;
using ASC.Core;

namespace ASC.Feed.Aggregator
{
    public static class Helper
    {
        public static UserWrapper GetUser(Guid id)
        {
            return new UserWrapper(CoreContext.UserManager.GetUsers(id));
        }

        public static string GetUsersString(HashSet<Guid> users)
        {
            var usersString = users.Select(GetUser)
                                   .Aggregate(string.Empty, (current, user) => current + (user.DisplayName + ", "));
            if (!string.IsNullOrEmpty(usersString))
            {
                usersString = usersString.Remove(usersString.Length - 2);
            }

            return usersString;
        }

        public static string GetContactsString(HashSet<int> contacts, ContactDao dao)
        {
            if (contacts == null || dao == null)
            {
                return null;
            }

            var contactsString = contacts
                .Select(dao.GetByID)
                .Aggregate(string.Empty, (current, contact) => current + (contact.GetTitle() + ", "));
            
            if (!string.IsNullOrEmpty(contactsString))
            {
                contactsString = contactsString.Remove(contactsString.Length - 2);
            }

            return contactsString;
        }

        public static string GetText(string html)
        {
            return HtmlUtil.GetText(html);
        }

        public static string GetHtmlDescription(string description)
        {
            return !string.IsNullOrEmpty(description) ? description.Replace("\n", "<br/>") : description;
        }
    }
}