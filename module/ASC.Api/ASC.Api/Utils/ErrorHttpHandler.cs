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

using System.Web;

#endregion

namespace ASC.Api.Utils
{
    public class ErrorHttpHandler : IHttpHandler
    {
        private readonly int _code;
        private readonly string _message;

        public ErrorHttpHandler(int code, string message)
        {
            _code = code;
            _message = message;
        }

        #region IHttpHandler Members

        public void ProcessRequest(HttpContext context)
        {
            context.Response.StatusCode = _code;
            context.Response.StatusDescription = _message;
            context.Response.Write(string.IsNullOrEmpty(_message)?"error":_message);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        #endregion
    }
}