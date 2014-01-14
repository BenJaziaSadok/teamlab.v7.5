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
using System.Web;
using System.Text.RegularExpressions;

namespace ASC.Web.UserControls.Wiki
{
    public class PageNameUtil
    {
        public static string ReplaceSpaces(string str)
        {
            return str.Replace(" ", "_");
        }
        
        public static string Clean(string str)
        {
            return str.Replace("_", " ");
        }

        public static string Encode(string str)
        {
            string result = str.Replace(" ", "_");
            return HttpUtility.UrlEncode(result);
        }

        public static string Decode (string str)
        {
            string result = str; //HttpUtility.UrlDecode(str);//BUG: removed due + problem
            Regex nameReg = new Regex(@"_", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            result = nameReg.Replace(result, " ");
            return result;
        }

        public static string NormalizeNameCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            return str;
        }
    }
}
