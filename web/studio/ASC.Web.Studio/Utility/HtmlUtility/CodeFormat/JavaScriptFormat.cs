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

#region Copyright � 2001-2003 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */ 
#endregion

namespace ASC.Web.Studio.Utility.HtmlUtility.CodeFormat
{
    /// <summary>
    /// Generates color-coded HTML 4.01 from JavaSctript source code.
    /// </summary>
    internal class JavaScriptFormat : CLikeFormat
    {
        /// <summary>
        /// The list of JavaScript keywords.
        /// </summary>
        protected override string Keywords
        {
            get
            {
                return "var function abstract as base bool break byte case catch char "
                       + "checked class const continue decimal default delegate do double else "
                       + "enum event explicit extern false finally fixed float for foreach goto "
                       + "if implicit in int interface internal is lock long namespace new null "
                       + "object operator out override params private protected public readonly "
                       + "ref return sbyte sealed short sizeof stackalloc static string struct "
                       + "switch this throw true try typeof uint ulong unchecked unsafe ushort "
                       + "using virtual void while";
            }
        }

        /// <summary>
        /// Use the pre-processor color to mark directives that start with @.
        /// </summary>
        protected override string Preprocessors
        {
            get { return @"@\w*"; }
        }
    }
}