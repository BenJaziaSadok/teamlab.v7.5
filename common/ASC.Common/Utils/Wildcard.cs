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

namespace ASC.Common.Utils
{
    public static class Wildcard
    {
        public static bool WildcardMatch(this string input, string pattern)
        {
            return WildcardMatch(input, pattern, true);
        }

        public static bool WildcardMatch(this string input, string pattern, bool ignoreCase)
        {
            if (!string.IsNullOrEmpty(input))
            {
                return IsMatch(pattern, input, ignoreCase);
            }
            return false;
        }

        public static bool IsMatch(string pattern, string input)
        {
            return IsMatch(pattern, input, true);
        }

        public static bool IsMatch(string pattern, string input, bool ignoreCase)
        {
            int offsetInput = 0;
            bool isAsterix = false;
            int i;
            while (true)
            {
                for (i = 0; i < pattern.Length;)
                {
                    switch (pattern[i])
                    {
                        case '?':
                            isAsterix = false;
                            offsetInput++;
                            break;
                        case '*':
                            isAsterix = true;
                            while (i < pattern.Length &&
                                   pattern[i] == '*')
                            {
                                i++;
                            }
                            if (i >= pattern.Length)
                                return true;
                            continue;
                        default:
                            if (offsetInput >= input.Length)
                                return false;
                            if ((ignoreCase
                                     ? char.ToLower(input[offsetInput])
                                     : input[offsetInput])
                                !=
                                (ignoreCase
                                     ? char.ToLower(pattern[i])
                                     : pattern[i]))
                            {
                                if (!isAsterix)
                                    return false;
                                offsetInput++;
                                continue;
                            }
                            offsetInput++;
                            break;
                    }
                    i++;
                }

                if (i > input.Length)
                    return false;

                while (i < pattern.Length && pattern[i] == '*')
                    ++i;

                return (offsetInput == input.Length);
            }
        }
    }
}