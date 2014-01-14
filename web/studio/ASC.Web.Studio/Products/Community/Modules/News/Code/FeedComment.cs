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
using ASC.Common.Security;
using ASC.Common.Security.Authorizing;
using ASC.Core;
using ASC.Core.Tenants;

namespace ASC.Web.Community.News.Code
{
    [Serializable]
    public class FeedComment : ISecurityObject
    {
        public long Id { get; set; }

        public long FeedId { get; set; }

        public string Comment { get; set; }

        public long ParentId { get; set; }

        public DateTime Date { get; set; }

        public string Creator { get; set; }

        public bool Inactive { get; set; }

        public Feed Feed { get; set; }

        public FeedComment(long feedId)
        {
            FeedId = feedId;
            Creator = SecurityContext.CurrentAccount.ID.ToString();
            Date = TenantUtil.DateTimeNow();
            Inactive = false;
        }

        public FeedComment(long feedId, Feed feed)
        {
            FeedId = feedId;
            Creator = SecurityContext.CurrentAccount.ID.ToString();
            Date = TenantUtil.DateTimeNow();
            Inactive = false;
            Feed = feed;
        }

        public FeedComment()
        {
        }

        public bool IsRoot()
        {
            return ParentId == 0;
        }

        public List<FeedComment> SelectChildLevel(List<FeedComment> from)
        {
            return SelectChildLevel(Id, from);
        }

        public static List<FeedComment> SelectRootLevel(List<FeedComment> from)
        {
            return SelectChildLevel(0, from);
        }

        public static List<FeedComment> SelectChildLevel(long forParentId, List<FeedComment> from)
        {
            return from.FindAll(comm => comm.ParentId == forParentId);
        }

        #region ISecurityObjectId Members

        public Type ObjectType
        {
            get { return GetType(); }
        }

        public object SecurityId
        {
            get { return Id; }
        }

        #endregion

        #region ISecurityObjectProvider Members

        public IEnumerable<IRole> GetObjectRoles(ISubject account, ISecurityObjectId objectId,
                                                 SecurityCallContext callContext)
        {
            var roles = new List<IRole>();

            if (Equals(account.ID, new Guid(Creator)))
            {
                roles.Add(Constants.Owner);
            }

            return roles;
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        #endregion

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var c = obj as FeedComment;
            return c != null && c.Id == Id;
        }
    }
}