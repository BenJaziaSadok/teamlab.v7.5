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

using System.Collections.Generic;
using System.Runtime.Serialization;
using ASC.Mail.Aggregator.Collection;
using ASC.Mail.Aggregator.Filter;
using ASC.Mail.Service.DAO;
using ASC.Mail.Aggregator;

namespace ASC.Mail.Service
{
    [DataContract(Name = "CommonHash", Namespace = "")]
    public class CommonHash
    {
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int FolderId { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public int Page { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string PrecisedTimeAll { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string PrecisedTimeFolder { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ItemList<MailMessageItem> Messages { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public long? TotalMessagesFiltered { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<MailFolder> Folders { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ItemList<MailTag> Tags { get; set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public List<MailAccount> Accounts { get; set; }

        public CommonHash(ItemList<MailMessageItem> messages,
            List<MailFolder> folders, ItemList<MailTag> tags, List<MailAccount> accounts,
            int folder_id, int page, long? total_messages,
            string precised_time_all, string precised_time_folder)
        {
            PrecisedTimeAll = precised_time_all;
            PrecisedTimeFolder = precised_time_folder;
            Messages = messages;
            Folders = folders;
            Tags = tags;
            FolderId = folder_id;
            Page = page;
            TotalMessagesFiltered = total_messages;
            Accounts = accounts;
        }

        private CommonHash()
        {
        }

        public static CommonHash FoldersResponse(List<MailFolder> folders, string precised_time_all)
        {
            return new CommonHash() { Folders = folders, PrecisedTimeAll = precised_time_all };
        }

        public static CommonHash TagsResponse(ItemList<MailTag> tags)
        {
            return new CommonHash() { Tags = tags };
        }

        public static CommonHash MessagesResponse(ItemList<MailMessageItem> messages, long total_messages, int page, string precised_time_folder)
        {
            return new CommonHash() { 
                Messages = messages, 
                TotalMessagesFiltered = total_messages, 
                Page = page,
                PrecisedTimeFolder = precised_time_folder };
        }

    }
}
