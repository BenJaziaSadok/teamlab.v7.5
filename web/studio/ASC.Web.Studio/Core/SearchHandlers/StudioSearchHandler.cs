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
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Web.Core.Utility;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Studio.Core.SearchHandlers
{
    public class StudioSearchHandler : BaseSearchHandlerEx
    {
        public override ImageOptions Logo
        {
            get { return new ImageOptions { ImageFileName = "common_search_icon.png", PartID = Guid.Empty }; }
        }

        public override string SearchName
        {
            get { return Resources.Resource.Search; }
        }

        public override SearchResultItem[] Search(string text)
        {
            return new SearchResultItem[0];
        }
    }
}