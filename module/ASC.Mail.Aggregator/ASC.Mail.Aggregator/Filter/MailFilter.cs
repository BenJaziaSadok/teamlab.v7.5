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
using System.Linq;
using System.Runtime.Serialization;
using ASC.Mail.Aggregator.Collection;

namespace ASC.Mail.Aggregator.Filter
{
    [DataContract(Namespace = "")]
    public class MailFilter
    {
        public int Id { get; set; }
        public string FilterName { get; set; }
        public int TennantId { get; set; }
        public string UserName { get; set; }

        [DataMember(Name = "PrimaryFolder")]
        public int PrimaryFolder { get; set; }

        [DataMember(Name = "Unread")]
        public bool? Unread { get; set; }

        [DataMember(Name = "Attachments")]
        public bool Attachments { get; set; }

        [DataMember(Name = "Period_from")]
        public long Period_from { get; set; }

        [DataMember(Name = "Period_to")]
        public long Period_to { get; set; }

        [DataMember(Name = "Important")]
        public bool Important { get; set; }

        [DataMember(Name = "FindAddress")]
        public string FindAddress { get; set; }

        [DataMember(Name = "MailboxId")]
        public int? MailboxId { get; set; }

        [DataMember(Name = "CustomLabels")]
        public ItemList<int> CustomLabels { get; set; }

        [DataMember(Name = "Sort")]
        public string Sort { get; set; }

        [DataMember(Name = "SortOrder")]
        public string SortOrder { get; set; }

        [DataMember(Name = "SearchFilter")]
        public string SearchFilter { get; set; }

        [DataMember(Name = "Page")]
        public int Page { get; set; }

        [DataMember(Name = "PageSize")]
        public int PageSize { get; set; }

        public int? SetLabel { get; set; }

        public MailFilter()
        {
            CustomLabels = new ItemList<int>();
        }
    }
}