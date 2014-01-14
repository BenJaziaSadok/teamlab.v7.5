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
using ASC.Common.Utils;
using ASC.Notify.Messages;
using NVelocity;
using NVelocity.App.Events;

namespace ASC.Notify.Patterns
{
    public sealed class NVelocityPatternFormatter : PatternFormatter
    {
        public const string DefaultPattern = @"(^|[^\\])\$[\{]{0,1}(?<tagName>[a-zA-Z0-9_]+)";
        public const string NoStylePreffix = "==";
        public const string NoStyleSuffix = "==";

        private VelocityContext _nvelocityContext;

        public NVelocityPatternFormatter()
            : base(DefaultPattern)
        {
        }

        protected override void BeforeFormat(INoticeMessage message, ITagValue[] tagsValues)
        {
            _nvelocityContext = new VelocityContext();
            _nvelocityContext.AttachEventCartridge(new EventCartridge());
            _nvelocityContext.EventCartridge.ReferenceInsertion += EventCartridgeReferenceInsertion;
            foreach (var tagValue in tagsValues)
            {
                _nvelocityContext.Put(tagValue.Tag, tagValue.Value);
            }
            base.BeforeFormat(message, tagsValues);
        }

        protected override string FormatText(string text, ITagValue[] tagsValues)
        {
            if (String.IsNullOrEmpty(text)) return text;
            return VelocityFormatter.FormatText(text, _nvelocityContext);
        }

        protected override void AfterFormat(INoticeMessage message)
        {
            _nvelocityContext = null;
            base.AfterFormat(message);
        }

        private static void EventCartridgeReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
        {
            var originalString = e.OriginalValue as string;
            if (originalString == null) return;
            var lines = originalString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0) return;
            e.NewValue = string.Empty;
            for (var i = 0; i < lines.Length - 1; i++)
            {
                e.NewValue += string.Format("{0}{1}{2}\n", NoStylePreffix, lines[i], NoStyleSuffix);
            }
            e.NewValue += string.Format("{0}{1}{2}", NoStylePreffix, lines[lines.Length - 1], NoStyleSuffix);
        }
    }
}