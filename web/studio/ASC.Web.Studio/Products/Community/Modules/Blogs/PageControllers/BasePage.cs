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
using System.Text;
using System.Web;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Core;
using ASC.Data.Storage;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Community.Blogs
{
    public abstract class BasePage : MainPage
    {
        protected int GetBlogMaxImageWidth
        {
            get { return 570; }
        }

        public static BlogsEngine GetEngine()
        {
            return BlogsEngine.GetEngine(TenantProvider.CurrentTenantID);
        }

        /// <summary>
        /// Page_Load of the Page Controller pattern.
        /// See http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnpatterns/html/ImpPageController.asp
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            Utility.RegisterTypeForAjax(typeof (AddBlog));
            PageLoad();
            RenderScripts();
        }

        protected abstract void PageLoad();

        protected string GetLimitedText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            return text.Length > ASC.Blogs.Core.Constants.MAX_TEXT_LENGTH ? text.Substring(0, ASC.Blogs.Core.Constants.MAX_TEXT_LENGTH) : text;
        }

        protected virtual string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=blogs");
        }

        protected void RenderScripts()
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/js/blogs.js"));
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/plugins/tagsautocompletebox.js"));
            Page.RegisterStyleControl(VirtualPathUtility.ToAbsolute("~/products/community/modules/blogs/app_themes/default/blogstyle.css"));

            var script = @"
function createSearchHelper() {

	var ForumTagSearchHelper = new SearchHelper(
		'_txtTags', 
		'tagAutocompleteItem', 
		'tagAutocompleteSelectedItem', 
		'', 
		'', 
		'BlogsPage',
		'GetSuggest',
		'', 
		true,
		false
	);
}
";

            Page.ClientScript.RegisterClientScriptBlock(typeof (string), "blogsTagsAutocompleteInitScript", script, true);
        }
    }
}