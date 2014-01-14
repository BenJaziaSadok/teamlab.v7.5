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
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using ASC.Blogs.Core;
using ASC.Web.Core.ModuleManagement.Common;
using ASC.Core;
using ASC.Blogs.Core.Security;

namespace ASC.Web.Community.Blogs
{    
    public class BlogsShortcutProvider : IShortcutProvider
    {
        public static string GetCreateContentPageUrl()
        {
            if (ASC.Core.SecurityContext.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(
                    SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost))

                return VirtualPathUtility.ToAbsolute(Constants.BaseVirtualPath + "addblog.aspx");

            return null;
        }

        public string GetAbsoluteWebPathForShortcut(Guid shortcutID, string currentUrl)
        {
            return "";
        }

        public bool CheckPermissions(Guid shortcutID, string currentUrl)
        {
            if (shortcutID.Equals(new Guid("98DB8D88-EDF2-4f82-B3AF-B95E87E3EE5C")) || 
                shortcutID.Equals(new Guid("20673DF0-665E-4fc8-9B44-D48B2A783508")))
            {
                return ASC.Core.SecurityContext.CheckPermissions(new PersonalBlogSecObject(CoreContext.UserManager.GetUsers(
                    SecurityContext.CurrentAccount.ID)), ASC.Blogs.Core.Constants.Action_AddPost);
            }            
            
            return false;
        }
    }
}
