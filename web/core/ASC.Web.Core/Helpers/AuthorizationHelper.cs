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

using ASC.Core;
using System;
using System.Text;
using System.Web;

namespace ASC.Web.Core.Helpers
{
    public class AuthorizationHelper
    {
        public static bool ProcessBasicAuthorization(HttpContext context)
        {
            string authCookie;
            return ProcessBasicAuthorization(context, out authCookie);
        }

        public static bool ProcessBasicAuthorization(HttpContext context, out string authCookie)
        {
            authCookie = null;
            try
            {
                //Try basic
                var authorization = context.Request.Headers["Authorization"];
                if (string.IsNullOrEmpty(authorization))
                {
                    return false;
                }

                authorization = authorization.Trim();
                if (0 <= authorization.IndexOf("Basic", 0))
                {
                    var arr = Encoding.ASCII.GetString(Convert.FromBase64String(authorization.Substring(6))).Split(new[] { ':' });
                    var username = arr[0];
                    var password = arr[1];
                    var u = CoreContext.UserManager.GetUserByEmail(username);
                    if (u != null && u.ID != ASC.Core.Users.Constants.LostUser.ID)
                    {
                        authCookie = SecurityContext.AuthenticateMe(u.ID.ToString(), password);
                    }
                }
                else
                {
                    if (SecurityContext.AuthenticateMe(authorization))
                    {
                        authCookie = authorization;
                    }
                }
            }
            catch (Exception) { }
            return SecurityContext.IsAuthenticated;
        }
    }
}