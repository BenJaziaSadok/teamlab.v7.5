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
using System.Text.RegularExpressions;

namespace ASC.Web.Studio.Utility.BBCodeParser
{
    public class RegularExpressionTemplate
    {
        public Regex RegExpression { get; private set; }
        public string Replacement { get; private set; }

        public RegularExpressionTemplate(Regex regExpression, string replacement)
        {
            RegExpression = regExpression;
            Replacement = replacement;
        }

        public string Parse(string text)
        {
            const int start = 0;
            var m = RegExpression.Match(text, start);
            while (m.Success)
            {
                text = text.Remove(m.Index, m.Length);
                var insertion = String.Format(Replacement, m.Value);
                text = text.Insert(m.Index, insertion);
                m = m.NextMatch();
            }

            return text;
        }

        public static RegularExpressionTemplate HTMLReferenceExpression
        {
            get
            {
                return new RegularExpressionTemplate(
                    new Regex("((http|ftp|https|gopher|mailto|news|nntp|telnet)://){1}([0-9a-zA-Z]+[0-9a-zA-Z]+[0-9a-zA-Z\\-_]*\\.{0,1}[0-9a-zA-Z]+[0-9a-zA-Z/\\.{0,1}\\-_:]*){1}(\\?[0-9a-zA-Z;/?@&=+$\\.\\-_!~*'#()%]*)?",
                              RegexOptions.IgnoreCase | RegexOptions.Compiled),
                    "<a href=\"{0}\" target=_blank>{0}</a>");
            }
        }
    }
}