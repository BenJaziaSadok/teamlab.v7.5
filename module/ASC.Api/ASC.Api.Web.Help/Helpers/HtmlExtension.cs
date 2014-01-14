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
using System.Web.Mvc;

namespace ASC.Api.Web.Help.Helpers
{

    public static class HtmlExtensions
    {
        public static CssFileCollection Style(this HtmlHelper helper, string outurl)
        {
            return Style(helper, outurl, !HttpContext.Current.IsDebuggingEnabled);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, bool combine)
        {
            return Style(helper, outurl, combine, 0);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, int version)
        {
            return Style(helper, outurl, !HttpContext.Current.IsDebuggingEnabled, version);
        }

        public static CssFileCollection Style(this HtmlHelper helper, string outurl, bool combine, int version)
        {
            var url = ((Controller) helper.ViewContext.Controller).Url;
            return new CssFileCollection(outurl,helper,url,combine, version);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl, bool combine)
        {
            return Script(helper, outurl, combine, 0);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl)
        {
            return Script(helper, outurl, 0);
        }

        public static JsFileCollection Script(this HtmlHelper helper, string outurl, int version)
        {
            return Script(helper, outurl, !HttpContext.Current.IsDebuggingEnabled, version);
        }



        public static JsFileCollection Script(this HtmlHelper helper, string outurl, bool combine, int version)
        {
            var url = ((Controller)helper.ViewContext.Controller).Url;
            return new JsFileCollection(outurl, helper, url,combine,version);
        }
    }
}