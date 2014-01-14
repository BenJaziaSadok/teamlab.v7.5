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

using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Linq;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Client.HttpHandlers;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Core;

namespace ASC.Web.Studio.Masters.MasterResources
{
    public class MasterUserResources : ClientScript
    {
        protected override string BaseNamespace
        {
            get { return "ASC.Resources.Master"; }
        }

        protected override IEnumerable<KeyValuePair<string, object>> GetClientVariables(HttpContext context)
        {
            var users = CoreContext.UserManager.GetUsers(EmployeeStatus.Active).Select(r => new 
            { 
                id = r.ID, 
                displayName = DisplayUserSettings.GetFullUserName(r), 
                avatarSmall = UserPhotoManager.GetSmallPhotoURL(r.ID),
                groups = CoreContext.UserManager.GetUserGroups(r.ID).Select(x => new { 
                    id = x.ID,
                    name = x.Name,
                    manager = CoreContext.UserManager.GetUsers(CoreContext.UserManager.GetDepartmentManager(x.ID)).UserName})
                    .ToList(),
                isVisitor = r.IsVisitor(),
                isAdmin = r.IsAdmin(),
                isOwner = r.IsOwner()
            }).ToList();

            var groups = CoreContext.UserManager.GetDepartments().Select(x => new
            {
                id = x.ID,
                name = x.Name
            }).ToList();

            yield return RegisterObject("ApiResponsesMyProfile", new { response = users.FirstOrDefault(r => r.id.Equals(SecurityContext.CurrentAccount.ID)) });
            yield return RegisterObject("ApiResponses_Profiles", new { response = users });
            yield return RegisterObject("ApiResponses_Groups", new { response = groups });

        }

        protected override string GetCacheHash()
        {
            /* return users and groups last mod time */
            return SecurityContext.CurrentAccount.ID.ToString() +
                CoreContext.UserManager.GetMaxUsersLastModified().Ticks.ToString(CultureInfo.InvariantCulture) +
                CoreContext.GroupManager.GetMaxGroupsLastModified().Ticks.ToString(CultureInfo.InvariantCulture);
        }
    }
}