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

namespace MultiLanguage
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security;

    /// Thanks to jannewe for finding the fix!
    /// http://www.codeproject.com/KB/recipes/DetectEncoding.aspx?msg=3247475#xx3247475xx
    [StructLayout(LayoutKind.Sequential)]
    public struct DetectEncodingInfo
    {
        public uint nLangID;
        public uint nCodePage;
        public int nDocPercent;
        public int nConfidence;
    }
}
