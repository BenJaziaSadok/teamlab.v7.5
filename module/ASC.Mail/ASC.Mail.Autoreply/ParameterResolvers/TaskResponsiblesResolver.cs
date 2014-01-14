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
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ASC.Core;
using ASC.Core.Users;
using ASC.Mail.Autoreply.Utility;
using ASC.Mail.Net.Mail;

namespace ASC.Mail.Autoreply.ParameterResolvers
{
    internal class TaskResponsiblesResolver : IParameterResolver
    {
        public static readonly Regex Pattern = new Regex(@"@\w+\s+\w+", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            if (!Pattern.IsMatch(mailMessage.Subject))
                return null;

            var users = new List<object>();
            foreach (Match match in Pattern.Matches(mailMessage.Subject))
            {
                var words = match.Value.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);
                Guid user;
                if (TryGetUser(words[0], words[1], out user))
                {
                    users.Add(user);
                }
            }

            return users;
        }

        private static bool TryGetUser(string word1, string word2, out Guid userId)
        {
            userId = Guid.Empty;

            var users = CoreContext.UserManager.GetUsers()
                                  .GroupBy(u => GetDistance(u, word1, word2))
                                  .OrderBy(g => g.Key)
                                  .FirstOrDefault(g => g.Key < 3);

            if (users != null && users.Count() == 1)
            {
                userId = users.First().ID;
                return true;
            }

            return false;
        }

        private static int GetDistance(UserInfo user, string word1, string word2)
        {
            var distance1 = StringDistance.LevenshteinDistance(user.FirstName, word1) + StringDistance.LevenshteinDistance(user.LastName, word2);
            var distance2 = StringDistance.LevenshteinDistance(user.FirstName, word2) + StringDistance.LevenshteinDistance(user.LastName, word1);
            return Math.Min(distance1, distance2);
        }
    }

    internal class TaskResponsibleResolver : IParameterResolver
    {
        public object ResolveParameterValue(Mail_Message mailMessage)
        {
            var responsibles = new TaskResponsiblesResolver().ResolveParameterValue(mailMessage) as List<object>;

            return responsibles != null ? responsibles.FirstOrDefault() : null;
        }
    }
}
