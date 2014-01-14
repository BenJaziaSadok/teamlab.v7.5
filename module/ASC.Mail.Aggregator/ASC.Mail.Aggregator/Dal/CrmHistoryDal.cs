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
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Mail.Aggregator.DbSchema;
using ASC.Specific;
using System.Xml.XPath;

namespace ASC.Mail.Aggregator.Dal
{
    public class CrmHistoryDal
    {
        [DataContract]
        internal class CrmHistoryContent
        {
            [DataMember]
            public long message_id;
        }

        private const int MailCrmHistoryCategory = -3;

        private readonly MailBoxManager _manager;
        private readonly string _baseUrl;
        private readonly int _tenant_id;
        private readonly Guid _user_id;

        private string BaseUrl{get { return _baseUrl; }}
        private MailBoxManager Manager { get { return _manager; } }
        private int TenantId { get { return _tenant_id; } }
        private Guid UserId { get { return _user_id; } }

        public CrmHistoryDal(MailBoxManager manager, int tenant_id, string user_id)
        {
            _manager = manager;
            _baseUrl = WebConfigurationManager.AppSettings["api.url"].Trim('~', '/');
            _tenant_id = tenant_id;
            _user_id = new Guid(user_id);
        }

        public void AddRelationshipEvents(MailMessageItem item)
        {
            CoreContext.TenantManager.SetCurrentTenant(TenantId);
            foreach (var contact_entity in item.LinkedCrmEntityIds)
            {
                var file_ids = StoreAttachmentsToCrm(item, contact_entity);
                var content_string = GetHistoryContentJson(item);
                AddRelationshipEventWithCrmApi(item, contact_entity, content_string, file_ids);
            }
        }

        private List<int> StoreAttachmentsToCrm(MailMessageItem item, CrmContactEntity entity)
        {
            var file_ids = new List<int>();
            foreach (var attachment in item.Attachments)
            {
                using (var file = Manager.GetAttachmentStream(attachment))
                {
                    var uploaded_file_id = UploadFileToCrm(file.FileStream, file.FileName, attachment.contentType, entity);
                    if (uploaded_file_id > 0)
                    {
                        file_ids.Add(uploaded_file_id);
                    }
                }
            }
            return file_ids;
        }

        private int UploadFileToCrm(Stream file, string filename, string content_type, CrmContactEntity entity)
        {
            var post_parameters = new Dictionary<string, object>
            {
                {"entityType", entity.Type.StringName()},
                {"enitityid", entity.Id},
                {"storeOriginalFileFlag", true}
            };

            var request_uri_builder = GetUploadToCrmUrl(entity.Id, entity.Type.StringName());
            var auth_cookie = GetAuthCookie(request_uri_builder);

            post_parameters.Add("file", new FormUpload.FileParameter(file, filename, content_type));
            var responce = FormUpload.MultipartFormDataPost(request_uri_builder.Uri.ToString(), "", post_parameters, auth_cookie);
            var uploaded_file_id = ParseUploadResponse(responce);
            return uploaded_file_id;
        }

        private static int ParseUploadResponse(HttpWebResponse responce)
        {
            if (responce != null)
            {
                var responce_stream = responce.GetResponseStream();
                if (responce_stream != null)
                {
                    var xdoc = new XPathDocument(responce_stream);
                    var navigator = xdoc.CreateNavigator();
                    var res = navigator.SelectSingleNode("/result/response/id");
                    if (res != null)
                    {
                        return res.ValueAsInt;
                    }
                }
            }
            return -1;
        }

        private Cookie GetAuthCookie(UriBuilder request_uri_builder)
        {
            return new Cookie("asc_auth_key", Manager.GetAuthCookie(UserId),"/",
                              request_uri_builder.Host);
        }

        private UriBuilder GetUploadToCrmUrl(int id, string crm_entity_type)
        {
            var upload_url = String.Format("{2}/crm/{0}/{1}/files/upload.xml", crm_entity_type, id, BaseUrl);
            return GetApiRequestUrl(upload_url);
        }

        private UriBuilder GetAddRelationshipEventCrmUrl()
        {
            var add_url = string.Format("{0}/{1}", BaseUrl, "crm/history.json");
            return GetApiRequestUrl(add_url);
        }

        private static UriBuilder GetApiRequestUrl(string api_url)
        {
            var temp_url = api_url;
            var request_uri_builder =
                new UriBuilder(HttpContext.Current != null ? HttpContext.Current.Request.Url.Scheme : Uri.UriSchemeHttp,
                               CoreContext.TenantManager.GetCurrentTenant().TenantAlias);

            if (CoreContext.TenantManager.GetCurrentTenant().TenantAlias == "localhost")
            {
                var virtual_dir = WebConfigurationManager.AppSettings["core.virtual-dir"];
                if (!string.IsNullOrEmpty(virtual_dir)) temp_url = virtual_dir.Trim('/') + "/" + temp_url;

                var host = WebConfigurationManager.AppSettings["core.host"];
                if (!string.IsNullOrEmpty(host)) request_uri_builder.Host = host;

                var port = WebConfigurationManager.AppSettings["core.port"];
                if (!string.IsNullOrEmpty(port)) request_uri_builder.Port = int.Parse(port);
            }
            else
                request_uri_builder.Host += "." + WebConfigurationManager.AppSettings["core.base-domain"];

            request_uri_builder.Path = temp_url;
            return request_uri_builder;
        }

        private void AddRelationshipEventWithCrmApi(MailMessageItem item, CrmContactEntity entity, string content_string, List<int> files_id)
        {
            var body_builder = new StringBuilder();
            body_builder
                .AppendFormat("content={0}", HttpUtility.UrlEncode(content_string))
                .AppendFormat("&categoryId={0}", MailCrmHistoryCategory)
                .AppendFormat("&created={0}", HttpUtility.UrlEncode(new ApiDateTime(item.Date).ToString()));

            var crm_entity_type = entity.Type.StringName();
            if (crm_entity_type == ChainXCrmContactEntity.CrmEntityTypeNames.contact)
            {
                body_builder.AppendFormat("&contactId={0}&entityId=0", entity.Id);
            }
            else
            {
                if (crm_entity_type != ChainXCrmContactEntity.CrmEntityTypeNames.Case
                    && crm_entity_type != ChainXCrmContactEntity.CrmEntityTypeNames.opportunity)
                    throw new ArgumentException(String.Format("Invalid crm entity type: {0}", crm_entity_type));
                body_builder.AppendFormat("&contactId=0&entityId={0}&entityType={1}", entity.Id, crm_entity_type);
            }

            if (files_id != null)
            {
                foreach (var id in files_id)
                {
                    body_builder.AppendFormat("&fileId[]={0}", id);
                }
            }

            var request_uri_builder = GetAddRelationshipEventCrmUrl();
            var auth_cookie = GetAuthCookie(request_uri_builder);

            byte[] body_bytes = Encoding.UTF8.GetBytes(body_builder.ToString());
            var request_stream = new MemoryStream();
            request_stream.Write(body_bytes, 0, body_bytes.Length);
            request_stream.Position = 0;
            FormUpload.PostForm(request_uri_builder.ToString(), "", "application/x-www-form-urlencoded",
                                request_stream, auth_cookie);
        }

        private static string GetHistoryContentJson(MailMessageItem item)
        {
            string content_string;

            var content_struct = new CrmHistoryContent
                {
                    message_id = item.Id
                };

            var serializer = new DataContractJsonSerializer(typeof (CrmHistoryContent));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, content_struct);
                content_string = Encoding.UTF8.GetString(stream.GetCorrectBuffer());
            }
            return content_string;
        }
    }
}
