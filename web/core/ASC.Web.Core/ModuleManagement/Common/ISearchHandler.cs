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
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Core.Utility.Skins;

namespace ASC.Web.Core.ModuleManagement.Common
{
    public class ItemSearchControl : WebControl, IItemControl
    {
        public List<SearchResultItem> Items { get; set; }

        public string Text { get; set; }

        public int MaxCount { get; set; }

        public string SpanClass { get; set; }


        public ItemSearchControl()
            : base(HtmlTextWriterTag.Div)
        {
            MaxCount = 5;
            SpanClass = "describe-text";
        }

        public virtual void RenderContent(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
        }

        /// <summary>
        /// This method needs to keep item height
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        protected string CheckEmptyValue(string value)
        {
            return String.IsNullOrEmpty(value) ? "&nbsp;" : value;
        }
    }

    public interface IItemControl
    {
        List<SearchResultItem> Items { get; set; }

        string Text { get; set; }
    }


    public class SearchResultItem
    {
        /// <summary>
        /// Absolute URL
        /// </summary>
        public string URL { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? Date { get; set; }
        public Dictionary<string, object> Additional { get; set; }
    }

    public interface ISearchHandlerEx
    {
        Guid ProductID { get; }

        Guid ModuleID { get; }

        /// <summary>
        /// Interface log 
        /// </summary>
        ImageOptions Logo { get; }

        /// <summary>
        /// Search display name
        /// <remarks>Ex: "forum search"</remarks>
        /// </summary>
        string SearchName { get; }

        IItemControl Control { get; }

        /// <summary>
        /// Do search
        /// </summary>
        /// <param name="text">Search text</param>
        /// <returns>If nothing found - empty array</returns>
        SearchResultItem[] Search(string text);
    }
}
