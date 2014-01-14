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
using ASC.Mail.Net.Mail;

namespace ASC.Mail.Autoreply.ParameterResolvers
{
    internal class TaskDeadlineResolver : IParameterResolver
    {
        public static readonly Regex Pattern = new Regex(@"\^(?'year'\d{4})-(?'month'\d{1,2})-(?'day'\d{1,2})", RegexOptions.Compiled);

        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            if (!Pattern.IsMatch(mailMessage.Subject))
                return null;

            var match = Pattern.Match(mailMessage.Subject);
            return new DateTime(Convert.ToInt32(match.Groups["year"].Value), Convert.ToInt32(match.Groups["month"].Value), Convert.ToInt32(match.Groups["day"].Value));
        }
    }
}
