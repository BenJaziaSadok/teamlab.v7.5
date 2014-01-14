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
using System.Text;
using System.Runtime.Serialization;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Core.Calendars
{
    public class SharingOptions : ICloneable
    {
        [DataContract(Name = "PublicItem")]
        public class PublicItem
        {
            public Guid Id { get; set; }
            public bool IsGroup { get; set; }
        }

        public bool SharedForAll { get; set; }

        public List<PublicItem> PublicItems { get; set; }

        public SharingOptions()
        {
            this.PublicItems = new List<PublicItem>();
        }

        public bool PublicForItem(Guid itemId)
        {
            if (SharedForAll)
                return true;

            if(PublicItems.Exists(i=> i.Id.Equals(itemId)))
                return true;

            var u = CoreContext.UserManager.GetUsers(itemId);
            if(u!=null && u.ID!= ASC.Core.Users.Constants.LostUser.ID)
            {
                var userGroups = new List<GroupInfo>(CoreContext.UserManager.GetUserGroups(itemId));
                userGroups.AddRange(CoreContext.UserManager.GetUserGroups(itemId, Constants.SysGroupCategoryId));
                return userGroups.Exists(g => PublicItems.Exists(i => i.Id.Equals(g.ID)));
            }

            return false;
        }

        #region ICloneable Members

        public object Clone()
        {
            var o = new SharingOptions();
            o.SharedForAll = this.SharedForAll;
            foreach (var i in this.PublicItems)            
                o.PublicItems.Add(new PublicItem() { Id = i.Id, IsGroup = i.IsGroup });
            
            return o;
        }

        #endregion
    }

   
}
