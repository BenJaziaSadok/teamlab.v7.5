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
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    public class UploadProgressModule : IHttpModule
    {
        private static readonly FieldInfo RequestWorkerField = typeof(HttpRequest).GetField("_wr", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly Regex IsUrlWithExtension = new Regex(@"[^\.]+\.a[^x]+x", RegexOptions.Compiled);

        public void Init(HttpApplication application)
        {
            application.BeginRequest += OnBeginRequest;
            application.EndRequest += OnEndRequest;
        }

        public void Dispose()
        {
        }


        private void OnBeginRequest(object sender, EventArgs e)
        {
            var request = ((HttpApplication)sender).Context.Request;
            var origWr = (HttpWorkerRequest)RequestWorkerField.GetValue(request);

            if (UploadProgressUtils.IsUpload(origWr))
            {
                var s = request.RawUrl;

                if (string.IsNullOrEmpty(s))
                    return;

                if (!IsUrlWithExtension.IsMatch(s))
                    return;

                var newWr = new HttpUploadWorkerRequest(origWr);
                RequestWorkerField.SetValue(request, newWr);
            }
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var origWr = RequestWorkerField.GetValue(((HttpApplication)sender).Context.Request) as HttpUploadWorkerRequest;
            if (origWr != null)
            {
                origWr.EndOfUploadRequest();
            }
        }
    }
}