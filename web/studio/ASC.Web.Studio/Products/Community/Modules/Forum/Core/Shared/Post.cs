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

#region Usings

using System;
using System.Collections.Generic;
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Users;

#endregion

namespace ASC.Forum
{
    public enum PostTextFormatter
    { 
        BBCode = 0,
        FCKEditor =1
    }

	public class Post : ISecurityObject
    {
        public int ID { get; set; }

        public Guid PosterID { get; set; }

        public UserInfo Poster
        {
            get { return CoreContext.UserManager.GetUsers(PosterID); }
        }

        public Guid EditorID { get; set; }

        public UserInfo Editor
        {
            get { return CoreContext.UserManager.GetUsers(EditorID); }
        }

        public string Subject { get; set; }

        public string Text { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime EditDate { get; set; }

        public int EditCount { get; set; }       

        public bool IsApproved { get; set; }

        public List<Attachment> Attachments { get; set; }

        public PostTextFormatter Formatter { get; set; }

        public int TenantID { get; set; }

        public int TopicID { get; set; }

        public int ParentPostID { get; set; }

        public Topic Topic { get; set; }


        public Post()
        {
            Attachments = new List<Attachment>();
            CreateDate = Core.Tenants.TenantUtil.DateTimeNow();
            EditDate = DateTime.MinValue;            
            EditCount = 0;
            Formatter = PostTextFormatter.BBCode;
            PosterID = SecurityContext.CurrentAccount.ID;
        }

        public Post(string subject, string text)
        {
            Attachments = new List<Attachment>();
            CreateDate = Core.Tenants.TenantUtil.DateTimeNow();
            EditDate = DateTime.MinValue;            
            EditCount = 0;
            Subject = subject;
            Text = text;
            PosterID = SecurityContext.CurrentAccount.ID;
        }

        #region ISecurityObjectId Members

        public object SecurityId
        {
            get { return ID; }
        }

        public Type ObjectType
        {
            get { return GetType(); }
        }

        #endregion

        #region ISecurityObjectProvider Members

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            var roles = new List<IRole>();
            if (account.ID.Equals(PosterID))
                roles.Add(Common.Security.Authorizing.Constants.Owner);

            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            return new Topic {ID = TopicID };
        }

        public bool InheritSupported
        {
            get { return true; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion
    }
}
