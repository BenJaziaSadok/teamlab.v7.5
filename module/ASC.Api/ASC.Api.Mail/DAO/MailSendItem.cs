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
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ASC.Data.Storage;
using ASC.Mail.Aggregator.Extension;
using ASC.Mail.Aggregator.Collection;
using HtmlAgilityPack;
using ASC.Mail.Aggregator;
using ActiveUp.Net.Mail;
using ASC.Api.Mail.Resources;

namespace ASC.Api.Mail.DAO
{
    internal class MailSendItem
    {
        public MailSendItem()
        {
            Attachments = new List<MailAttachment>();
            AttachmentsEmbedded = new List<MailAttachment>();
            To = new List<string>();
            Cc = new List<string>();
            Bcc = new List<string>();
            Labels = new List<int>();
        }

        public List<string> To { get; set; }

        public List<string> Cc { get; set; }

        public List<string> Bcc { get; set; }

        public bool Important { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string HtmlBody { get; set; }

        public List<MailAttachment> Attachments { get; set; }

        public List<MailAttachment> AttachmentsEmbedded { get; set; }

        public string StreamId { get; set; }

        public string DisplayName
        {
            get { return string.IsNullOrEmpty(_displayName) ? "" : _displayName; }
            set { _displayName = value; }
        }

        public int ReplyToId { get; set; }

        public List<int> Labels { get; set; }

        private string _displayName = string.Empty;

        private const string EMBEDDED_DOMAIN = "mail";

        public MailMessageItem ToMailMessageItem(int tenant, string user)
        {
            Address from_verified;
            if (Validator.ValidateSyntax(From))
                from_verified = new Address(From, DisplayName);
            else
                throw new ArgumentException(MailApiResource.ErrorIncorrectEmailAddress
                                   .Replace("%1", MailApiResource.FieldNameFrom));

            var message_item = new MailMessageItem
                {
                    From = from_verified.ToString(),
                    FromEmail = from_verified.Email,
                    To = string.Join(", ", To.ToArray()),
                    Cc = Cc != null ? string.Join(", ", Cc.ToArray()) : "",
                    Bcc = Bcc != null ? string.Join(", ", Bcc.ToArray()) : "",
                    Subject = Subject,
                    Date = DateTime.Now,
                    Important = Important,
                    HtmlBody = HtmlBody,
                    Introduction = MailMessageItem.GetIntroduction(HtmlBody),
                    StreamId = StreamId,
                    TagIds = Labels != null && Labels.Count != 0 ? new ItemList<int>(Labels) : null,
                    Size = HtmlBody.Length
                };

            if (message_item.Attachments == null) 
                message_item.Attachments = new List<MailAttachment>();

            Attachments.ForEach(attachment => { attachment.tenant = tenant; attachment.user = user; });

            message_item.Attachments.AddRange(Attachments);
            return message_item;
        }

        public Message ToMimeMessage(int tenant, string user, bool load_attachments)
        {
            var mime_message = new Message
                {
                    Date = DateTime.Now,
                    From = new Address(From, string.IsNullOrEmpty(DisplayName) ? "" : Codec.RFC2047Encode(DisplayName))
                };

            if (Important)
                mime_message.Priority = MessagePriority.High;

            mime_message.To.AddRange(To.ConvertAll(address =>
                {
                    var addr = Parser.ParseAddress(address);
                    addr.Name = string.IsNullOrEmpty(addr.Name) ? "" : Codec.RFC2047Encode(addr.Name);
                    return new Address(addr.Email, addr.Name);
                }));

            mime_message.Cc.AddRange(Cc.ConvertAll(address =>
                {
                    var addr = Parser.ParseAddress(address);
                    addr.Name = string.IsNullOrEmpty(addr.Name) ? "" : Codec.RFC2047Encode(addr.Name);
                    return new Address(addr.Email, addr.Name);
                }));

            mime_message.Bcc.AddRange(Bcc.ConvertAll(address =>
                {
                    var addr = Parser.ParseAddress(address);
                    addr.Name = string.IsNullOrEmpty(addr.Name) ? "" : Codec.RFC2047Encode(addr.Name);
                    return new Address(addr.Email, addr.Name);
                }));

            mime_message.Subject = Codec.RFC2047Encode(Subject);

            // Set correct body
            if (Attachments.Any() || AttachmentsEmbedded.Any())
            {
                foreach (var attachment in Attachments)
                {
                    attachment.user = user;
                    attachment.tenant = tenant;
                    var attach = CreateAttachment(attachment, load_attachments);
                    if (attach != null)
                        mime_message.Attachments.Add(attach);
                }

                foreach (var embedded_attachment in AttachmentsEmbedded)
                {
                    embedded_attachment.user = user;
                    embedded_attachment.tenant = tenant;
                    var attach = CreateAttachment(embedded_attachment, true);
                    if (attach != null)
                        mime_message.EmbeddedObjects.Add(attach);
                }
            }

            mime_message.BodyText.Charset = Encoding.UTF8.HeaderName;
            mime_message.BodyText.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            mime_message.BodyText.Text = "";

            mime_message.BodyHtml.Charset = Encoding.UTF8.HeaderName;
            mime_message.BodyHtml.ContentTransferEncoding = ContentTransferEncoding.QuotedPrintable;
            mime_message.BodyHtml.Text = HtmlBody;

            return mime_message;
        }

        public List<MailAttachment> ChangeEmbededAttachmentLinksForStoring(int tenant, string username, int mail_id, MailBoxManager manager)
        {
            //Todo: This method can be separated in two
            var fck_storage = GetDataStoreForFckImages(tenant);
            var attachment_storage = GetDataStoreForAttachments(tenant);
            
            var current_mail_fckeditor_url = fck_storage.GetUri(EMBEDDED_DOMAIN, "").ToString();
            var current_mail_attachment_folder_url = GetThisMailFolder(username);
            var current_user_storage_url = GetUserFolder(username);

            var xpath_query = GetXpathQueryForAttachmentsToResaving(current_mail_fckeditor_url, current_mail_attachment_folder_url, current_user_storage_url);
            var attachments_for_saving = new List<MailAttachment>();

            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlBody);
            
            var link_nodes = doc.DocumentNode.SelectNodes(xpath_query);

            if (link_nodes != null)
            {
                foreach (var link_node in link_nodes)
                {
                    var link = link_node.Attributes["src"].Value;
                    var is_fck_image = link.StartsWith(current_mail_fckeditor_url);
                    var prefix_length = is_fck_image ? current_mail_fckeditor_url.Length 
                                                     : link.IndexOf(current_user_storage_url, StringComparison.Ordinal) + current_user_storage_url.Length + 1;
                    var file_link = HttpUtility.UrlDecode(link.Substring(prefix_length));

                    var file_name = Path.GetFileName(file_link);
                    var attach = CreateEmbbededAttachment(file_name, link, file_link, username, tenant);

                    var saved_attachment_id = manager.GetAttachmentId(mail_id, attach.contentId);
                    var attachment_was_saved = saved_attachment_id != 0;
                    var current_img_storage = is_fck_image ? fck_storage : attachment_storage;
                    var domain = is_fck_image ? EMBEDDED_DOMAIN : username;

                    if (mail_id == 0 || !attachment_was_saved)
                    {
                        LoadAttachmentData(attach, domain, file_link, current_img_storage);
                        //TODO: Add quota if needed
                        manager.StoreAttachmentWithoutQuota(tenant, username, attach);
                        attachments_for_saving.Add(attach);
                    }

                    if (attachment_was_saved)
                    {
                        attach = manager.GetMessageAttachment(saved_attachment_id, tenant, username);
                        var path = attach.GerStoredFilePath();
                        current_img_storage = attachment_storage;
                        attach.storedFileUrl = current_img_storage.GetUri("", path).ToString();
                    }

                    link_node.SetAttributeValue("src", attach.storedFileUrl);
                }

                HtmlBody = doc.DocumentNode.OuterHtml;
            }

            return attachments_for_saving;
        }

