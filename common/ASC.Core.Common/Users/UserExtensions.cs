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


namespace ASC.Core.Users
{
    public static class UserExtensions
    {
        public static bool IsOwner(this UserInfo ui)
        {
            if (ui == null) return false;

            var tenant = CoreContext.TenantManager.GetCurrentTenant();
            return tenant != null && tenant.OwnerId.Equals(ui.ID);
        }

        public static bool IsMe(this UserInfo ui)
        {
            return ui != null && ui.ID == SecurityContext.CurrentAccount.ID;
        }

        public static bool IsAdmin(this UserInfo ui)
        {
            return ui != null && CoreContext.UserManager.IsUserInGroup(ui.ID, Constants.GroupAdmin.ID);
        }

        public static bool IsVisitor(this UserInfo ui)
        {
            return ui != null && CoreContext.UserManager.IsUserInGroup(ui.ID, Constants.GroupVisitor.ID);
        }
    }
}
