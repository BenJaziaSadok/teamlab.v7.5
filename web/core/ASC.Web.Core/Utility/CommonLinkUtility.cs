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
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Configuration;
using ASC.Core;
using ASC.Core.Users;
using ASC.Security.Cryptography;
using ASC.Web.Core;
using log4net;

namespace ASC.Web.Studio.Utility
{
    public enum ManagementType
    {
        General = 0,
        ProductsAndInstruments = 1,
        Mail = 2,
        AuthorizationKeys = 3,
        Statistic = 5,
        Account = 6,
        Customization = 7,
        AccessRights = 8,
        HelpCenter = 9
    }

    public static class CommonLinkUtility
    {
        private static readonly Regex RegFilePathTrim = new Regex("/[^/]*\\.aspx", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static Uri _serverRoot;
        private static string _vpath;
        private static string _hostname;

        public const string ParamName_ProductSysName = "product";
        public const string ParamName_UserUserName = "user";
        public const string ParamName_UserUserID = "uid";

        static CommonLinkUtility()
        {
            try
            {
                var uriBuilder = new UriBuilder(Uri.UriSchemeHttp, "localhost");
                if (HttpContext.Current != null && HttpContext.Current.Request != null)
                {
                    var u = HttpContext.Current.Request.GetUrlRewriter();
                    uriBuilder = new UriBuilder(u.Scheme, "localhost", u.Port);
                }
                _serverRoot = uriBuilder.Uri;

                try
                {
                    _hostname = Dns.GetHostName();
                }
                catch { }

                DocServiceApiUrl = WebConfigurationManager.AppSettings["files.docservice.url.api"] ?? "";
            }
            catch (Exception error)
            {
                LogManager.GetLogger("ASC.Web").Error(error.StackTrace);
            }
        }

        public static void Initialize(string serverUri)
        {
            if (string.IsNullOrEmpty(serverUri)) throw new ArgumentNullException("serverUri");

            var uri = new Uri(serverUri.Replace('*', 'x').Replace('+', 'x'));
            _serverRoot = new UriBuilder(uri.Scheme, _serverRoot.Host, uri.Port).Uri;
            _vpath = "/" + uri.AbsolutePath.Trim('/');
        }

        public static string VirtualRoot
        {
            get { return ToAbsolute("~"); }
        }

        public static string ServerRootPath
        {
            get
            {
                /*
                 * NOTE: fixed bug with warning on SSL certificate when coming from Email to teamlab. 
                 * Valid only for users that have custom domain set. For that users we should use a http scheme
                 * Like https://mydomain.com that maps to <alias>.teamlab.com
                */
                var basedomain = WebConfigurationManager.AppSettings["core.base-domain"];
                var http = !string.IsNullOrEmpty(basedomain) && !CoreContext.TenantManager.GetCurrentTenant().TenantDomain.EndsWith("." + basedomain, StringComparison.OrdinalIgnoreCase);

                var u = HttpContext.Current != null ? HttpContext.Current.Request.GetUrlRewriter() : _serverRoot;
                var uriBuilder = new UriBuilder(http ? Uri.UriSchemeHttp : u.Scheme, u.Host, http && u.IsDefaultPort ? 80 : u.Port);

                if (uriBuilder.Uri.IsLoopback || CoreContext.Configuration.Standalone)
                {
                    var tenant = CoreContext.TenantManager.GetCurrentTenant();
                    if (!string.IsNullOrEmpty(tenant.MappedDomain))
                    {
                        uriBuilder = new UriBuilder(Uri.UriSchemeHttp + Uri.SchemeDelimiter + tenant.TenantDomain); // use TenantDomain, not MappedDomain
                    }
                    else
                    {
                        uriBuilder.Host = tenant.TenantDomain;
                    }
                }
                if (uriBuilder.Uri.IsLoopback && !string.IsNullOrEmpty(_hostname))
                {
                    uriBuilder.Host = _hostname;
                }

                return uriBuilder.Uri.ToString().TrimEnd('/');
            }
        }

        public static string GetFullAbsolutePath(string virtualPath)
        {
            if (String.IsNullOrEmpty(virtualPath))
                return ServerRootPath;

            if (virtualPath.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("javascript:", StringComparison.InvariantCultureIgnoreCase) ||
                virtualPath.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
                return virtualPath;

            if (string.IsNullOrEmpty(virtualPath) || virtualPath.StartsWith("/"))
            {
                return ServerRootPath + virtualPath;
            }
            return ServerRootPath + VirtualRoot.TrimEnd('/') + "/" + virtualPath.TrimStart('~', '/');
        }

        public static string ToAbsolute(string virtualPath)
        {
            if (_vpath == null)
            {
                return VirtualPathUtility.ToAbsolute(virtualPath);
            }

            if (string.IsNullOrEmpty(virtualPath) || virtualPath.StartsWith("/"))
            {
                return virtualPath;
            }
            return (_vpath != "/" ? _vpath : string.Empty) + "/" + virtualPath.TrimStart('~', '/');
        }

        public static string Logout
        {
            get { return ToAbsolute("~/auth.aspx") + "?t=logout"; }
        }

        public static string GetDefault()
        {
            return VirtualRoot;
        }

        public static string GetMyStaff()
        {
            return ToAbsolute("~/products/people/profile.aspx");
        }

        public static string GetEmployees()
        {
            return GetEmployees(EmployeeStatus.Active);
        }

        public static string GetEmployees(EmployeeStatus empStatus)
        {
            return ToAbsolute("~/products/people/") +
                   (empStatus == EmployeeStatus.Terminated ? "#type=disabled" : string.Empty);
        }

        public static string GetDepartment(Guid depId)
        {
            return depId != Guid.Empty ? ToAbsolute("~/products/people/#group=") + depId.ToString() : GetEmployees();
        }

        #region user profile link

        public static string GetUserProfile()
        {
            return GetUserProfile(null);
        }

        public static string GetUserProfile(Guid userID)
        {
            if (!CoreContext.UserManager.UserExists(userID))
                return GetEmployees();

            return GetUserProfile(userID.ToString());
        }

        public static string GetUserProfile(string user)
        {
            return GetUserProfile(user, true);
        }

        public static string GetUserProfile(string user, bool absolute)
        {
            var queryParams = "";

            if (!String.IsNullOrEmpty(user))
            {
                var guid = Guid.Empty;
                if (!String.IsNullOrEmpty(user) && 32 <= user.Length && user[8] == '-')
                {
                    try
                    {
                        guid = new Guid(user);
                    }
                    catch
                    {
                    }
                }

                queryParams = guid != Guid.Empty ? GetUserParamsPair(guid) : ParamName_UserUserName + "=" + HttpUtility.UrlEncode(user);
            }

            var url = absolute ? ToAbsolute("~/products/people/") : "/products/people/";
            url += "profile.aspx?";
            url += queryParams;

            return url;
        }

        #endregion

        public static Guid GetProductID()
        {
            var productID = Guid.Empty;

            if (HttpContext.Current != null)
            {
                IProduct product;
                IModule module;
                GetLocationByRequest(out product, out module);
                if (product != null) productID = product.ID;
            }

            if (productID == Guid.Empty)
            {
                var pid = CallContext.GetData("asc.web.product_id");
                if (pid != null) productID = (Guid)pid;
            }

            return productID;
        }

        public static void GetLocationByRequest(out IProduct currentProduct, out IModule currentModule)
        {
            GetLocationByRequest(out currentProduct, out currentModule, HttpContext.Current);
        }

        public static void GetLocationByRequest(out IProduct currentProduct, out IModule currentModule, HttpContext context)
        {
            var currentURL = string.Empty;
            if (context != null && context.Request != null)
            {
                currentURL = HttpContext.Current.Request.GetUrlRewriter().AbsoluteUri;

                // http://[hostname]/[virtualpath]/[AjaxPro.Utility.HandlerPath]/[assembly],[classname].ashx
                if (currentURL.Contains("/" + AjaxPro.Utility.HandlerPath + "/") && HttpContext.Current.Request.UrlReferrer != null)
                {
                    currentURL = HttpContext.Current.Request.UrlReferrer.AbsoluteUri;
                }
            }

            GetLocationByUrl(currentURL, out currentProduct, out currentModule);
        }

        public static IWebItem GetWebItemByUrl(string currentURL)
        {
            if (!String.IsNullOrEmpty(currentURL))
            {

                var itemName = GetWebItemNameFromUrl(currentURL);
                if (!string.IsNullOrEmpty(itemName))
                {
                    foreach (var item in WebItemManager.Instance.GetItemsAll())
                    {
                        var _itemName = GetWebItemNameFromUrl(item.StartURL);
                        if (String.Compare(itemName, _itemName, StringComparison.InvariantCultureIgnoreCase) == 0)
                            return item;
                    }
                }
                else
                {
                    var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
                    var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
                    var pid = productByName == null ? Guid.Empty : productByName.ID;

                    if (pid == Guid.Empty && !String.IsNullOrEmpty(urlParams["pid"]))
                    {
                        try
                        {
                            pid = new Guid(urlParams["pid"]);
                        }
                        catch
                        {
                            pid = Guid.Empty;
                        }
                    }

                    if (pid != Guid.Empty)
                        return WebItemManager.Instance[pid];
                }
            }

            return null;
        }

        public static void GetLocationByUrl(string currentURL, out IProduct currentProduct, out IModule currentModule)
        {
            currentProduct = null;
            currentModule = null;

            if (String.IsNullOrEmpty(currentURL)) return;

            var urlParams = HttpUtility.ParseQueryString(new Uri(currentURL).Query);
            var productByName = GetProductBySysName(urlParams[ParamName_ProductSysName]);
            var pid = productByName == null ? Guid.Empty : productByName.ID;

            if (pid == Guid.Empty && !String.IsNullOrEmpty(urlParams["pid"]))
            {
                try
                {
                    pid = new Guid(urlParams["pid"]);
                }
                catch
                {
                    pid = Guid.Empty;
                }
            }

            var productName = GetProductNameFromUrl(currentURL);
            var moduleName = GetModuleNameFromUrl(currentURL);

            if (!string.IsNullOrEmpty(productName) || !string.IsNullOrEmpty(moduleName))
            {
                foreach (var product in WebItemManager.Instance.GetItemsAll<IProduct>())
                {
                    var _productName = GetProductNameFromUrl(product.StartURL);
                    if (!string.IsNullOrEmpty(_productName))
                    {
                        if (String.Compare(productName, _productName, StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            currentProduct = product;

                            if (!String.IsNullOrEmpty(moduleName))
                            {
                                foreach (var module in WebItemManager.Instance.GetSubItems(product.ID).OfType<IModule>())
                                {
                                    var _moduleName = GetModuleNameFromUrl(module.StartURL);
                                    if (!string.IsNullOrEmpty(_moduleName))
                                    {
                                        if (String.Compare(moduleName, _moduleName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                        {
                                            currentModule = module;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                foreach (var module in WebItemManager.Instance.GetSubItems(product.ID).OfType<IModule>())
                                {
                                    if (!module.StartURL.Equals(product.StartURL) && currentURL.Contains(RegFilePathTrim.Replace(module.StartURL, string.Empty)))
                                    {
                                        currentModule = module;
                                        break;
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            if (pid != Guid.Empty)
                currentProduct = WebItemManager.Instance[pid] as IProduct;
        }

        private static string GetWebItemNameFromUrl(string url)
        {
            var name = GetModuleNameFromUrl(url);
            if (String.IsNullOrEmpty(name))
            {
                name = GetProductNameFromUrl(url);
                if (String.IsNullOrEmpty(name))
                {
                    try
                    {
                        var pos = url.IndexOf("/addons/", StringComparison.InvariantCultureIgnoreCase);
                        if (0 <= pos)
                        {
                            url = url.Substring(pos + 8).ToLower();
                            pos = url.IndexOf('/');
                            return 0 < pos ? url.Substring(0, pos) : url;
                        }
                    }
                    catch
                    {
                    }
                    return null;
                }

            }

            return name;
        }

        private static string GetProductNameFromUrl(string url)
        {
            try
            {
                var pos = url.IndexOf("/products/", StringComparison.InvariantCultureIgnoreCase);
                if (0 <= pos)
                {
                    url = url.Substring(pos + 10).ToLower();
                    pos = url.IndexOf('/');
                    return 0 < pos ? url.Substring(0, pos) : url;
                }
            }
            catch
            {
            }
            return null;
        }

        private static string GetModuleNameFromUrl(string url)
        {
            try
            {
                var pos = url.IndexOf("/modules/", StringComparison.InvariantCultureIgnoreCase);
                if (0 <= pos)
                {
                    url = url.Substring(pos + 9).ToLower();
                    pos = url.IndexOf('/');
                    return 0 < pos ? url.Substring(0, pos) : url;
                }
            }
            catch
            {
            }
            return null;
        }

        private static IProduct GetProductBySysName(string sysName)
        {
            IProduct result = null;

            if (!String.IsNullOrEmpty(sysName))
                foreach (var product in WebItemManager.Instance.GetItemsAll<IProduct>())
                {
                    if (String.CompareOrdinal(sysName, WebItemExtension.GetSysName(product as IWebItem)) == 0)
                    {
                        result = product;
                        break;
                    }
                }

            return result;
        }

        public static string GetUserParamsPair(Guid userID)
        {
            return
                CoreContext.UserManager.UserExists(userID)
                    ? GetUserParamsPair(CoreContext.UserManager.GetUsers(userID))
                    : "";
        }

        public static string GetUserParamsPair(UserInfo user)
        {
            if (user == null || string.IsNullOrEmpty(user.UserName))
                return "";

            return String.Format("{0}={1}", ParamName_UserUserName, HttpUtility.UrlEncode(user.UserName.ToLowerInvariant()));
        }

        #region Help Centr

        public static string GetHelpLink(bool inCurrentCulture)
        {
            var url = WebConfigurationManager.AppSettings["web.help-center"] ?? string.Empty;
            if (url.Contains("{"))
            {
                var parts = url.Split('{');
                url = parts[0];
                if (inCurrentCulture && parts[1].Contains(CultureInfo.CurrentCulture.TwoLetterISOLanguageName))
                {
                    url += CultureInfo.CurrentCulture.TwoLetterISOLanguageName + "/";
                }
            }
            return url;
        }

        #endregion

        #region management links

        public static string GetAdministration(ManagementType managementType)
        {
            if (managementType == ManagementType.General)
                return ToAbsolute("~/management.aspx") + string.Empty;

            return ToAbsolute("~/management.aspx") + "?" + "type=" + ((int)managementType).ToString();
        }

        #endregion

        #region files

        public const string FilesBaseVirtualPath = "~/products/files/";
        public const string EditorPage = "doceditor.aspx";

        public static string FilesBaseAbsolutePath
        {
            get { return ToAbsolute(FilesBaseVirtualPath); }
        }

        public const string FileId = "fileid";
        public const string FolderId = "folderid";
        public const string Version = "version";
        public const string FileUri = "fileuri";
        public const string FileTitle = "title";
        public const string Action = "action";
        public const string AuthKey = "asc_auth_key";
        public const string DocShareKey = "doc";
        public const string TryParam = "try";
        public const string FolderUrl = "folderurl";
        public const string OutType = "outputtype";

        public static string FileHandlerPath
        {
            get { return FilesBaseAbsolutePath + "httphandlers/filehandler.ashx"; }
        }

        public static string DocServiceApiUrl { get; private set; }

        public static string FileViewUrlString
        {
            get { return FileHandlerPath + "?" + Action + "=view&" + FileId + "={0}"; }
        }

        public static string GetFileViewUrl(object fileId)
        {
            return GetFileViewUrl(fileId, 0);
        }

        public static string GetFileViewUrl(object fileId, int fileVersion)
        {
            return string.Format(FileViewUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? string.Empty : "&" + Version + "=" + fileVersion);
        }

        public static string FileDownloadUrlString
        {
            get { return FileHandlerPath + "?" + Action + "=download&" + FileId + "={0}"; }
        }

        public static string GetFileDownloadUrl(object fileId)
        {
            return GetFileDownloadUrl(fileId, 0, string.Empty);
        }

        public static string GetFileDownloadUrl(object fileId, int fileVersion, string convertToExtension)
        {
            return string.Format(FileDownloadUrlString, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty)
                   + (string.IsNullOrEmpty(convertToExtension) ? string.Empty : "&" + OutType + "=" + convertToExtension);
        }

        public static string GetFileWebImageViewUrl(object fileId)
        {
            return FilesBaseAbsolutePath + "#preview/" + HttpUtility.UrlEncode(fileId.ToString());
        }

        public static string FileWebViewerUrlString
        {
            get { return FileWebEditorUrlString + "&" + Action + "=view&"; }
        }

        public static string GetFileWebViewerUrlForMobile(object fileId, int fileVersion)
        {
            var viewerUrl = ToAbsolute("~/../products/files/") + EditorPage + "?" + FileId + "={0}";

            return string.Format(viewerUrl, HttpUtility.UrlEncode(fileId.ToString()))
                   + (fileVersion > 0 ? "&" + Version + "=" + fileVersion : string.Empty);
        }

        public static string FileWebViewerExternalUrlString
        {
            get { return FilesBaseAbsolutePath + EditorPage + "?" + FileUri + "={0}&" + FileTitle + "={1}"; }
        }

        public static string GetFileWebViewerExternalUrl(string fileUri, string fileTitle)
        {
            return string.Format(FileWebViewerExternalUrlString, HttpUtility.UrlEncode(fileUri), HttpUtility.UrlEncode(fileTitle));
        }

        public static string FileWebEditorUrlString
        {
            get { return FilesBaseAbsolutePath + EditorPage + "?" + FileId + "={0}"; }
        }

        public static string GetFileWebEditorUrl(object fileId)
        {
            return string.Format(FileWebEditorUrlString, HttpUtility.UrlEncode(fileId.ToString()));
        }

        public static string GetFileWebEditorTryUrl(FileType fileType)
        {
            return FilesBaseAbsolutePath + EditorPage + "?" + TryParam + "=" + fileType;
        }

        public static string FileWebEditorExternalUrlString
        {
            get { return FileHandlerPath + "?" + Action + "=create&" + FileUri + "={0}&" + FileTitle + "={1}"; }
        }

        public static string GetFileWebEditorExternalUrl(string fileUri, string fileTitle)
        {
            return string.Format(FileWebEditorExternalUrlString, HttpUtility.UrlEncode(fileUri), HttpUtility.UrlEncode(fileTitle));
        }

        public static string GetFileWebPreviewUrl(string fileTitle, object fileId)
        {
            if (FileUtility.CanImageView(fileTitle))
                return GetFileWebImageViewUrl(fileId);

            if (FileUtility.CanWebView(fileTitle))
            {
                if (FileUtility.ExtsMustConvert.Contains(FileUtility.GetFileExtension(fileTitle)))
                    return string.Format(FileWebViewerUrlString, HttpUtility.UrlEncode(fileId.ToString()));
                return GetFileWebEditorUrl(fileId);
            }

            return GetFileViewUrl(fileId);
        }

        public static string GetFileRedirectPreviewUrl(object enrtyId, bool isFile)
        {
            return FileHandlerPath + "?" + Action + "=redirect&" + (isFile ? FileId : FolderId) + "=" + enrtyId;
        }

        public static string GetInitiateUploadSessionUrl(object folderId, object fileId, string fileName, long contentLength)
        {
            var queryString = string.Format("?initiate=true&name={0}&fileSize={1}&tid={2}&userid={3}",
                                            fileName, contentLength, TenantProvider.CurrentTenantID,
                                            HttpUtility.UrlEncode(InstanceCrypto.Encrypt(SecurityContext.CurrentAccount.ID.ToString())));

            if (fileId != null)
                queryString = queryString + "&fileid=" + fileId;

            if (folderId != null)
                queryString = queryString + "&folderid=" + folderId;

            return GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath(getServiceUrl: contentLength > 0) + queryString);
        }

        public static string GetUploadChunkLocationUrl(string uploadId, bool serviceUrl)
        {
            var queryString = "?uid=" + uploadId;
            return GetFullAbsolutePath(GetFileUploaderHandlerVirtualPath(serviceUrl) + queryString);
        }

        private static string GetFileUploaderHandlerVirtualPath(bool getServiceUrl)
        {
            string virtualPath = getServiceUrl
                                     ? (WebConfigurationManager.AppSettings["files.uploader.url"] ?? "~")
                                     : (WebConfigurationManager.AppSettings["files.uploader.url.local"] ?? "~/products/files"); 

            return virtualPath.EndsWith(".ashx") ? virtualPath : virtualPath.TrimEnd('/') + "/ChunkedUploader.ashx";
        }

        #endregion
    }
}