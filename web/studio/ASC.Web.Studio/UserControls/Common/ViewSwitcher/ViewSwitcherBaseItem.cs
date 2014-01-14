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

using System.Text;
using System.Web.UI;

namespace ASC.Web.Studio.UserControls.Common.ViewSwitcher
{
    public abstract class ViewSwitcherBaseItem : Control
    {
        public string SortUrl { get; set; }

        public string SortLabel { get; set; }

        public bool IsSelected { get; set; }

        internal bool IsLast { get; set; }

        public string DivID { get; set; }

        public string AdditionalHtml { get; set; }

        public string HintText { get; set; }

        public string GetSortLink
        {
            get
            {
                var idString = string.Empty;
                if (!string.IsNullOrEmpty(DivID))
                {
                    idString = string.Format(" id='{0}' ", DivID);
                }
                var cssClass = "viewSwitcherItem";
                if (IsSelected)
                {
                    cssClass = "viewSwithcerSelectedItem";
                }
                var sb = new StringBuilder();
                sb.AppendFormat("<div {0} class='{1}'>{2}{3}</div>", idString, cssClass, GetLink(), AdditionalHtml ?? string.Empty);
                return sb.ToString();
            }
        }

        public abstract string GetLink();
    }
}