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
using System.Web;
using ASC.Common.Notify.Patterns;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

namespace ASC.Notify.Textile
{
    public class JabberStyler : IPatternStyler
    {
        static readonly Regex VelocityArguments = new Regex(NVelocityPatternFormatter.NoStylePreffix + "(?<arg>.*?)" + NVelocityPatternFormatter.NoStyleSuffix, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex LinkReplacer = new Regex(@"""(?<text>[\w\W]+?)"":""(?<link>[^""]+)""", RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex TextileReplacer = new Regex(@"(h1\.|h2\.|\*|h3\.|\^)", RegexOptions.Singleline | RegexOptions.Compiled);
        static readonly Regex BrReplacer = new Regex(@"<br\s*\/*>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);
        static readonly Regex TagReplacer = new Regex(@"<(.|\n)*?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.Singleline);

        public void ApplyFormating(NoticeMessage message)
        {
            var body = string.Empty;
            if (!string.IsNullOrEmpty(message.Subject))
            {
                body += VelocityArguments.Replace(message.Subject, ArgMatchReplace) + Environment.NewLine;
                message.Subject = string.Empty;
            }
            if (string.IsNullOrEmpty(message.Body)) return;
            var lines = message.Body.Split(new[] {Environment.NewLine, "\n"}, StringSplitOptions.None);
            for (var i = 0; i < lines.Length - 1; i++)
            {
                if (string.IsNullOrEmpty(lines[i])) { body += Environment.NewLine; continue; }
                lines[i] = VelocityArguments.Replace(lines[i], ArgMatchReplace);
                body += LinkReplacer.Replace(lines[i], EvalLink) + Environment.NewLine;
            }
            lines[lines.Length - 1] = VelocityArguments.Replace(lines[lines.Length - 1], ArgMatchReplace);
            body += LinkReplacer.Replace(lines[lines.Length - 1], EvalLink);
            body = TextileReplacer.Replace(HttpUtility.HtmlDecode(body), ""); //Kill textile markup
            body = BrReplacer.Replace(body, Environment.NewLine);
            body = TagReplacer.Replace(body, Environment.NewLine);
            message.Body = body;
        }

        private static string EvalLink(Match match)
        {
            if (match.Success)
            {
                if (match.Groups["text"].Success && match.Groups["link"].Success)
                {
                    if (match.Groups["text"].Value.Equals(match.Groups["link"].Value,StringComparison.OrdinalIgnoreCase))
                    {
                        return " " + match.Groups["text"].Value + " ";
                    }
                    return match.Groups["text"].Value + string.Format(" ( {0} )", match.Groups["link"].Value);
                }
            }
            return match.Value;
        }

        private static string ArgMatchReplace(Match match)
        {
            return match.Result("${arg}");
        }
    }
}
