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
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Community.News.Common;
using System;


namespace ASC.Web.Community.News.Controls
{
    

    public class BreadcrumbsControl : WebControl
    {
        // Fields
        private IList<BreadcrumbPath> breadCrumbPath = new List<BreadcrumbPath>();

        // Methods
        public void AddBreadcrumb(string name, Uri url)
        {
            this.breadCrumbPath.Add(new BreadcrumbPath(name, url.ToString()));
        }

        

        protected override void RenderContents(HtmlTextWriter writer)
        {
            base.RenderContents(writer);
            writer.Write(@"<div style=""padding:0px 0px 8px 0px;"">");
            for (int i = 0; i < (this.breadCrumbPath.Count - 1); i++)
            {
                if(i > 0)
                {
                    writer.Write(@"<span class=""textBase""> > </span>");
                }
                BreadcrumbPath path = this.breadCrumbPath[i];
                writer.Write(@"<a class=""breadCrumbs"" title=""{0}"" href=""{1}"">{0}</a>",path.Name, path.Link);
            }
            writer.Write(@"</div>");
            writer.Write(@"<div>{0}</div>", this.breadCrumbPath[this.breadCrumbPath.Count - 1].Name);

        }

        
    }
}