        private static void LoadAttachmentData(MailAttachment attach, string domain, string file_link, IDataStore current_img_storage)
        {
            using (var stream = current_img_storage.GetReadStream(domain, file_link))
            {
                attach.data = stream.GetCorrectBuffer();
            }
        }

        private MailAttachment CreateEmbbededAttachment(string file_name, string link, string file_link, string username, int tenant)
        {
            return new MailAttachment
                {
                    fileName = file_name,
                    storedName = file_name,
                    contentId = link.GetMD5(),
                    storedFileUrl = file_link,
                    streamId = StreamId,
                    user = username,
                    tenant = tenant
                };
        }

        private static string GetXpathQueryForAttachmentsToResaving(string this_mail_fckeditor_url,
                                                                    string this_mail_attachment_folder_url,
                                                                    string this_user_storage_url)
        {
            const string src_condition_format = "contains(@src,'{0}')";
            var added_by_user_to_fck = String.Format(src_condition_format, this_mail_fckeditor_url);
            var added_to_this_mail = String.Format(src_condition_format, this_mail_attachment_folder_url);
            var added_to_this_user_mails = String.Format(src_condition_format, this_user_storage_url);
            var xpath_query = String.Format("//img[@src and ({0} or {1}) and not({2})]", added_by_user_to_fck,
                                            added_to_this_user_mails, added_to_this_mail);
            return xpath_query;
        }

