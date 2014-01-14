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
using System.Linq;
using ConfigurationConstants = ASC.Core.Configuration.Constants;
using UserConstants = ASC.Core.Users.Constants;

namespace ASC.Data.Backup.Tasks.Modules
{
    internal static class Helpers
    {
        private static readonly string[] SystemUsers = new[]
            {
                Guid.Empty.ToString("D"),
                ConfigurationConstants.CoreSystem.ID.ToString("D"),
                ConfigurationConstants.Guest.ID.ToString("D"),
                UserConstants.LostUser.ID.ToString("D")
            };

        private static readonly string[] SystemGroups = new[]
                {
                    Guid.Empty.ToString("D"),
                    UserConstants.LostGroupInfo.ID.ToString("D"),
                    UserConstants.GroupAdmin.ID.ToString("D"),
                    UserConstants.GroupEveryone.ID.ToString("D"),
                    UserConstants.GroupVisitor.ID.ToString("D"),
                    UserConstants.GroupUser.ID.ToString("D")
                };

        public static bool IsEmptyOrSystemUser(string id)
        {
            return string.IsNullOrEmpty(id) || SystemUsers.Contains(id);
        }

        public static bool IsEmptyOrSystemGroup(string id)
        {
            return string.IsNullOrEmpty(id) || SystemGroups.Contains(id);
        }
    }
}
