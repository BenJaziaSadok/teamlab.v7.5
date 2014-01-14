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
using ASC.Core;
using ASC.Web.Files.Controls;
using ASC.Web.Files.Resources;
using ASC.Web.Studio;

namespace ASC.Web.Files
{
    public partial class _Default : MainPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            const bool enableThirdParty = true;
            var enableHelp = !CoreContext.Configuration.YourDocs;

            if (SecurityContext.IsAuthenticated)
            {
                var mainMenu = (MainMenu) LoadControl(MainMenu.Location);
                mainMenu.EnableImport = true;
                mainMenu.EnableThirdParty = enableThirdParty;
                mainMenu.EnableHelp = enableHelp;
                CommonSideHolder.Controls.Add(mainMenu);
            }

            var mainContent = (MainContent) LoadControl(MainContent.Location);
            mainContent.TitlePage = FilesCommonResource.TitlePage;
            mainContent.EnableThirdParty = enableThirdParty;
            mainContent.EnableHelp = enableHelp;
            CommonContainerHolder.Controls.Add(mainContent);

            CommonContainerHolder.Controls.Add(LoadControl(AccessRights.Location));
        }
    }
}