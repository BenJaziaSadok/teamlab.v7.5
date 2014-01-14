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

using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASC.Web.Studio.Controls.FileUploader
{
    [ToolboxData("<{0}:FileUploaderRegistrator runat=server></{0}:FileUploaderRegistrator>")]
    public class FileUploaderRegistrator : WebControl
    {
        public static string ToHtml
        {
            get { return Resources.Resource.SwitchUploadToHtml; }
        }

        public static string ToFlash
        {
            get { return Resources.Resource.SwitchUploadToFlash; }
        }

        public static string DefaultRuntimes
        {
            get { return "html5,flash,html4"; }
        }

        public static string GetFlashUrl()
        {
            return VirtualPathUtility.ToAbsolute("~/js/uploader/plupload.flash.swf");
        }
    }
}