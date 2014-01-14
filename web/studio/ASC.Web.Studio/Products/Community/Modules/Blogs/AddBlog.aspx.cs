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

using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Blogs.Core.Resources;
using ASC.Blogs.Core.Security;
using ASC.Core;
using ASC.Web.Community.Blogs.Views;
using ASC.Web.Community.Product;
using ASC.Web.Core.Mobile;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Utility.HtmlUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace ASC.Web.Community.Blogs
{
    [AjaxNamespace("BlogsPage")]
    public partial class AddBlog : BasePage
    {
        protected bool _mobileVer = false;
        protected string _text = "";

        protected override void PageLoad()
        {

            if (!CommunitySecurity.CheckPermissions(Constants.Action_AddPost))
                Response.Redirect(Constants.DefaultPageUrl, true);

            _mobileVer = MobileDetector.IsRequestMatchesMobile(Context);

            //fix for IE 10 && IE11
            var browser = HttpContext.Current.Request.Browser.Browser;

            var userAgent = Context.Request.Headers["User-Agent"];
            var regExp = new Regex("MSIE 10.0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regExpIe11 = new Regex("(?=.*Trident.*rv:11.0).+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (browser == "IE" && regExp.Match(userAgent).Success || regExpIe11.Match(userAgent).Success)
            {
                _mobileVer = true;
            }

            //lbtnPost.Text = BlogsResource.PostButton;
            FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            FCKeditor.ToolbarSet = "BlogToolbar";
            FCKeditor.EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath;
            FCKeditor.Visible = !_mobileVer;

            if (_mobileVer && Request["mobiletext"] != null)
                _text = Request["mobiletext"];

            if (CheckTitle(txtTitle.Text))
            {
                mainContainer.Options.InfoMessageText = "";
            }

            mainContainer.CurrentPageCaption = BlogsResource.NewPost;
            Title = HeaderStringHelper.GetPageTitle(BlogsResource.NewPost);

            InitPreviewTemplate();

            lbCancel.Attributes["name"] = FCKeditor.ClientID;

            if (IsPostBack)
            {
                var control = FindControl(Request.Params["__EVENTTARGET"]);
                if (lbCancel.Equals(control))
                {
                    Response.Redirect(Constants.DefaultPageUrl);
                }
                else
                {
                    TryPostBlog(GetEngine());
                }
            }

            InitScript();
        }

        private void InitScript()
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/core/decoder.js"));
            Page.RegisterInlineScript("BlogsManager.IsMobile = " + _mobileVer.ToString().ToLower() + ";");

            var script = @" function FCKConfig_OnLoad(config)
                            {
                                config.RedirectUrlToUpload('" + RenderRedirectUpload() + @"');
                                config.MaxImageWidth = " + GetBlogMaxImageWidth + @";
                            }
    
                            function FCKeditor_OnComplete(instance) {
                                instance.Focus();
                                jq('[id$=txtTitle]').focus();
                            }";

            Page.RegisterInlineScript(script, true, false);

            if (_mobileVer)
            {
                script = @"jq(function() {
                            var node = jq('<div>' + jq('#mobiletext').val() + '</div>').get(0);
                            jq('#mobiletextEdit').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
                        });";
                Page.RegisterInlineScript(script, true, false);
            }

        }

        private static bool CheckTitle(string title)
        {
            return !string.IsNullOrEmpty(title.Trim());
        }

        private static bool IsExistsTagName(Post post, string tagName)
        {
            return post.TagList.Any(tag => tag.Content == tagName);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public AjaxResponse GetSuggest(string text, string varName, int limit)
        {
            var resp = new AjaxResponse();

            var startSymbols = text;
            var ind = startSymbols.LastIndexOf(",");
            if (ind != -1)
                startSymbols = startSymbols.Substring(ind + 1);

            startSymbols = startSymbols.Trim();

            var engine = GetEngine();

            var tags = new List<string>();

            if (!string.IsNullOrEmpty(startSymbols))
            {
                tags = engine.GetTags(startSymbols, limit);
            }

            var resNames = new StringBuilder();
            var resHelps = new StringBuilder();

            foreach (var tag in tags)
            {
                resNames.Append(tag);
                resNames.Append("$");
                resHelps.Append(tag);
                resHelps.Append("$");
            }
            resp.rs1 = resNames.ToString().TrimEnd('$');
            resp.rs2 = resHelps.ToString().TrimEnd('$');
            resp.rs3 = text;
            resp.rs4 = varName;

            return resp;
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string[] GetPreview(string title, string html)
        {
            var result = new string[2];

            result[0] = HttpUtility.HtmlEncode(title);
            result[1] = HtmlUtility.GetFull(html, CommunityProduct.ID);

            return result;
        }

        private Post AddNewBlog(BlogsEngine engine)
        {
            var authorId = SecurityContext.CurrentAccount.ID;

            if (CommunitySecurity.CheckPermissions(
                new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(authorId)),
                Constants.Action_AddPost))
            {
                var newPost = new Post
                    {
                        Content = _mobileVer ? (Request["mobiletext"] ?? "") : FCKeditor.Value
                    };

                var dateNow = ASC.Core.Tenants.TenantUtil.DateTimeNow();

                newPost.Datetime = dateNow;
                newPost.Title = GetLimitedText(txtTitle.Text);
                newPost.UserID = authorId;

                newPost.TagList = new List<Tag>();
                foreach (var tagName in txtTags.Text.Split(','))
                {
                    if (tagName == string.Empty || IsExistsTagName(newPost, tagName))
                        continue;

                    var tag = new Tag
                        {
                            Content = GetLimitedText(tagName.Trim())
                        };
                    newPost.TagList.Add(tag);
                }
                engine.SavePost(newPost, true, Request.Form["notify_comments"] == "on");

                CommonControlsConfigurer.FCKEditingComplete("blogs", newPost.ID.ToString(), newPost.Content, false);

                return newPost;
            }

            Response.Redirect("addblog.aspx");
            return null;
        }

        private void TryPostBlog(BlogsEngine engine)
        {
            if (CheckTitle(txtTitle.Text))
            {
                var post = AddNewBlog(engine);

                if (post != null)
                    Response.Redirect("viewblog.aspx?blogid=" + post.ID.ToString());
                else
                    Response.Redirect(Constants.DefaultPageUrl);
            }
            else
            {
                mainContainer.Options.InfoMessageText = BlogsResource.BlogTitleEmptyMessage;
                mainContainer.Options.InfoType = InfoType.Alert;
            }
        }

        private void InitPreviewTemplate()
        {
            var post = new Post
                {
                    Datetime = ASC.Core.Tenants.TenantUtil.DateTimeNow(),
                    Title = string.Empty,
                    Content = string.Empty,
                    UserID = SecurityContext.CurrentAccount.ID
                };

            var control = (ViewBlogView)LoadControl("~/products/community/modules/blogs/views/viewblogview.ascx");
            control.IsPreview = true;
            control.post = post;

            PlaceHolderPreview.Controls.Add(new Literal { Text = "<div class='headerPanel' style='margin-top:25px;'>" + BlogsResource.PreviewButton + "</div>" });
            PlaceHolderPreview.Controls.Add(control);
            PlaceHolderPreview.Controls.Add(new Literal { Text = "<div style='margin-top:20px;'><a class='button blue big' href='javascript:void(0);' onclick='BlogsManager.HidePreview(); return false;'>" + BlogsResource.HideButton + "</a></div>" });
        }

        #region Events

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKEditingCancel("blogs");
            Response.Redirect(Constants.DefaultPageUrl);
        }

        #endregion
    }
}