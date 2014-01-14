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
using ASC.Web.Files.Classes;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Files.Controls
{
    public partial class CreateMenu : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("CreateMenu/CreateMenu.ascx"); }
        }

        public bool EnableCreateFile = true;
        public bool EnableUpload = true;
        public bool EnableImport = true;
        public bool EnableThirdParty = true;

        public object FolderIDCurrentRoot { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            EnableCreateFile = EnableCreateFile
                               && !Web.Core.Mobile.MobileDetector.IsRequestMatchesMobile(Context)
                               && FileUtility.ExtsWebEdited.Count != 0;
        }
    }
}