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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Security.Cryptography;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using ASC.Web.Host.Common;
using ASC.Web.Host.Config;
using log4net;
using Microsoft.Win32.SafeHandles;

namespace ASC.Web.Host.HttpRequestProcessor
{
    class Request : SimpleWorkerRequest
    {
        private string _allRawHeaders;
        private Connection _connection;
        private readonly IStackWalk _connectionPermission;
        private int _contentLength;
        private int _endHeadersOffset;
        private string _filePath;
        private byte[] _headerBytes;
        private ArrayList _headerByteStrings;
        private bool _headersSent;
        private readonly Host _host;
        private string[] _knownRequestHeaders;
        private string _path;
        private string _pathInfo;
        private string _pathTranslated;
        private byte[] _preloadedContent;
        private int _preloadedContentLength;
        private string _prot;
        private string _queryString;
        private byte[] _queryStringBytes;
        private ArrayList _responseBodyBytes;
        private StringBuilder _responseHeadersBuilder;
        private int _responseStatus;
        private readonly Server _server;
        private bool _specialCaseStaticFileHeaders;
        private int _startHeadersOffset;
        private string[][] _unknownRequestHeaders;
        private string _url;
        private string _verb;
        private static IList<string> cachedExtentions = new List<string>();
        private static readonly char[] badPathChars = new[] { '%', '>', '<', ':', '\\' };
        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");

        [ThreadStatic]
        private static readonly MD5 md5 = MD5.Create();

        private static readonly char[] IntToHex = new[]
                                                      {
                                                          '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a',
                                                          'b', 'c', 'd', 'e', 'f'
                                                      };

        private readonly IIdentity _clientIdentity;

        static Request()
        {
            cachedExtentions.Add(".js");
            cachedExtentions.Add(".css");
            cachedExtentions.Add(".less");
            cachedExtentions.Add(".html");
        }

        public Request(Server server, Host host, Connection connection, IIdentity client)
            : base(string.Empty, string.Empty, null)
        {
            _connectionPermission = new PermissionSet(PermissionState.Unrestricted);
            _server = server;
            _host = host;
            _connection = connection;
            _clientIdentity = client;
        }

        public override void CloseConnection()
        {
            _connectionPermission.Assert();
            _connection.Close();
        }

        public override IntPtr GetUserToken()
        {
            var windowsIdentity = _clientIdentity as WindowsIdentity;
            return windowsIdentity != null ? windowsIdentity.Token : IntPtr.Zero;
        }

        public override void EndOfRequest()
        {
            Connection conn = _connection;
            if (conn != null)
            {
                _connection = null;
                _server.OnRequestEnd(conn);
            }
        }

        public override void FlushResponse(bool finalFlush)
        {
            _connectionPermission.Assert();

            if (!_headersSent)
            {
                _connection.WriteHeaders(_responseStatus, _responseHeadersBuilder.ToString());
                _headersSent = true;
            }
            for (int i = 0; i < _responseBodyBytes.Count; i++)
            {
                byte[] data = (byte[])_responseBodyBytes[i];
                _connection.WriteBody(data, 0, data.Length);
            }
            _responseBodyBytes = new ArrayList();

            if (finalFlush)
            {
                _connection.Close();
            }
        }

        public override string GetAppPath()
        {
            return _host.VirtualPath;
        }

        public override string GetAppPathTranslated()
        {
            return _host.PhysicalPath;
        }

        public override string GetFilePath()
        {
            return _filePath;
        }

        public override string GetFilePathTranslated()
        {
            return _pathTranslated;
        }

        public override string GetHttpVerbName()
        {
            return _verb;
        }

        public override string GetHttpVersion()
        {
            return _prot;
        }

        public override string GetKnownRequestHeader(int index)
        {
            return _knownRequestHeaders[index];
        }

        public override string GetLocalAddress()
        {
            _connectionPermission.Assert();
            return _connection.LocalIP;
        }

        public override int GetLocalPort()
        {
            return _host.Port;
        }

        public override string GetPathInfo()
        {
            return _pathInfo;
        }

        public override byte[] GetPreloadedEntityBody()
        {
            return _preloadedContent;
        }

        public override string GetQueryString()
        {
            return _queryString;
        }

        public override byte[] GetQueryStringRawBytes()
        {
            return _queryStringBytes;
        }

        public override string GetRawUrl()
        {


            return _url;
        }

