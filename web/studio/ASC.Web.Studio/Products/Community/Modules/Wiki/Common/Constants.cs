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
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Web.UserControls.Wiki.Resources;
using AuthAction = ASC.Common.Security.Authorizing.Action;

namespace ASC.Web.Community.Wiki.Common
{
    public class Constants
    {
        public static readonly AuthAction Action_AddPage = new AuthAction(new Guid("D49F4E30-DA10-4b39-BC6D-B41EF6E039D3"), "New Page");
        public static readonly AuthAction Action_EditPage = new AuthAction(new Guid("D852B66F-6719-45e1-8657-18F0BB791690"), "Edit page");
        public static readonly AuthAction Action_RemovePage = new AuthAction(new Guid("557D6503-633B-4490-A14C-6473147CE2B3"), "Delete page");
        public static readonly AuthAction Action_UploadFile = new AuthAction(new Guid("088D5940-A80F-4403-9741-D610718CE95C"), "Upload file");
        public static readonly AuthAction Action_RemoveFile = new AuthAction(new Guid("7CB5C0D1-D254-433f-ABE3-FF23373EC631"), "Delete file");
        public static readonly AuthAction Action_AddComment = new AuthAction(new Guid("C426C349-9AD4-47cd-9B8F-99FC30675951"), "Add Comment");
        public static readonly AuthAction Action_EditRemoveComment = new AuthAction(new Guid("B630D29B-1844-4bda-BBBE-CF5542DF3559"), "Edit/Delete comment");

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