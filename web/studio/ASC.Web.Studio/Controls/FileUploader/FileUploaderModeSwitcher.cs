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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxPro;

namespace ASC.Web.Studio.Controls.FileUploader
{
    [AjaxNamespace("FUploaderModeSwitcher")]
    public class FileUploaderModeSwitcher : WebControl
    {
        private static bool SupportFlashUpload()
        {
            return (
                       HttpContext.Current != null && HttpContext.Current.Session != null
                       && (!HttpContext.Current.Request.Browser.Browser.ToLower().Contains("safari")
                           || HttpContext.Current.Request.UserAgent.ToLower().Contains("chrome"))
                       && !HttpContext.Current.Request.Browser.Browser.ToLower().Contains("opera")
                   );
        }

        private bool _isFlashMode;

        public FileUploaderModeSwitcher()
        {
            _isFlashMode = IsFlashMode();
        }

        public FileUploaderModeSwitcher(bool isFlashMode)
        {
            _isFlashMode = isFlashMode;
        }

        public static bool IsFlashMode()
        {
            return !(HttpContext.Current != null && HttpContext.Current.Session != null
                     && HttpContext.Current.Session["fileuploader_mode"] != null
                     && (bool)HttpContext.Current.Session["fileuploader_mode"] == false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            AjaxPro.Utility.RegisterTypeForAjax(this.GetType());

            var sb = new StringBuilder();
            sb.Append(@"
                if (typeof (ASC) == 'undefined')
		            ASC = {};

	            if (typeof (ASC.Controls) == 'undefined')
		            ASC.Controls = {};

                if (typeof (ASC.Controls.FileUploaderGlobalConfig) == 'undefined')
		            ASC.Controls.FileUploaderGlobalConfig = {};

                if (typeof (ASC.Controls.FileUploadSwitcher) == 'undefined')
                    ASC.Controls.FileUploadSwitcher = new function()
                    {
                          this.IsFlash = " + (SupportFlashUpload() && _isFlashMode ? "true" : "false") + @";

                          ASC.Controls.FileUploaderGlobalConfig.DisableFlash = !this.IsFlash;

                          this.SwitchMode = function()
                          {
                               AjaxPro.onLoading = function(b) {};
                               FUploaderModeSwitcher.SwitchMode(!this.IsFlash, function(result){
                                      ASC.Controls.FileUploadSwitcher.IsFlash = !ASC.Controls.FileUploadSwitcher.IsFlash;
                                      window.location.reload(true);
                                                
                               });
                          }
                    };");

            Page.RegisterInlineScript(sb.ToString());
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            var sb = new StringBuilder();
            sb.Append("<div id=\"studio_fuplswitcherBox\">" + RenderNote() + "</div>");
            sb.Append("<input type='file' id='FileMultipleUploadInput' multiple='multiple' style='display: none;' onchange='FileHtml5Uploader.OnMultipleInputChange(this.files);'/>");
            writer.Write(sb.ToString());
        }

        private string RenderNote()
        {
            return SupportFlashUpload() ? String.Format(
                (_isFlashMode
                     ? string.Format(Resources.Resource.SwitchUploadToHtmlMode, "<div>", "<a class=\"linkAction\" href=\"{0}\">", "</a></div>")
                     : string.Format(Resources.Resource.SwitchUploadToFlashMode, "<div>", "<a class=\"linkAction\" href=\"{0}\">", "</a></div>")),
                "javascript:ASC.Controls.FileUploadSwitcher.SwitchMode()")
                       : "<div> </div>";
        }

        [AjaxMethod(HttpSessionStateRequirement.ReadWrite)]
        public string SwitchMode(bool isFlash)
        {
            HttpContext.Current.Session["fileuploader_mode"] = isFlash;
            _isFlashMode = isFlash;

            return RenderNote();
        }
    }
}