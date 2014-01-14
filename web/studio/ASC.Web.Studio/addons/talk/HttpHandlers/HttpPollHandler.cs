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
using System.IO;
using System.Net;
using System.Web;
using System.Web.Configuration;

namespace ASC.Web.Talk.HttpHandlers
{
    public class HttpPollHandler : IHttpAsyncHandler
    {
        private static readonly Uri boshUri;


        static HttpPollHandler()
        {
            var uri = WebConfigurationManager.AppSettings["BoshPath"] ?? "http://localhost:5280/http-poll/";
            boshUri = new Uri(VirtualPathUtility.AppendTrailingSlash(uri));
        }


        public bool IsReusable
        {
            get { return true; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            var request = (HttpWebRequest)WebRequest.Create(boshUri);

            request.Method = context.Request.HttpMethod;

            // headers
            request.UserAgent = context.Request.UserAgent;
            request.Accept = string.Join(",", context.Request.AcceptTypes);
            if (!string.IsNullOrEmpty(context.Request.Headers["Accept-Encoding"]))
            {
                request.Headers["Accept-Encoding"] = context.Request.Headers["Accept-Encoding"];
            }
            request.ContentType = context.Request.ContentType;
            request.ContentLength = context.Request.ContentLength;
            // body
            using (var stream = request.GetRequestStream())
            {
                CopyStream(context.Request.InputStream, stream);
            }

            return request.BeginGetResponse(cb, new object[] { context, request });
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var context = (HttpContext)((object[])result.AsyncState)[0];
            var request = (HttpWebRequest)((object[])result.AsyncState)[1];
            using (var response = request.EndGetResponse(result))
            {
                context.Response.ContentType = response.ContentType;

                // headers
                foreach (string h in response.Headers)
                {
                    context.Response.AppendHeader(h, response.Headers[h]);
                }
                // body
                using (var stream = response.GetResponseStream())
                {
                    CopyStream(stream, context.Response.OutputStream);
                }

                response.Close();
                context.Response.Flush();
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotSupportedException();
        }

        
        private void CopyStream(Stream from, Stream to)
        {
            var buffer = new byte[1024];
            while (true)
            {
                var read = from.Read(buffer, 0, buffer.Length);
                if (read == 0) break;

                to.Write(buffer, 0, read);
            }
        }
    }
}
