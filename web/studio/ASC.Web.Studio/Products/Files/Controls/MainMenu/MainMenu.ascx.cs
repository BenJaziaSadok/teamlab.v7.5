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
using System.Web.UI;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Files.Classes;
using ASC.Web.Files.Import;
using ASC.Web.Studio.UserControls.Common.HelpCenter;
using ASC.Web.Studio.UserControls.Common.Support;

namespace ASC.Web.Files.Controls
{
    public partial class MainMenu : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("MainMenu/MainMenu.ascx"); }
        }

        public bool EnableCreateFile = true;
        public bool EnableImport = true;
        public bool EnableThirdParty = true;
        public bool EnableHelp = true;

        public object FolderIDCurrentRoot { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var isVisitor = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor();

            EnableImport = EnableImport
                           && !isVisitor
                           && ImportConfiguration.SupportImport;
            if (EnableImport)
                ControlHolder.Controls.Add(LoadControl(ImportControl.Location));

            EnableThirdParty = EnableThirdParty
                               && ImportConfiguration.SupportInclusion
                               && !isVisitor
                               && (Global.IsAdministrator
                                   || CoreContext.Configuration.YourDocs
                                   || FilesSettings.EnableThirdParty);

            CreateMenuHolder.Controls.Add(LoadControl(CreateMenu.Location));

            var tree = (Tree) LoadControl(Tree.Location);
            tree.FolderIDCurrentRoot = FolderIDCurrentRoot;
            ControlHolder.Controls.Add(tree);

            if (EnableHelp)
            {
                var helpCenter = (HelpCenter) LoadControl(HelpCenter.Location);
                helpCenter.IsSideBar = true;
                sideHelpCenter.Controls.Add(helpCenter);
            }
            sideSupport.Controls.Add(LoadControl(Support.Location));
        }
    }
}