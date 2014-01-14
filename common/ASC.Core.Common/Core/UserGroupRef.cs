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
using System.Diagnostics;

namespace ASC.Core
{
    [DebuggerDisplay("{UserId} - {GroupId}")]
    public class UserGroupRef
    {
        public Guid UserId
        {
            get;
            set;
        }

        public Guid GroupId
        {
            get;
            set;
        }

        public bool Removed
        {
            get;
            set;
        }

        public DateTime LastModified
        {
            get;
            set;
        }

        public UserGroupRefType RefType
        {
            get;
            set;
        }

        public int Tenant
        {
            get;
            set;
        }


        public UserGroupRef()
        {
        }

        public UserGroupRef(Guid userId, Guid groupId, UserGroupRefType refType)
        {
            UserId = userId;
            GroupId = groupId;
            RefType = refType;
        }

        public static string CreateKey(int tenant, Guid userId, Guid groupId, UserGroupRefType refType)
        {
            return tenant.ToString() + userId.ToString("N") + groupId.ToString("N") + ((int)refType).ToString();
        }

        public string CreateKey()
        {
            return CreateKey(Tenant, UserId, GroupId, RefType);
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() ^ GroupId.GetHashCode() ^ Tenant.GetHashCode() ^ RefType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var r = obj as UserGroupRef;
            return r != null && r.Tenant == Tenant && r.UserId == UserId && r.GroupId == GroupId && r.RefType == RefType;
        }
    }
}
