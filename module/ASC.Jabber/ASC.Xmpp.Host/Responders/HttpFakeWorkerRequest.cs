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
using System.Net;
using System.Web;

namespace ASC.Xmpp.Host.Responders
{
    public class HttpFakeWorkerRequest:HttpWorkerRequest
    {
        private readonly HttpListenerRequest _request;

        public HttpFakeWorkerRequest(HttpListenerRequest request)
        {
            _request = request;
        }

        public override string GetUriPath()
        {
            return _request.Url.LocalPath;
        }

        public override string GetQueryString()
        {
            return _request.Url.Query;
        }

        public override string GetRawUrl()
        {
            return _request.RawUrl;
        }

        public override string GetHttpVerbName()
        {
            return _request.HttpMethod;
        }

        public override string GetHttpVersion()
        {
            return _request.HttpMethod;
        }

        public override string GetRemoteAddress()
        {
            return _request.RemoteEndPoint.Address.ToString();
        }

        public override int GetRemotePort()
        {
            return _request.RemoteEndPoint.Port;
        }

        public override string GetLocalAddress()
        {
            return _request.Url.Host;
        }

        public override int GetLocalPort()
        {
            return _request.Url.Port;
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            
        }

        public override void FlushResponse(bool finalFlush)
        {
            
        }

        public override void EndOfRequest()
        {
           
        }
    }
}