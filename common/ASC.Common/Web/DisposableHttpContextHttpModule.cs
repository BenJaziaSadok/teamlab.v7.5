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

#endregion

namespace ASC.Common.Web
{
    public class DisposableHttpContextHttpModule : IHttpModule
    {
        #region IHttpModule

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += Application_EndRequest;
        }

        #endregion

        private void Application_EndRequest(Object source, EventArgs e)
        {
            var application = (HttpApplication) source;
            new DisposableHttpContext(application.Context).Dispose();
        }
    }
}