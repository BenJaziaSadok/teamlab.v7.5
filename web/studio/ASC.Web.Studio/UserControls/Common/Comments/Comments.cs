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
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASC.Web.Studio.Controls.Common;
using ASC.Web.Studio.Utility;
using ASC.Web.Studio.Utility.HtmlUtility;
using Resources;

namespace ASC.Web.Studio.UserControls.Common.Comments
{
    [ToolboxData("<{0}:CommentsList runat=server></{0}:CommentsList>")]
    public class CommentsList : WebControl
    {
        public delegate string FCKBasePathRequestHandler();

        public event FCKBasePathRequestHandler FCKBasePathRequest;

        public CommentsList()
        {
            ProductId = Guid.Empty;
            var codeHighlighter = new CodeHighlighter();
            Controls.Add(codeHighlighter);
        }

        #region Members

        private static bool _isClientScriptRegistered;
        private int _maxDepthLevel = 8;
        private int _commentIndex;

        private string JsObjName
        {
            get { return String.IsNullOrEmpty(BehaviorID) ? "__comments" + UniqueID : BehaviorID; }
        }

        private IList<CommentInfo> _items = new List<CommentInfo>(0);
        private string _inactiveMessage = string.Empty;
        private string _editCommentToolTip = string.Empty;
        private string _responseCommentToolTip = string.Empty;
        private string _removeCommentToolTip = string.Empty;

        private string _editCommentLink = "Edit";
        private string _removeCommentLink = "Remove";
        private string _responseCommentLink = "Answer";

        private string _previewButton = string.Empty;
        private string _saveButton = string.Empty;
        private string _cancelButton = string.Empty;
        private string _hidePrevuewButton = string.Empty;
        private string _addCommentLink = string.Empty;
        private string _cancelCommentLink = string.Empty;
        private string _commentsTitle = string.Empty;
        private string _commentsCountTitle = string.Empty;
        private string _javaScriptRemoveCommentFunctionName = string.Empty;
        private string _javaScriptPreviewCommentFunctionName = string.Empty;
        private string _javaScriptAddCommentFunctionName = string.Empty;
        private string _javaScriptUpdateCommentFunctionName = string.Empty;
        private string _javaScriptLoadBBcodeCommentFunctionName = string.Empty;
        private string _javaScriptCallBackAddComment = string.Empty;
        private string _objectID = string.Empty;
        private bool _isShowAddCommentBtn = true;
        private string _confirmRemoveCommentMessage = string.Empty;
        private bool _showCaption = true;
        private bool _simple;

        #endregion

        #region Properties

        public string FckDomainName { get; set; }

        public string OnEditedCommentJS { get; set; }
        public string OnRemovedCommentJS { get; set; }
        public string OnCanceledCommentJS { get; set; }

        public int MaxDepthLevel
        {
            get { return _maxDepthLevel; }
            set { _maxDepthLevel = value; }
        }

        public string CommentSendingMsg { get; set; }

        public string LoaderImage { get; set; }

        public string AttachButton { get; set; }

        public string RemoveAttachButton { get; set; }

        public Guid ProductId { get; set; }

        public bool EnableAttachmets { get; set; }

        public string HandlerTypeName { get; set; }

        public bool DisableCtrlEnter { get; set; }

        public string AdditionalSubmitText { get; set; }

        public string PID { get; set; }

        public string EditCommentLink
        {
            get { return _editCommentLink; }
            set { _editCommentLink = value; }
        }

        public string RemoveCommentLink
        {
            get { return _removeCommentLink; }
            set { _removeCommentLink = value; }
        }

        public string ResponseCommentLink
        {
            get { return _responseCommentLink; }
            set { _responseCommentLink = value; }
        }

        public bool Simple
        {
            get { return _simple; }
            set { _simple = value; }
        }

        //FCKEditorsOprions
        public string FCKBasePath
        {
            get
            {
                if (ViewState["FCKBasePath"] == null || ViewState["FCKBasePath"].ToString().Equals(string.Empty))
                {
                    if (FCKBasePathRequest != null)
                    {
                        var result = FCKBasePathRequest();
                        if (!string.IsNullOrEmpty(result))
                        {
                            ViewState["FCKBasePath"] = result.TrimEnd('/') + "/";
                            return result.TrimEnd('/') + "/";
                        }
                    }

                    throw new HttpException("BasePath for FCKEditor is empty.");
                }

                return ViewState["FCKBasePath"].ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["FCKBasePath"] = value.TrimEnd('/') + "/";
            }
        }

