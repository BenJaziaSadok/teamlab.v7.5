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
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using System.Web.Configuration;
using System.Web.Services;
using ASC.Common.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Data.Storage.S3;
using ASC.Files.Core;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Core;
using ASC.Web.Files.Resources;
using ASC.Web.Files.Services.DocumentService;
using ASC.Web.Files.Utils;
using ASC.Web.Studio.Core;
using ASC.Web.Studio.UserControls.Statistics;
using ASC.Web.Studio.Utility;
using File = ASC.Files.Core.File;
using SecurityContext = ASC.Core.SecurityContext;
using System.Diagnostics;
using MimeMapping = System.Web.MimeMapping;

namespace ASC.Web.Files
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class FileHandler : AbstractHttpAsyncHandler
    {
        public static string FileHandlerPath
        {
            get { return CommonLinkUtility.FileHandlerPath; }
        }

        public override void OnProcessRequest(HttpContext context)
        {
            var action = context.Request[CommonLinkUtility.Action];
            if (string.IsNullOrEmpty(action))
                throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);

            action = action.ToLower();

            var publicActions = new[] { "view", "download", "save", "stream" };

            if (!publicActions.Contains(action)
                && !SecurityContext.AuthenticateMe(CookiesManager.GetCookies(CookiesType.AuthKey)))
            {
                context.Response.Redirect("~/");
                return;
            }

            if (TenantStatisticsProvider.IsNotPaid())
            {
                context.Response.Redirect(TenantExtra.GetTariffPageLink());
            }

            try
            {
                switch (action)
                {
                    case "view":
                        DownloadFile(context, true);
                        break;
                    case "download":
                        DownloadFile(context, false);
                        break;
                    case "bulk":
                        BulkDownloadFile(context);
                        break;
                    case "save":
                        SaveFile(context);
                        break;
                    case "stream":
                        StreamFile(context);
                        break;
                    case "create":
                        CreateFile(context);
                        break;
                    case "redirect":
                        Redirect(context);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

            }
            catch (InvalidOperationException e)
            {
                throw new HttpException((int)HttpStatusCode.InternalServerError, FilesCommonResource.ErrorMassage_BadRequest, e);
            }
        }

        private static void BulkDownloadFile(HttpContext context)
        {
            var store = Global.GetStore();
            var path = string.Format(@"{0}\{1}.zip", SecurityContext.CurrentAccount.ID, UrlConstant.DownloadTitle);
            if (!store.IsFile(FileConstant.StorageDomainTmp, path))
            {
                context.Response.Redirect("~/404.htm");
            }
            else
            {
                if (store is S3Storage)
                {
                    var url = store.GetPreSignedUri(FileConstant.StorageDomainTmp, path, TimeSpan.FromMinutes(5), null).ToString();
                    context.Response.Redirect(url);
                }
                else
                {
                    context.Response.Clear();
                    context.Response.ContentType = "application/zip";
                    context.Response.AddHeader("Content-Disposition", ContentDispositionUtil.GetHeaderValue(UrlConstant.DownloadTitle + ".zip"));

                    using (var readStream = store.IronReadStream(FileConstant.StorageDomainTmp, path, 40))
                    {
                        context.Response.AddHeader("Content-Length", readStream.Length.ToString());
                        readStream.StreamCopyTo(context.Response.OutputStream);
                    }
                    try
                    {
                        context.Response.Flush();
                        context.Response.End();
                    }
                    catch (HttpException)
                    {
                    }
                }
            }
        }

        private static void DownloadFile(HttpContext context, bool inline)
        {
            if (!string.IsNullOrEmpty(context.Request[CommonLinkUtility.TryParam]))
            {
                DownloadTry(context);
                return;
            }

            try
            {
                var id = context.Request[CommonLinkUtility.FileId];
                var shareLinkKey = context.Request[CommonLinkUtility.DocShareKey] ?? "";

                using (var fileDao = Global.DaoFactory.GetFileDao())
                {
                    File file;
                    var checkLink = FileShareLink.Check(shareLinkKey, true, fileDao, out file);
                    if (!checkLink && file == null)
                    {
                        int version;
                        file = int.TryParse(context.Request[CommonLinkUtility.Version], out version) && version > 0
                                   ? fileDao.GetFile(id, version)
                                   : fileDao.GetFile(id);
                    }

                    if (file == null)
                    {
                        context.Response.Redirect("~/404.htm");

                        return;
                    }

                    if (!checkLink && !Global.GetFilesSecurity().CanRead(file))
                    {
                        context.Response.Redirect((context.Request.UrlReferrer != null
                                                       ? context.Request.UrlReferrer.ToString()
                                                       : PathProvider.StartURL)
                                                  + "#" + UrlConstant.Error + "/" +
                                                  HttpUtility.UrlEncode(FilesCommonResource.ErrorMassage_SecurityException_ReadFile));
                        return;
                    }

                    if (!fileDao.IsExistOnStorage(file))
                    {
                        Global.Logger.ErrorFormat("Download file error. File is not exist on storage. File id: {0}.", file.ID);
                        context.Response.Redirect("~/404.htm");

                        return;
                    }

                    FileMarker.RemoveMarkAsNew(file);

                    context.Response.Clear();
                    context.Response.ContentType = MimeMapping.GetMimeMapping(file.Title);
                    context.Response.Charset = "utf-8";

                    var browser = context.Request.Browser.Browser;
                    var title = file.Title.Replace(',', '_');

                    var ext = FileUtility.GetFileExtension(file.Title);

                    var outType = string.Empty;
                    var curQuota = TenantExtra.GetTenantQuota();
                    if (curQuota.DocsEdition || FileUtility.InternalExtension.Values.Contains(ext))
                        outType = context.Request[CommonLinkUtility.OutType];

                    if (!string.IsNullOrEmpty(outType) && !inline)
                    {
                        outType = outType.Trim();
                        if (FileUtility.ExtsConvertible[ext].Contains(outType))
                        {
                            ext = outType;

                            title = FileUtility.ReplaceFileExtension(title, ext);
                        }
                    }

                    context.Response.AddHeader("Content-Disposition", ContentDispositionUtil.GetHeaderValue(title, inline));

                    if (inline && string.Equals(context.Request.Headers["If-None-Match"], GetEtag(file)))
                    {
                        //Its cached. Reply 304
                        context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                        context.Response.Cache.SetETag(GetEtag(file));
                    }
                    else
                    {
                        context.Response.CacheControl = "public";
                        context.Response.Cache.SetETag(GetEtag(file));
                        context.Response.Cache.SetCacheability(HttpCacheability.Public);

                        Stream fileStream = null;

                        try
                        {
                            if (file.ContentLength <= SetupInfo.AvailableFileSize)
                            {
                                if (file.ConvertedType == null && (string.IsNullOrEmpty(outType) || inline))
                                {

                                    context.Response.AddHeader("Content-Length", file.ContentLength.ToString(CultureInfo.InvariantCulture));

                                    if (fileDao.IsSupportedPreSignedUri(file))
                                    {
                                        context.Response.Redirect(fileDao.GetPreSignedUri(file, TimeSpan.FromHours(1)).ToString(), true);

                                        return;
                                    }

                                    fileStream = fileDao.GetFileStream(file);
                                }
                                else
                                    fileStream = FileConverter.Exec(file, ext);

                                fileStream.StreamCopyTo(context.Response.OutputStream);

                                if (!context.Response.IsClientConnected)
                                    Global.Logger.Error(String.Format("Download file error {0} {1} Connection is lost. Too long to buffer the file", file.Title, file.ID));

                                context.Response.Flush();
                            }
                            else
                            {
                                long offset = 0;

                                if (context.Request.Headers["Range"] != null)
                                {
                                    context.Response.StatusCode = 206;
                                    var range = context.Request.Headers["Range"].Split(new[] { '=', '-' });
                                    offset = Convert.ToInt64(range[1]);
                                }

                                if (offset > 0)
                                    Global.Logger.Info("Starting file download offset is " + offset);

                                context.Response.AddHeader("Connection", "Keep-Alive");
                                context.Response.AddHeader("Accept-Ranges", "bytes");

                                if (offset > 0)
                                {
                                    context.Response.AddHeader("Content-Range", String.Format(" bytes {0}-{1}/{2}", offset, file.ContentLength - 1, file.ContentLength));
                                }

                                var dataToRead = file.ContentLength;
                                const int bufferSize = 1024;
                                var buffer = new Byte[bufferSize];

                                if (file.ConvertedType == null && (string.IsNullOrEmpty(outType) || inline))
                                {
                                    if (fileDao.IsSupportedPreSignedUri(file))
                                    {
                                        context.Response.Redirect(fileDao.GetPreSignedUri(file, TimeSpan.FromHours(1)).ToString(), true);

                                        return;
                                    }

                                    fileStream = fileDao.GetFileStream(file, offset);
                                    context.Response.AddHeader("Content-Length", (file.ContentLength - offset).ToString(CultureInfo.InvariantCulture));
                                }
                                else
                                {
                                    fileStream = FileConverter.Exec(file, ext);

                                    if (offset > 0)
                                    {
                                        var startBytes = offset;

                                        while (startBytes > 0)
                                        {
                                            long readCount;

                                            if (bufferSize >= startBytes)
                                            {
                                                readCount = startBytes;
                                            }
                                            else
                                            {
                                                readCount = bufferSize;
                                            }

                                            var length = fileStream.Read(buffer, 0, (int)readCount);

                                            startBytes -= length;
                                        }
                                    }
                                }

                                while (dataToRead > 0)
                                {
                                    int length;

                                    try
                                    {
                                        length = fileStream.Read(buffer, 0, bufferSize);
                                    }
                                    catch (HttpException exception)
                                    {
                                        Global.Logger.Error(
                                            String.Format("Read from stream is error. Download file {0} {1}. Maybe Connection is lost.?? Error is {2} ",
                                                          file.Title,
                                                          file.ID,
                                                          exception
                                                ));

                                        throw;
                                    }

                                    if (context.Response.IsClientConnected)
                                    {
                                        context.Response.OutputStream.Write(buffer, 0, length);
                                        dataToRead = dataToRead - length;
                                    }
                                    else
                                    {
                                        dataToRead = -1;
                                        Global.Logger.Error(String.Format("IsClientConnected is false. Why? Download file {0} {1} Connection is lost. ", file.Title, file.ID));
                                    }
                                }
                            }
                        }
                        catch (HttpException e)
                        {
                            throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
                        }
                        finally
                        {
                            if (fileStream != null)
                            {
                                fileStream.Flush();
                                fileStream.Close();
                                fileStream.Dispose();
                            }
                        }

                        try
                        {
                            context.Response.End();
                        }
                        catch (HttpException)
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Get stack trace for the exception with source file information
                var st = new StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                Global.Logger.ErrorFormat("Url: {0} {1} IsClientConnected:{2}, line number:{3} frame:{4}", context.Request.Url, ex, context.Response.IsClientConnected, line, frame);
                context.Response.StatusCode = 400;
                context.Response.Write(HttpUtility.HtmlEncode(ex.Message));
            }
        }

        private static void DownloadTry(HttpContext context)
        {
            FileType tryType;
            try
            {
                tryType = (FileType)Enum.Parse(typeof(FileType), context.Request[CommonLinkUtility.TryParam]);
            }
            catch
            {
                tryType = FileType.Document;
            }

            var title = string.IsNullOrEmpty(context.Request[CommonLinkUtility.FileTitle]) ? "Demo" : "new";
            title += FileUtility.InternalExtension[tryType];

            context.Response.Clear();
            context.Response.ContentType = MimeMapping.GetMimeMapping(title);
            context.Response.Charset = "utf-8";

            var browser = context.Request.Browser.Browser;
            var format = browser == "IE" || browser == "Safari"
                             ? "{0}; filename=\"{1}\""
                             : "{0}; filename*=utf-8''{1}";

            var filename = browser == "Safari" ? title : HttpUtility.UrlPathEncode(title);
            var contentDisposition = string.Format(format, "attachment", filename);
            context.Response.AddHeader("Content-Disposition", contentDisposition);

            //NOTE: always pass files through handler
            using (var readStream = Global.GetStoreTemplate().IronReadStream("", title, 10))
            {
                context.Response.AddHeader("Content-Length", readStream.Length.ToString()); //BUG:Can be bugs
                readStream.StreamCopyTo(context.Response.OutputStream);
            }

            try
            {
                context.Response.Flush();
                context.Response.End();
            }
            catch (HttpException)
            {
            }
        }

        private static void StreamFile(HttpContext context)
        {
            try
            {
                var id = context.Request[CommonLinkUtility.FileId];
                var auth = context.Request[CommonLinkUtility.AuthKey];
                int version;
                int.TryParse(context.Request[CommonLinkUtility.Version], out version);

                int validateTimespan;
                int.TryParse(WebConfigurationManager.AppSettings["files.stream-url-minute"], out validateTimespan);
                if (validateTimespan <= 0) validateTimespan = 5;

                var validateResult = EmailValidationKeyProvider.ValidateEmailKey(id + version, auth, TimeSpan.FromMinutes(validateTimespan));
                if (validateResult != EmailValidationKeyProvider.ValidationResult.Ok)
                {
                    var exc = new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException);
                    Global.Logger.Error(string.Format("{0} {1}: {2}", CommonLinkUtility.AuthKey, validateResult, context.Request.Url), exc);

                    throw exc;
                }

                using (var fileDao = Global.DaoFactory.GetFileDao())
                {
                    var file = version > 0
                                   ? fileDao.GetFile(id, version)
                                   : fileDao.GetFile(id);
                    using (var stream = fileDao.GetFileStream(file))
                    {
                        context.Response.AddHeader("Content-Length", stream.Length.ToString(CultureInfo.InvariantCulture));
                        stream.StreamCopyTo(context.Response.OutputStream);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.Write(ex.Message);
                Global.Logger.Error(ex.Message, ex);
            }
            try
            {
                context.Response.Flush();
                context.Response.End();
            }
            catch (HttpException)
            {
            }
        }

        private static string GetEtag(File file)
        {
            return file.ID + ":" + file.Version + ":" + file.Title.GetHashCode();
        }

        private static void SaveFile(HttpContext context)
        {
            try
            {
                var shareLinkKey = context.Request[CommonLinkUtility.DocShareKey] ?? "";

                var fileID = context.Request[CommonLinkUtility.FileId];

                if (string.IsNullOrEmpty(fileID)) throw new ArgumentNullException(fileID);

                var downloadUri = context.Request[CommonLinkUtility.FileUri];
                if (string.IsNullOrEmpty(downloadUri)) throw new ArgumentNullException(downloadUri);

                using (var fileDao = Global.DaoFactory.GetFileDao())
                {
                    File file;

                    var checkLink = FileShareLink.Check(shareLinkKey, false, fileDao, out file);
                    if (!checkLink && file == null)
                        file = fileDao.GetFile(fileID);

                    if (file == null) throw new HttpException((int)HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FileNotFound);
                    if (!checkLink && (!Global.GetFilesSecurity().CanEdit(file) || CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor())) throw new SecurityException(FilesCommonResource.ErrorMassage_SecurityException_EditFile);
                    if (file.RootFolderType == FolderType.TRASH) throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_ViewTrashItem);

                    var versionEdit = context.Request[CommonLinkUtility.Version];
                    var currentType = file.ConvertedType ?? FileUtility.GetFileExtension(file.Title);
                    var newType = FileUtility.GetFileExtension(downloadUri);
                    var updateVersion = file.Version > 1 || file.ConvertedType == null || string.IsNullOrEmpty(context.Request[UrlConstant.New]);

                    if ((string.IsNullOrEmpty(versionEdit) || file.Version <= Convert.ToInt32(versionEdit) || currentType != newType)
                        && updateVersion
                        && !FileLocker.LockVersion(file.ID))
                    {
                        file.Version++;
                    }

                    file.ConvertedType = newType;

                    if (file.ProviderEntry && !newType.Equals(currentType))
                    {
                        var key = DocumentServiceConnector.GenerateRevisionId(downloadUri);

                        DocumentServiceConnector.GetConvertedUri(downloadUri, newType, currentType, key, false, out downloadUri);
                    }

                    var req = (HttpWebRequest)WebRequest.Create(downloadUri);

                    using (var editedFileStream = new ResponseStream(req.GetResponse()))
                    {
                        file.ContentLength = editedFileStream.Length;

                        file = fileDao.SaveFile(file, editedFileStream);
                    }

                    bool checkRight;
                    var tabId = new Guid(context.Request["tabId"]);
                    FileLocker.ProlongLock(file.ID, tabId, true, out checkRight);
                    if (checkRight) FileLocker.ChangeRight(file.ID, SecurityContext.CurrentAccount.ID, false);

                    FileMarker.MarkAsNew(file);
                    FileMarker.RemoveMarkAsNew(file);
                }
            }
            catch (Exception ex)
            {
                Global.Logger.Error(ex.Message, ex);
                context.Response.Write("{ \"error\": \"true\", \"message\": \"" + ex.Message + "\" }");
            }
        }

        private static void CreateFile(HttpContext context)
        {
            var folderId = context.Request[CommonLinkUtility.FolderId];
            if (string.IsNullOrEmpty(folderId))
                folderId = Global.FolderMy.ToString();
            Folder folder;

            using (var folderDao = Global.DaoFactory.GetFolderDao())
            {
                folder = folderDao.GetFolder(folderId);
            }
            if (folder == null) throw new HttpException((int)HttpStatusCode.NotFound, FilesCommonResource.ErrorMassage_FolderNotFound);
            if (!Global.GetFilesSecurity().CanCreate(folder)) throw new HttpException((int)HttpStatusCode.Forbidden, FilesCommonResource.ErrorMassage_SecurityException_Create);

            File file;
            var fileUri = context.Request[CommonLinkUtility.FileUri];
            var fileTitle = context.Request[CommonLinkUtility.FileTitle];
            if (!string.IsNullOrEmpty(fileUri))
            {
                file = CreateFileFromUri(folder, fileUri, fileTitle);
            }
            else
            {
                var template = context.Request[UrlConstant.Template];
                var docType = context.Request[UrlConstant.DocumentType];
                file = CreateFileFromTemplate(folder, template, fileTitle, docType);
            }

            FileMarker.MarkAsNew(file);

            context.Response.Redirect(
                (context.Request["openfolder"] ?? "").Equals("true")
                    ? PathProvider.GetFolderUrl(file.FolderID)
                    : CommonLinkUtility.GetFileWebEditorUrl(file.ID));
        }

        private static File CreateFileFromTemplate(Folder folder, string template, string fileTitle, string docType)
        {
            var storeTemp = Global.GetStoreTemplate();

            var lang = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).GetCulture().TwoLetterISOLanguageName;

            var templatePath = FileConstant.TemplateDocPath + lang + "/";
            if (!storeTemp.IsDirectory(templatePath))
                templatePath = FileConstant.TemplateDocPath + "default/";

            string templateName;
            var fileExt = FileUtility.InternalExtension[FileType.Document];
            if (!string.IsNullOrEmpty(docType))
            {
                var tmpFileType = DocumentServiceParams.DocType.FirstOrDefault(r => r.Value.Equals(docType, StringComparison.OrdinalIgnoreCase));
                string tmpFileExt;
                FileUtility.InternalExtension.TryGetValue(tmpFileType.Key, out tmpFileExt);
                if (!string.IsNullOrEmpty(tmpFileExt))
                    fileExt = tmpFileExt;
            }

            if (string.IsNullOrEmpty(template))
            {
                templateName = "new" + fileExt;
                templatePath = templateName;
            }
            else
            {
                templateName = template + fileExt;
                templatePath += templateName;

                if (!storeTemp.IsFile(templatePath))
                {
                    templatePath = FileConstant.TemplateDocPath + "default/";
                    templatePath += templateName;
                }
            }

            if (string.IsNullOrEmpty(fileTitle))
            {
                fileTitle = templateName;
            }
            else
            {
                fileTitle = fileTitle + fileExt;
            }

            var file = new File
                {
                    Title = fileTitle,
                    ContentLength = storeTemp.GetFileSize(templatePath),
                    FolderID = folder.ID
                };

            file.ConvertedType = FileUtility.GetInternalExtension(file.Title);

            using (var fileDao = Global.DaoFactory.GetFileDao())
            using (var stream = storeTemp.IronReadStream("", templatePath, 10))
            {
                return fileDao.SaveFile(file, stream);
            }
        }

        private static File CreateFileFromUri(Folder folder, string fileUri, string fileTitle)
        {
            if (string.IsNullOrEmpty(fileTitle))
                fileTitle = Path.GetFileName(HttpUtility.UrlDecode(fileUri));

            var file = new File
                {
                    Title = fileTitle,
                    FolderID = folder.ID
                };

            var req = (HttpWebRequest)WebRequest.Create(fileUri);

            using (var fileDao = Global.DaoFactory.GetFileDao())
            using (var fileStream = new ResponseStream(req.GetResponse()))
            {
                file.ContentLength = fileStream.Length;

                return fileDao.SaveFile(file, fileStream);
            }
        }

        private static void Redirect(HttpContext context)
        {
            var urlRedirect = string.Empty;
            int id;
            var folderId = context.Request[CommonLinkUtility.FolderId];
            if (!string.IsNullOrEmpty(folderId) && int.TryParse(folderId, out id))
            {
                try
                {
                    urlRedirect = PathProvider.GetFolderUrl(id);
                }
                catch (ArgumentNullException e)
                {
                    throw new HttpException((int)HttpStatusCode.BadRequest, e.Message);
                }
            }

            var fileId = context.Request[CommonLinkUtility.FileId];
            if (!string.IsNullOrEmpty(fileId) && int.TryParse(fileId, out id))
            {
                using (var fileDao = Global.DaoFactory.GetFileDao())
                {
                    var file = fileDao.GetFile(id);
                    if (file == null)
                    {
                        context.Response.Redirect(PathProvider.StartURL
                                                  + "#" + UrlConstant.Error + "/" +
                                                  HttpUtility.UrlEncode(FilesCommonResource.ErrorMassage_FileNotFound));
                        return;
                    }

                    urlRedirect = CommonLinkUtility.GetFileWebPreviewUrl(file.Title, file.ID);
                }
            }

            if (string.IsNullOrEmpty(urlRedirect))
                throw new HttpException((int)HttpStatusCode.BadRequest, FilesCommonResource.ErrorMassage_BadRequest);
            context.Response.Redirect(urlRedirect);
        }
    }
}