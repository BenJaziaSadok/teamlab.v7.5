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
using ASC.Web.Files.Classes;
using ASC.Web.Files.Controls;
using ASC.Web.Studio;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files
{
    public partial class Share : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var accessRights = (AccessRights)LoadControl(AccessRights.Location);
            accessRights.IsPopup = false;
            CommonContainerHolder.Controls.Add(accessRights);

            InitScript();
        }

        private void InitScript()
        {
            Page.RegisterStyleControl(CommonLinkUtility.FilesBaseAbsolutePath + "controls/accessrights/accessrights.css");
            Page.RegisterBodyScripts(VirtualPathUtility.ToAbsolute("~/js/third-party/zeroclipboard.js"));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("common.js", true));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("templatemanager.js", true));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("servicemanager.js", true));
            Page.RegisterBodyScripts(PathProvider.GetFileStaticRelativePath("ui.js", true));

            var script = new StringBuilder();
            script.AppendFormat("ASC.Files.Share.getSharedInfo('file', '{0}','{1}', true);", Request[CommonLinkUtility.FileId], Request[CommonLinkUtility.FileTitle]);
            Page.RegisterInlineScript(script.ToString());
        }
    }
}