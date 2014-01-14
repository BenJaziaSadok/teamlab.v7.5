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

using System.Collections.Generic;

namespace ASC.Web.Studio.Utility.BBCodeParser
{
    public class TagConfiguration
    {
        public List<TagParamOption> ParamOptions { get; set; }
        public bool IsSingleTag { get; private set; }
        public string Tag { get; set; }
        public string Replacement { get; set; }
        public string AlternativeReplacement { get; set; }
        public bool IsParseContent { get; set; }
        public bool IsParseTextReplacement { get; set; }
        public bool IsParseTextReqularExpressions { get; set; }

        public TagConfiguration(string tag, string replacement) : this(tag, replacement, null, true)
        {
        }

        public TagConfiguration(string tag, string replacement, string alternativeReplacement) : this(tag, replacement, alternativeReplacement, true)
        {
        }

        public TagConfiguration(string tag, string replacement, bool isParseContent) : this(tag, replacement, null, isParseContent)
        {
        }

        public TagConfiguration(string tag, string replacement, string alternativeReplacement, bool isParseContent)
        {
            IsParseTextReplacement = true;
            IsParseTextReqularExpressions = true;

            Tag = tag;
            Replacement = replacement;
            AlternativeReplacement = alternativeReplacement;
            IsParseContent = isParseContent;
            ParamOptions = new List<TagParamOption>(0);

            // If there is a '{0}' in the replacement string the tag cannot be a single tag.
            IsSingleTag = !replacement.Contains("{0}");
        }
    }
}