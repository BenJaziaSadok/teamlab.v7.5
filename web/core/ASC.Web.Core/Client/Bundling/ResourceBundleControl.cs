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

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;

namespace ASC.Web.Core.Client.Bundling
{

    public class ResourceBundleControl : UserControl
    {
        public HashSet<string> Scripts { get; private set; }
        public HashSet<string> Styles { get; private set; }
        public String CategoryName { get; set; }


        public ResourceBundleControl()
        {
            Scripts = new HashSet<string>();
            Styles = new HashSet<string>();
        }

        protected override void Render(HtmlTextWriter writer)
        {
            if (ClientSettings.BundlingEnabled)
            {
                using (var html = new StringWriter())
                using (var htmlWriter = new HtmlTextWriter(html))
                {
                    Write(htmlWriter);
                    writer.Write(BundleHtml(CategoryName, html.ToString()));
                }
            }
            else
            {
                Write(writer);
            }
        }

        private void Write(HtmlTextWriter writer)
        {
            base.Render(writer);
            foreach (var s in Scripts)
            {
                writer.WriteLine(BundleHelper.HtmlScript(ClientSettings.BundlingEnabled ? s : VirtualPathUtility.ToAbsolute(s), false, true));
            }
            foreach (var s in Styles)
            {
                writer.WriteLine(BundleHelper.HtmlLink(ClientSettings.BundlingEnabled ? s : VirtualPathUtility.ToAbsolute(s), false));
            }
        }

        private string BundleHtml(string category, string html)
        {
            var result = new StringBuilder();

            var hash = HttpServerUtility.UrlTokenEncode(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(html)));
            var path = string.Format("~{0}{1}-{2}", BundleHelper.BUNDLE_VPATH, GetCategory(category), hash);
            var pathcss = path + ".css";
            var pathjs = path + ".js";

            var bundlecss = BundleHelper.GetCssBundle(pathcss);
            var bundlejs = BundleHelper.GetJsBundle(pathjs);

            if (bundlecss == null && bundlejs == null)
            {
                var document = new HtmlDocument();
                document.LoadHtml(html);

                if (bundlecss == null)
                {
                    var styles = document.DocumentNode.SelectNodes("/style | /link[@rel='stylesheet'] | /link[@rel='stylesheet/less']");
                    if (styles != null && 0 < styles.Count)
                    {
                        bundlecss = BundleHelper.CssBundle(pathcss);
                        foreach (var style in styles)
                        {
                            if (style.Name == "style" && !string.IsNullOrEmpty(style.InnerHtml))
                            {
                                throw new NotSupportedException("Embedded styles not supported.");
                            }
                            bundlecss.Include(style.Attributes["href"].Value);
                        }
                        BundleHelper.AddBundle(bundlecss);
                    }
                }

                if (bundlejs == null)
                {
                    var scripts = document.DocumentNode.SelectNodes("/script");
                    if (scripts != null && 0 < scripts.Count)
                    {
                        bundlejs = BundleHelper.JsBundle(pathjs);
                        foreach (var script in scripts)
                        {
                            if (script.Attributes["src"] == null && !string.IsNullOrEmpty(script.InnerHtml))
                            {
                                throw new NotSupportedException("Embedded scripts not supported.");
                            }
                            bundlejs.Include(script.Attributes["src"].Value, script.Attributes["notobfuscate"] == null);
                        }
                        BundleHelper.AddBundle(bundlejs);
                    }
                }
            }

            if (bundlecss != null)
            {
                result.AppendLine(BundleHelper.HtmlLink(pathcss));
            }
            if (bundlejs != null)
            {
                result.AppendLine(BundleHelper.HtmlScript(pathjs));
            }
            return result.ToString();
        }

        private string GetCategory(string category)
        {
            if (string.IsNullOrEmpty(category))
            {
                category = "common";
                if (HttpContext.Current.Request.Url != null && !string.IsNullOrEmpty(HttpContext.Current.Request.Url.AbsolutePath))
                {
                    var matches = Regex.Match(HttpContext.Current.Request.Url.AbsolutePath, "(products|addons)/(\\w+)/?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                    if (matches.Success && 2 < matches.Groups.Count && matches.Groups[2].Success)
                    {
                        category = matches.Groups[2].Value;
                    }
                }
            }
            return category;
        }
    }
}