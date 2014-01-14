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
    internal class CppFormat : CLikeFormat
    {
        protected override string Keywords
        {
            get
            {
                return "and and_eq asm auto bitand bitor bool break case catch char class compl "
                       + "const const_cast continue default delete do double dynamic_cast else enum "
                       + "explicit export extern false float for friend goto if inline int long mutable "
                       + "namespace new not not_eq operator or or_eq private protected public register "
                       + "reinterpret_cast return short signed sizeof static static_cast struct switch "
                       + "template this throw true try typedef typeid typename union unsigned using "
                       + "virtual void volatile wchar_t while xor xor_eq";
            }
        }

        protected override string Preprocessors
        {
            get
            {
                return "#define #error #if #ifdef #ifndef #else #elif #endif "
                       + "#include #line #pragma #undef";
            }
        }
    }
}