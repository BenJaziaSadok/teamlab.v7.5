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

namespace ASC.Web.Files.Controls
{
    public partial class Tree : UserControl
    {
        public static string Location
        {
            get { return PathProvider.GetFileStaticRelativePath("Tree/Tree.ascx"); }
        }

        public object FolderIDCurrentRoot { get; set; }

        protected bool IsVisitor;

        protected void Page_Load(object sender, EventArgs e)
        {
            IsVisitor = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID).IsVisitor();
        }
    }
}