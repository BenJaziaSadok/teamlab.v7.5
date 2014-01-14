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
    public sealed class ReplacePatternFormatter : PatternFormatter
    {
        public const string DefaultPattern = @"[[]%(?<tagName>[a-zA-Z0-9_\-.]+)%[]]";

        public ReplacePatternFormatter()
            : base(DefaultPattern)
        {
        }

        internal ReplacePatternFormatter(string tagPattern, bool formatMessage)
            : base(tagPattern, formatMessage)
        {
        }

        protected override string FormatText(string text, ITagValue[] tagsValues)
        {
            if (String.IsNullOrEmpty(text)) return text;

            var formattedText = RegEx.Replace(text,
                match =>
                {
                    var value = Array.Find(tagsValues, v => v.Tag == match.Groups["tagName"].Value);
                    return value != null && value.Value != null ? Convert.ToString(value.Value) : match.Value;
                });
            return formattedText;
        }
    }
}
