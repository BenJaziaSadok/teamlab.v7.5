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
using ASC.Forum;
using ASC.Specific;

namespace ASC.Api.Forums
{
    [DataContract(Name = "post", Namespace = "")]
    public class ForumTopicPostAttachmentWrapper : IApiSortableDate
    {
        [DataMember(Order = 3)]
        public ApiDateTime Created { get; set; }

        [DataMember(Order = 3)]
        public ApiDateTime Updated { get; set; }

        [DataMember(Order = 2)]
        public string Name { get; set; }

        [DataMember(Order = 2)]
        public string ContentType { get; set; }

        [DataMember(Order = 3)]
        public int Size { get; set; }

        [DataMember(Order = 5)]
        public string Path { get; set; }

        public ForumTopicPostAttachmentWrapper(Attachment attachment)
        {
            ContentType = attachment.ContentType.ToString();
            Updated = Created = (ApiDateTime)attachment.CreateDate;

            Name = attachment.Name;
            Size = attachment.Size;
            Path = attachment.OffsetPhysicalPath;//TODO: add through datastorage
        }

        private ForumTopicPostAttachmentWrapper()
        {
        }

        public static ForumTopicPostAttachmentWrapper GetSample()
        {
            return new ForumTopicPostAttachmentWrapper()
                       {
                           ContentType = "image/jpeg",
                           Created = (ApiDateTime)DateTime.Now,
                           Updated = (ApiDateTime)DateTime.Now,
                           Name = "picture.jpg",
                           Path = "url to file",
                           Size = 122345
                       };
        }
    }
}