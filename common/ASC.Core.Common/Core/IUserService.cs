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
using ASC.Core.Users;

namespace ASC.Core
{
    public interface IUserService
    {
        IDictionary<Guid, UserInfo> GetUsers(int tenant, DateTime from);

        UserInfo GetUser(int tenant, Guid id);

        UserInfo GetUser(int tenant, string login, string passwordHash);

        UserInfo SaveUser(int tenant, UserInfo user);

        void RemoveUser(int tenant, Guid id);

        byte[] GetUserPhoto(int tenant, Guid id);

        void SetUserPhoto(int tenant, Guid id, byte[] photo);

        string GetUserPassword(int tenant, Guid id);

        void SetUserPassword(int tenant, Guid id, string password);


        IDictionary<Guid, Group> GetGroups(int tenant, DateTime from);

        Group GetGroup(int tenant, Guid id);

        Group SaveGroup(int tenant, Group group);

        void RemoveGroup(int tenant, Guid id);


        IDictionary<string, UserGroupRef> GetUserGroupRefs(int tenant, DateTime from);

        UserGroupRef SaveUserGroupRef(int tenant, UserGroupRef r);

        void RemoveUserGroupRef(int tenant, Guid userId, Guid groupId, UserGroupRefType refType);
    }
}
