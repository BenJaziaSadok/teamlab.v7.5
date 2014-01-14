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
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility.HtmlUtility;
using AjaxPro;
using ASC.Core;
using ASC.Core.Tenants;
using ASC.Core.Users;
using ASC.Web.Community.News.Code;
using ASC.Web.Community.News.Code.DAO;
using ASC.Web.Community.News.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core.Utility.Skins;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;
using FeedNS = ASC.Web.Community.News.Code;

namespace ASC.Web.Community.News
{
    [AjaxNamespace("EditNews")]
    public partial class EditNews : MainPage
    {
        private RequestInfo info;
        protected bool _mobileVer = false;
        protected string _text = "";

        private RequestInfo Info
        {
            get { return info ?? (info = new RequestInfo(Request)); }
        }

        public long FeedId
        {
            get { return ViewState["FeedID"] != null ? Convert.ToInt32(ViewState["FeedID"], CultureInfo.CurrentCulture) : 0; }
            set { ViewState["FeedID"] = value; }
        }

        private void BindNewsTypes()
        {
            feedType.DataSource = new[]
                {
                    FeedTypeInfo.FromFeedType(FeedType.News),
                    FeedTypeInfo.FromFeedType(FeedType.Order),
                    FeedTypeInfo.FromFeedType(FeedType.Advert)
                };
            feedType.DataBind();

            if (!string.IsNullOrEmpty(Request["type"]))
            {
                var requestFeedType = (FeedType)Enum.Parse(typeof(FeedType), Request["type"], true);
                var feedTypeInfo = FeedTypeInfo.FromFeedType(requestFeedType);

                var item = feedType.Items.FindByText(feedTypeInfo.TypeName);

                feedType.SelectedValue = item.Value;
            }
            else
            {
                feedType.SelectedIndex = 0;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            Utility.RegisterTypeForAjax(GetType());

            if (!CommunitySecurity.CheckPermissions(NewsConst.Action_Add))
                Response.Redirect(FeedUrls.MainPageUrl, true);

            _mobileVer = Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);

            //fix for IE 10
            var browser = HttpContext.Current.Request.Browser.Browser;

            var userAgent = Context.Request.Headers["User-Agent"];
            var regExp = new Regex("MSIE 10.0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regExpIe11 = new Regex("(?=.*Trident.*rv:11.0).+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (browser == "IE" && regExp.Match(userAgent).Success || regExpIe11.Match(userAgent).Success)
            {
                _mobileVer = true;
            }

            var storage = FeedStorageFactory.Create();
            FeedNS.Feed feed = null;
            if (!string.IsNullOrEmpty(Request["docID"]))
            {
                long docID;
                if (long.TryParse(Request["docID"], out docID))
                {
                    feed = storage.GetFeed(docID);
                    (Master as NewsMaster).CurrentPageCaption = NewsResource.NewsEditBreadCrumbsNews;
                    Title = HeaderStringHelper.GetPageTitle(NewsResource.NewsEditBreadCrumbsNews);
                    _text = (feed != null ? feed.Text : "").HtmlEncode();
                }
            }
            else
            {
                (Master as NewsMaster).CurrentPageCaption = NewsResource.NewsAddBreadCrumbsNews;
                Title = HeaderStringHelper.GetPageTitle(NewsResource.NewsAddBreadCrumbsNews);
            }

            if (_mobileVer && IsPostBack)
                _text = Request["mobiletext"] ?? "";

            if (!IsPostBack)
            {
                //feedNameRequiredFieldValidator.ErrorMessage = NewsResource.RequaredFieldValidatorCaption;
                HTML_FCKEditor.BasePath = VirtualPathUtility.ToAbsolute(CommonControlsConfigurer.FCKEditorBasePath);
                HTML_FCKEditor.ToolbarSet = "NewsToolbar";
                HTML_FCKEditor.EditorAreaCSS = WebSkin.BaseCSSFileAbsoluteWebPath;
                HTML_FCKEditor.Visible = !_mobileVer;
                BindNewsTypes();

                if (feed != null)
                {
                    if (!CommunitySecurity.CheckPermissions(feed, NewsConst.Action_Edit))
                    {
                        Response.Redirect(FeedUrls.MainPageUrl, true);
                    }
                    feedName.Text = feed.Caption;
                    HTML_FCKEditor.Value = feed.Text;
                    FeedId = feed.Id;
                    feedType.SelectedIndex = (int)Math.Log((int)feed.FeedType, 2);
                }
                else
                {
                    if (!string.IsNullOrEmpty(Request["type"]))
                    {
                        var requestFeedType = (FeedType)Enum.Parse(typeof(FeedType), Request["type"], true);
                        var feedTypeInfo = FeedTypeInfo.FromFeedType(requestFeedType);
                        var item = feedType.Items.FindByText(feedTypeInfo.TypeName);

                        feedType.SelectedValue = item.Value;
                        feedType.SelectedIndex = (int)Math.Log((int)requestFeedType, 2);
                    }
                }
            }
            else
            {
                var control = FindControl(Request.Params["__EVENTTARGET"]);
                if (lbCancel.Equals(control))
                {
                    CancelFeed(sender, e);
                }
                else
                {
                    SaveFeed();
                }
            }

            lbCancel.Attributes["name"] = HTML_FCKEditor.ClientID;
            RenderScripts();
        }

        protected void RenderScripts()
        {
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/asc/core/decoder.js"));

            var scriptSb = new System.Text.StringBuilder();
            scriptSb.AppendLine("var isMobile = " + _mobileVer.ToString().ToLower() + ";");
            scriptSb.AppendLine(@"function FeedPrevShow() {
                    jq('#feedPrevDiv_Caption').val(jq('#" + feedName.ClientID + @"').val());
                    if (isMobile) {
                        var text = jq('#" + SimpleText.ClientID + @"').val();
                        jq('#feedPrevDiv_Body').html(ASC.Controls.HtmlHelper.Text2EncodedHtml(text));
                    } else {
                        jq('#feedPrevDiv_Body').html(FCKeditorAPI.GetInstance('" + HTML_FCKEditor.ClientID + @"').GetXHTML(true));
                    }
                    jq('#feedPrevDiv').show();
                    jq.scrollTo(jq('#feedPrevDiv').position().top, {speed:500});
                }"
                );
            scriptSb.AppendLine(@"function HidePreview() {
                    jq('#feedPrevDiv').hide();
                    jq.scrollTo(jq('#newsCaption').position().top, {speed:500});
                }"
                );
            scriptSb.AppendLine(@"function FCKConfig_OnLoad(config) {
                    config.RedirectUrlToUpload('" + RedirectUpload() + @"');
                    config.MaxImageWidth = 650;
                }"
                );
            scriptSb.AppendLine(@"function PreSaveMobile() {
                    if (isMobile) {
                        var text = jq('#" + SimpleText.ClientID + @"').val();
                        jq('#mobiletext').val(ASC.Controls.HtmlHelper.Text2EncodedHtml(text));
                    }
                }"
                );
            scriptSb.AppendLine(@"jq(function() {
                    jq('#" + feedType.ClientID + @"').tlCombobox();
                    jq('#" + feedType.ClientID + @"').removeClass('display-none');
                });"
                );
            if (_mobileVer)
            {
                scriptSb.AppendLine(@"jq(function() {
                        var node = jq('<div>' + jq('#mobiletext').val() + '</div>').get(0);
                        jq('#" + SimpleText.ClientID + @"').val(ASC.Controls.HtmlHelper.HtmlNode2FormattedText(node));
                    });"
                    );
            }

            Page.RegisterInlineScript(scriptSb.ToString(), true, false);
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public FeedAjaxInfo FeedPreview(string captionFeed, string bodyFeed)
        {
            var feed = new FeedAjaxInfo
                {
                    FeedCaption = captionFeed,
                    FeedText = HtmlUtility.GetFull(bodyFeed, CommunityProduct.ID),
                    Date = TenantUtil.DateTimeNow().Ago(),
                    UserName = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).RenderProfileLink(CommunityProduct.ID)
                };
            return feed;
        }

        protected void CancelFeed(object sender, EventArgs e)
        {
            var url = FeedUrls.MainPageUrl;

            if (FeedId != 0)
            {
                CommonControlsConfigurer.FCKEditingCancel("news", FeedId.ToString());
                url += "?docid=" + FeedId.ToString();
            }
            else
                CommonControlsConfigurer.FCKEditingCancel("news");

            Response.Redirect(url, true);
        }

        protected void SaveFeed()
        {

            if (string.IsNullOrEmpty(feedName.Text))
            {
                ((NewsMaster)Master).SetInfoMessage(NewsResource.RequaredFieldValidatorCaption, InfoType.Alert);
                //pnlError.Visible = true;
                return;
            }

            var storage = FeedStorageFactory.Create();
            var isEdit = (FeedId != 0);
            var feed = isEdit ? storage.GetFeed(FeedId) : new FeedNews();
            feed.Caption = feedName.Text;
            feed.Text = _mobileVer ? (Request["mobiletext"] ?? "") : HTML_FCKEditor.Value;
            feed.FeedType = (FeedType)int.Parse(feedType.SelectedValue, CultureInfo.CurrentCulture);
            storage.SaveFeed(feed, isEdit, FeedType.News);

            CommonControlsConfigurer.FCKEditingComplete("news", feed.Id.ToString(), feed.Text, isEdit);

            Response.Redirect(FeedUrls.GetFeedUrl(feed.Id, Info.UserId));
        }

        protected string RedirectUpload()
        {
            return string.Format("{0}://{1}:{2}{3}", Request.GetUrlRewriter().Scheme, Request.GetUrlRewriter().Host, Request.GetUrlRewriter().Port,
                                 VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx?esid=news" + (FeedId != 0 ? "&iid=" + FeedId.ToString() : ""));
        }
    }

    public class FeedAjaxInfo
    {
        public string FeedCaption { get; set; }

        public string FeedText { get; set; }

        public string Date { get; set; }

        public string UserName { get; set; }
    }
}