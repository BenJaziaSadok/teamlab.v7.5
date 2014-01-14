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
using System.Text;
using System.Threading;
using System.Web;

namespace ASC.Web.Core.Client.Bundling
{
    internal class AsyncBundlingRequestState : IAsyncResult
    {
        private readonly HttpContext _ctx;
        internal AsyncCallback Cb;
        internal object ExtraData;


        public AsyncBundlingRequestState(HttpContext ctx,
                                         AsyncCallback cb,
                                         object extraData)
        {
            _ctx = ctx;
            Cb = cb;
            ExtraData = extraData;
            ResourceData = new StringBuilder();
        }

        public StringBuilder ResourceData { get; set; }

        public HttpContext Context
        {
            get { return _ctx; }
        }

        public bool IsFromCache { get; set; }

        // IAsyncResult interface property implementations

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return (ExtraData); }
        }

        public bool CompletedSynchronously { get; private set; }

        public bool IsCompleted { get; private set; }

        public WaitHandle AsyncWaitHandle
        {
            get { return null; }
        }

        #endregion

        public void OnCompleted()
        {
            OnCompleted(false);
        }

        public void OnCompleted(bool synchronous)
        {
            IsCompleted = true;
            CompletedSynchronously = synchronous;
            if (Cb != null)
                Cb(this);
        }

        public void OnCompletedAsync()
        {
            IsCompleted = true;
            if (Cb != null)
                Cb.BeginInvoke(this, result => Cb.EndInvoke(result), null);
        }
    }
}