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

using System.Text.RegularExpressions;
using System.Web;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    internal class ContentDispositionInfo
    {
        internal string name;
        internal string filename;

        internal bool IsFile
        {
            get { return !string.IsNullOrEmpty(filename); }
        }
    }

    internal static class UploadProgressUtils
    {

        private const string PostProtocol = "POST";
        private const string GetVerbName = "GET";

        private static Regex _getUploadId = new Regex(@"(?<=GetUploadProgress\.ashx\?__UixdId=)[^&]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _isMultiPartFormData = new Regex(@"^multipart\/form-data", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _getBoundary = new Regex(@"(?<=boundary=)[^;]+", RegexOptions.Compiled | RegexOptions.Compiled);
        private static readonly Regex _getContentDisposition = new Regex(@"(?<=Content-Disposition\:\s)[^$]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static readonly Regex _getContentDispositionName = new Regex(@"(?<=;\s*name\s*="")[^""]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex _getContentDispositionFileName = new Regex(@"(?<=;\s*filename\s*="")[^""]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        internal static bool IsUploadStatusRequest(HttpWorkerRequest request, out string id)
        {
            id = string.Empty;

            if (request.GetHttpVerbName() != GetVerbName)
                return false;

            Match m = _getUploadId.Match(request.GetRawUrl());

            if (!m.Success)
                return false;

            id = m.Value;

            return true;
        }


        internal static ContentDispositionInfo GetContentDisposition(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            Match m = _getContentDisposition.Match(value);
            if (!m.Success)
                return null;

            string val = m.Value;

            ContentDispositionInfo cdi = new ContentDispositionInfo();

            m = _getContentDispositionName.Match(val);
            if (m.Success)
                cdi.name = m.Value;


            m = _getContentDispositionFileName.Match(val);
            if (m.Success)
                cdi.filename =GetFileName(m.Value);

            return cdi;
        }

        private static string GetFileName(string path)
        {
            var name = path ?? "";
            var ind = name.LastIndexOf('\\');
            if (ind != -1)
                return name.Substring(ind+1);

            return name;
        }

        internal static bool IsMultiPartFormData(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            return _isMultiPartFormData.IsMatch(contentType);
        }

        internal static bool HasBoundary(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return false;

            return _getBoundary.IsMatch(contentType);
        }

        internal static string GetBoundary(string contentType)
        {
            if (string.IsNullOrEmpty(contentType))
                return string.Empty;

            Match m = _getBoundary.Match(contentType);
            return !m.Success ? string.Empty : m.Value;
        }

        internal static bool IsPost(HttpWorkerRequest request)
        {
            return request.GetHttpVerbName() == PostProtocol;
        }

        internal static bool IsUpload(HttpWorkerRequest request)
        {
            string contentType = request.GetKnownRequestHeader(HttpWorkerRequest.HeaderContentType);
            return IsPost(request) && IsMultiPartFormData(contentType) && HasBoundary(contentType);
        }

    }
}