        public override string GetRemoteAddress()
        {
            _connectionPermission.Assert();
            return _connection.RemoteIP;
        }

        public override int GetRemotePort()
        {
            return 0;
        }

        public override void SetEndOfSendNotification(HttpWorkerRequest.EndOfSendNotification callback, object extraData)
        {
            base.SetEndOfSendNotification(callback, extraData);
        }

        public override string GetServerName()
        {

            if (!wcf_hacked && !wcf_hacking) HackWcf();

            int knownRequestHeaderIndex = GetKnownRequestHeaderIndex("Host");
            string knownRequestHeader = GetKnownRequestHeader(knownRequestHeaderIndex);
            if (string.IsNullOrEmpty(knownRequestHeader))
            {
                return base.GetServerName();
            }
            knownRequestHeader = knownRequestHeader.Split(new[] { ':' })[0].Trim();
            if (knownRequestHeader.Length == 0)
            {
                return base.GetServerName();
            }
            return knownRequestHeader;
        }

        public override string GetServerVariable(string name)
        {
            string str = string.Empty;
            string str2 = name;

            if (_clientIdentity != null)
            {
                switch (name)
                {
                    case "LOGON_USER":
                        return _clientIdentity.Name;
                    case "AUTH_TYPE":
                        return _clientIdentity.AuthenticationType;
                }
            }

            if (str2 == null)
            {
                return str;
            }
            if (!(str2 == "ALL_RAW"))
            {
                if (str2 != "SERVER_PROTOCOL")
                {
                    if (str2 != "SERVER_SOFTWARE")
                    {
                        return str;
                    }
                    return ("ASC Web Server/" + Messages.VersionString);
                }
            }
            else
            {
                return _allRawHeaders;
            }
            return _prot;
        }

        public override string GetUnknownRequestHeader(string name)
        {
            int length = _unknownRequestHeaders.Length;
            for (int i = 0; i < length; i++)
            {
                if (string.Compare(name, _unknownRequestHeaders[i][0], StringComparison.OrdinalIgnoreCase) ==
                    0)
                {
                    return _unknownRequestHeaders[i][1];
                }
            }
            return null;
        }

        public override string[][] GetUnknownRequestHeaders()
        {
            return _unknownRequestHeaders;
        }

        public override string GetUriPath()
        {
            return _path;
        }

        public override bool HeadersSent()
        {
            return _headersSent;
        }

        private bool IsBadPath()
        {
            return ((_path.IndexOfAny(badPathChars) >= 0) ||
                    ((CultureInfo.InvariantCulture.CompareInfo.IndexOf(_path, "..", CompareOptions.Ordinal) >=
                      0) ||
                     (CultureInfo.InvariantCulture.CompareInfo.IndexOf(_path, "//", CompareOptions.Ordinal) >=
                      0)));
        }

        public override bool IsClientConnected()
        {
            _connectionPermission.Assert();
            return _connection.Connected;
        }

        public override bool IsEntireEntityBodyIsPreloaded()
        {
            return (_contentLength == _preloadedContentLength);
        }

        private bool IsRequestForRestrictedDirectory()
        {
            string str = CultureInfo.InvariantCulture.TextInfo.ToLower(_path);
            if (_host.VirtualPath != "/")
            {
                str = str.Substring(_host.VirtualPath.Length);
            }
            foreach (string str2 in RequestConfiguration.RestrictedDirs)
            {
                if (str.StartsWith(str2, StringComparison.Ordinal) &&
                    ((str.Length == str2.Length) || (str[str2.Length] == '/')))
                {
                    return true;
                }
            }
            return false;
        }

