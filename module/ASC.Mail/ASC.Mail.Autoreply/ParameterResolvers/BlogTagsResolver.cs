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
using System.Linq;
using System.Text.RegularExpressions;
using ASC.Mail.Net.Mail;

namespace ASC.Mail.Autoreply.ParameterResolvers
{
    internal class BlogTagsResolver : IParameterResolver
    {
        public static readonly Regex Pattern = new Regex(@"#[\w,]+", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            if (!Pattern.IsMatch(mailMessage.Subject))
                return null;

            var tags = Pattern.Matches(mailMessage.Subject).Cast<Match>()
                              .SelectMany(x => x.Value.TrimStart('#').Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                              .ToArray();

            return string.Join(",", tags);
        }
    }
}
