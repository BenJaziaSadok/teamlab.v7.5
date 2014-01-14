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

using System.Web;

namespace ASC.Web.Core.Client
{
    public static class ClientCapabilities
    {
        public static long GetMaxEmbeddableImageSize(HttpRequestBase request)
        {
            var ieVersion = GetInternetExplorerVersion(request);
            if (ieVersion<0)
            {
                //Not an IE
                return ClientSettings.MaxImageEmbeddingSize;
            }
            if (ieVersion<8)
            {
                return 0;//Can't embed at all
            }
            if (ieVersion<9)
            {
                //IE8 can embed up to 32kb
                return 32*1024;
            }
            return ClientSettings.MaxImageEmbeddingSize;
        }

        public static double GetInternetExplorerVersion(HttpRequestBase request)
        {
            double rv = -1;
            var browser = request.Browser;
            if (browser.Browser == "IE")
                rv = browser.MajorVersion + browser.MinorVersion;
            return rv;
        }
    }
}