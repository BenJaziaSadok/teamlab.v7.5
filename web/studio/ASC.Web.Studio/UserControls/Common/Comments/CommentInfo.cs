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

namespace ASC.Web.Studio.UserControls.Common.Comments
{
    public class Attachment
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }

    public class CommentInfo
    {
        public string CommentID { get; set; }
        public Guid UserID { get; set; }
        public string UserPost { get; set; }
        public string UserFullName { get; set; }
        public string UserAvatar { get; set; }
        public string CommentBody { get; set; }
        public bool Inactive { get; set; }
        public bool IsRead { get; set; }
        public bool IsEditPermissions { get; set; }
        public bool IsResponsePermissions { get; set; }
        public string JavascriptEdit { get; set; }
        public string JavascriptResponse { get; set; }
        public string JavascriptRemove { get; set; }
        public DateTime TimeStamp { get; set; }
        public string TimeStampStr { get; set; }
        public IList<CommentInfo> CommentList { get; set; }
        public IList<Attachment> Attachments { get; set; }
    }
}