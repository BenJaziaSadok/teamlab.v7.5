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
using ASC.Core;
using ASC.Web.Core.ModuleManagement.Common;

namespace ASC.Web.Community.News.Code.Module
{
	public class ShortcutProvider : IShortcutProvider
	{

        public static string GetCreateContentPageUrl()
        {
            if(SecurityContext.CheckPermissions(NewsConst.Action_Add))
                return FeedUrls.EditNewsUrl;
            
            return null;
        }

		public string GetAbsoluteWebPathForShortcut(Guid shortcutID, string currentUrl)
		{
			if (shortcutID.Equals(new Guid("499FCB8B-F715-45b2-A112-E99826F4B401")))//News
			{
				return FeedUrls.EditNewsUrl;
			}
			return string.Empty;
		}

		public bool CheckPermissions(Guid shortcutID, string currentUrl)
		{
			if (shortcutID.Equals(new Guid("499FCB8B-F715-45b2-A112-E99826F4B401")))//News
			{
				return SecurityContext.CheckPermissions(NewsConst.Action_Edit);
			}
			return true;
		}
	}
}