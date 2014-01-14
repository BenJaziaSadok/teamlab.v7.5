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
using System.Collections.Generic;
using System.Web;
using ASC.Web.UserControls.Wiki.Handlers;

namespace ASC.Web.UserControls.Wiki
{
    public class ActionHelper
    {
        public static string GetAddPagePath(string mainPath)
        {
            return string.Format("{0}?action=New", mainPath);
        }

        public static string GetAddFilePath(string mainPath)
        {
            return string.Format("{0}?action=NewFile", mainPath);
        }

        public static string GetEditPagePath(string mainPath, string pageName)
        {
            return string.Format("{0}?page={1}&action=Edit", mainPath, PageNameUtil.Encode(pageName));
        }

        public static string GetViewPagePath(string mainPath, string pageName, string spetial)
        {
            return string.Format("{0}?page={1}", mainPath, string.IsNullOrEmpty(spetial) ? PageNameUtil.Encode(pageName) : string.Format(@"{0}:{1}", spetial, PageNameUtil.Encode(pageName)));
        }
        public static string GetViewPagePath(string mainPath, string pageName)
        {
			if (pageName == null)
			{
				return mainPath;
			}
            return string.Format("{0}?page={1}", mainPath, PageNameUtil.Encode(pageName));
        }

        public static string GetViewPagePathWithVersion(string mainPath, string pageName, int version)
        {
            return string.Format("{0}?page={1}&ver={2}", mainPath, PageNameUtil.Encode(pageName), version);
        }

        public static string GetEditFilePath(string mainPath, string pageName)
        {
            return string.Format("{0}?file={1}&action=Edit", mainPath, PageNameUtil.Encode(pageName));
        }

        public static string GetViewFilePath(string mainPath, string pageName)
        {
            return string.Format("{0}?file={1}", mainPath, pageName);
        }
    }
}
