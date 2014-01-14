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

using System.Web;

namespace ASC.Mail.Service
{
    public class CookiesManager
    {
        private const string _authCookiesName = "asc_auth_key";

        internal static string GetCookiesName()
        {
            if (HttpContext.Current!=null && HttpContext.Current.Request!=null)
            {
                return string.Format("{0}", _authCookiesName, HttpContext.Current.Request.Url.Port);
            }
            return "";
        }

        internal static string GetAuthCookie()
        {
            if (HttpContext.Current != null && HttpContext.Current.Request != null)
            {
                var cookie = HttpContext.Current.Request.Cookies[GetCookiesName()];
                if (cookie != null)
                {
                    return cookie.Value;
                }
            }
            return string.Empty;
        }
    }
}