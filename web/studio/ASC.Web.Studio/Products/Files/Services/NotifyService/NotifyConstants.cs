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

using ASC.Notify.Model;

namespace ASC.Web.Files.Services.NotifyService
{
    public static class NotifyConstants
    {
        #region Events

        public static readonly INotifyAction Event_ShareDocument = new NotifyAction("ShareDocument", "share document");
        public static readonly INotifyAction Event_ShareFolder = new NotifyAction("ShareFolder", "share folder");
        public static readonly INotifyAction Event_LinkToEmail = new NotifyAction("LinkToEmail", "link to email");

        #endregion

        #region  Tags

        public static readonly string Tag_FolderID = "FolderID";
        public static readonly string Tag_DocumentTitle = "DocumentTitle";
        public static readonly string Tag_DocumentUrl = "DocumentURL";
        public static readonly string Tag_AccessRights = "AccessRights";
        public static readonly string Tag_Message = "Message";
       
    
        #endregion
    }
}