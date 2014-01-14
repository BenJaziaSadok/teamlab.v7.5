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
using ASC.Web.Core;

namespace ASC.Web.Studio.Utility
{
    public static class HeaderStringHelper
    {
        public static string GetHTMLSearchHeader(string searchString)
        {
            return String.Format("{0}: \"{1}\"", Resources.Resource.SearchResult, searchString.HtmlEncode());
        }

        public static string GetPageTitle(string pageTitle)
        {
            var productName = "";
            var product = WebItemManager.Instance[CommonLinkUtility.GetProductID()];
            if (product != null)
                productName = product.Name;

            productName = String.IsNullOrEmpty(productName) ? Resources.Resource.WebStudioName : productName;

            return
                string.IsNullOrEmpty(pageTitle)
                    ? productName
                    : String.Format("{0} - {1}", pageTitle, productName);
        }
    }
}