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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using ASC.Data.Storage;
using ASC.Data.Storage.S3;
using ASC.Security.Cryptography;
using ASC.Web.Studio.Utility;

namespace ASC.Mail.Aggregator.Utils
{
    public static class MailStoragePathCombiner
    {
        private static readonly Dictionary<char, string> _replacements = new Dictionary<char, string>
                {
                    {'+', "%2b"}, {'#', "%23"}, {'|', "_"}, {'<', "_"}, {'>', "_"}, {'"', "_"}, {':', "_"}, {'~', "_"}, {'?', "_"}
                };

        private const string BAD_CHARS_IN_PATH = "|<>:\"~?";

        private static string ComplexReplace(string str, string replacement)
        {
            return replacement.Aggregate(str, (current, bad_char) => current.Replace(bad_char.ToString(CultureInfo.InvariantCulture), _replacements[bad_char]));
        }

        public static string PrepareAttachmentName(string name)
        {
            return ComplexReplace(name, BAD_CHARS_IN_PATH);
        }

        public static string GetPreSignedUri(int file_id, int id_tenant, string id_user, string stream, int file_number,
                                          string file_name, IDataStore data_store)
        {
            var attachment_path = GetFileKey(id_user, stream, file_number, file_name);

            if (data_store == null)
                data_store = MailBoxManager.GetDataStore(id_tenant);

            string url;

            if (data_store is S3Storage)
            {
                var content_disposition_file_name = ContentDispositionUtil.GetHeaderValue(file_name);
                var headers_for_url = new []{"Content-Disposition:" + content_disposition_file_name};
                url = data_store.GetPreSignedUri("", attachment_path, TimeSpan.FromMinutes(5), headers_for_url).ToString();
            }
            else
            {
                //TODO: Move url to config;
                attachment_path = "/addons/mail/httphandlers/download.ashx";

                var uri_builder = new UriBuilder(CommonLinkUtility.GetFullAbsolutePath(attachment_path));
                if (uri_builder.Uri.IsLoopback)
                {
                    uri_builder.Host = Dns.GetHostName();
                }
                var query = uri_builder.Query;

                query += "attachid=" + file_id + "&";
                query += "stream=" + stream + "&";
                query += CommonLinkUtility.AuthKey + "=" + EmailValidationKeyProvider.GetEmailKey(file_id + stream);

                url = uri_builder.Uri + "?" + query;
            }

            return url;
        }

        public static string GetStoredUrl(Uri uri)
        {
            return GetStoredUrl(!uri.IsAbsoluteUri ? CommonLinkUtility.GetFullAbsolutePath(uri.ToString()) : uri.ToString());
        }

        private static string GetStoredUrl(string full_url)
        {
            return ComplexReplace(full_url, "#");
        }

        public static string GetFileKey(string id_user, string stream, int file_number, string file_name)
        {
            return String.Format("{0}/{1}/attachments/{2}/{3}", id_user, stream, file_number, ComplexReplace(file_name, BAD_CHARS_IN_PATH));
        }

        public static string GetBodyKey(string stream)
        {
            return String.Format("{0}/body.html", stream);
        }

        public static string GetBodyKey(string id_user, string stream)
        {
            return String.Format("{0}/{1}/body.html", id_user, stream);
        }
    }
}