        public string FCKToolbar
        {
            get
            {
                if (ViewState["FCKToolbar"] == null || ViewState["FCKToolbar"].ToString().Equals(string.Empty))
                {
                    return "Mini";
                }

                return ViewState["FCKToolbar"].ToString();
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    ViewState["FCKToolbar"] = value;
            }
        }

        public int FCKHeight
        {
            get
            {
                var result = 0;
                try
                {
                    result = Convert.ToInt32(ViewState["FCKHeight"]);
                }
                catch
                {
                }

                return result > 0 ? result : 250;
            }
            set { ViewState["FCKHeight"] = value; }
        }

        public Unit FCKWidth
        {
            get
            {
                if (ViewState["FCKWidth"] == null || Unit.Parse(ViewState["FCKWidth"].ToString()).IsEmpty)
                {
                    return Unit.Parse("100%");
                }

                return Unit.Parse(ViewState["FCKWidth"].ToString());
            }
            set { ViewState["FCKWidth"] = value.ToString(); }
        }

        public string FCKEditorAreaCss
        {
            get { return ViewState["FCKEditorAreaCss"] == null ? string.Empty : ViewState["FCKEditorAreaCss"].ToString(); }
            set { ViewState["FCKEditorAreaCss"] = value; }
        }

        public bool IsShowAddCommentBtn
        {
            get { return _isShowAddCommentBtn; }
            set { _isShowAddCommentBtn = value; }
        }

        public bool ShowCaption
        {
            get { return _showCaption; }
            set { _showCaption = value; }
        }

        public string BehaviorID { get; set; }

        public IList<CommentInfo> Items
        {
            get { return _items; }
            set { _items = value; }
        }

        public Func<string, string> UserProfileUrlResolver { get; set; }

        public string InactiveMessage
        {
            get { return _inactiveMessage; }
            set { _inactiveMessage = value; }
        }

        public string EditCommentToolTip
        {
            get { return _editCommentToolTip; }
            set { _editCommentToolTip = value; }
        }

        public string ResponseCommentToolTip
        {
            get { return _responseCommentToolTip; }
            set { _responseCommentToolTip = value; }
        }

        public string RemoveCommentToolTip
        {
            get { return _removeCommentToolTip; }
            set { _removeCommentToolTip = value; }
        }

        public string PreviewButton
        {
            get { return _previewButton; }
            set { _previewButton = value; }
        }

        public string SaveButton
        {
            get { return _saveButton; }
            set { _saveButton = value; }
        }

        public string CancelButton
        {
            get { return _cancelButton; }
            set { _cancelButton = value; }
        }

        public string HidePrevuewButton
        {
            get { return _hidePrevuewButton; }
            set { _hidePrevuewButton = value; }
        }

        public string AddCommentLink
        {
            get { return _addCommentLink; }
            set { _addCommentLink = value; }
        }

        public string CancelCommentLink
        {
            get { return _cancelCommentLink; }
            set { _cancelCommentLink = value; }
        }

        public string CommentsTitle
        {
            get { return _commentsTitle; }
            set { _commentsTitle = value; }
        }

        public string CommentsCountTitle
        {
            get { return _commentsCountTitle; }
            set { _commentsCountTitle = value; }
        }

        public string JavaScriptRemoveCommentFunctionName
        {
            get { return _javaScriptRemoveCommentFunctionName; }
            set { _javaScriptRemoveCommentFunctionName = value; }
        }

        public string JavaScriptPreviewCommentFunctionName
        {
            get { return _javaScriptPreviewCommentFunctionName; }
            set { _javaScriptPreviewCommentFunctionName = value; }
        }

        public string JavaScriptAddCommentFunctionName
        {
            get { return _javaScriptAddCommentFunctionName; }
            set { _javaScriptAddCommentFunctionName = value; }
        }

        public string JavaScriptUpdateCommentFunctionName
        {
            get { return _javaScriptUpdateCommentFunctionName; }
            set { _javaScriptUpdateCommentFunctionName = value; }
        }

        public string JavaScriptLoadBBcodeCommentFunctionName
        {
            get { return _javaScriptLoadBBcodeCommentFunctionName; }
            set { _javaScriptLoadBBcodeCommentFunctionName = value; }
        }

        public string JavaScriptCallBackAddComment
        {
            get { return _javaScriptCallBackAddComment; }
            set { _javaScriptCallBackAddComment = value; }
        }

        public string ConfirmRemoveCommentMessage
        {
            get { return _confirmRemoveCommentMessage; }
            set { _confirmRemoveCommentMessage = value; }
        }

