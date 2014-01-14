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
using System.Runtime.Serialization;

namespace ASC.Mail.Aggregator
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class MailTag
    {
        [CollectionDataContract(Namespace = "", ItemName = "Address")]
        public class AddressesList<TItem> : List<TItem>
        {
            public AddressesList()
                : base()
            {
            }
            public AddressesList(IEnumerable<TItem> items)
            : base(items)
            {
            }
        }

        [DataMember(IsRequired = true)]
        public int Id
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public string Name
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public string Style
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public AddressesList<string> Addresses
        {
            get;
            set;
        }

        [DataMember]
        public int LettersCount { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public MailTag(int id, string name, IEnumerable<string> addresses, string style, int lettersCount)
        {
            Id = id;
            Name = name;
            Addresses = new AddressesList<string>(addresses);
            Style = style;
            LettersCount = lettersCount;
        }
    }
}
