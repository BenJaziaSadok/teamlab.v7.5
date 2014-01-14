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
using ASC.Mail.Service.DAO;

namespace ASC.Mail.Service
{
    [DataContract(Name = "Hash", Namespace = "")]
    public class ContactList
    {
        [DataMember]
        public string Type;

        [DataMember]
        public ItemDictionary<long, MailContactItem> Items { get; set; }

        public ContactList(IDictionary<long, MailContactItem> items)
        {
            Items = new ItemDictionary<long, MailContactItem>(items);
        }
    }
}