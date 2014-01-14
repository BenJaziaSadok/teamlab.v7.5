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
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Configuration;
using ASC.Common.Web;
using ASC.Web.Host.Common;
using ASC.Web.Host.Config;
using log4net;
using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Web.Host.HttpRequestProcessor
{
    class Connection : MarshalByRefObject
    {
        private static string _localServerIP;
        private readonly Server _server;
        private static readonly ILog log = LogManager.GetLogger("ASC.Web.Host");
        private static CustomErrorsSection customErrorsSection;
        private static bool customErrorsInit = false;


        public HttpListenerContext ListenerContext
        {
            get;
            private set;
        }

        internal Connection(Server server, HttpListenerContext ctx)
        {
            _server = server;
            ListenerContext = ctx;
            ListenerContext.Response.SendChunked = false;
        }


        internal void Close()
        {
            try
            {
                if (ListenerContext != null)
                {
                    ListenerContext.Response.Close();
                }
            }
            catch (Exception)
            {
                log.Warn(ServerMessages.ErrorWhileClosingSocket);
            }
            finally
            {
                ListenerContext = null;
            }
        }

        private string GetErrorResponseBody(int statusCode, string message)
        {
            string str = Messages.FormatErrorMessageBody(statusCode, _server.VirtualPath);
            if (!string.IsNullOrEmpty(message))
            {
                str = string.Format("{0}\r\n<!--\r\n{1}\r\n-->", str, message);
            }
            return str;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        private static string MakeContentTypeHeader(string fileName)
        {
            return (string.Format("Content-Type: {0}\r\n", MimeMapping.GetMimeMapping(fileName)));
        }


        private void MakeResponseHeaders(int statusCode, string moreHeaders, int contentLength, bool keepAlive)
        {
            if (ListenerContext != null)
            {
                ListenerContext.Response.StatusCode = statusCode;
                if (contentLength != -1)
                {
                    ListenerContext.Response.ContentLength64 = contentLength;
                }

                ListenerContext.Response.ProtocolVersion = new Version(1, 1);
                ListenerContext.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(statusCode);

                if (moreHeaders != null)
                {
                    string[] headers = moreHeaders.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string header in headers)
                    {
                        try
                        {
                            int dotIndex = header.IndexOf(':');
                            string headerName = header.Substring(0, dotIndex).Trim();
                            string headerValue = header.Substring(dotIndex + 2, header.Length - dotIndex - 2).Trim();
                            if (!headerName.Equals("Content-Length"))
                            {
                                /*if (ListenerContext.Response.Headers.Get(headerName) != null) {
                                    ListenerContext.Response.Headers.Set(headerName, headerValue);
                                }
                                else {*/
                                ListenerContext.Response.Headers.Add(headerName, headerValue);
                                //}
                            }
                            else
                            {
                                ListenerContext.Response.ContentLength64 = long.Parse(headerValue);
                            }
                        }
                        catch (Exception)
                        {
                            ListenerContext.Response.StatusCode = 500;
                        }
                    }
                }
                ListenerContext.Response.KeepAlive = keepAlive;
                ListenerContext.Response.Headers.Add("Server", string.Format(ServerMessages.ASCWebServer, Messages.VersionString));
                ListenerContext.Response.Headers.Add("Date", DateTime.Now.ToUniversalTime().ToString("R", DateTimeFormatInfo.InvariantInfo));
            }
        }

        internal byte[] ReadRequestBytes(int maxBytes)
        {
            try
            {
                if (WaitForRequestBytes() == 0)
                {
                    return null;
                }
                long num = ListenerContext.Request.ContentLength64;
                if (num > maxBytes)
                {
                    num = maxBytes;
                }
                int count = 0;
                byte[] src = new byte[num];
                if (num > 0)
                {
                    count = ListenerContext.Request.InputStream.Read(src, 0, (int)num);
                }
                if (count < num)
                {
                    byte[] dst = new byte[count];
                    if (count > 0)
                    {
                        Buffer.BlockCopy(src, 0, dst, 0, count);
                    }
                    src = dst;
                }
                return src;
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal int WaitForRequestBytes()
        {
            int num = (int)(ListenerContext.Request.ContentLength64 + 1);
            return num;
        }

        internal void Write100Continue()
        {
            WriteEntireResponseFromString(100, null, null, true);
        }

        internal void WriteBody(byte[] data, int offset, int length)
        {
            try
            {
                ListenerContext.Response.OutputStream.Write(data, offset, length);
            }
            catch (Exception)
            {
                log.Warn(ServerMessages.ErrorWhileWritingResponceBody);
            }
        }

        internal void WriteEntireResponseFromFile(string fileName, bool keepAlive)
        {
            WriteEntireResponseFromFile(fileName, keepAlive, false);
        }

        internal void WriteEntireResponseFromFile(string fileName, bool keepAlive, bool compress)
        {
            if (!File.Exists(fileName))
            {
                WriteErrorAndClose(404);
            }
            else
            {
                string moreHeaders = MakeContentTypeHeader(fileName);
                if (moreHeaders == null)
                {
                    WriteErrorAndClose(403);
                }
                else
                {
                    try
                    {
#if !(GZIP)
						compress = false;
#endif
                        if (compress)
                        {
                            using (var f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            using (var ms = new MemoryStream())
                            using (var gzip = new GZipStream(ms, CompressionMode.Compress))
                            {
                                byte[] buffer = new byte[ServerConfiguration.BufferSize];
                                int readed = 0;

                                while ((readed = f.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    gzip.Write(buffer, 0, readed);
                                }
                                //Write some empty block
                                byte[] rn = Encoding.UTF8.GetBytes(Environment.NewLine);
                                for (int i = 0; i < 3; i++)
                                {
                                    gzip.Write(rn, 0, rn.Length);
                                }
                                gzip.Flush();

                                buffer = ms.GetBuffer();
                                int contentLen = (int)ms.Length;
                                moreHeaders += "Content-Encoding: gzip";
                                MakeResponseHeaders(200, moreHeaders, contentLen, keepAlive);
                                WriteBody(buffer, 0, contentLen);
                            }
                        }
                        else
                        {
                            using (var f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                MakeResponseHeaders(200, moreHeaders, (int)f.Length, keepAlive);
                                var buffer = new byte[ServerConfiguration.BufferSize];
                                int readed = 0;
                                while ((readed = f.Read(buffer, 0, buffer.Length)) != 0)
                                {
                                    WriteBody(buffer, 0, readed);
                                }
                            }
                        }
                    }
                    catch
                    {
                        Close();
                        throw;
                    }
                    finally
                    {
                        if (!keepAlive)
                        {
                            Close();
                        }
                    }
                }
            }
        }

        internal void WriteEntireResponseFromString(int statusCode, string extraHeaders, string body, bool keepAlive)
        {
            try
            {
                if (400 <= statusCode)
                {
                    var redirect = GetErrorRedirect(statusCode, ListenerContext.Request.IsLocal);
                    if (!string.IsNullOrEmpty(redirect))
                    {
                        if (redirect.StartsWith("~/")) redirect = redirect.Substring(2);
                        ListenerContext.Response.Redirect(_server.VirtualPath + redirect);
                        ListenerContext.Response.Close();
                        return;
                    }
                }

                int contentLength = (body != null) ? Encoding.UTF8.GetByteCount(body) : 0;
                MakeResponseHeaders(statusCode, extraHeaders, contentLength, keepAlive);
                if (contentLength > 0)
                {
                    if (body != null)
                    {
                        WriteBody(Encoding.UTF8.GetBytes(body), 0, contentLength);
                    }
                }
            }
            finally
            {
                if (!keepAlive)
                {
                    Close();
                }
            }
        }

        internal void WriteCustom()
        {
            WriteEntireResponseFromFile(_server.CustomHtml, false);
        }

        internal void WriteErrorAndClose(int statusCode)
        {
            WriteErrorAndClose(statusCode, null);
        }

        internal void WriteErrorAndClose(int statusCode, string message)
        {
            WriteEntireResponseFromString(statusCode, null, GetErrorResponseBody(statusCode, message), false);
        }

        internal void WriteErrorWithExtraHeadersAndKeepAlive(int statusCode, string extraHeaders)
        {
            WriteEntireResponseFromString(statusCode,
                                          extraHeaders,
                                          GetErrorResponseBody(statusCode, null),
                                          true);
        }

        internal void WriteHeaders(int statusCode, string extraHeaders)
        {
            MakeResponseHeaders(statusCode, extraHeaders, -1, true);
        }

        internal bool Connected
        {
            get { return ListenerContext != null; }
        }

        internal bool IsLocal
        {
            get
            {
                string remoteIP = RemoteIP;
                return (remoteIP.Equals("127.0.0.1") || LocalServerIP.Equals(remoteIP));
            }
        }

        internal string LocalIP
        {
            get
            {
                IPEndPoint point = ListenerContext.Request.LocalEndPoint;
                if ((point != null) && (point.Address != null))
                {
                    return point.Address.ToString();
                }
                return "127.0.0.1";
            }
        }

        private static string LocalServerIP
        {
            get
            {
                if (_localServerIP == null)
                {
                    _localServerIP = Dns.GetHostEntry(Environment.MachineName).AddressList[0].ToString();
                }
                return _localServerIP;
            }
        }

        internal string RemoteIP
        {
            get
            {
                IPEndPoint point = ListenerContext.Request.RemoteEndPoint;
                if ((point != null) && (point.Address != null))
                {
                    return point.Address.ToString();
                }
                return "127.0.0.1";
            }
        }

        internal byte[] ReadHeadersAsByte()
        {
            //Read posted content



            StringBuilder header = new StringBuilder();

            header.Append(string.Format("{1} {2} HTTP/{0}\r\n",
                                        ListenerContext.Request.ProtocolVersion,
                                        ListenerContext.Request.HttpMethod,
                                        ListenerContext.Request.RawUrl));
            header.AppendFormat(string.Format("User-Agent: {0}\r\n", ListenerContext.Request.UserAgent));
            //header.AppendFormat(string.Format("Referer: {0}\r\n", _context.Request.UrlReferrer));
            for (int i = 0; i < ListenerContext.Request.Headers.Count; i++)
            {
                header.AppendFormat(string.Format("{0}: {1}\r\n",
                                                  ListenerContext.Request.Headers.GetKey(i),
                                                  ListenerContext.Request.Headers[i]));
            }
            header.Append("\r\n");
            byte[] headerBuffer = Encoding.UTF8.GetBytes(header.ToString());
            byte[] buffer = headerBuffer;
            if (ListenerContext.Request.HasEntityBody)
            {
                //Add posted content
                byte[] posted = new byte[ListenerContext.Request.ContentLength64];
                int numBytesToRead = (int)ListenerContext.Request.ContentLength64;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    // Read may return anything from 0 to numBytesToRead.
                    int n = ListenerContext.Request.InputStream.Read(posted, numBytesRead, numBytesToRead);
                    // The end of the file is reached.
                    if (n == 0)
                        break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }

                buffer = new byte[headerBuffer.Length + posted.Length];
                Buffer.BlockCopy(headerBuffer, 0, buffer, 0, headerBuffer.Length);
                Buffer.BlockCopy(posted, 0, buffer, headerBuffer.Length, posted.Length);

            }

            return buffer;
        }

        public bool CheckVirutualPath()
        {
            //Check trailing slash at the end of virtual dir
            if ((ListenerContext.Request.Url.AbsolutePath + "/").ToLower().Equals(_server.VirtualPath.ToLower()))
            {
                ListenerContext.Response.Redirect(_server.VirtualPath);
                ListenerContext.Response.Close();
                return false;
            }
            return true;
        }

        public void CheckRewritePath()
        {

        }

        private string GetErrorRedirect(int errorCode, bool isLocal)
        {
            if (!customErrorsInit)
            {
                var map = new ExeConfigurationFileMap()
                {
                    ExeConfigFilename = Path.Combine(_server.PhysicalPath, "Web.config")
                };
                var cfg = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
                customErrorsSection = cfg.GetSection("system.web/customErrors") as CustomErrorsSection;
                customErrorsInit = true;
            }
            if (customErrorsSection != null &&
                customErrorsSection.Mode != CustomErrorsMode.Off &&
                (!isLocal || (isLocal && customErrorsSection.Mode != CustomErrorsMode.RemoteOnly)))
            {
                var customError = customErrorsSection.Errors[errorCode.ToString()];
                return (customError != null ? customError.Redirect : customErrorsSection.DefaultRedirect);
            }
            return null;
        }
    }
}