        public string ObjectID
        {
            get { return _objectID; }
            set { _objectID = value; }
        }

        public int TotalCount { get; set; }

        #endregion

        #region Methods

        internal string RealUserProfileLinkResolver(string userID)
        {
            return UserProfileUrlResolver != null
                       ? UserProfileUrlResolver(userID)
                       : CommonLinkUtility.GetUserProfile(userID);
        }

        public string GetClientScripts(string siteLink, Page Page)
        {
            _isClientScriptRegistered = true;

            var sb = new StringBuilder();

            if (!_simple)
            {
                sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + FCKBasePath + "fckeditor.js\"></script>");
            }

            var scriptLocation = ResolveUrl("~/usercontrols/common/comments/js/comments.js");
            sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + scriptLocation + "\"></script>");

            var paramsScript = string.Format(@"jq(document).ready (function(){{
                    CommentsManagerObj.javaScriptAddCommentFunctionName = '{0}';
                    CommentsManagerObj.javaScriptLoadBBcodeCommentFunctionName = '{1}';
                    CommentsManagerObj.javaScriptUpdateCommentFunctionName = '{2}';
                    CommentsManagerObj.javaScriptCallBackAddComment = '{3}';
                    CommentsManagerObj.javaScriptPreviewCommentFunctionName = '{4}';
                    CommentsManagerObj.isSimple = {5};                    
                    CommentsManagerObj._jsObjName = '{6}';
                    CommentsManagerObj.PID = '{7}';                    
                    CommentsManagerObj.isDisableCtrlEnter = {8};
                    CommentsManagerObj.inactiveMessage = '{9}';
                    CommentsManagerObj.EnableAttachmets = {10};
                    CommentsManagerObj.RemoveAttachButton = '{11}';
                    CommentsManagerObj.FckUploadHandlerPath = '{12}';
                    CommentsManagerObj.maxLevel = {13};
                    ",
                                             _javaScriptAddCommentFunctionName, _javaScriptLoadBBcodeCommentFunctionName,
                                             _javaScriptUpdateCommentFunctionName, _javaScriptCallBackAddComment,
                                             _javaScriptPreviewCommentFunctionName, _simple.ToString().ToLower(), JsObjName,
                                             PID, DisableCtrlEnter.ToString().ToLower(), _inactiveMessage, EnableAttachmets.ToString().ToLower(),
                                             RemoveAttachButton, string.Format("{0}{1}", siteLink, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx"), MaxDepthLevel);

            paramsScript += string.Format(@"
                    CommentsManagerObj.OnEditedCommentJS = '{0}';
                    CommentsManagerObj.OnRemovedCommentJS = '{1}';
                    CommentsManagerObj.OnCanceledCommentJS = '{2}';
                    CommentsManagerObj.FckDomainName = '{3}';",
                                          OnEditedCommentJS, OnRemovedCommentJS, OnCanceledCommentJS, FckDomainName);

            paramsScript +=
                "\n" +
                "if(jq('#comments_Uploader').length>0 && '" + HandlerTypeName + "' != '')\n" +
                "{\n" +
                "new AjaxUpload('comments_Uploader', {\n" +
                "action: 'ajaxupload.ashx?type=" + HandlerTypeName + "',\n" +
                "onComplete: CommentsManagerObj.UploadCallBack\n" +
                "});\n}\n" +
                "});";

            if (!Simple)
            {
                paramsScript += string.Format(@"function InitEditor(){0}CommentsManagerObj.InitEditor('{1}','{2}','{3}','{4}','{5}');{6}", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
                // paramsScript += string.Format(@"jq(document).ready(function(){0}CommentsManagerObj.InitEditor('{1}','{2}','{3}','{4}','{5}');{6});", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
            }

            sb.Append("<script type=\"text/javascript\" language=\"javascript\">" + paramsScript + "</script>");


            if (Simple && !DisableCtrlEnter)
            {
                scriptLocation = ResolveUrl("~/usercontrols/common/comments/js/onReady.js");
                sb.Append(@"<script type=""text/javascript"" language=""javascript"" src=""" + scriptLocation + "\"></script>");
            }

            return sb.ToString();
        }

        private void RegisterClientScripts()
        {
            Page.RegisterBodyScripts(FCKBasePath + "fckeditor.js");
            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/ajaxupload.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/comments/js/comments.js"));

            var paramsScript = string.Format(@"
                    CommentsManagerObj.javaScriptAddCommentFunctionName = '{0}';
                    CommentsManagerObj.javaScriptLoadBBcodeCommentFunctionName = '{1}';
                    CommentsManagerObj.javaScriptUpdateCommentFunctionName = '{2}';
                    CommentsManagerObj.javaScriptCallBackAddComment = '{3}';
                    CommentsManagerObj.javaScriptPreviewCommentFunctionName = '{4}';
                    CommentsManagerObj.isSimple = {5};                    
                    CommentsManagerObj._jsObjName = '{6}';
                    CommentsManagerObj.PID = '{7}';                    
                    CommentsManagerObj.isDisableCtrlEnter = {8};
                    CommentsManagerObj.inactiveMessage = '{9}';
                    CommentsManagerObj.EnableAttachmets = {10};
                    CommentsManagerObj.RemoveAttachButton = '{11}';
                    CommentsManagerObj.FckUploadHandlerPath = '{12}';
                    CommentsManagerObj.maxLevel = {13};
                    ",
                                             _javaScriptAddCommentFunctionName, _javaScriptLoadBBcodeCommentFunctionName,
                                             _javaScriptUpdateCommentFunctionName, _javaScriptCallBackAddComment,
                                             _javaScriptPreviewCommentFunctionName, _simple.ToString().ToLower(), JsObjName,
                                             PID, DisableCtrlEnter.ToString().ToLower(), _inactiveMessage, EnableAttachmets.ToString().ToLower(),
                                             RemoveAttachButton, string.Format("{0}://{1}:{2}{3}", Page.Request.GetUrlRewriter().Scheme, Page.Request.GetUrlRewriter().Host, Page.Request.GetUrlRewriter().Port, VirtualPathUtility.ToAbsolute("~/") + "fckuploader.ashx"), MaxDepthLevel);

            paramsScript += string.Format(@"
                    CommentsManagerObj.OnEditedCommentJS = '{0}';
                    CommentsManagerObj.OnRemovedCommentJS = '{1}';
                    CommentsManagerObj.OnCanceledCommentJS = '{2}';
                    CommentsManagerObj.FckDomainName = '{3}';",
                                          OnEditedCommentJS, OnRemovedCommentJS, OnCanceledCommentJS, FckDomainName);

            paramsScript +=
                "\n" +
                "if(jq('#comments_Uploader').length>0 && '" + HandlerTypeName + "' != '')\n" +
                "{\n" +
                "new AjaxUpload('comments_Uploader', {\n" +
                "action: 'ajaxupload.ashx?type=" + HandlerTypeName + "',\n" +
                "onComplete: CommentsManagerObj.UploadCallBack\n" +
                "});\n}\n";

            if (!Simple)
            {
                paramsScript += string.Format(@"
                        CommentsManagerObj.InitEditor('{1}', '{2}', '{3}', '{4}', '{5}');", "{", FCKBasePath, FCKToolbar, FCKHeight, FCKWidth, FCKEditorAreaCss, "}");
            }

            Page.RegisterInlineScript(paramsScript);


            if (Simple && !DisableCtrlEnter)
            {
                Page.RegisterBodyScripts(ResolveUrl("~/usercontrols/common/comments/js/onReady.js"));
            }
        }

        #endregion

        #region Events

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _simple = Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context);

            //fix for IE 10
            var browser = HttpContext.Current.Request.Browser.Browser;

            var userAgent = Context.Request.Headers["User-Agent"];
            var regExp = new Regex("MSIE 10.0", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var regExpIe11 = new Regex("(?=.*Trident.*rv:11.0).+", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline);
            if (browser == "IE" && regExp.Match(userAgent).Success || regExpIe11.Match(userAgent).Success)
            {
                _simple = true;
            }

            if (Visible)
            {
                RegisterClientScripts();
                _isClientScriptRegistered = true;
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (!_isClientScriptRegistered)
                RegisterClientScripts();

            var sb = new StringBuilder();

            var visibleCommentsCount = TotalCount;

            var isEmpty = CommentsHelper.IsEmptyComments(_items);

            if (_showCaption)
            {
                sb.Append("<div id='commentsTitle' style=\"margin-left:5px;\" class=\"headerPanel\" >" + _commentsTitle + "</div>");
            }
            sb.Append("<a name=\"comments\"></a>");

            sb.Append("<div id=\"noComments\" style=\"" + (!isEmpty ? "display:none;" : "") + "\">" + UserControlsCommonResource.NoComments + "</div>");

            sb.Append("<div id=\"mainContainer\" style='width:100%; margin-top:5px; " + (visibleCommentsCount%2 == 0 ? "border-bottom:1px solid #ddd;" : "") + "word-wrap: break-word;" + (isEmpty ? "display:none;" : "") + "'>");
            sb.Append(RenderComments() + "</div>");

            sb.Append("<br />");

            if (_isShowAddCommentBtn)
            {
                sb.Append("<a id=\"add_comment_btn\" onclick=\"javascript:CommentsManagerObj.AddNewComment();\">" + _addCommentLink + "</a>");
            }
            sb.Append("<div id=\"commentBox\" style=\"margin-top: 5px; display:none;\">");
            sb.Append("<input type=\"hidden\" id=\"hdnParentComment\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnAction\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnCommentID\" value=\"\" />");
            sb.Append("<input type=\"hidden\" id=\"hdnObjectID\" value=\"" + _objectID + "\" />");
            sb.AppendFormat("<input type='hidden' id='EmptyCommentErrorMessage' value='{0}' />", UserControlsCommonResource.EmptyCommentErrorMessage);
            sb.AppendFormat("<input type='hidden' id='CancelNonEmptyCommentErrorMessage' value='{0}' />", UserControlsCommonResource.CancelNonEmptyCommentErrorMessage);

            sb.Append("<a name='add_comment'></a>");
            sb.Append("<div id=\"CommentsFckEditorPlaceHolder_" + JsObjName + "\">");

            if (Simple)
                sb.Append("<textarea id='simpleTextArea' name='simpleTextArea' style='width: 100%; height:124px;'></textarea>");

            sb.Append("</div>");
            sb.Append("<div id=\"comment_attachments\" style=\"padding:5px;\">");
            sb.Append("</div>");
            sb.Append("<input id=\"hdn_comment_attachments\" type=\"hidden\" value=\"\" />");
            sb.Append("<div id='comments_btns' style=\"margin-top:10px;height:20px;\" >");
            sb.Append("<a href=\"javascript:;\"  id=\"btnAddComment\" class=\"button\" onclick=\"javascript:CommentsManagerObj.AddComment_Click();return false;\" style=\"margin-right:8px;\">" + _saveButton + "</a>");

            if (EnableAttachmets)
            {
                sb.Append("<a href=\"javascript:void(0);\" id=\"comments_Uploader\" class=\"button\" style=\"margin-right:8px;\">" + AttachButton + "</a>");
            }

            sb.AppendFormat("<a href='javascript:;' id='btnPreview' class='button' onclick='javascript:CommentsManagerObj.Preview_Click();return false;' style='margin-right:8px;'>{0}</a>", _previewButton);
            sb.AppendFormat("<a href='javascript:void(0);' id='btnCancel' class='button gray cancelFckEditorChangesButtonMarker' name='{1}' onclick='CommentsManagerObj.Cancel();' />{0}</a>", _cancelButton, "CommentsFckEditor_" + JsObjName);

            sb.Append("</div>");

            sb.Append("<div class='clearFix' id='comments_loader' style=\"margin-top:10px;display:none;\" >");
            sb.AppendFormat("<div class='text-medium-describe'>{0}</div><img src='{1}'/>", CommentSendingMsg, LoaderImage);
            sb.Append("</div>");


            sb.Append("<div id=\"previewBox\" style=\"display: none; margin-top: 20px;\">");
            sb.Append("<div class='headerPanel' style=\"margin-top: 0px;\">" + _previewButton + "</div>");
            sb.Append("<div id=\"previewBoxBody\"></div>");
            sb.Append("<br/><a href=\"javascript:void(0);\"  onclick=\"CommentsManagerObj.HidePreview(); return false;\" class=\"button blue\" style=\"margin-right:8px;\">" + _hidePrevuewButton + "</a>");
            sb.Append("</div>");

            sb.Append("</div>");

            writer.Write(sb.ToString());
        }

        #endregion

        #region Methods

        private string RenderComments()
        {
            _commentIndex = 1;

            return RenderComments(_items, 1);
        }

        private string RenderComments(IList<CommentInfo> comments, int commentLevel)
        {
            var sb = new StringBuilder();
            if (comments != null && comments.Count > 0)
            {
                foreach (var comment in comments)
                {
                    comment.CommentBody = HtmlUtility.GetFull(comment.CommentBody, ProductId);
                    sb.Append(
                        CommentsHelper.GetOneCommentHtmlWithContainer(
                            this,
                            comment,
                            commentLevel == 1 || commentLevel > _maxDepthLevel,
                            cmnts => RenderComments(cmnts, commentLevel + 1),
                            ref _commentIndex
                            )
                        );
                }
            }
            return sb.ToString();
        }

        #endregion
    }
}