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
using ASC.Web.Files.Classes;

namespace ASC.Web.Files.Controls
{
    public partial class ImportControl : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("ImportControl/ImportControl.ascx"); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Page.RegisterBodyScripts(ResolveUrl("~/products/files/Controls/ImportControl/importcontrol.js"));

            ImportDialogTemp.Options.IsPopup = true;
            LoginDialogTemp.Options.IsPopup = true;
        }
    }
}