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

using ASC.Blogs.Core.Resources;
using AjaxPro;
using ASC.Blogs.Core;
using ASC.Blogs.Core.Domain;
using ASC.Web.Community.Blogs.Views;
using ASC.Web.Community.Product;
using ASC.Web.Core.Mobile;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI.WebControls;

namespace ASC.Web.Community.Blogs
{
    public partial class EditBlog : BasePage
    {
        protected bool _mobileVer = false;
        protected string _text = "";

        private string BlogId
        {
            get { return Request.QueryString["blogid"]; }
        }

        protected override void PageLoad()
        {
            Utility.RegisterTypeForAjax(typeof(AddBlog));

            if (String.IsNullOrEmpty(BlogId))
                Response.Redirect(Constants.DefaultPageUrl);

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

            var engine = GetEngine();

            Utility.RegisterTypeForAjax(typeof(EditBlog), Page);

            FCKeditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
            FCKeditor.ToolbarSet = "BlogToolbar";
            FCKeditor.EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath;

            FCKeditor.Visible = !_mobileVer;

            if (_mobileVer && IsPostBack)
                _text = Request["mobiletext"];

            mainContainer.CurrentPageCaption = BlogsResource.EditPostTitle;
            Title = HeaderStringHelper.GetPageTitle(BlogsResource.EditPostTitle);

            ShowForEdit(engine);

            lbCancel.Attributes["name"] = FCKeditor.ClientID;
            if (IsPostBack)
            {
                var control = FindControl(Request.Params["__EVENTTARGET"]);
                if (lbCancel.Equals(control))
                {
                    Response.Redirect("viewblog.aspx?blogid=" + Request.Params["blogid"]);
                }
                else
                {
                    if (CheckTitle(txtTitle.Text))
                    {
                        var pageEngine = GetEngine();
                        var post = pageEngine.GetPostById(new Guid(hidBlogID.Value));
                        UpdatePost(post, engine);
                    }
                    else
                    {
                        mainContainer.Options.InfoMessageText = BlogsResource.BlogTitleEmptyMessage;
                        mainContainer.Options.InfoType = InfoType.Alert;
                    }
                }
            }
            InitScript();
        }

        private void InitScript()
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/core/decoder.js"));
            Page.RegisterInlineScript("BlogsManager.IsMobile = " + _mobileVer.ToString().ToLower() + ";");

            if (_mobileVer)
            {
                var script = @"jq(function() {
                            var node = jq('<div>' + jq('#mobiletext').val() + '</div>').get(0);
                            jq('#mobiletextEdit').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
                        });";
                Page.RegisterInlineScript(script);
            }
        }

        private void InitPreviewTemplate(Post post)
        {
            var control = (ViewBlogView)LoadControl("~/Products/Community/Modules/Blogs/Views/ViewBlogView.ascx");
            control.IsPreview = true;
            control.post = post;

            PlaceHolderPreview.Controls.Add(new Literal { Text = "<div class='headerPanel' style='margin-top:20px;'>" + BlogsResource.PreviewButton + "</div>" });
            PlaceHolderPreview.Controls.Add(control);
            PlaceHolderPreview.Controls.Add(new Literal { Text = "<div style='margin-top:25px;'><a class='button blue big' href='javascript:void(0);' onclick='BlogsManager.HidePreview(); return false;'>" + BlogsResource.HideButton + "</a></div>" });
        }


        private void ShowForEdit(BlogsEngine engine)
        {
            if (!IsPostBack)
            {
                var post = engine.GetPostById(new Guid(BlogId));

                InitPreviewTemplate(post);

                if (post != null && CommunitySecurity.CheckPermissions(post, Constants.Action_EditRemovePost))
                {
                    hdnUserID.Value = post.UserID.ToString();

                    if (Request.QueryString["action"] == "delete")
                    {
                        foreach (var comment in engine.GetPostComments(post.ID))
                        {
                            CommonControlsConfigurer.FCKUploadsRemoveForItem("blogs_comments", comment.ID.ToString());
                        }

                        engine.DeletePost(post);
                        CommonControlsConfigurer.FCKUploadsRemoveForItem("blogs", post.ID.ToString());
                        Response.Redirect(Constants.DefaultPageUrl);
                        return;
                    }
                    else
                    {
                        txtTitle.Text = Server.HtmlDecode(post.Title);

                        if (_mobileVer)
                            _text = post.Content;
                        else
                            FCKeditor.Value = post.Content;

                        hidBlogID.Value = post.ID.ToString();

                        LoadTags(post.TagList);
                    }
                }
                else
                {
                    Response.Redirect(Constants.DefaultPageUrl);
                    return;
                }
            }
        }

        private void LoadTags(IList<Tag> tags)
        {
            var sb = new StringBuilder();

            var i = 0;
            foreach (var tag in tags)
            {
                if (i != 0)
                    sb.Append(", " + tag.Content);
                else
                    sb.Append(tag.Content);
                i++;
            }

            txtTags.Text = sb.ToString();
        }


        private static bool CheckTitle(string title)
        {
            return !string.IsNullOrEmpty(title) && !string.IsNullOrEmpty(title.Trim());
        }

        private static bool IsExistsTagName(Post post, string tagName)
        {
            return post.TagList.Any(tag => tag.Content == tagName);
        }

        protected override string RenderRedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=blogs&iid=" + BlogId);
        }

        public void UpdatePost(Post post, BlogsEngine engine)
        {
            post.Title = GetLimitedText(txtTitle.Text);
            post.Content = _mobileVer ? (Request["mobiletext"] ?? "") : FCKeditor.Value;

            post.TagList = new List<Tag>();

            foreach (var tagName in txtTags.Text.Split(','))
            {
                if (tagName == string.Empty || IsExistsTagName(post, tagName))
                    continue;

                var tag = new Tag(post)
                    {
                        Content = GetLimitedText(tagName.Trim())
                    };
                post.TagList.Add(tag);
            }

            engine.SavePost(post, false, false);

            CommonControlsConfigurer.FCKEditingComplete("blogs", post.ID.ToString(), post.Content, true);

            Response.Redirect("viewblog.aspx?blogid=" + post.ID.ToString());
        }

        #region Events

        protected void lbCancel_Click(object sender, EventArgs e)
        {
            CommonControlsConfigurer.FCKEditingCancel("blogs", BlogId);
            Response.Redirect(Constants.DefaultPageUrl);
        }

        #endregion
    }
}