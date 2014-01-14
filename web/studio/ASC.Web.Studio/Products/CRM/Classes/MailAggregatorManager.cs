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
using System.Text;
using System.Threading;
using log4net;
using ASC.Web.Studio.Utility;
using ASC.Web.Core;
using System.Collections.Generic;
using ASC.Web.Studio.Core;
using System.Web;

namespace ASC.Web.CRM.Classes
{
    class AsyncRequestSender
    {
        public string RequestUrl { get; set; }
        public string RequestMethod { get; set; }
        public string RequestBody { get; set; }

        public void BeginGetResponse()
        {
            var request = (HttpWebRequest)WebRequest.Create(CommonLinkUtility.GetFullAbsolutePath(RequestUrl));

            request.Headers.Add("Authorization", CookiesManager.GetCookies(CookiesType.AuthKey));
            request.Method = RequestMethod;
            request.ContentType = "application/x-www-form-urlencoded";

            request.ContentLength = RequestBody.Length;

            var getRequestStream = request.BeginGetRequestStream(null, null);
            var writer = new StreamWriter(request.EndGetRequestStream(getRequestStream));
            writer.Write(RequestBody);
            writer.Close();

            request.BeginGetResponse(OnAsyncCallback, request);
        }

        private static void OnAsyncCallback(IAsyncResult asyncResult)
        {
            //var httpWebRequest = (HttpWebRequest)asyncResult.AsyncState;
            //var response = httpWebRequest.EndGetResponse(asyncResult);
            //var stream = response.GetResponseStream();
            //if (stream != null)
            //{
            //    var reader = new StreamReader(stream);
            //    var responseString = reader.ReadToEnd();
            //}
        }

    }

    public class MailAggregatorManager
    {
        public void UpdateMailAggregator(IEnumerable<string> emails, IEnumerable<Guid> userIds)
        {
            var apiServer = new Api.ApiServer();

            var body = GetPostBody(emails, userIds);

            apiServer.GetApiResponse(
                String.Format("{0}mail/messages/update_crm.json", SetupInfo.WebApiBaseUrl),
                "POST",
                body);
        }

        public void UpdateMailAggregatorAsync(IEnumerable<string> emails, IEnumerable<Guid> userIds)
        {
            var sender = new AsyncRequestSender
            {
                RequestUrl = String.Format("{0}mail/messages/update_crm.json", SetupInfo.WebApiBaseUrl),
                RequestMethod = "POST",
                RequestBody = GetPostBody(emails, userIds)
            };

            sender.BeginGetResponse();
        }

        private string GetPostBody(IEnumerable<string> emails, IEnumerable<Guid> userIds)
        {
            var sb = new StringBuilder();

            var itemFormat = HttpUtility.UrlEncode("emails[]") + "={0}&";
            foreach (var data in emails)
            {
                sb.Append(String.Format(itemFormat, HttpUtility.UrlEncode(data)));
            }

            itemFormat = HttpUtility.UrlEncode("userIds[]") + "={0}&";
            foreach (var data in userIds)
            {
                sb.Append(String.Format(itemFormat, HttpUtility.UrlEncode(data.ToString())));
            }

            return sb.ToString();
        }
    }
}
