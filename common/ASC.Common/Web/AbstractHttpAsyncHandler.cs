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
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace ASC.Common.Web
{
    public abstract class AbstractHttpAsyncHandler : IHttpAsyncHandler, IReadOnlySessionState
    {
        private Action<HttpContext> processRequest;
        private CultureInfo culture;


        public bool IsReusable
        {
            get { return false; }
        }


        public void ProcessRequest(HttpContext context)
        {
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            HttpContext.Current = context;
            OnProcessRequest(context);
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            culture = Thread.CurrentThread.CurrentCulture;
            processRequest = ProcessRequest;
            return processRequest.BeginInvoke(context, cb, extraData);
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            processRequest.EndInvoke(result);
        }


        public abstract void OnProcessRequest(HttpContext context);
    }
}
