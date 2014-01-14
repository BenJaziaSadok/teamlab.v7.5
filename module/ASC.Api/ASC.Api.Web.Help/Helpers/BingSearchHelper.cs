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
using ASC.Api.Web.Help.BingSearch;
using ASC.Api.Web.Help.DocumentGenerator;

namespace ASC.Api.Web.Help.Helpers
{
    public static class BingSearchHelper
    {
        public static Dictionary<MsDocEntryPoint, Dictionary<MsDocEntryPointMethod, string>> CreateResults(WebResult[] webResults)
        {
            var points = (from webResult in webResults
            let point = Documentation.GetByUri(new Uri(webResult.Url, UriKind.Absolute))
            where point != null
            select new KeyValuePair<MsDocEntryPointMethod,string>(point,webResult.Description)).Distinct(new BingComparer());

            return points.GroupBy(x=>x.Key.Parent)
                .ToDictionary(x=>x.Key,y=>y.ToDictionary(key=>key.Key,value=>value.Value));
        }
    }

    public class BingComparer : IEqualityComparer<KeyValuePair<MsDocEntryPointMethod, string>>
    {
        public bool Equals(KeyValuePair<MsDocEntryPointMethod, string> x, KeyValuePair<MsDocEntryPointMethod, string> y)
        {
            return x.Key == y.Key;
        }

        public int GetHashCode(KeyValuePair<MsDocEntryPointMethod, string> obj)
        {
            return obj.Key.GetHashCode();
        }
    }
}