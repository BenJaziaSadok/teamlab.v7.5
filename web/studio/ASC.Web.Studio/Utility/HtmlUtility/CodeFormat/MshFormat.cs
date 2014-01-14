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

namespace ASC.Web.Studio.Utility.HtmlUtility.CodeFormat
{
    /// <summary>
    /// Generates color-coded HTML 4.01 from MSH (code name Monad) source code.
    /// </summary>
    internal class MshFormat : CodeFormat
    {
        /// <summary>
        /// Regular expression string to match single line comments (#).
        /// </summary>
        protected override string CommentRegEx
        {
            get { return @"#.*?(?=\r|\n)"; }
        }

        /// <summary>
        /// Regular expression string to match string and character literals. 
        /// </summary>
        protected override string StringRegEx
        {
            get { return @"@?""""|@?"".*?(?!\\).""|''|'.*?(?!\\).'"; }
        }

        /// <summary>
        /// The list of MSH keywords.
        /// </summary>
        protected override string Keywords
        {
            get
            {
                return "function filter global script local private if else"
                       + " elseif for foreach in while switch continue break"
                       + " return default param begin process end throw trap";
            }
        }

        /// <summary>
        /// Use preprocessors property to hilight operators.
        /// </summary>
        protected override string Preprocessors
        {
            get
            {
                return "-band -bor -match -notmatch -like -notlike -eq -ne"
                       + " -gt -ge -lt -le -is -imatch -inotmatch -ilike"
                       + " -inotlike -ieq -ine -igt -ige -ilt -ile";
            }
        }
    }
}