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
    public abstract class AsyncHttpHandlerBase : IHttpAsyncHandler, IReadOnlySessionState
    {
        private Action<HttpContext> _processRequest;

        #region IHttpAsyncHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            HttpContext.Current = context;
            OnProcessRequest(context);
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var reqState = new AsyncRequestState(context, cb, extraData);
            _processRequest = OnProcessRequest;
            new Thread(new AsyncRequest(reqState).ProcessRequest).Start(_processRequest);
            return reqState;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var ars = result as AsyncRequestState;
            if (ars != null)
            {
                // here you could perform some cleanup, write something else to the
                // Response, or whatever else you need to do
            }
        }

        #endregion

        public abstract void OnProcessRequest(HttpContext context);
    }



    internal class AsyncRequest
    {
        private readonly AsyncRequestState _asyncRequestState;

        public AsyncRequest(AsyncRequestState ars)
        {
            _asyncRequestState = ars;
        }

        public void ProcessRequest(object parameter)
        {
            ((Action<HttpContext>) parameter)(_asyncRequestState.Ctx);
            _asyncRequestState.CompleteRequest();
        }
    }

    internal class AsyncRequestState : IAsyncResult
    {
        internal AsyncCallback Cb;
        internal HttpContext Ctx;
        internal object ExtraData;
        private EventWaitHandle _callCompleteEvent;
        private bool _isCompleted;

        public AsyncRequestState(HttpContext ctx,
                                 AsyncCallback cb,
                                 object extraData)
        {
            Ctx = ctx;
            Cb = cb;
            ExtraData = extraData;
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
            get { return (_isCompleted); }
        }

        public WaitHandle AsyncWaitHandle
        {
            get
            {
                lock (this)
                {
                    return _callCompleteEvent ?? (_callCompleteEvent = new ManualResetEvent(false));
                }
            }
        }

        #endregion

        internal void CompleteRequest()
        {
            _isCompleted = true;
            lock (this)
            {
                if (_callCompleteEvent != null)
                    _callCompleteEvent.Set();
            }
            // if a callback was registered, invoke it now
            if (Cb != null)
                Cb(this);
        }
    }
}