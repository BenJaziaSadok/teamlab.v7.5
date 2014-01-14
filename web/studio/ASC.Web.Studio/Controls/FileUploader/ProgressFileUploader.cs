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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MimeMapping = ASC.Common.Web.MimeMapping;

namespace ASC.Web.Studio.Controls.FileUploader
{
    [Themeable(true)]
    [ToolboxData("<{0}:ProgressFileUploader runat=server></{0}:ProgressFileUploader>")]
    public class ProgressFileUploader : WebControl
    {
        public bool EnableHtml5 { get; set; }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/ajaxupload.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/fileuploader.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/swfupload.js"));
            Page.RegisterBodyScripts(ResolveUrl("~/js/uploader/fileHtml5Uploader.js"));
            Page.RegisterInlineScript(string.Format("FileHtml5Uploader.EnableHtml5Flag = {0};", EnableHtml5.ToString().ToLower()));
            Page.RegisterInlineScript(string.Format("ASC.Controls.FileUploaderSWFLocation = '{0}';", ResolveUrl("~/js/uploader/swfupload.swf")));
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.Write("<div id=\"asc_fileuploaderSWFContainer\" style='position:absolute;'><span id=\"asc_fileuploaderSWFObj\"></span></div>");
        }

        private static bool IsHtml5Upload(HttpContext context)
        {
            return "html5".Equals(context.Request["type"]);
        }

        private static string GetFileName(HttpContext context)
        {
            return context.Request["fileName"];
        }

        private static string GetFileContentType(HttpContext context)
        {
            return context.Request["fileContentType"];
        }

        public static bool HasFilesToUpload(HttpContext context)
        {
            return 0 < context.Request.Files.Count || (IsHtml5Upload(context) && context.Request.InputStream != null);
        }

        public class FileToUpload
        {
            public string FileName { get; private set; }
            public Stream InputStream { get; private set; }
            public string FileContentType { get; private set; }
            public long ContentLength { get; private set; }

            public FileToUpload(HttpContext context)
            {
                if (IsHtml5Upload(context))
                {
                    FileName = GetFileName(context);
                    InputStream = context.Request.InputStream;
                    FileContentType = GetFileContentType(context);
                    ContentLength = (int)context.Request.InputStream.Length;
                }
                else
                {
                    var file = context.Request.Files[0];
                    FileName = file.FileName;
                    InputStream = file.InputStream;
                    FileContentType = file.ContentType;
                    ContentLength = file.ContentLength;
                }
                if (string.IsNullOrEmpty(FileContentType))
                {
                    FileContentType = MimeMapping.GetMimeMapping(FileName) ?? string.Empty;
                }
                FileName = FileName.Replace("'", "_").Replace("\"", "_");
            }
        }
    }
}