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

using ASC.Data.Storage;
using ASC.Web.Core.Client.PageExtensions;
using dotless.Core;
using dotless.Core.configuration;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Yahoo.Yui.Compressor;
using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Web.Core.Client.Bundling
{
    public class BundlerHttpHandler : IHttpAsyncHandler
    {
        private static readonly ManualResetEvent ProcessingEvent = new ManualResetEvent(true);

        private static readonly Regex ReplaceImportRegex = new Regex(@"@import ""(.*?)"";",
                                                                     RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                                     RegexOptions.Compiled);

        private static readonly Regex ReplaceUrlRegex = new Regex(@"url\((.*?)\)",
                                                                  RegexOptions.IgnoreCase | RegexOptions.Singleline |
                                                                  RegexOptions.Compiled);

        private readonly Queue<BundleResource> _bundleResources;
        private readonly ContentType _contentType;
        private readonly Dictionary<string, string> _embeddedImageCache = new Dictionary<string, string>();
        private readonly ILog _log = LogManager.GetLogger("ASC.Web.BundlerHttpHandler");
        private readonly string _token;
        private readonly string _verison;
        private long _maxEmbeddableSize;

        public BundlerHttpHandler(IEnumerable<BundleResource> bundleResources, string token, string verison,
                                  ContentType contentType)
        {
            _bundleResources = new Queue<BundleResource>(bundleResources);
            _token = token;
            _verison = verison;
            _contentType = contentType;
        }

        #region IHttpAsyncHandler Members

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            //Make thread barrier here
            if (!ProcessingEvent.WaitOne(TimeSpan.FromSeconds(90)))
            {
                //If we didn't wait finishing then redirect here again
                context.Response.Redirect(context.Request.GetUrlRewriter().ToString(), true);
            }

            var asyncState = new AsyncBundlingRequestState(context, cb, extraData);
            if (!string.IsNullOrEmpty(context.Request.Headers["If-Modified-Since"]))
            {
                //Respond 304
                //Return always 304 because
                context.Response.StatusCode = (int)HttpStatusCode.NotModified;
                asyncState.IsFromCache = true;
                asyncState.OnCompleted(true);
            }
            else
            {

                //Try cached
                var cached = HttpRuntime.Cache.Get(GetKey(new HttpRequestWrapper(context.Request))) as StringBuilder;
                if (cached != null)
                {
                    asyncState.IsFromCache = true;
                    asyncState.ResourceData = cached;
                    asyncState.OnCompleted(true);
                }
                else
                {
                    //Beign Read
                    _maxEmbeddableSize = ClientCapabilities.GetMaxEmbeddableImageSize(new HttpRequestWrapper(context.Request));
                    ProcessingEvent.Reset();

                    if (_bundleResources.Count(r=> r.Content.Contains("jquery"))<= 0)
                    {
                        var content =
@"/*
    Copyright (c) Ascensio System SIA " + DateTime.UtcNow.Year + @". All rights reserved.
    http://www.teamlab.com
*/";
                        asyncState.ResourceData.Append(content).Append(Environment.NewLine);

                        asyncState.Context.Response.Write(content);
                        asyncState.Context.Response.Write(Environment.NewLine);
                    }
                   

                    ProcessItem(_bundleResources.Dequeue(), asyncState);
                }
            }
            return asyncState;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var reqState = (AsyncBundlingRequestState)result;
      
            if (reqState.ResourceData.Length > 0)
            {
                //Write response
                if (reqState.IsFromCache)
                {
                    //Write only if it cached
                    reqState.Context.Response.Write(reqState.ResourceData.ToString());
                }
            }

            if (String.IsNullOrEmpty(reqState.ResourceData.ToString()) && string.IsNullOrEmpty(reqState.Context.Request.Headers["If-Modified-Since"]))
                _log.ErrorFormat("BundlerHttpHandler ResourceData is Empty for Request URI {0}. Is from cache {1}", reqState.Context.Request.Url.ToString(), reqState.IsFromCache);

         
            if (ClientSettings.IsResourceCachingEnabled)
            {
                if (!reqState.IsFromCache)
                {
                    HttpRuntime.Cache.Insert(GetKey(new HttpRequestWrapper(reqState.Context.Request)),
                                            reqState.ResourceData,
                                             new CacheDependency(new string[0], new[] { _token }),
                                             Cache.NoAbsoluteExpiration,
                                             Cache.NoSlidingExpiration, 
                                             CacheItemPriority.Default,
                                             RemovedCallback
                                             );
                }
                reqState.Context.Response.Cache.SetLastModified(new DateTime(DateTime.UtcNow.Year, 1, 1));
                reqState.Context.Response.Cache.SetAllowResponseInBrowserHistory(true);
                reqState.Context.Response.Cache.SetCacheability(HttpCacheability.Public);
                reqState.Context.Response.Cache.SetRevalidation(HttpCacheRevalidation.None);
                reqState.Context.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(3));
                reqState.Context.Response.Cache.SetMaxAge(TimeSpan.FromDays(365 * 3));
                reqState.Context.Response.Cache.SetValidUntilExpires(true);
            }

            reqState.Context.Response.Charset = Encoding.UTF8.WebName;
            reqState.Context.Response.ContentType = _contentType.ToString();
            ProcessingEvent.Set();
        }

        public void RemovedCallback(String key, Object reason, CacheItemRemovedReason r)
        {

        }

        #endregion

        private void ProcessItem(BundleResource resource, AsyncBundlingRequestState asyncState)
        {
            if (resource == BundleResource.Empty)
            {
                return;
            }

            if (resource.Type == BundleResourceType.EmbeddedScript || resource.Type == BundleResourceType.EmbeddedStyle)
            {
                resource.ServerPath = null;
                WriteResponse(resource, asyncState);

                ProcessItem(GetNextResource(asyncState), asyncState);
            }
            else if (resource.Type == BundleResourceType.ClientScript)
            {
                try
                {
                    var handler = ClientScriptBundle.GetHttpHandler(resource.ServerPath.Split('/').Last().Split('.').First());
                    var content = handler.GetData(asyncState.Context);

                    asyncState.ResourceData.Append(content).Append(Environment.NewLine);
                    asyncState.Context.Response.Write(content);
                    asyncState.Context.Response.Write(Environment.NewLine);

                    ProcessItem(GetNextResource(asyncState), asyncState);
                }
                catch (Exception e)
                {
                    _log.Error("ClientScriptError", e);
                }
            }
            else
            {
                //If it's file begin read and go to IOCP thread
                if (File.Exists(resource.Content))
                {
                    FileStream file = File.Open(resource.Content, FileMode.Open, FileAccess.Read, FileShare.Read);
                    var buffer = new byte[file.Length];
                    file.BeginRead(buffer, 0, (int)file.Length, result =>
                                                                     {
                                                                         try
                                                                         {
                                                                             int readed = file.EndRead(result);
                                                                             var asyncObject =
                                                                                 result.AsyncState as
                                                                                 AsyncBundlingRequestState;
                                                                             HttpContext.Current = asyncState.Context;
                                                                             resource.Content =
                                                                                 Encoding.UTF8.GetString(buffer, 0,
                                                                                                         readed);
                                                                             WriteResponse(resource, asyncState);
                                                                             ProcessItem(GetNextResource(asyncObject),
                                                                                         asyncObject);
                                                                         }
                                                                         catch (Exception e)
                                                                         {
                                                                             _log.Error(
                                                                                 string.Format(
                                                                                     "Error while processing file:{0}",
                                                                                     resource.Content), e);
                                                                         }
                                                                         finally
                                                                         {
                                                                             //We got wht we need in buffer. close stream
                                                                             file.Close();
                                                                             file.Dispose();
                                                                         }
                                                                     }, asyncState);
                }
            }
        }

        private void WriteResponse(BundleResource bundleResource, AsyncBundlingRequestState asyncState)
        {

            string content = TransformContent(bundleResource, asyncState.Context);
            asyncState.ResourceData.Append(content).Append(Environment.NewLine);

            asyncState.Context.Response.Write(content);
            asyncState.Context.Response.Write(Environment.NewLine);

        }
       
        private BundleResource GetNextResource(AsyncBundlingRequestState asyncState)
        {
            if (_bundleResources.Count == 0)
            {
                asyncState.OnCompleted();
                return BundleResource.Empty;
            }
            return _bundleResources.Dequeue();
        }

        private string TransformContent(BundleResource bundleResource, HttpContext context)
        {
            var bundleType = bundleResource.Type;
            var content = bundleResource.Content;
            var serverpath = bundleResource.ServerPath;

            try
            {
                if (bundleType == BundleResourceType.EmbeddedScript || bundleType == BundleResourceType.ScriptFile)
                {
                    var compressor = new JavaScriptCompressor
                    {
                        CompressionType = CompressionType.Standard,
                        Encoding = Encoding.UTF8,
                        ObfuscateJavascript = bundleResource.ObfuscateJs
                    };


                    //Minimize
                    var contentOut = compressor.Compress(content.Trim()) + ";";

                    //Return deffered execution
                    if (ClientSettings.IsJavascriptDefferingEnabled)
                    {
                        return string.Format(ClientSettings.JavascriptDefferingScript,
                                      JsonConvert.SerializeObject(contentOut));
                    }
                    return contentOut;
                }
                if (!string.IsNullOrEmpty(serverpath))
                {
                    string directoryName = Path.GetDirectoryName(serverpath);
                    if (directoryName != null)
                        serverpath = directoryName.Replace('\\', '/');
                }
                if (bundleType == BundleResourceType.EmbeddedStyle || bundleType == BundleResourceType.StyleFile)
                {
                    var compressor = new CssCompressor
                                         {
                                             CompressionType = CompressionType.Standard,
                                             RemoveComments = true
                                         };

                    return ReplaceUrls(compressor.Compress(content), serverpath, context);
                }
                if (bundleType == BundleResourceType.LessFile)
                {
                    DotlessConfiguration cfg = DotlessConfiguration.GetDefaultWeb();
                    cfg.Web = true;
                    cfg.MinifyOutput = true;
                    cfg.MapPathsToWeb = true;
                    cfg.CacheEnabled = false;

                    //Prefilter
                    content = ReplaceImportRegex.Replace(content, match => ReplaceImports(match, serverpath));
                    string processed = ReplaceUrls(LessWeb.Parse(content, cfg), serverpath, context);
                    return processed;
                }
            }
            catch (EcmaScript.NET.EcmaScriptException e)
            {
                _log.ErrorFormat("EcmaScriptException: {0} in {1} at {2} ({3}, {4}) at ", e.Message, serverpath, e.LineSource, e.LineNumber, e.ColumnNumber, e.ScriptStackTrace);
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            return content;
        }

        private string ReplaceUrls(string cssContent, string serverPath, HttpContext context)
        {
            return ReplaceUrlRegex.Replace(cssContent, match => ReplaceUris(match, serverPath, context));
        }
      
        private string ReplaceUris(Match match, string serverpath, HttpContext context)
        {
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    string path = match.Groups[1].Value.Trim('\'', '"');
                    if (Uri.IsWellFormedUriString(path, UriKind.Relative))
                    {
                        path = PathCombine(serverpath, path);

                        if (ClientSettings.IsResourceStoreEnabled)
                        {
                            var rootpath = context.Server.MapPath("~/").ToLower();
                            var physicalPath = context.Server.MapPath(path).ToLower();
                            
                            physicalPath =  physicalPath.Remove(0,rootpath.Length);
                            physicalPath = physicalPath.Replace("\\", "/");
                            path = WebPath.GetPath(physicalPath);
                        }

                    }
                  
                    if (ClientSettings.IsImageEmbeddingEnabled)
                    {
                        string base64Encoded;
                        if (!_embeddedImageCache.TryGetValue(path, out base64Encoded))
                        {
                            //Try add
                            string filePath = context.Server.MapPath(path);
                            if (File.Exists(filePath))
                            {

                                var info = new FileInfo(filePath);
                                if (info.Length < _maxEmbeddableSize)
                                {
                                    //Embed!!!!
                                    byte[] data = File.ReadAllBytes(filePath);
                                    base64Encoded = string.Format("data:{0};base64,{1}",
                                                                  MimeMapping.GetMimeMapping(filePath),
                                                                  Convert.ToBase64String(data));
                                }
                                else
                                {
                                    base64Encoded = string.Format("\"{0}\"", path);//Just path
                                }
                                _embeddedImageCache[path] = base64Encoded;
                            }
                        }
                        return match.Value.Replace(match.Groups[1].Value, base64Encoded);
                    }

                  
                    //If embedding is disabled just return path
                    return match.Value.Replace(match.Groups[1].Value, string.Format("{0}", path));
                 
                }
            }
            return match.Value;
        }

        private string PathCombine(string rootPath, string combinePath)
        {
            if (rootPath == null) throw new ArgumentNullException("rootPath");
            if (combinePath == null) throw new ArgumentNullException("combinePath");

            if (combinePath[0] == '/')
                return combinePath;
            var path = new Stack<string>(rootPath.Split('/'));
            foreach (string part in combinePath.Split('/'))
            {
                if (part == "..")
                {
                    path.Pop();
                }
                else if (part != ".")
                {
                    path.Push(part);
                }
            }
            string[] pathArray = path.ToArray();
            Array.Reverse(pathArray);
            return string.Join("/", pathArray);
        }


        private string ReplaceImports(Match match, string serverpath)
        {
            if (match.Success)
            {
                if (match.Groups[1].Success)
                {
                    string path = match.Groups[1].Value;
                    if (path.StartsWith("~"))
                    {
                        path = VirtualPathUtility.ToAbsolute(path);
                    }
                    else if (Uri.IsWellFormedUriString(path, UriKind.Relative))
                    {
                        path = PathCombine(serverpath, path);
                    }
                    return match.Value.Replace(match.Groups[1].Value, path);
                }
            }
            return match.Value;
        }


        private string GetKey(HttpRequestBase request)
        {
            return _token + _verison + _contentType + ClientCapabilities.GetMaxEmbeddableImageSize(request);
        }
    }
}