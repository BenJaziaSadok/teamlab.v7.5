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
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Users;
using AuthConst = ASC.Common.Security.Authorizing.Constants;
using ConfConst = ASC.Core.Configuration.Constants;

namespace ASC.Core.Security.Authorizing
{
    class RoleProvider : IRoleProvider
    {
        public List<IRole> GetRoles(ISubject account)
        {
            var roles = new List<IRole>();
            if (!(account is ISystemAccount))
            {
                if (account is IRole)
                {
                    roles = GetParentRoles(account.ID).ToList();
                }
                else if (account is IUserAccount)
                {
                    roles = CoreContext.UserManager
                                       .GetUserGroups(account.ID, IncludeType.Distinct | IncludeType.InParent)
                                       .Select(g => (IRole) g)
                                       .ToList();
                }
            }
            return roles;
        }

        public bool IsSubjectInRole(ISubject account, IRole role)
        {
            return CoreContext.UserManager.IsUserInGroup(account.ID, role.ID);
        }

        private static List<IRole> GetParentRoles(Guid roleID)
        {
            var roles = new List<IRole>();
            var gi = CoreContext.GroupManager.GetGroupInfo(roleID);
            if (gi != null)
            {
                var parent = gi.Parent;
                while (parent != null)
                {
                    roles.Add(parent);
                    parent = parent.Parent;
                }
            }
            return roles;
        }
    }
}