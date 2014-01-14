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
using ASC.Common.Utils;

namespace ASC.Web.Studio.Core.Notify
{
    public class PatternHelper
    {
        public string Unhtml(object htmlString)
        {
            if (htmlString == null || Convert.ToString(htmlString) == String.Empty)
                return "";

            var html = htmlString.ToString();
            try
            {
                return HtmlUtil.ToPlainText(html);
            }
            catch
            {
                return HtmlUtil.GetText(html);
            }
        }

        public string Right(object str, int count)
        {
            if (str == null || Convert.ToString(str) == String.Empty)
                return "";

            if (count > str.ToString().Length)
            {
                return str.ToString();
            }
            var cutTo = str.ToString().LastIndexOfAny(new[] { ' ', ',' }, count, count);
            if (cutTo == -1)
            {
                cutTo = count;
            }
            return str.ToString().Substring(0, cutTo);
        }

        public string Left(object str, int count)
        {
            if (str == null || Convert.ToString(str) == String.Empty)
                return "";

            if (count > str.ToString().Length)
            {
                return str.ToString();
            }
            var cutTo = str.ToString().IndexOfAny(new[] { ' ', ',' }, count);
            if (cutTo == -1)
            {
                cutTo = count;
            }
            return str.ToString().Substring(0, cutTo);
        }

    }
}