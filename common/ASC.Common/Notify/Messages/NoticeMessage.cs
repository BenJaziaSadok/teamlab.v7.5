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
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Notify.Messages
{
    [Serializable]
    public class NoticeMessage : INoticeMessage
    {
        [NonSerialized]
        private readonly List<ITagValue> arguments = new List<ITagValue>();

        [NonSerialized]
        private IPattern pattern;

        public NoticeMessage()
        {
        }

        public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            Recipient = recipient;
            Action = action;
            ObjectID = objectID;
        }

        public NoticeMessage(IDirectRecipient recipient, INotifyAction action, string objectID, IPattern pattern)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (pattern == null) throw new ArgumentNullException("pattern");
            Recipient = recipient;
            Action = action;
            Pattern = pattern;
            ObjectID = objectID;
            ContentType = pattern.ContentType;
        }

        public NoticeMessage(IDirectRecipient recipient, string subject, string body, string contentType)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (body == null) throw new ArgumentNullException("body");
            Recipient = recipient;
            Subject = subject;
            Body = body;
            ContentType = contentType;
        }

        public string ObjectID { get; private set; }

        public IDirectRecipient Recipient { get; private set; }

        public IPattern Pattern
        {
            get { return pattern; }
            internal set { pattern = value; }
        }

        public INotifyAction Action { get; private set; }

        public ITagValue[] Arguments
        {
            get { return arguments.ToArray(); }
        }

        public void AddArgument(params ITagValue[] tagValues)
        {
            if (tagValues == null) throw new ArgumentNullException("tagValues");
            Array.ForEach(tagValues,
                tagValue => 
                {   
                    if (!arguments.Exists(tv => Equals(tv.Tag, tagValue.Tag)))
                    {
                        arguments.Add(tagValue);
                    }
                });
        }

        public ITagValue GetArgument(string tag)
        {
            return arguments.Find(r => r.Tag == tag);
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public string ContentType { get; internal set; }
    }
}