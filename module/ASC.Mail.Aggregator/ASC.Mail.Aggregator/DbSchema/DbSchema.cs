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

namespace ASC.Mail.Aggregator.DbSchema {
    public static class AttachmentTable
    {
        public const string name = "mail_attachment";

        public static class Columns
        {
            public const string id = "id";
            public const string id_mail = "id_mail";
            public const string name = "name";
            public const string stored_name = "stored_name";
            public const string type = "type";
            public const string size = "size";
            public const string need_remove = "need_remove";
            public const string file_number = "file_number";
            public const string content_id = "content_id";
            public const string id_tenant = "tenant";
        }
    }


    public static class Garbage
    {
        public const string table = "mail_garbage";

        public static class Columns
        {
            public const string id = "id";
            public const string tenant = "tenant";
            public const string path = "path";
            public const string is_processed = "is_processed";
            public const string time_modified = "time_modified";
        };
    }


    public static class MailTable
    {
        public const string name = "mail_mail";

        public static class Columns
        {
            public const string id = "id";
            public const string id_mailbox = "id_mailbox";
            public const string id_user = "id_user";
            public const string id_tenant = "tenant";
            public const string address = "address";
            public const string uidl = "uidl";
            public const string md5 = "md5";
            public const string from = "from_text";
            public const string to = "to_text";
            public const string reply = "reply_to";
            public const string cc = "cc";
            public const string bcc = "bcc";
            public const string subject = "subject";
            public const string introduction = "introduction";
            public const string importance = "importance";
            public const string date_received = "date_received";
            public const string date_sent = "date_sent";
            public const string size = "size";
            public const string attach_count = "attachments_count";
            public const string unread = "unread";
            public const string is_answered = "is_answered";
            public const string is_forwarded = "is_forwarded";
            public const string is_from_crm = "is_from_crm";
            public const string is_from_tl = "is_from_tl";
            public const string stream = "stream";
            public const string folder = "folder";
            public const string folder_restore = "folder_restore";
            public const string spam = "spam";
            public const string is_removed = "is_removed";
            public const string time_modified = "time_modified";
            public const string mime_message_id = "mime_message_id";
            public const string mime_in_reply_to = "mime_in_reply_to";
            public const string chain_id = "chain_id";
            public const string chain_date = "chain_date";
            public const string is_text_body_only = "is_text_body_only";
        };
    }


    public static class ImapFlags
    {
        public const string table = "mail_imap_flags";

        public static class Columns
        {
            public const string name = "name";
            public const string folder_id = "folder_id";
            public const string skip = "skip";
        };
    }


    public static class ImapSpecialMailbox
    {
        public const string table = "mail_imap_special_mailbox";

        public static class Columns
        {
            public const string name = "name";
            public const string server = "server";
            public const string folder_id = "folder_id";
            public const string skip = "skip";
        };
    }


    public static class PopUnorderedDomain
    {
        public const string table = "mail_pop_unordered_domain";

        public static class Columns
        {
            public const string server = "server";
        };
    }


    public static class ChainTable
    {
        public const string name = "mail_chain";

        public static class Columns
        {
            public const string id = "id";
            public const string id_mailbox = "id_mailbox";
            public const string id_tenant = "tenant";
            public const string id_user = "id_user";
            public const string folder = "folder";
            public const string length = "length";
            public const string unread = "unread";
            public const string has_attachments = "has_attachments";
            public const string importance = "importance";
            public const string tags = "tags";
        };
    }

    public class CrmContactEntity
    {
        public int Id { get; set;}
        public ChainXCrmContactEntity.EntityTypes Type { get; set;}
    }

    public static class ChainXCrmContactEntity
    {
        public const string name = "mail_chain_x_crm_entity";

        public static class Columns
        {
            public const string id_tenant = "id_tenant";
            public const string id_mailbox = "id_mailbox";
            public const string id_chain = "id_chain";
            public const string entity_id = "entity_id";
            public const string entity_type = "entity_type";
        }

        public enum EntityTypes
        {
            Contact = 1,
            Case = 2,
            Opportunity = 3
        }

        public static class CrmEntityTypeNames
        {
            public const string contact = "contact";
            public const string Case = "case";
            public const string opportunity = "opportunity";
        }

        public static string StringName(this EntityTypes type)
        {
            switch (type)
            {
                case EntityTypes.Contact:
                    return CrmEntityTypeNames.contact;
                case EntityTypes.Case:
                    return CrmEntityTypeNames.Case;
                case EntityTypes.Opportunity:
                    return CrmEntityTypeNames.opportunity;
                default:
                    throw new ArgumentException(String.Format("Invalid CrmEntityType: {0}", type), "type");
            }
        }
    }
}