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

namespace LumiSoft.Net
{
    /// <summary>
    /// Implements TEL URI. Defined in RFC 2806.
    /// </summary>
    public class TEL_Uri : AbsoluteUri
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        internal TEL_Uri()
        {
        }


        #region Properties implementation

        public bool IsGlobal
        {
            get{ return false; }
        }

        public string PhoneNmber
        {
            get{ return ""; }
        }


        #endregion

    }
}
