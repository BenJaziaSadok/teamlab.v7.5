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

namespace ASC.Bookmarking.Business.Permissions
{
    public class BookmarkPermissionSecurityObject : ISecurityObject
    {
        public Type ObjectType
        {
            get { return GetType(); }
        }

        public object SecurityId { get; set; }

        public Guid CreatorID { get; set; }


        public BookmarkPermissionSecurityObject(Guid userID, Guid id)
        {
            CreatorID = userID;
            SecurityId = id;
        }

        public BookmarkPermissionSecurityObject(Guid userID)
            : this(userID, Guid.NewGuid())
        {
        }

        public bool ObjectRolesSupported
        {
            get { return true; }
        }

        public IEnumerable<IRole> GetObjectRoles(ASC.Common.Security.Authorizing.ISubject account, ISecurityObjectId objectId, SecurityCallContext callContext)
        {
            return account.ID == CreatorID ? new[] { Constants.Owner } : new IRole[0];
        }

        public ISecurityObjectId InheritFrom(ISecurityObjectId objectId)
        {
            throw new NotImplementedException();
        }

        public bool InheritSupported
        {
            get { return false; }
        }
    }
}
