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
using System.Collections.Generic;
using System.Text;

namespace MultiLanguage
{
    public enum MLDETECTCP {
        // Default setting will be used. 
        MLDETECTCP_NONE = 0,

        // Input stream consists of 7-bit data. 
        MLDETECTCP_7BIT = 1,

        // Input stream consists of 8-bit data. 
        MLDETECTCP_8BIT = 2,

        // Input stream consists of double-byte data. 
        MLDETECTCP_DBCS = 4,

        // Input stream is an HTML page. 
        MLDETECTCP_HTML = 8,

        //Not currently supported. 
        MLDETECTCP_NUMBER = 16
    } 
}
