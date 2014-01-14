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
using NLog;
using System.Collections.Generic;

namespace ASC.Mail.Aggregator
{
    // Looks like this class is only  for logging requests from MailQueueItem to MailBoxManager
    public class MailItemManager
    {
        private MailBoxManager mailBoxManager;
        private Logger _log = null;

        public MailItemManager(MailBoxManager mailBoxManager)
        {
            if (mailBoxManager == null) throw new ArgumentNullException("mailBoxManager");
            this.mailBoxManager = mailBoxManager;
            _log = LogManager.GetLogger("MailItemManager");
        }

        public void GetStoredMessagesUIDL_MD5(MailQueueItem account, Dictionary<int, string> uidl_list, Dictionary<int, string> md5_list)
        {
            mailBoxManager.GetStoredMessagesUIDL_MD5(account.Account.MailBoxId, uidl_list, md5_list);
        }

        public DateTime GetFolderModifyDate(MailQueueItem account, int folder_id)
        {
            return mailBoxManager.GetMailboxFolderModifyDate(account.Account.MailBoxId, folder_id);
        }

        public void OnRetrieveNewMessage(MailQueueItem account,
            ActiveUp.Net.Mail.Message message,
            int folder_id,
            string uidl,
            string md5_hash,
            bool unread,
            int[] tags_ids)
        {
            if (mailBoxManager.MailReceive(account.Account, message, folder_id, uidl, md5_hash, unread, tags_ids) < 1)
                throw new Exception("MailReceive() returned id < 1;");
        }

        public void OnUpdateUidl(MailBox account, int message_id, string new_uidl)
        {
            mailBoxManager.UpdateMessageUidl(account.TenantId, account.UserId, message_id, new_uidl);
        }

        private void SetDelayExpires(MailBox mailbox) {
            var expires = DateTime.UtcNow + TimeSpan.FromSeconds(mailbox.ServerLoginDelay);
            mailBoxManager.SetEmailLoginDelayExpires(mailbox.EMail.ToString(), expires);
        }

        public void OnAuthSucceed(MailBox mailbox)
        {
            SetDelayExpires(mailbox);
            mailBoxManager.SetAuthError(mailbox, false);
        }

        public void OnAuthFailed(MailBox mailbox, string response_line)
        {
            SetDelayExpires(mailbox);
            mailBoxManager.SetAuthError(mailbox, true);
        }

        public void OnDone(MailQueueItem item, bool quota_error)
        {
            mailBoxManager.SetMailboxQuotaError(item.Account, quota_error);
        }

        public int[] OnGetOrCreateTags(string[] tags_names, int tenant, string user)
        {
            return mailBoxManager.GetOrCreateTags(tenant, user, tags_names);
        }

        public void OnUpdateMessagesTags(int tenant, string user_id, int[] tag_ids, int[] message_ids)
        {
            foreach (var tag_id in tag_ids)
                mailBoxManager.SetMessagesTag(tenant, user_id, tag_id, message_ids);
        }

        public void OnCheckedTimeUpdate(int id_mailbox, Int64 utc_ticks_time)
        {
            mailBoxManager.LockMailbox(id_mailbox, utc_ticks_time);
        }
    }
}