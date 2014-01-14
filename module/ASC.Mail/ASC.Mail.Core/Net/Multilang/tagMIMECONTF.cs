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
    using System.Security;

    public enum tagMIMECONTF
    {
        MIMECONTF_BROWSER = 2,
        MIMECONTF_EXPORT = 0x400,
        MIMECONTF_IMPORT = 8,
        MIMECONTF_MAILNEWS = 1,
        MIMECONTF_MIME_IE4 = 0x10000000,
        MIMECONTF_MIME_LATEST = 0x20000000,
        MIMECONTF_MIME_REGISTRY = 0x40000000,
        MIMECONTF_MINIMAL = 4,
        MIMECONTF_PRIVCONVERTER = 0x10000,
        MIMECONTF_SAVABLE_BROWSER = 0x200,
        MIMECONTF_SAVABLE_MAILNEWS = 0x100,
        MIMECONTF_VALID = 0x20000,
        MIMECONTF_VALID_NLS = 0x40000
    }
}
