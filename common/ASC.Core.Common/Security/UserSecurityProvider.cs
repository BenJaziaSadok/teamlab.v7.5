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
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Users
{
    public class UserSecurityProvider : ISecurityObject
    {
        public Type ObjectType
        {
            get;
            private set;
        }

        public object SecurityId
        {
            get;
            private set;
        }


        public UserSecurityProvider(Guid userId)
        {
            SecurityId = userId;
            ObjectType = typeof(UserInfo);
        }


        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (account.ID.Equals(objectId.SecurityId))
            {
                roles.Add(ASC.Common.Security.Authorizing.Constants.Self);
            }
            return roles;
        }

        public bool InheritSupported
        {
            get { return false; }
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }
    }
}