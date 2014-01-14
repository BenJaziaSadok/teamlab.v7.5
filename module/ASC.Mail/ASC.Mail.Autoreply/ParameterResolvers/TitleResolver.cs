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

using System.Text.RegularExpressions;
using ASC.Mail.Net.Mail;

namespace ASC.Mail.Autoreply.ParameterResolvers
{
    internal class TitleResolver : IParameterResolver
    {
        private readonly Regex[] _ignorePatterns;

        public TitleResolver(params Regex[] ignorePatterns)
        {
            _ignorePatterns = ignorePatterns;
        }

        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            var subject = mailMessage.Subject;

            foreach (var pattern in _ignorePatterns)
            {
                subject = pattern.Replace(subject, "");
            }

            return Regex.Replace(subject, @"\s+", " ").Trim(' ');
        }
    }
}
