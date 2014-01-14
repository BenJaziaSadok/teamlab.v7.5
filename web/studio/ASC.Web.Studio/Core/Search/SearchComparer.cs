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
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Studio.Core.Search
{
    public sealed class SearchComparer : IComparer<SearchResult>
    {
        public int Compare(SearchResult x, SearchResult y)
        {
            if (x.Name == y.Name)
                return 0;
            if(y.Name == Users.CustomNamingPeople.Substitute<Resources.Resource>("Employees"))
                return 1;
            return -1;
        }
    }

    public sealed class DateSearchComparer : IComparer<SearchResultItem>
    {
        public int Compare(SearchResultItem x, SearchResultItem y)
        {
            if (x.Date == y.Date)
                return 0;
            if (x.Date > y.Date)
                return -1;
            return 1;
        }
    }
}
