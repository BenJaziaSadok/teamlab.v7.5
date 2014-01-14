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
using System.Web;
using ASC.Core.Security.Authentication;
using ASC.Security.Cryptography;
using ASC.Core.Tenants;

namespace ASC.Core.Security.Authentication
{
    class CookieStorage
    {
        public static bool DecryptCookie(string cookie, out int tenant, out Guid userid, out string login, out string password)
        {
            tenant = Tenant.DEFAULT_TENANT;
            userid = Guid.Empty;
            login = null;
            password = null;

            if (string.IsNullOrEmpty(cookie))
            {
                return false;
            }

            try
            {
                cookie = HttpUtility.UrlDecode(cookie).Replace(' ', '+');
                var s = InstanceCrypto.Decrypt(cookie).Split('$');

                if (0 < s.Length) login = s[0];
                if (1 < s.Length) tenant = int.Parse(s[1]);
                if (2 < s.Length) password = s[2];
                if (4 < s.Length) userid = new Guid(s[4]);
                return true;
            }
            catch { }
            return false;
        }

        public static string EncryptCookie(int tenant, Guid userid, string login, string password)
        {
            var s = string.Format("{0}${1}${2}${3}${4}",
                (login ?? string.Empty).ToLowerInvariant(),
                tenant,
                password,
                GetUserDepenencySalt(),
                userid.ToString("N"));
            return InstanceCrypto.Encrypt(s);
        }


        private static string GetUserDepenencySalt()
        {
            var data = string.Empty;
            try
            {
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    var forwarded = HttpContext.Current.Request.Headers["X-Forwarded-For"];
                    data = string.IsNullOrEmpty(forwarded) ? HttpContext.Current.Request.UserHostAddress : forwarded.Split(':')[0];
                }
            }
            catch { }
            return Hasher.Base64Hash(data ?? string.Empty, HashAlg.SHA256);
        }
    }
}