        public void ChangeEmbededAttachmentLinks(int tenant, string id_user)
        {
            var base_attachment_folder = GetThisMailFolder(id_user);

            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlBody);
            var link_nodes = doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'" + base_attachment_folder + "'))]");
            if (link_nodes != null)
            {
                foreach (var link_node in link_nodes)
                {
                    var link = link_node.Attributes["src"].Value;
                    var file_link = HttpUtility.UrlDecode(link.Substring(base_attachment_folder.Length));
                    var file_name = Path.GetFileName(file_link);

                    var attach = CreateEmbbededAttachment(file_name, link, file_link, id_user, tenant);
                    AttachmentsEmbedded.Add(attach);
                    link_node.SetAttributeValue("src", "cid:" + attach.contentId);
                }
                HtmlBody = doc.DocumentNode.OuterHtml;
            }
        }

        private string GetThisMailFolder(string id_user)
        {
            return String.Format("{0}/{1}", id_user, StreamId);
        }

        private string GetUserFolder(string id_user)
        {
            return id_user;
        }

        public void ChangeSmileLinks()
        {
            var base_smile_url = SmileToAttachmentConvertor.SmileBaseUrl;

            var doc = new HtmlDocument();
            doc.LoadHtml(HtmlBody);
            var link_nodes = doc.DocumentNode.SelectNodes("//img[@src and (contains(@src,'" + base_smile_url + "'))]");
            if (link_nodes != null)
            {
                var smile_convertor = new SmileToAttachmentConvertor();
                foreach (var link_node in link_nodes)
                {
                    var link = link_node.Attributes["src"].Value;
                    var attach = smile_convertor.ToMailAttachment(link);
                    link_node.SetAttributeValue("src", "cid:" + attach.contentId);

                    if (AttachmentsEmbedded.All(x => x.contentId != attach.contentId))
                    {
                        AttachmentsEmbedded.Add(attach);
                    }
                }
                HtmlBody = doc.DocumentNode.OuterHtml;
            }
        }

        private static MimePart CreateAttachment(MailAttachment attachment, bool load_attachments)
        {
            var ret_val = new MimePart();

            var s3_key = attachment.GerStoredFilePath();
            var file_name = attachment.fileName ?? Path.GetFileName(s3_key);

            if (load_attachments)
            {
                var byte_array = attachment.data;

                if (byte_array == null || byte_array.Length == 0)
                {
                    using (var stream = GetDataStoreForAttachments(attachment.tenant).GetReadStream(s3_key))
                    {
                        byte_array = stream.GetCorrectBuffer();
                    }
                }

                ret_val = new MimePart(byte_array, file_name);

                if (attachment.contentId != null) ret_val.ContentId = attachment.contentId;
            }
            else
            {
                var conent_type = Common.Web.MimeMapping.GetMimeMapping(s3_key);
                ret_val.ContentType = new ContentType {Type = conent_type};
                ret_val.Filename = file_name;
                if (attachment.contentId != null) ret_val.ContentId = attachment.contentId;
                ret_val.TextContent = "";
            }

            return ret_val;
        }

        private static IDataStore GetDataStoreForFckImages(int tenant)
        {
            return StorageFactory.GetStorage(null, tenant.ToString(CultureInfo.InvariantCulture), "fckuploaders", null, null);
        }

        private static IDataStore GetDataStoreForAttachments(int tennant_id)
        {
            return StorageFactory.GetStorage(null, tennant_id.ToString(CultureInfo.InvariantCulture), "mailaggregator", null, null);
        }

        public void Validate()
        {
            ValidateAddresses(MailApiResource.FieldNameFrom, new[] {From}, true);
            ValidateAddresses(MailApiResource.FieldNameTo, To, true);
            ValidateAddresses(MailApiResource.FieldNameCc, Cc, false);
            ValidateAddresses(MailApiResource.FieldNameBcc, Bcc, false);

            if (string.IsNullOrEmpty(StreamId))
                throw new ArgumentException("no streamId");
        }

        private static void ValidateAddresses(string field_name, IEnumerable<string> addresses, bool strong_validation)
        {
            var invalid_email_found = false;
            if (addresses != null)
            {
                if (addresses.Any(addr => !Validator.ValidateSyntax(addr)))
                    invalid_email_found = true;

                if (invalid_email_found)
                    throw new ArgumentException(MailApiResource.ErrorIncorrectEmailAddress.Replace("%1", field_name));
            }
            else if (strong_validation)
                throw new ArgumentException(MailApiResource.ErrorEmptyField.Replace("%1", field_name));
        }


    }
}
