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
using System.Web;
using System.Web.UI;
using ASC.Web.Studio.Utility;
using ASC.Web.Core.Utility;

namespace ASC.Web.Files.Masters
{
    public partial class BasicTemplate : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page is Share)
            {
                Master.DisabledHelpTour = true;
                Master.DisabledSidePanel = true;
                Master.DisabledTopStudioPanel = true;
            }
            else
            {
                Page.RegisterStyleControl(LoadControl(VirtualPathUtility.ToAbsolute("~/products/files/masters/styles.ascx")));
                Page.RegisterBodyScripts(LoadControl(VirtualPathUtility.ToAbsolute("~/products/files/masters/FilesScripts.ascx")));
            }
            Page.RegisterClientLocalizationScript(typeof(ClientScripts.FilesLocalizationResources));
            Page.RegisterClientScript(typeof(ClientScripts.FilesConstantsResources));
            Page.RegisterInlineScript("if (typeof ZeroClipboard != 'undefined') {ZeroClipboard.setMoviePath('" + CommonLinkUtility.ToAbsolute("~/js/flash/zeroclipboard/zeroclipboard10.swf") + "');}", true);
            Page.RegisterStyleControl(ColorThemesSettings.GetThemeFolderName(CommonLinkUtility.FilesBaseAbsolutePath + "/app_themes/<theme_folder>/leftmenu.less"), true);
        }
    }
}