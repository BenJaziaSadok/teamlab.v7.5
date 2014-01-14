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

namespace ASC.Thrdparty
{
    public class TokenHolder
    {
        private static void EnsureHttpSession()
        {
            if (HttpContext.Current == null || HttpContext.Current.Session == null)
            {
                throw new InvalidOperationException(
                    "This operation can be run only in Http context and with Http session");
            }
        }

        public static string GetToken(string tokenKey)
        {
            EnsureHttpSession();
            return HttpContext.Current.Session[tokenKey] as string;
        }

        public static void AddToken(string tokenKey, string tokenValue)
        {
            EnsureHttpSession();
            HttpContext.Current.Session[tokenKey] = tokenValue;
        }
    }
}