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
using System.Threading;
using System.Web;
using System.Web.SessionState;

namespace ASC.Api.Publisher
{
    internal class AsyncWaitRequestState : IAsyncResult
    {
        internal AsyncCallback Cb;
        private readonly HttpContext _ctx;
        internal object ExtraData;


        public AsyncWaitRequestState(HttpContext ctx,
                                 AsyncCallback cb,
                                 object extraData)
        {
            _ctx = ctx;
            Cb = cb;
            ExtraData = extraData;
        }
        
        public void OnCompleted()
        {
            if (Cb != null)
                Cb(this);
        }

        // IAsyncResult interface property implementations

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return (ExtraData); }
        }

        public bool CompletedSynchronously
        {
            get { return (false); }
        }

        public bool IsCompleted
        {
            get { return true; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                return null;
            }
        }

        public HttpContext Context
        {
            get { return _ctx; }
        }

        #endregion

    }
}