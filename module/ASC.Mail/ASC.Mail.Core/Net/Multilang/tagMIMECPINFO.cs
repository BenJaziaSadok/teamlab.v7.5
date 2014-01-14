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

    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct tagMIMECPINFO
    {
        public uint dwFlags;
        public uint uiCodePage;
        public uint uiFamilyCodePage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x40)]
        public ushort[] wszDescription;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=50)]
        public ushort[] wszWebCharset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=50)]
        public ushort[] wszHeaderCharset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=50)]
        public ushort[] wszBodyCharset;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]
        public ushort[] wszFixedWidthFont;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=0x20)]
        public ushort[] wszProportionalFont;
        public byte bGDICharset;
    }
}
