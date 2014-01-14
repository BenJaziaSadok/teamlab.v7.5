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
using System.Diagnostics;

namespace ASC.Notify.Patterns
{
    [DebuggerDisplay("{Tag}: {Value}")]
    public class TagValue : ITagValue
    {
        public string Tag
        {
            get;
            private set;
        }

        public object Value
        {
            get;
            private set;
        }


        public TagValue(string tag, object value)
        {
            if (string.IsNullOrEmpty(tag)) throw new ArgumentNullException("tag");

            Tag = tag;
            Value = value;
        }
    }

    public class AdditionalSenderTag : TagValue
    {
        public AdditionalSenderTag(string senderName)
            : base("__AdditionalSender", senderName)
        {
        }
    }
}