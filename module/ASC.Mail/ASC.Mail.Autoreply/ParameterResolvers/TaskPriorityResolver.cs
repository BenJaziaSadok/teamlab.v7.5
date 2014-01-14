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
    internal class TaskPriorityResolver : IParameterResolver
    {
        public static readonly Regex Pattern = new Regex(@"!(0|1)", RegexOptions.Compiled);

        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            if (!Pattern.IsMatch(mailMessage.Subject))
                return null;

            return Convert.ToInt32(Pattern.Match(mailMessage.Subject).Value.TrimStart('!'));
        }
    }
}
