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
using System.Text.RegularExpressions;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

namespace ASC.Notify.Textile
{
    public class PushStyler : IPatternStyler
    {
        private static readonly Regex VelocityArgumentsRegex = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?'arg'.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public void ApplyFormating(NoticeMessage message)
        {
            if (!string.IsNullOrEmpty(message.Subject))
            {
                message.Subject = VelocityArgumentsRegex.Replace(message.Subject, m => m.Groups["arg"].Value);
                message.Subject = message.Subject.Replace(Environment.NewLine, " ").Trim();
            }
            if (!string.IsNullOrEmpty(message.Body))
            {
                message.Body = VelocityArgumentsRegex.Replace(message.Body, m => m.Groups["arg"].Value);
                message.Body = message.Body.Replace(Environment.NewLine, " ").Trim();
            }
        }
    }
}
