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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Core.Search;

namespace ASC.Web.Studio.UserControls.Common.Search
{
    public partial class SearchResults : UserControl
    {
        public static string Location
        {
            get { return "~/UserControls/Common/Search/SearchResults.ascx"; }
        }

        internal int MaxResultCount = 5;

        public IEnumerable<SearchResult> SearchResultsData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/usercontrols/common/search/css/searchresults.less"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/search/js/searchresults.js"));

            results.ItemDataBound += ResultsItemDataBound;
            results.DataSource = SearchResultsData;
            results.DataBind();
        }

        private static void ResultsItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var control = ((SearchResult)e.Item.DataItem).PresentationControl;
                if (control == null)
                    return;
                control.Items = ((SearchResult)e.Item.DataItem).Items;
                e.Item.FindControl("resultItems").Controls.Add(control);
            }
        }
    }
}