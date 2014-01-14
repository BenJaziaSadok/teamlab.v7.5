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
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core.Mobile;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Controls;
using ASC.Web.Files.Core;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;
using Global = ASC.Web.Files.Classes.Global;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Web.Files
{
    public partial class DocEditor : MainPage
    {
        #region Member

        private DocumentServiceParams _docParams;
        private string _docKeyForTrack;
        private Guid _tabId = Guid.Empty;
        private bool _fileNew;
        private string _errorMessage;
        private bool _lockVersion;
        private bool _editByUrl;

        protected bool IsMobile;

        #endregion

        #region RequestParams

        private string RequestFileId
        {
            get { return Request[CommonLinkUtility.FileId]; }
        }

        private string RequestShareLinkKey
        {
            get { return Request[CommonLinkUtility.DocShareKey] ?? string.Empty; }
        }

        private bool _valideShareLink;

        private string RequestFileUrl
        {
            get { return Request[CommonLinkUtility.FileUri]; }
        }

        private bool RequestView
        {
            get { return (Request[CommonLinkUtility.Action] ?? "").Equals("view", StringComparison.InvariantCultureIgnoreCase); }
        }

        private bool RequestEmbedded
        {
            get
            {
                return
                    Global.EnableEmbedded
                    && (Request[CommonLinkUtility.Action] ?? "").Equals("embedded", StringComparison.InvariantCultureIgnoreCase)
                    && !string.IsNullOrEmpty(RequestShareLinkKey);
            }
        }

        protected bool ItsTry
        {
            get
            {
                return !RequestView
                       && string.IsNullOrEmpty(RequestFileId)
                       && string.IsNullOrEmpty(RequestShareLinkKey)
                       && string.IsNullOrEmpty(RequestFileUrl)
                       && (SecurityContext.IsAuthenticated || CoreContext.Configuration.YourDocsDemo);
            }
        }

        #endregion

        #region Event

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            _valideShareLink = !string.IsNullOrEmpty(FileShareLink.Parse(RequestShareLinkKey));
            CheckAuth();

            if (!TenantExtra.GetTenantQuota().DocsEdition)
                Response.Redirect(CommonLinkUtility.FileHandlerPath + "?" + Context.Request.QueryString
                                  + (string.IsNullOrEmpty(Context.Request[CommonLinkUtility.Action]) ? "&" + CommonLinkUtility.Action + "=view" : string.Empty));
        }

        private void CheckAuth()
        {
            if (SecurityContext.IsAuthenticated)
                return;
            if (ItsTry)
                return;
            if (_valideShareLink)
                return;

            var refererURL = Request.Url.AbsoluteUri;
            Session["refererURL"] = refererURL;
            Response.Redirect("~/auth.aspx");
        }

        protected override void OnLoad(EventArgs e)
        {
            IsMobile = MobileDetector.IsRequestMatchesMobile(Context);
            PageLoad();
            InitScript();
            RenderCustomScript();
        }

        private void PageLoad()
        {
            var editPossible = !RequestEmbedded && !IsMobile;
            var isExtenral = false;

            File file;
            var fileUri = string.Empty;
            if (!ItsTry)
            {
                try
                {
                    if (string.IsNullOrEmpty(RequestFileUrl))
                    {
                        _fileNew = !string.IsNullOrEmpty(Request[UrlConstant.New]) && Request[UrlConstant.New] == "true";

                        var ver = string.IsNullOrEmpty(Request[CommonLinkUtility.Version]) ? -1 : Convert.ToInt32(Request[CommonLinkUtility.Version]);

                        file = DocumentServiceHelper.GetParams(RequestFileId, ver, RequestShareLinkKey, _fileNew, editPossible, !RequestView, out _docParams);

                        _fileNew = file.Version == 1 && file.ConvertedType != null && _fileNew && file.CreateOn == file.ModifiedOn;
                    }
                    else
                    {
                        isExtenral = true;

                        fileUri = RequestFileUrl;
                        var fileTitle = Request[CommonLinkUtility.FileTitle];
                        if (string.IsNullOrEmpty(fileTitle))
                            fileTitle = Path.GetFileName(HttpUtility.UrlDecode(fileUri)) ?? "";

                        if (CoreContext.Configuration.Standalone)
                        {
                            try
                            {
                                var webRequest = WebRequest.Create(RequestFileUrl);
                                using (var response = webRequest.GetResponse())
                                using (var responseStream = new ResponseStream(response))
                                {
                                    fileUri = DocumentServiceConnector.GetExternalUri(responseStream, MimeMapping.GetMimeMapping(fileTitle), "new");
                                }
                            }
                            catch (Exception error)
                            {
                                Global.Logger.Error("Cannot receive external url for \"" + RequestFileUrl + "\"", error);
                            }
                        }

                        file = new File
                            {
                                ID = fileUri.GetHashCode(),
                                Title = Global.ReplaceInvalidCharsAndTruncate(fileTitle)
                            };

                        file = DocumentServiceHelper.GetParams(file, true, true, true, false, false, false, out _docParams);
                        _docParams.CanEdit = editPossible && !CoreContext.Configuration.Standalone;
                        _editByUrl = true;

                        _docParams.FileUri = fileUri;
                    }
                }
                catch (Exception ex)
                {
                    _errorMessage = ex.Message;
                    return;
                }
            }
            else
            {
                FileType tryType;
                try
                {
                    tryType = (FileType)Enum.Parse(typeof(FileType), Request[CommonLinkUtility.TryParam]);
                }
                catch
                {
                    tryType = FileType.Document;
                }

                var fileTitle = "Demo";
                fileTitle += FileUtility.InternalExtension[tryType];

                var relativeUri = string.Format(CommonLinkUtility.FileHandlerPath + UrlConstant.ParamsDemo, tryType);
                fileUri = new Uri(Request.Url, relativeUri).ToString();

                file = new File
                    {
                        ID = Guid.NewGuid(),
                        Title = Global.ReplaceInvalidCharsAndTruncate(fileTitle)
                    };

                file = DocumentServiceHelper.GetParams(file, true, true, true, editPossible, editPossible, true, out _docParams);

                _docParams.FileUri = fileUri;
                _editByUrl = true;
            }

            if (_docParams.ModeWrite && FileConverter.MustConvert(file))
            {
                try
                {
                    file = FileConverter.ExecDuplicate(file, RequestShareLinkKey);
                }
                catch (Exception e)
                {
                    _docParams = null;
                    _errorMessage = e.Message;
                    return;
                }

                var comment = "#message/" + HttpUtility.UrlEncode(FilesCommonResource.CopyForEdit);

                Response.Redirect(CommonLinkUtility.GetFileWebEditorUrl(file.ID) + comment);
                return;
            }

            Title = HeaderStringHelper.GetPageTitle(file.Title);

            if (string.IsNullOrEmpty(_docParams.FolderUrl))
            {
                _docParams.FolderUrl = Request[CommonLinkUtility.FolderUrl] ?? "";
            }
            if (MobileDetector.IsRequestMatchesMobile(Context.Request.UserAgent, true))
            {
                _docParams.FolderUrl = string.Empty;
            }

            if (RequestEmbedded)
            {
                _docParams.Type = DocumentServiceParams.EditorType.Embedded;

                var shareLinkParam = "&" + CommonLinkUtility.DocShareKey + "=" + RequestShareLinkKey;
                _docParams.ViewerUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FilesBaseAbsolutePath + CommonLinkUtility.EditorPage + "?" + CommonLinkUtility.Action + "=view" + shareLinkParam);
                _docParams.DownloadUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FileHandlerPath + "?" + CommonLinkUtility.Action + "=download" + shareLinkParam);
                _docParams.EmbeddedUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FilesBaseAbsolutePath + CommonLinkUtility.EditorPage + "?" + CommonLinkUtility.Action + "=embedded" + shareLinkParam);
            }
            else
            {
                _docParams.Type = IsMobile ? DocumentServiceParams.EditorType.Mobile : DocumentServiceParams.EditorType.Desktop;

                if (FileSharing.CanSetAccess(file))
                {
                    _docParams.SharingSettingsUrl = CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FilesBaseAbsolutePath + "share.aspx" + "?" + CommonLinkUtility.FileId + "=" + file.ID + "&" + CommonLinkUtility.FileTitle + "=" + HttpUtility.UrlEncode(file.Title));
                }
            }

            if (!isExtenral)
            {
                _docKeyForTrack = DocumentServiceHelper.GetDocKey(file.ID, -1, DateTime.MinValue);

                if (!ItsTry)
                    FileMarker.RemoveMarkAsNew(file);
            }

            if (_docParams.ModeWrite)
            {
                _tabId = FileLocker.Add(file.ID, _fileNew);
                _lockVersion = FileLocker.LockVersion(file.ID);

                if (ItsTry)
                {
                    AppendAuthControl();
                }
            }
            else
            {
                _docParams.LinkToEdit = _editByUrl
                                            ? CommonLinkUtility.GetFullAbsolutePath(string.Format(CommonLinkUtility.FileWebEditorExternalUrlString, HttpUtility.UrlEncode(fileUri), file.Title))
                                            : FileConverter.MustConvert(_docParams.File)
                                                  ? CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.GetFileWebEditorUrl(file.ID))
                                                  : string.Empty;
            }

            if (CoreContext.Configuration.YourDocsDemo && IsMobile)
            {
                _docParams.CanEdit = false;
            }
        }

        private void InitScript()
        {
            var inlineScript = new StringBuilder();
            inlineScript.Append("\n<script language=\"javascript\" type=\"text/javascript\" src=\"" + CommonLinkUtility.DocServiceApiUrl + "\"></script>");

            inlineScript.Append("\n<script language=\"javascript\" type=\"text/javascript\">");

            inlineScript.AppendFormat("\nASC.Files.Constants.URL_WCFSERVICE = \"{0}\";" +
                                      "ASC.Files.Constants.URL_HANDLER_SAVE = \"{1}\";" +
                                      "ASC.Files.Constants.URL_FILES_START = \"{2}\";" +
                                      "ASC.Files.Constants.REQUEST_TRACK_DELAY = {3};",
                                      PathProvider.GetFileServicePath,
                                      CommonLinkUtility.FileHandlerPath + UrlConstant.ParamsSave,
                                      CommonLinkUtility.FilesBaseAbsolutePath,
                                      FileLocker.EditTimeout.TotalMilliseconds);

            if (SecurityContext.IsAuthenticated && !CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor() || ItsTry)
                inlineScript.AppendFormat("\nASC.Files.Constants.URL_HANDLER_CREATE = \"{0}\";",
                                          CommonLinkUtility.GetFullAbsolutePath(CommonLinkUtility.FileHandlerPath));

            inlineScript.AppendFormat("\nASC.Files.Editor.fileSaveAsNew = \"{0}\";" +
                                      "ASC.Files.Editor.docKeyForTrack = \"{1}\";" +
                                      "ASC.Files.Editor.shareLinkParam = \"{2}\";" +
                                      "ASC.Files.Editor.serverErrorMessage = \"{3}\";" +
                                      "ASC.Files.Editor.editByUrl = ({4} == true);" +
                                      "ASC.Files.Editor.mustAuth = ({5} == true);" +
                                      "ASC.Files.Editor.lockVersion = ({6} == true);" +
                                      "ASC.Files.Editor.tabId = \"{7}\";",
                                      _fileNew ? "&" + UrlConstant.New + "=true" : "",
                                      _docKeyForTrack,
                                      string.IsNullOrEmpty(RequestShareLinkKey) ? string.Empty : "&" + CommonLinkUtility.DocShareKey + "=" + RequestShareLinkKey,
                                      _errorMessage.HtmlEncode(),
                                      _editByUrl.ToString().ToLower(),
                                      (!SecurityContext.IsAuthenticated && ItsTry).ToString().ToLower(),
                                      _lockVersion.ToString().ToLower(),
                                      _tabId);

            if (_fileNew)
            {
                inlineScript.AppendFormat("\nASC.Files.Editor.options = {{ \"isEmpty\" : ({0} == true) }};",
                                          _fileNew.ToString().ToLower());
            }
            else if (!string.IsNullOrEmpty(Request["options"]))
            {
                inlineScript.AppendFormat("\nASC.Files.Editor.options = {0};",
                                          Request["options"]);
            }

            inlineScript.AppendFormat("\nASC.Files.Editor.docServiceParams = {0};",
                                      DocumentServiceParams.Serialize(_docParams));

            inlineScript.Append("</script>");

            ScriptsPlaceHolder.Controls.Add(new Literal { Text = inlineScript.ToString() });
        }

        private void RenderCustomScript()
        {
            var inlineScript = new StringBuilder();
            //custom scripts
            foreach (var script in SetupInfo.CustomScripts.Where(script => !String.IsNullOrEmpty(script)))
            {
                inlineScript.Append("<script language=\"javascript\" src=\"" + script + "\" type=\"text/javascript\"></script>");
            }

            ScriptsPlaceHolder.Controls.Add(new Literal { Text = inlineScript.ToString() });
        }

        private void AppendAuthControl()
        {
            if (SecurityContext.IsAuthenticated || !CoreContext.Configuration.YourDocsDemo) return;
            CommonPlaceHolder.Controls.Add(LoadControl(LoginDialog.Location));
        }

        #endregion
    }
}