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

namespace ASC.Mail.Aggregator.Collection
{
    [DataContract(Name = "list", Namespace = "")]
    public class TypedList<T>
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public ItemList<T> Items { get; set; }

        public TypedList(IEnumerable<T> items)
        {
            Items = new ItemList<T>(items);
        }
    }
}