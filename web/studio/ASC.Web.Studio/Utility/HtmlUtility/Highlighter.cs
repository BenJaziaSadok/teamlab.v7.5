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

using ASC.Web.Studio.Utility.HtmlUtility.CodeFormat;

namespace ASC.Web.Studio.Utility.HtmlUtility
{
    public class Highlight
    {
        public static string HighlightToHTML(string source, LangType type)
        {
            return HighlightToHTML(source, type, false);
        }

        public static string HighlightToHTML(string source, LangType type, bool customProtectionTags)
        {
            SourceFormat sf = null;

            switch (type)
            {
                case LangType.C:
                case LangType.CPP:
                    sf = new CppFormat();
                    break;
                case LangType.CS:
                    sf = new CSharpFormat();
                    break;
                case LangType.Html:
                case LangType.Xml:
                case LangType.Asp:
                    sf = new HtmlFormat();
                    break;
                case LangType.JS:
                    sf = new JavaScriptFormat();
                    break;
                case LangType.Msh:
                    sf = new MshFormat();
                    break;
                case LangType.TSql:
                    sf = new TsqlFormat();
                    break;
                case LangType.VB:
                    sf = new VisualBasicFormat();
                    break;
            }

            if (sf == null) return source;

            sf.CustomProtectedTags = customProtectionTags;
            return sf.FormatCode(source);
        }
    }
}