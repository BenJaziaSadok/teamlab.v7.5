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

namespace ASC.Api.Utils
{
    public class PathUtils
    {
        private static readonly Regex _domainReplaceRegex = new Regex(@".+\|(?'domainData'[a-zA-Z])\|.+");

        public static string GetPath(string path)
        {
            return _domainReplaceRegex.Replace(path, new MatchEvaluator(EvalAppDomainData));
        }

        private static string EvalAppDomainData(Match match)
        {
            return match.Value;
        }
    }
}