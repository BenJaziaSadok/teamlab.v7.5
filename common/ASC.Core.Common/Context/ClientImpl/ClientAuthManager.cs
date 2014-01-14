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
using ASC.Common.Security.Authentication;
using ASC.Core.Security.Authentication;
using ASC.Core.Users;

namespace ASC.Core
{
    class ClientAuthManager : IAuthManagerClient
    {
        private readonly IUserService userService;


        public ClientAuthManager(IUserService service)
        {
            this.userService = service;
        }


        public IUserAccount[] GetUserAccounts()
        {
            return CoreContext.UserManager.GetUsers(EmployeeStatus.Active).Select(u => ToAccount(u)).ToArray();
        }

        public void SetUserPassword(Guid userID, string password)
        {
            userService.SetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID, password);
        }

        public string GetUserPasswordHash(Guid userID)
        {
            return userService.GetUserPassword(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID);
        }

        public IAccount GetAccountByID(Guid id)
        {
            var s = ASC.Core.Configuration.Constants.SystemAccounts.FirstOrDefault(a => a.ID == id);
            if (s != null) return s;
 
            var u = CoreContext.UserManager.GetUsers(id);
            return !Constants.LostUser.Equals(u) && u.Status == EmployeeStatus.Active ? (IAccount)ToAccount(u) : ASC.Core.Configuration.Constants.Guest;
        }


        private IUserAccount ToAccount(UserInfo u)
        {
            return new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId);
        }
    }
}