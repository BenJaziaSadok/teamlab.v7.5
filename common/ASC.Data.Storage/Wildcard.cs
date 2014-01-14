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

namespace ASC.Data.Storage
{
    public class Wildcard
    {
        public static bool IsMatch(string pattern, string input)
        {
            return IsMatch(pattern, input, false);
        }

        public static bool IsMatch(string pattern, string input, bool caseInsensitive)
        {
            int offsetInput = 0;
            bool isAsterix = false;

            while (true)
            {
                int i;
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

                            if ((caseInsensitive
                                     ? char.ToLower(input[offsetInput])
                                     : input[offsetInput])
                                !=
                                (caseInsensitive
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
                    } // end switch
                    i++;
                } // end for

                // have we finished parsing our input?
                if (i > input.Length)
                    return false;

                // do we have any lingering asterixes we need to skip?
                while (i < pattern.Length && pattern[i] == '*')
                    ++i;

                // final evaluation. The index should be pointing at the
                // end of the string.
                return (offsetInput == input.Length);
            }
        }
    }
}