        public override string MapPath(string path)
        {
            string physicalPath;

            if ((string.IsNullOrEmpty(path)) || path.Equals("/"))
            {
                physicalPath = _host.VirtualPath == "/" ? _host.PhysicalPath : Environment.SystemDirectory;
            }
            else if (_host.IsVirtualPathAppPath(path))
            {
                physicalPath = _host.PhysicalPath;
            }
            else if (_host.IsVirtualPathInApp(path))
            {
                physicalPath = _host.PhysicalPath + path.Substring(_host.NormalizedVirtualPath.Length);
            }
            else if (path.StartsWith("/", StringComparison.Ordinal))
            {
                physicalPath = _host.PhysicalPath + path.Substring(1);
            }
            else
            {
                physicalPath = _host.PhysicalPath + path;
            }
            physicalPath = physicalPath.Replace('/', '\\');
            if (path != null)
            {
                if ((physicalPath.EndsWith(@"\", StringComparison.Ordinal) &&
                     !physicalPath.EndsWith(@":\", StringComparison.Ordinal)) &&
                    (!path.EndsWith(@"\", StringComparison.Ordinal) &&
                     !path.EndsWith("/", StringComparison.Ordinal)))
                {
                    physicalPath = physicalPath.Substring(0, physicalPath.Length - 1);
                }
            }
            return physicalPath;
        }

        private void ParseHeaders()
        {
            _knownRequestHeaders = new string[40];
            ArrayList list = new ArrayList();
            for (int i = 1; i < _headerByteStrings.Count; i++)
            {
                string str = ((ByteString)_headerByteStrings[i]).GetString();
                int index = str.IndexOf(':');
                if (index >= 0)
                {
                    string header = str.Substring(0, index).Trim();
                    string str3 = str.Substring(index + 1).Trim();
                    int knownRequestHeaderIndex = GetKnownRequestHeaderIndex(header);
                    if (knownRequestHeaderIndex >= 0)
                    {
                        _knownRequestHeaders[knownRequestHeaderIndex] = str3;
                    }
                    else
                    {
                        list.Add(header);
                        list.Add(str3);
                    }
                }
            }
            int num4 = list.Count / 2;
            _unknownRequestHeaders = new string[num4][];
            int num5 = 0;
            for (int j = 0; j < num4; j++)
            {
                _unknownRequestHeaders[j] = new[] { (string)list[num5++], (string)list[num5++] };
            }
            _allRawHeaders = _headerByteStrings.Count > 1
                                 ? Encoding.UTF8.GetString(_headerBytes,
                                                           _startHeadersOffset,
                                                           _endHeadersOffset - _startHeadersOffset)
                                 : string.Empty;
        }

        private void ParsePostedContent()
        {
            _contentLength = 0;
            _preloadedContentLength = 0;
            string s = _knownRequestHeaders[11];
            if (s != null)
            {
                try
                {
                    _contentLength = int.Parse(s, CultureInfo.InvariantCulture);
                }
                catch { }
            }
            if (_headerBytes.Length > _endHeadersOffset)
            {
                _preloadedContentLength = _headerBytes.Length - _endHeadersOffset;
                if (_preloadedContentLength > _contentLength)
                {
                    _preloadedContentLength = _contentLength;
                }
                if (_preloadedContentLength > 0)
                {
                    _preloadedContent = new byte[_preloadedContentLength];
                    Buffer.BlockCopy(_headerBytes,
                                     _endHeadersOffset,
                                     _preloadedContent,
                                     0,
                                     _preloadedContentLength);
                }
            }
        }

        private void ParseRequestLine()
        {
            ByteString[] strArray = ((ByteString)_headerByteStrings[0]).Split(' ');
            if (((strArray == null) || (strArray.Length < 2)) || (strArray.Length > 3))
            {
                _connection.WriteErrorAndClose(400);
            }
            else
            {
                _verb = strArray[0].GetString();
                ByteString str2 = strArray[1];
                _url = str2.GetString();
                _prot = strArray.Length == 3 ? strArray[2].GetString() : "HTTP/1.0";
                int index = str2.IndexOf('?');
                _queryStringBytes = index > 0 ? str2.Substring(index + 1).GetBytes() : new byte[0];
                index = _url.IndexOf('?');
                if (index > 0)
                {
                    _path = _url.Substring(0, index);
                    _queryString = _url.Substring(index + 1);
                }
                else
                {
                    _path = _url;
                    _queryStringBytes = new byte[0];
                }
                if (_path.IndexOf('%') >= 0)
                {
                    _path = HttpUtility.UrlDecode(_path, Encoding.UTF8);
                    index = _url.IndexOf('?');
                    if (index >= 0)
                    {
                        _url = _path + _url.Substring(index);
                    }
                    else
                    {
                        _url = _path;
                    }
                }
                int startIndex = _path.LastIndexOf('.');
                int num3 = _path.LastIndexOf('/');
                if (((startIndex >= 0) && (num3 >= 0)) && (startIndex < num3))
                {
                    int length = _path.IndexOf('/', startIndex);
                    _filePath = _path.Substring(0, length);
                    _pathInfo = _path.Substring(length);
                }
                else
                {
                    _filePath = _path;
                    _pathInfo = string.Empty;
                }
                _path = _path.Replace("//", "/");

                _filePath = _filePath.Replace("//", "/"); // Fix of // file
                _pathTranslated = MapPath(_filePath);
            }
        }

        private void PrepareResponse()
        {
            _headersSent = false;
            _responseStatus = 200;
            _responseHeadersBuilder = new StringBuilder();
            _responseBodyBytes = new ArrayList();
        }


        public void Process()
        {
            if (_connection.CheckVirutualPath())
            {
                _connection.CheckRewritePath();
                if (TryParseRequest())
                {
                    if (((_verb == "POST") && (_contentLength > 0)) &&
                        (_preloadedContentLength < _contentLength))
                    {
                        _connection.Write100Continue();
                    }
                    if (_server.IsCustomOutput)
                    {
                        _connection.WriteCustom();
                    }
                    else if (IsRequestForRestrictedDirectory())
                    {
                        _connection.WriteErrorAndClose(403);
                    }
                    else if (!ProcessDirectoryListingRequest())
                    {
                        PrepareResponse();

                        try
                        {
                            HttpRuntime.ProcessRequest(this);
                        }
                        catch (Exception ex)
                        {
                            log.Warn(ServerMessages.ErrorProcessing, ex);
                        }
                    }
                }
            }
        }

        static bool wcf_hacked = false;
        static bool wcf_hacking = false;

        private void HackWcf()
        {

            if (!_server.HackWCFBinding)
            {
                wcf_hacked = true;
                return;
            }

            try
            {
                wcf_hacking = true;

                Type type_msc = Type.GetType("System.ServiceModel.Activation.MetabaseSettingsCassini, System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                Type type_hhrar = Type.GetType("System.ServiceModel.Activation.HostedHttpRequestAsyncResult,  System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                Type type_hctm = Type.GetType("System.ServiceModel.Activation.HostedTransportConfigurationManager,  System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
                Type type_she = Type.GetType("System.ServiceModel.ServiceHostingEnvironment,  System.ServiceModel, Version=3.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

                //initialize service hosting environment
                var mi_init_she = type_she.GetMethod("EnsureInitialized", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                mi_init_she.Invoke(null, null);


                var ci_type_hhrar = type_hhrar.GetConstructor(new Type[] { typeof(HttpApplication), typeof(bool), typeof(AsyncCallback), typeof(object) });
                var ci_type_msc = type_msc.GetConstructor(
                                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                                            null, new Type[] { type_hhrar }, null);

                //create fake instance of HostedHttpRequestAsyncResult for creating MetabaseSettingsCassini
                object hhrar = ci_type_hhrar.Invoke(new object[] { HttpContext.Current.ApplicationInstance, false, null, null });
                //need uri initialized for MetabaseSettingsCassini ctor
                var fi_uri_hhrar = type_hhrar.GetField("originalRequestUri", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                Uri requestUri = HttpContext.Current.Request.Url;
                fi_uri_hhrar.SetValue(hhrar, requestUri);

                //create fake instance of MetabaseSettingsCassini
                object msc = ci_type_msc.Invoke(new object[] { hhrar });
                //add binding to :{port}:* By default - :{port}:localhost
                var pi_bindings_msc = type_msc.GetProperty("Bindings", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                string binding = String.Format(System.Globalization.CultureInfo.InvariantCulture, ":{0}:{1}", requestUri.Port.ToString(System.Globalization.NumberFormatInfo.InvariantInfo), "*");
                pi_bindings_msc.SetValue(msc,
                        new System.Collections.Generic.Dictionary<string, string[]> { { requestUri.Scheme, new string[] { binding } } },
                        null
                    );

                //update singleton instance of HostedTransportConfigurationManager, configured from hacked settings
                var fi_singleton_hctm = type_hctm.GetField("singleton", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                object s = fi_singleton_hctm.GetValue(null);
                var ci_hctm = type_hctm.GetConstructor(
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null, new Type[] { type_msc }, null);

                var new_hctm = ci_hctm.Invoke(new object[] { msc });

                var fi_syncRoot_hctm = type_hctm.GetField("syncRoot", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                object syncRoot_hctm = fi_syncRoot_hctm.GetValue(null);

                lock (syncRoot_hctm)
                {
                    fi_singleton_hctm.SetValue(null, new_hctm);
                }

                wcf_hacked = true;
            }
            catch { }
            finally
            {
                wcf_hacking = false;
            }

        }

        private bool ProcessDirectoryListingRequest()
        {
            if (_verb != "GET")
            {
                return false;
            }
            string path = _pathTranslated;
            if (_pathInfo.Length > 0)
            {
                path = MapPath(_path);
            }
            if (!Directory.Exists(path))
            {
                return false;
            }
            if (!_path.EndsWith("/", StringComparison.Ordinal))
            {
                string str2 = _path + "/";
                string extraHeaders = "Location: " + UrlEncodeRedirect(str2) + "\r\n";
                string body =
                    "<html><head><title>Object moved</title></head><body>\r\n<h2>Object moved to <a href='" +
                    str2 + "'>here</a>.</h2>\r\n</body></html>\r\n";
                _connection.WriteEntireResponseFromString(0x12e, extraHeaders, body, false);
                return true;
            }
            foreach (string str5 in DefaultDocuments)
            {
                string str6 = Path.Combine(path, str5);
                if (File.Exists(str6))
                {
                    _path = _path + str5;
                    _filePath = _path;
                    _url = (_queryString != null) ? (_path + "?" + _queryString) : _path;
                    _pathTranslated = str6;
                    return false;
                }
            }
            _connection.WriteErrorAndClose(403);
            return true;
        }

        private void ReadAllHeaders()
        {
            _headerBytes = null;
            do
            {
                if (!TryReadAllHeaders())
                {
                    return;
                }
            }
            while (_endHeadersOffset < 0);
        }

        public override int ReadEntityBody(byte[] buffer, int size)
        {
            int count = 0;
            _connectionPermission.Assert();
            byte[] src = _connection.ReadRequestBytes(size);
            if ((src != null) && (src.Length > 0))
            {
                count = src.Length;
                Buffer.BlockCopy(src, 0, buffer, 0, count);
            }
            return count;
        }

        private void Reset()
        {
            _headerBytes = null;
            _startHeadersOffset = 0;
            _endHeadersOffset = 0;
            _headerByteStrings = null;
            _verb = null;
            _url = null;
            _prot = null;
            _path = null;
            _filePath = null;
            _pathInfo = null;
            _pathTranslated = null;
            _queryString = null;
            _queryStringBytes = null;
            _contentLength = 0;
            _preloadedContentLength = 0;
            _preloadedContent = null;
            _allRawHeaders = null;
            _unknownRequestHeaders = null;
            _knownRequestHeaders = null;
            _specialCaseStaticFileHeaders = false;
        }

        public override void SendCalculatedContentLength(int contentLength)
        {
            if (!_headersSent)
            {
                _responseHeadersBuilder.Append("Content-Length: ");
                _responseHeadersBuilder.Append(contentLength.ToString(CultureInfo.InvariantCulture));
                _responseHeadersBuilder.Append("\r\n");
            }
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            if (!_headersSent)
            {
                switch (index)
                {
                    case 1:
                    case 2:
                    case 0x1a:
                        return;

                    case 0x12:
                    case 0x13:
                        if (!_specialCaseStaticFileHeaders)
                        {
                            break;
                        }
                        return;

                    case 20:
                        if (!(value == "bytes"))
                        {
                            break;
                        }
                        _specialCaseStaticFileHeaders = true;
                        return;
                }
                _responseHeadersBuilder.Append(GetKnownResponseHeaderName(index));
                _responseHeadersBuilder.Append(": ");
                _responseHeadersBuilder.Append(value);
                _responseHeadersBuilder.Append("\r\n");
            }
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            if (length != 0L)
            {
                using (var handle2 = new SafeFileHandle(handle, false))
                using (var f = new FileStream(handle2, FileAccess.Read))
                {
                    SendResponseFromFileStream(f, offset, length);
                }
            }
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            if (length != 0L)
            {
                if (!CheckFileCache(filename))
                {
                    using (var f = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        SendResponseFromFileStream(f, offset, length);
                    }
                }
            }
        }

        private void SendGzippedFile(long length, FileStream f, long offset)
        {
            SendUnknownResponseHeader("Content-Encoding", "gzip");
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress))
                {
                    f.Lock(offset, length);
                    f.Seek(offset, SeekOrigin.Begin);
                    const int readBuffLength = 4096;
                    byte[] buffer = new byte[readBuffLength];
                    int readed;
                    while ((readed = f.Read(buffer, 0, readBuffLength)) != 0)
                    {
                        gzip.Write(buffer, 0, readed);
                    }
                    f.Unlock(offset, length);
                    //Write some empty block
                    byte[] rn = Encoding.UTF8.GetBytes(Environment.NewLine);
                    for (int i = 0; i < 3; i++)
                    {
                        gzip.Write(rn, 0, rn.Length);
                    }

                    gzip.Flush();
                    ms.Seek(0, SeekOrigin.Begin);
#if (ETAG)
					FileInfo finfo = new FileInfo(f.Name);
					string etag = string.Format("\"{0}.{1}\"", finfo.LastWriteTimeUtc.ToFileTimeUtc(), ms.Length);
                    SendUnknownResponseHeader("Etag", etag);
#endif
                    SendCalculatedContentLength(ms.Length);
                    SendResponseFromMemoryInternal(ms.GetBuffer(), (int)ms.Length);
                }
            }
        }

        private bool CheckFileCache(string filename)
        {
#if (CACHECONTROL)
            FileInfo finfo = new FileInfo(filename);
            string nochacecontrol = GetKnownRequestHeader(GetKnownRequestHeaderIndex("Cache-Control"));
            if (!string.IsNullOrEmpty(nochacecontrol) && nochacecontrol.IndexOf("no-cache") != -1)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(nochacecontrol) && nochacecontrol.IndexOf("only-if-cached") != -1)
            {
                return true;
            }
            string ifModifiedScince = GetKnownRequestHeader(GetKnownRequestHeaderIndex("If-Modified-Since"));

            if (!string.IsNullOrEmpty(ifModifiedScince))
            {
                DateTime cacheModified;
                if (
                    !DateTime.TryParseExact(ifModifiedScince,
                                            "ddd, dd MMM yyyy hh",
                                            CultureInfo.InvariantCulture,
                                            DateTimeStyles.None,
                                            out cacheModified))
                {
                    DateTime.TryParseExact(ifModifiedScince,
                                           "R",
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out cacheModified);
                }
                if (cacheModified.Date >= finfo.LastWriteTimeUtc.Date)
                {
                    _connection.WriteEntireResponseFromString(304, null, "", false);
                    return true;
                }
            }

            //Split req etag
            string requestEtag = GetKnownRequestHeader(GetKnownRequestHeaderIndex("If-None-Match"));
            string etag = string.Format("\"{0}.{1}\"",
                                        finfo.LastWriteTimeUtc.ToFileTimeUtc(),
                                        finfo.Length);
            if (!string.IsNullOrEmpty(requestEtag))
            {
                string[] reqetags = requestEtag.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var reqetag in reqetags)
                {
                    if (etag.Equals(reqetag))
                    {
                        _connection.WriteEntireResponseFromString(304, null, "", false);
                        return true;
                    }
                }
            }
#if (ETAG)
            SendUnknownResponseHeader("Etag", etag);
#endif
            SendUnknownResponseHeader("Last-Modified", finfo.LastWriteTimeUtc.ToString("R"));
            SendUnknownResponseHeader("Expires", DateTime.UtcNow.AddDays(1).ToString("R"));
#endif
            return false;
        }

        private void SendResponseFromFileStream(Stream f, long offset, long length)
        {
#if (GZIP)
            string acceptEncoding = GetKnownRequestHeader(GetKnownRequestHeaderIndex("Accept-Encoding"));
            if (!string.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("gzip") && (f is FileStream))
            {
                //Read all file into gzip
                string extension = Path.GetExtension(((FileStream)f).Name);
                if (cachedExtentions.Contains(extension))
                {
                    SendGzippedFile(length, ((FileStream)f), offset);
                }
                else
                {
                    SendResponceFromStreamInternal(f, length, offset);
                }
            }
            else
            {
                SendResponceFromStreamInternal(f, length, offset);
            }
#else
            SendResponceFromStreamInternal(f, length, offset);
#endif
        }

        private void SendResponceFromStreamInternal(Stream f, long length, long offset)
        {
            long num = f.Length;
            if (length == -1L)
            {
                length = num - offset;
            }
            if (((length != 0L) && (offset >= 0L)) && (length <= (num - offset)))
            {
                if (offset > 0L)
                {
                    f.Seek(offset, SeekOrigin.Begin);
                }
                if (length <= 0x10000L)
                {
                    byte[] buffer = new byte[(int)length];
                    int num2 = f.Read(buffer, 0, (int)length);
                    SendResponseFromMemory(buffer, num2);
                }
                else
                {
                    byte[] buffer2 = new byte[0x10000];
                    int num3 = (int)length;
                    while (num3 > 0)
                    {
                        int count = (num3 < 0x10000) ? num3 : 0x10000;
                        int num5 = f.Read(buffer2, 0, count);
                        SendResponseFromMemory(buffer2, num5);
                        num3 -= num5;
                        if ((num3 > 0) && (num5 > 0))
                        {
                            FlushResponse(false);
                        }
                    }
                }
            }
        }

        private void SendResponseFromMemoryInternal(byte[] data, int length)
        {
            if (length > 0)
            {
                byte[] dst = new byte[length];
                Buffer.BlockCopy(data, 0, dst, 0, length);
                _responseBodyBytes.Add(dst);
            }
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            if (length > 0)
            {
                byte[] dst = new byte[length];
                Buffer.BlockCopy(data, 0, dst, 0, length);
                _responseBodyBytes.Add(dst);
            }
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            _responseStatus = statusCode;
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            if (!_headersSent)
            {
                _responseHeadersBuilder.Append(name);
                _responseHeadersBuilder.Append(": ");
                _responseHeadersBuilder.Append(value);
                _responseHeadersBuilder.Append("\r\n");
            }
        }

        private bool TryParseRequest()
        {
            Reset();
            ReadAllHeaders();
            if (((_headerBytes == null) || (_endHeadersOffset < 0)) ||
                ((_headerByteStrings == null) || (_headerByteStrings.Count == 0)))
            {
                _connection.WriteErrorAndClose(400);
                return false;
            }
            ParseRequestLine();
            if (IsBadPath())
            {
                _connection.WriteErrorAndClose(400);
                return false;
            }
            if (!_host.IsVirtualPathInApp(_path))
            {
                _connection.WriteErrorAndClose(404);
                return false;
            }
            ParseHeaders();
            ParsePostedContent();

            return true;
        }

        private bool TryReadAllHeaders()
        {
            byte[] src = _connection.ReadHeadersAsByte();

            if ((src == null) || (src.Length == 0))
            {
                return false;
            }
            if (_headerBytes != null)
            {
                int num = src.Length + _headerBytes.Length;
                if (num > 0x8000)
                {
                    return false;
                }
                byte[] dst = new byte[num];
                Buffer.BlockCopy(_headerBytes, 0, dst, 0, _headerBytes.Length);
                Buffer.BlockCopy(src, 0, dst, _headerBytes.Length, src.Length);
                _headerBytes = dst;
            }
            else
            {
                _headerBytes = src;
            }
            _startHeadersOffset = -1;
            _endHeadersOffset = -1;
            _headerByteStrings = new ArrayList();
            ByteParser parser = new ByteParser(_headerBytes);
            while (true)
            {
                ByteString str = parser.ReadLine();
                if (str == null)
                {
                    break;
                }
                if (_startHeadersOffset < 0)
                {
                    _startHeadersOffset = parser.CurrentOffset;
                }
                if (str.IsEmpty)
                {
                    _endHeadersOffset = parser.CurrentOffset;
                    break;
                }
                _headerByteStrings.Add(str);
            }
            return true;
        }

        private static string UrlEncodeRedirect(string path)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(path);
            int length = bytes.Length;
            int num2 = 0;
            for (int i = 0; i < length; i++)
            {
                if ((bytes[i] & 0x80) != 0)
                {
                    num2++;
                }
            }
            if (num2 > 0)
            {
                byte[] buffer2 = new byte[length + (num2 * 2)];
                int num4 = 0;
                for (int j = 0; j < length; j++)
                {
                    byte num6 = bytes[j];
                    if ((num6 & 0x80) == 0)
                    {
                        buffer2[num4++] = num6;
                    }
                    else
                    {
                        buffer2[num4++] = 0x25;
                        buffer2[num4++] = (byte)IntToHex[(num6 >> 4) & 15];
                        buffer2[num4++] = (byte)IntToHex[num6 & 15];
                    }
                }
                path = Encoding.ASCII.GetString(buffer2);
            }
            if (path.IndexOf(' ') >= 0)
            {
                path = path.Replace(" ", "%20");
            }
            return path;
        }

        public IEnumerable<string> DefaultDocuments
        {
            get
            {
                return RequestConfiguration.DefaultFileNames;
            }
        }
    }
}