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
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Studio.Core.Search
{
    public class SearchResult
    {
        public string LogoURL { get; set; }
        public string Name { get; set; }
        public Guid ProductID { get; set; }
        public List<SearchResultItem> Items { get; set; }

        public ItemSearchControl PresentationControl { get; set; }

        public SearchResult()
        {
            Items = new List<SearchResultItem>();
        }
    }
}