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

namespace ASC.Web.Studio.UserControls.Common.ViewSwitcher
{
    public class ViewSwitcherLinkItem : ViewSwitcherBaseItem
    {
        private string _linkCssClass;

        public string LinkCssClass
        {
            get
            {
                if (string.IsNullOrEmpty(this._linkCssClass))
                    return "linkAction";
                return _linkCssClass;
            }
            set { _linkCssClass = value; }
        }

        public bool ActiveItemIsLink { get; set; }

        public override string GetLink()
        {
            var sb = new StringBuilder();
            if (!ActiveItemIsLink)
            {
                if (!IsSelected)
                {
                    sb.AppendFormat("<a href=\"{0}\" class='{1}'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                }
                else
                {
                    sb.Append(SortLabel);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(_linkCssClass))
                    sb.AppendFormat("<a href=\"{0}\" class='{1}'>{2}</a>", SortUrl, LinkCssClass, SortLabel);
                else
                {
                    sb.AppendFormat(IsSelected
                                        ? "<a href=\"{0}\" class='{1}' style='font-weight:bold;'>{2}</a>"
                                        : "<a href=\"{0}\" class='{1}'>{2}</a>",
                                    SortUrl, LinkCssClass, SortLabel);
                }

            }
            return sb.ToString();
        }

        public ViewSwitcherLinkItem()
        {
            ActiveItemIsLink = false;
        }
    }
}