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
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;
using ASC.Mail.Aggregator;
using ASC.Mail.Aggregator.Dal;
using ActiveUp.Net.Mail;
using ASC.Api.Mail.DAO;
using System.Linq;
using ASC.Api.Mail.Resources;
using ASC.Mail.Aggregator.Authorization;

namespace ASC.Api.Mail
{
    class MailSendQueue
    {
        private readonly MailBoxManager _manager;

        public MailSendQueue(MailBoxManager manager)
        {
            _manager = manager;
        }

        private string GetAccessToken(MailBox mail_box)
        {
            var service_type = (AuthorizationServiceType)mail_box.ServiceType;

            switch (service_type)
            {
                case AuthorizationServiceType.Google:
                    var granted_access = new GoogleOAuth2Authorization()
                        .RequestAccessToken(mail_box.RefreshToken);

                    if (granted_access != null)
                        return granted_access.AccessToken;
                    break;
            }

            return "";
        }

        public int Send(int tenant_id, string username, MailSendItem item, int mail_id)
        {
            var mbox = _manager.GetMailBox(tenant_id, username, new MailAddress(item.From));
            if (mbox == null)
                throw new ArgumentException("no such mailbox");
            
            if (mbox.Name != "")
            {
                item.DisplayName = mbox.Name;
            }

            string mime_message_id, in_reply_to;
            var result_message = SaveToDraft(tenant_id, username, item, mail_id, out mime_message_id, out in_reply_to, mbox);

            if (result_message.Id > 0)
            {
                var user_culture = Thread.CurrentThread.CurrentCulture;
                var user_ui_culture = Thread.CurrentThread.CurrentUICulture;
                ThreadPool.QueueUserWorkItem(delegate
                {
                    Message message = null;
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = user_culture;
                        Thread.CurrentThread.CurrentUICulture = user_ui_culture;
                        item.ChangeEmbededAttachmentLinks(tenant_id, username);
                        item.ChangeSmileLinks();
                        message = item.ToMimeMessage(tenant_id, username, true);
                        message.MessageId = mime_message_id;

                        in_reply_to = in_reply_to.Trim();
                        if (!string.IsNullOrEmpty(in_reply_to))
                            message.InReplyTo = in_reply_to;

                        if (mbox.RefreshToken != null)
                        {
                            ActiveUp.Net.Mail.SmtpClient.SendSsl(message, mbox.SmtpServer, mbox.SmtpPort, mbox.SmtpAccount, GetAccessToken(mbox), SaslMechanism.OAuth2);
                        }
                        else if (mbox.OutcomingEncryptionType == EncryptionType.None)
                        {
                            if (mbox.AuthenticationTypeSmtp == SaslMechanism.None)
                                ActiveUp.Net.Mail.SmtpClient.Send(message, mbox.SmtpServer, mbox.SmtpPort);
                            else
                                ActiveUp.Net.Mail.SmtpClient.Send(message, mbox.SmtpServer, mbox.SmtpPort, mbox.SmtpAccount, mbox.SmtpPassword, mbox.AuthenticationTypeSmtp);
                        }
                        else
                        {
                            if (mbox.AuthenticationTypeSmtp == SaslMechanism.None)
                                ActiveUp.Net.Mail.SmtpClient.SendSsl(message, mbox.SmtpServer, mbox.SmtpPort, mbox.OutcomingEncryptionType);
                            else
                                ActiveUp.Net.Mail.SmtpClient.SendSsl(message, mbox.SmtpServer, mbox.SmtpPort, mbox.SmtpAccount, mbox.SmtpPassword, mbox.AuthenticationTypeSmtp, mbox.OutcomingEncryptionType);
                        }

                        //Move to_addresses sent
                        _manager.SetConversationsFolder(tenant_id, username, MailFolder.Ids.sent, new List<int> { (Int32)result_message.Id });
                        _manager.SetMessageFolderRestore(tenant_id, username, MailFolder.Ids.sent, (int)result_message.Id);

                        _manager.AddRelationshipEventForLinkedAccounts(mbox, result_message);
                    }
                    catch (Exception ex)
                    {
                        AddNotificationAlertToMailbox(tenant_id, username, item, result_message, ex, mbox, message);
                    }
                });
            }
            else
            {
                throw new ArgumentException("Failed to_addresses save draft");
            }

            return result_message.Id > 0 ? (Int32)result_message.Id : 1; // Callback in api will be raised if value > 0
        }


