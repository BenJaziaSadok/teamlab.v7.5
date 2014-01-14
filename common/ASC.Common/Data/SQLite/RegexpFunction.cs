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
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace ASC.Common.Data.SQLite
{
    [SQLiteFunction(Name = "REGEXP", Arguments = 2, FuncType = FunctionType.Scalar)]
    public class RegexpFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            return Regex.IsMatch(Convert.ToString(args[1]), fixRegExp(Convert.ToString(args[0])), MainOptions);
        }

        private static RegexOptions MainOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase |
                                                  RegexOptions.CultureInvariant;

        private static readonly Regex rxPartOfWord = new Regex(@"\[\[:(<|>):\]\]", MainOptions);
        private static readonly Regex rxclassPatterns = new Regex(@"\[(.*):(\w+):\]", MainOptions);

        private string fixRegExp(string inputStr)
        {
            string result = rxPartOfWord.Replace(inputStr, rxPartOfWordMatchEvaluator);
            result = rxclassPatterns.Replace(result, rxclassPatternsMatchEvaluator);
            return result;
        }

        private string rxPartOfWordMatchEvaluator(Match match)
        {
            string result = match.Value;
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                if (match.Groups[1].Value.Equals("<"))
                {
                    result = @"^|\W+\w*";
                }
                else
                {
                    result = @"\w*\W+|$";
                }
            }
            return result;
        }

        private string rxclassPatternsMatchEvaluator(Match match)
        {
            string prefix = (match.Groups[1].Value ?? string.Empty).Trim();
            string word = (match.Groups[2].Value ?? string.Empty).Trim();
            string result;
            switch (word)
            {
                case "alpha":
                    result = @"A-Za-z";
                    break;
                case "upper":
                    result = @"A-Z";
                    break;
                case "lower":
                    result = @"a-z";
                    break;
                case "digit":
                    result = @"0-9";
                    break;
                case "alnum":
                    result = @"A-Za-z0-9";
                    break;
                case "xdigit":
                    result = @"0-9A-Fa-f";
                    break;
                case "space":
                    result = @"\s";
                    break;
                case "print":
                    result = @"\S";
                    break;
                case "punct":
                    result = @"!'#$%&`()\*\+,-\./:;<=>\?@\[\\\]^_{\|}~";
                    break;
                default:
                    return match.Value;
            }
            result = string.Format("[{0}{1}]", prefix, result);
            return result;
        }
    }
}