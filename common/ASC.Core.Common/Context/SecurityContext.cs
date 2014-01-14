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
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Web;
using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Core.Security.Authentication;
using ASC.Core.Security.Authorizing;
using ASC.Security.Cryptography;
using AuthConst = ASC.Core.Configuration.Constants;
using UserConst = ASC.Core.Users.Constants;

namespace ASC.Core
{
    public static class SecurityContext
    {
        public static IAccount CurrentAccount
        {
            get { return Principal.Identity is IAccount ? (IAccount)Principal.Identity : AuthConst.Guest; }
        }

        public static bool IsAuthenticated
        {
            get { return CurrentAccount.IsAuthenticated; }
        }

        public static IPermissionResolver PermissionResolver
        {
            get;
            private set;
        }


        static SecurityContext()
        {
            var azManager = new AzManager(new RoleProvider(), new PermissionProvider());
            PermissionResolver = new PermissionResolver(azManager);
        }


        public static string AuthenticateMe(string login, string password)
        {
            if (login == null) throw new ArgumentNullException("login");
            if (password == null) throw new ArgumentNullException("password");

            var tenantid = CoreContext.TenantManager.GetCurrentTenant().TenantId;
            var u = CoreContext.UserManager.GetUsers(tenantid, login, Hasher.Base64Hash(password, HashAlg.SHA256));

            return AuthenticateMe(new UserAccount(u, tenantid));
        }

        public static bool AuthenticateMe(string cookie)
        {
            if (cookie == null) throw new ArgumentNullException("cookie");

            int tenant;
            Guid userid;
            string login;
            string password;
            if (CookieStorage.DecryptCookie(cookie, out tenant, out userid, out login, out password))
            {
                if (tenant != CoreContext.TenantManager.GetCurrentTenant().TenantId)
                {
                    return false;
                }

                try
                {
                    if (userid != Guid.Empty)
                    {
                        AuthenticateMe(new UserAccount(new ASC.Core.Users.UserInfo { ID = userid }, tenant));
                    }
                    else
                    {
                        AuthenticateMe(login, password);
                    }
                    return true;
                }
                catch { }
            }
            return false;
        }

        public static string AuthenticateMe(IAccount account)
        {
            if (account == null || account.Equals(AuthConst.Guest)) throw new System.Security.Authentication.InvalidCredentialException("account");

            var roles = new List<string>() { Role.Everyone };
            string cookie = null;


            if (account is ISystemAccount && account.ID == AuthConst.CoreSystem.ID)
            {
                roles.Add(Role.System);
            }

            if (account is IUserAccount)
            {
                var u = CoreContext.UserManager.GetUsers(account.ID);

                if (u.ID == UserConst.LostUser.ID)
                {
                    throw new System.Security.Authentication.InvalidCredentialException("Invalid username or password.");
                }
                if (u.Status != ASC.Core.Users.EmployeeStatus.Active)
                {
                    throw new SecurityException("Account disabled.");
                }
                if (CoreContext.UserManager.IsUserInGroup(u.ID, UserConst.GroupAdmin.ID))
                {
                    roles.Add(Role.Administrators);
                }
                roles.Add(Role.Users);

                account = new UserAccount(u, CoreContext.TenantManager.GetCurrentTenant().TenantId);
                cookie = CookieStorage.EncryptCookie(CoreContext.TenantManager.GetCurrentTenant().TenantId, account.ID, null, null);
            }

            Principal = new GenericPrincipal(account, roles.ToArray());

            return cookie;
        }

        public static string AuthenticateMe(Guid userId)
        {
            return AuthenticateMe(CoreContext.Authentication.GetAccountByID(userId));
        }

        public static void Logout()
        {
            Principal = null;
        }

        public static string SetUserPassword(Guid userID, string password)
        {
            CoreContext.Authentication.SetUserPassword(userID, password);
            if (CurrentAccount.ID == userID)
            {
                return CookieStorage.EncryptCookie(CoreContext.TenantManager.GetCurrentTenant().TenantId, userID, null, null);
            }
            return null;
        }


        public static bool CheckPermissions(params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, actions);
        }

        public static bool CheckPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            return CheckPermissions(securityObject, null, actions);
        }

        public static bool CheckPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            return PermissionResolver.Check(CurrentAccount, objectId, securityObjProvider, actions);
        }

        public static void DemandPermissions(params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, actions);
        }

        public static void DemandPermissions(ISecurityObject securityObject, params IAction[] actions)
        {
            DemandPermissions(securityObject, null, actions);
        }

        public static void DemandPermissions(ISecurityObjectId objectId, ISecurityObjectProvider securityObjProvider, params IAction[] actions)
        {
            PermissionResolver.Demand(CurrentAccount, objectId, securityObjProvider, actions);
        }


        private static IPrincipal Principal
        {
            get
            {
                return Thread.CurrentPrincipal;
            }
            set
            {
                Thread.CurrentPrincipal = value;
                if (HttpContext.Current != null) HttpContext.Current.User = value;
            }
        }
    }
}