        //Todo: Remove useless parameters from this method
        private void AddNotificationAlertToMailbox(int tenant_id, string username, MailSendItem item,
                                                   MailMessageItem result_message, Exception ex, MailBox mbox, Message message)
        {
            try
            {
                var sb_message = new StringBuilder(1024);
                var message_delivery = new MailSendItem {Subject = MailApiResource.DeliveryFailureSubject};
                message_delivery.To.Add(item.From);
                message_delivery.From = MailBoxManager.MAIL_DAEMON_EMAIL;
                message_delivery.Important = true;
                message_delivery.StreamId = _manager.CreateNewStreamId();
                sb_message.Append(@"<style>
                                            .button.blue:hover {
                                            color: white;
                                            background: #57A7D3;
                                            background: linear-gradient(top, #78BFE8, #57A7D3 50%, #57A7D3 51%, #3F96C3);
                                            background: -o-linear-gradient(top, #78BFE8, #57A7D3 50%, #57A7D3 51%, #3F96C3);
                                            background: -moz-linear-gradient(top, #78BFE8, #57A7D3 50%, #57A7D3 51%, #3F96C3);
                                            background: -webkit-linear-gradient(top, #78BFE8, #57A7D3 50%, #57A7D3 51%, #3F96C3);
                                            border: 1px solid #5EAAD5;
                                            }
                                            .button.blue {
                                            color: white;
                                            background: #3D96C6;
                                            background: linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -o-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -moz-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -webkit-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            border-width: 1px;
                                            border-style: solid;
                                            border-color: #4DA9DC #4098C9 #2D7399 #4098C9;
                                            }
                                            .button, .button:visited, .button:hover, .button:active {
                                            display: inline-block;
                                            font-weight: normal;
                                            text-align: center;
                                            text-decoration: none;
                                            vertical-align: middle;
                                            cursor: pointer;
                                            border-radius: 3px;
                                            -moz-border-radius: 3px;
                                            -webkit-border-radius: 3px;
                                            touch-callout: none;
                                            -o-touch-callout: none;
                                            -moz-touch-callout: none;
                                            -webkit-touch-callout: none;
                                            user-select: none;
                                            -o-user-select: none;
                                            -moz-user-select: none;
                                            -webkit-user-select: none;
                                            font-size: 12px;
                                            line-height: 14px;
                                            padding: 2px 12px 3px;
                                            color: white;
                                            background: #3D96C6;
                                            background: linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -o-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -moz-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            background: -webkit-linear-gradient(top, #59B1E2, #3D96C6 50%, #3D96C6 51%, #1A76A6);
                                            border-width: 1px;
                                            border-style: solid;
                                            border-color: #4DA9DC #4098C9 #2D7399 #4098C9;
                                            }
                                            body {
                                            color: #333;
                                            font: normal 12px Arial, Tahoma,sans-serif;
                                            }
                                            </style>");

                sb_message.AppendFormat("<div style=\"max-width:500px;\"><p style=\"color:gray;\">{0}</p>",
                                        MailApiResource.DeliveryFailureAutomaticMessage);

                sb_message.AppendFormat("<p>{0}</p>",
                                        MailApiResource.DeliveryFailureMessageIdentificator
                                                       .Replace("{subject}", item.Subject)
                                                       .Replace("{date}", DateTime.Now.ToString(CultureInfo.InvariantCulture)));

                sb_message.AppendFormat("<div><p>{0}:</p><ul style=\"color:#333;\">",
                                        MailApiResource.DeliveryFailureRecipients);

                item.To.ForEach(rcpt =>
                                sb_message.AppendFormat("<li>{0}</li>", HttpUtility.HtmlEncode(rcpt)));

                item.Cc.ForEach(rcpt =>
                                sb_message.AppendFormat("<li>{0}</li>", HttpUtility.HtmlEncode(rcpt)));

                item.Bcc.ForEach(rcpt =>
                                 sb_message.AppendFormat("<li>{0}</li>", HttpUtility.HtmlEncode(rcpt)));

                sb_message.AppendFormat("</ul>");

                sb_message.AppendFormat("<p>{0}</p>",
                                        MailApiResource.DeliveryFailureRecommendations
                                                       .Replace("{account_name}", "<b>" + item.From + "</b>"));

                sb_message.AppendFormat(
                    "<a id=\"delivery_failure_button\" mailid={0} class=\"button blue\" style=\"margin-right:8px;\">",
                    result_message.Id);
                sb_message.Append(MailApiResource.DeliveryFailureBtn + "</a></div>");

                sb_message.AppendFormat("<p>{0}</p>",
                                        MailApiResource.DeliveryFailureFAQInformation
                                                       .Replace("{url_begin}",
                                                                "<a id=\"delivery_failure_faq_link\" target=\"blank\" href=\"#gmail\">")
                                                       .Replace("{url_end}", "</a>"));

                var last_dot_index = ex.Message.LastIndexOf('.');
                var smtp_response = ex.Message;

                if (last_dot_index != -1 && last_dot_index != smtp_response.Length)
                {
                    try
                    {
                        smtp_response = smtp_response.Remove(last_dot_index + 1, smtp_response.Length - last_dot_index - 1);
                    }
                    catch
                    {
                    }
                }

                sb_message.AppendFormat(
                    "<p style=\"color:gray;\">" + MailApiResource.DeliveryFailureReason + ": \"{0}\"</p></div>", smtp_response);

                message_delivery.HtmlBody = sb_message.ToString();
                // SaveToDraft To Inbox
                var notify_message_item = message_delivery.ToMailMessageItem(tenant_id, username);
                notify_message_item.MessageId = _manager.CreateMessageId();
                notify_message_item.ChainId = notify_message_item.MessageId;
                notify_message_item.IsNew = true;
                notify_message_item.IsFromCRM = false;
                notify_message_item.IsFromTL = false;

                _manager.StoreMailBody(mbox.TenantId, mbox.UserId, notify_message_item);

// ReSharper disable UnusedVariable
                var id_mail = _manager.MailSave(mbox, notify_message_item, 0, MailFolder.Ids.inbox, MailFolder.Ids.inbox,
                                                string.Empty, string.Empty, false);
// ReSharper restore UnusedVariable

                //_manager.UpdateChain(notify_message_item.ChainId, folder, mbox.MailBoxId, mbox.TenantId, mbox.UserId);

                if (message != null)
                    _manager.CreateDeliveryFailureAlert(mbox.TenantId,
                                                        mbox.UserId,
                                                        item.Subject,
                                                        item.From,
                                                        (Int32) result_message.Id);
            }
            catch
            {
                /* TODO: add log here */
            }
        }

        public MailMessageItem SaveToDraft(int tenant_id, string username, MailSendItem item, int mail_id)
        {
            string mime_message_id;
            string in_reply_to;
            return SaveToDraft(tenant_id, username, item, mail_id, out mime_message_id, out in_reply_to);
        }

        private MailMessageItem SaveToDraft(int tenant_id, string username, MailSendItem item, int mail_id,
                                     out string mime_message_id, out string in_reply_to)
        {
            var mbox = _manager.GetMailBox(tenant_id, username, new MailAddress(item.From));

            if (mbox == null)
                throw new ArgumentException("no such mailbox");

            return SaveToDraft(tenant_id, username, item, mail_id, out mime_message_id, out in_reply_to, mbox);
        }

        private MailMessageItem SaveToDraft(int tenant_id, string username, MailSendItem item, int mail_id,
                                     out string mime_message_id, out string in_reply_to, MailBox mbox)
        {
            item.DisplayName = mbox.Name;
            var embeded_attachments_for_saving = item.ChangeEmbededAttachmentLinksForStoring(tenant_id, username, mail_id, _manager);
            var message_item = item.ToMailMessageItem(tenant_id, username);
            message_item.IsNew = false;
            message_item.Folder = MailFolder.Ids.drafts;

            mime_message_id = mail_id == 0 ? _manager.CreateMessageId() : _manager.GetMimeMessageIdByMessageId(mail_id);
            in_reply_to = item.ReplyToId != 0 ? _manager.GetMimeMessageIdByMessageId(item.ReplyToId) : "";

            if (!string.IsNullOrEmpty(mime_message_id))
            {
                message_item.MessageId = mime_message_id;
            }
            if (!string.IsNullOrEmpty(in_reply_to))
            {
                message_item.InReplyTo = in_reply_to;
            }

            message_item.ChainId = _manager.DetectChainId(mbox, message_item);

            var need_to_restore_attachments_from_fck_location = mail_id == 0 && message_item.Attachments.Any();
            if (need_to_restore_attachments_from_fck_location)
            {
                message_item.Attachments.ForEach(attachment => _manager.StoreAttachmentCopy(tenant_id, username, attachment, item.StreamId));
            }

            _manager.StoreMailBody(mbox.TenantId, mbox.UserId, message_item);

            var previous_mailbox_id = mail_id == 0
                                          ? mbox.MailBoxId
                                          : _manager.GetMailInfo(tenant_id, username, mail_id, false, false).MailboxId;

            var previous_chain_id = message_item.ChainId;
            if (previous_mailbox_id != mbox.MailBoxId)
            {
                var prev_mbox = new MailBox();
                prev_mbox.TenantId = tenant_id;
                prev_mbox.UserId = username;
                prev_mbox.MailBoxId = previous_mailbox_id;
                previous_chain_id = _manager.DetectChainId(prev_mbox, message_item);
            }

            mail_id = _manager.MailSave(mbox, message_item, mail_id, message_item.Folder, message_item.Folder, string.Empty, string.Empty, false);
            message_item.Id = mail_id;

            if (previous_mailbox_id != mbox.MailBoxId)
            {
                _manager.UpdateChain(previous_chain_id, message_item.Folder, previous_mailbox_id, tenant_id, username);
            }

            if (mail_id > 0 && need_to_restore_attachments_from_fck_location)
            {
                foreach (var attachment in message_item.Attachments)
                {
                    var new_id = _manager.SaveAttachment(tenant_id, mail_id, attachment);
                    attachment.fileId = new_id;
                }
            }

            if (mail_id > 0 && embeded_attachments_for_saving.Any())
            {
                _manager.SaveAttachments(tenant_id, mail_id, embeded_attachments_for_saving);
            }

            _manager.UpdateChain(message_item.ChainId, message_item.Folder, mbox.MailBoxId, mbox.TenantId, mbox.UserId);

            return message_item;
        }
    }
}
