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
using ASC.Core.Users;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using System.Collections.Generic;
using ASC.Core;

namespace ASC.Blogs.Core.Security
{
    public class CorporateBlogSecObject : SecurityObjectId, ISecurityObject
    {

        private GroupInfo groupInfo;

        public CorporateBlogSecObject(GroupInfo groupInfo)
            : base(groupInfo.ID, typeof(CorporateBlogSecObject))
        {
            this.groupInfo = groupInfo;
        }

        public override string ToString()
        {
            return string.Format("corpblog for {0}", groupInfo.Name);
        }

        #region ISecurityObjectProvider Members

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();

            var userGroups = new List<GroupInfo>(
                CoreContext.UserManager.GetUserGroups(account.ID, IncludeType.Distinct | IncludeType.InChild)
            );

            if (userGroups.Contains(groupInfo))
            {
                roles.Add(ASC.Common.Security.Authorizing.Constants.Member);
            }

            return roles;
        }

        #endregion
    }
}
