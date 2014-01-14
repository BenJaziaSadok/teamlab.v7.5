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

namespace ASC.Notify.Patterns
{
    public class Pattern : IPattern
    {
        public const string HTMLContentType = "html";

        public const string TextContentType = "text";

        public const string RtfContentType = "rtf";


        public string ID { get; private set; }

        public string Subject { get; private set; }

        public string Body { get; private set; }

        public string ContentType { get; internal set; }

        public string Styler { get; internal set; }

        
        public Pattern(string id, string subject, string body, string contentType)
        {
            if (String.IsNullOrEmpty(id)) throw new ArgumentException("id");
            if (subject == null) throw new ArgumentNullException("subject");
            if (body == null) throw new ArgumentNullException("body");
            ID = id;
            Subject = subject;
            Body = body;
            ContentType = string.IsNullOrEmpty(contentType) ? HTMLContentType : contentType;
        }


        public override bool Equals(object obj)
        {
            var p = obj as IPattern;
            return p != null && p.ID == ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public override string ToString()
        {
            return ID;
        }
    }
}