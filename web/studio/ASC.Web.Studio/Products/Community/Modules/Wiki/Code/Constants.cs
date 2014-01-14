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
using System.Linq;
using System.Web;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Wiki.Resources;

namespace ASC.Web.UserControls.Wiki
{
    public class Constants
    {
        public const string WikiCategoryKeyCaption = "Category";	
        public const string WikiInternalCategoriesKey = "Categories";
        public const string WikiInternalFilesKey = "Files";
        public const string WikiInternalHelpKey = "Help";
        public const string WikiInternalHomeKey = "Home";
        public const string WikiInternalIndexKey = "Index";
        public const string WikiInternalKeyCaption = "Internal";
        public const string WikiInternalNewPagesKey = "NewPages";
        public const string WikiInternalRecentlyKey = "Recently";

        public static INotifyAction NewPage = new NotifyAction("new wiki page", WikiResource.NotifyAction_NewPage);
        public static INotifyAction EditPage = new NotifyAction("edit wiki page", WikiResource.NotifyAction_ChangePage);
        public static INotifyAction AddPageToCat = new NotifyAction("add page to cat", WikiResource.NotifyAction_AddPageToCat);

        public static string TagPageName = "PageName";
        public static string TagURL = "URL";

        public static string TagUserName = "UserName";
        public static string TagUserURL = "UserURL";
        public static string TagDate = "Date";

        public static string TagPostPreview = "PagePreview";
        public static string TagCommentBody = "CommentBody";

        public static string TagChangePageType = "ChangeType";
        public static string TagCatName = "CategoryName";
    }
}