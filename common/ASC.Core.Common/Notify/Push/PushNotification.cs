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
using System.Runtime.Serialization;

namespace ASC.Core.Common.Notify.Push
{
    [DataContract]
    public class PushNotification
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ShortMessage { get; set; }

        [DataMember]
        public int? Badge { get; set; } 

        [DataMember]
        public PushModule Module { get; set; }

        [DataMember]
        public PushAction Action { get; set; }

        [DataMember]
        public PushItem Item { get; set; }

        [DataMember]
        public PushItem ParentItem { get; set; }

        [DataMember]
        public DateTime QueuedOn { get; set; }
        
        public static PushNotification ApiNotification(string message, int? badge)
        {
            return new PushNotification {Message = message, Badge = badge};
        }
    }

    [DataContract]
    public class PushItem
    {
        [DataMember]
        public PushItemType Type { get; set; }

        [DataMember]
        public string ID { get; set; }

        [DataMember]
        public string Description { get; set; }

        public PushItem()
        {
            
        }

        public PushItem(PushItemType type, string id, string description)
        {
            Type = type;
            ID = id;
            Description = description;
        }
    }
}
