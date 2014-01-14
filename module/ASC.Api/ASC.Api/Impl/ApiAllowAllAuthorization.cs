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

#region usings

using System;
using System.Web;
using ASC.Api.Interfaces;

#endregion

namespace ASC.Api.Impl
{
    public class ApiAllowAllAuthorization : IApiAuthorization
    {
        #region IApiAuthorization Members

        public bool Authorize(HttpContextBase context)
        {
            return true;
        }

        public bool OnAuthorizationFailed(HttpContextBase context)
        {
            return false;
        }

        #endregion
    }
}