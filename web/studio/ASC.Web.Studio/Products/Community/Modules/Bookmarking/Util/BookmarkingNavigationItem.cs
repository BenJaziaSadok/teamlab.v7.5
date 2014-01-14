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
using System.Web.UI;
using ASC.Web.Studio.Controls.Common;
using System.Web;

namespace ASC.Web.Community.Bookmarking.Util
{
    public class BookmarkingNavigationItem : NavigationItem
    {
        public string BookmarkingClientID { get; set; }

        public bool DisplayOnPage { get; set; }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (String.IsNullOrEmpty(BookmarkingClientID))
            {
                base.RenderContents(writer);
                return;
            }
            if (!String.IsNullOrEmpty(Name) && !String.IsNullOrEmpty(URL))
            {
                var display = DisplayOnPage ? "block" : "none";

                writer.Write(
                    String.Format("<a href=\"{0}\" title=\"{1}\" id='{2}' style='display:{3}' class='linkAction'>",
                                  ResolveUrl(URL), HttpUtility.HtmlEncode(Description), BookmarkingClientID, display
                        ));
                writer.Write(HttpUtility.HtmlEncode(Name));
                writer.Write("</a>");
            }
            else
                base.RenderContents(writer);
        }
    }
}