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
using System.Reflection;

namespace ASC.Forum
{
	public enum AttachmentContentType
    {
        Uknown = 0,

        Audio = 2,

        Video = 1,

        Image = 3,

        Application = 4,

        Office = 5,

        Text = 6,

        Zip = 7,

        Others = 8
    }

	public class Attachment
    {       
        public virtual int ID{get; set;}

        public virtual DateTime CreateDate { get; set; }
       
        public virtual string Name{get; set;}  
        
        public virtual int Size{get; set;}

        public virtual int DownloadCount{get; set;}

        public virtual AttachmentContentType ContentType { get; set; }      

        public virtual string MIMEContentType { get; set; }

        public virtual string OffsetPhysicalPath { get; set; }

        public virtual int PostID { get; set; }

        public virtual int TenantID { get; set; }

        public Attachment()
        {
            this.ID= 0;            
            this.Name = "";
            this.CreateDate = ASC.Core.Tenants.TenantUtil.DateTimeNow();
            this.Size = 0;
            this.MIMEContentType = "";
            this.ContentType = AttachmentContentType.Uknown;
            this.OffsetPhysicalPath = "";
        }
    }
}
