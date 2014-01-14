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

namespace ASC.Mail.Autoreply.Utility
{
    public static class StringDistance
    {
        public static int LevenshteinDistance(string s, string t)
        {
            return LevenshteinDistance(s, t, true);
        }

        public static int LevenshteinDistance(string s, string t, bool ignoreCase)
        {
            if (String.IsNullOrEmpty(s))
            {
                return String.IsNullOrEmpty(t) ? 0 : t.Length;
            }
            if (String.IsNullOrEmpty(t))
            {
                return s.Length;
            }

            if (ignoreCase)
            {
                s = s.ToLowerInvariant();
                t = t.ToLowerInvariant();
            }
            
            int n = s.Length;
            int m = t.Length;

            var d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[n, m];
        }
    }
}
