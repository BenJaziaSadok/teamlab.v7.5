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
using System.IO;

namespace ASC.Mail.Autoreply.Utility.Html
{
    public class Text2HtmlConverter
    {
        public static String Convert(String text)
        {
            if (String.IsNullOrEmpty(text))
                return text;

            text = text.Replace("  ", " &nbsp;");

            var sr = new StringReader(text);
            var sw = new StringWriter();

            while (sr.Peek() > -1)
            {
                sw.Write(sr.ReadLine() + "<br>");
            }

            return sw.ToString();
        }
    }
}
