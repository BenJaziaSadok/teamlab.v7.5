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

namespace ASC.Mail.Aggregator
{
    public static class MailItemQueueFactory
    {
        public static MailQueueItem CreateItemForAccount(MailBox account, MailItemManager manager)
        {
            var item = new MailQueueItem(account);
            item.GetStoredMessagesUidlMd5 += manager.GetStoredMessagesUIDL_MD5;
            item.OnRetrieveNewMessage += manager.OnRetrieveNewMessage;
            item.OnUpdateUidl += manager.OnUpdateUidl;
            item.OnAuthSucceed += manager.OnAuthSucceed;
            item.OnAuthFailed += manager.OnAuthFailed;
            item.OnDone += manager.OnDone;
            item.OnTimeCheckedUpdate += manager.OnCheckedTimeUpdate;
            item.OnGetOrCreateTags += manager.OnGetOrCreateTags;
            item.OnUpdateMessagesTags += manager.OnUpdateMessagesTags;
            return item;
        }
    }
}