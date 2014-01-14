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
using ASC.Core.Users;

namespace ASC.Core
{
    public interface IUserManagerClient
    {
        #region Users

        UserInfo[] GetUsers();

        UserInfo GetUsers(Guid userID);

        UserInfo GetUsers(int tenant, string login, string password);

        DateTime GetMaxUsersLastModified();

        UserInfo SaveUserInfo(UserInfo userInfo);

        void DeleteUser(Guid userID);

        void SaveUserPhoto(Guid userID, Guid moduleID, byte[] photo);

        byte[] GetUserPhoto(Guid userID, Guid moduleID);

        bool UserExists(Guid userID);

        UserInfo[] GetUsers(EmployeeStatus status);

        UserInfo[] GetUsers(EmployeeStatus status, EmployeeType type);

        string[] GetUserNames(EmployeeStatus status);

        UserInfo GetUserByUserName(string userName);

        bool IsUserNameExists(string userName);

        UserInfo GetUserByEmail(string email);

        UserInfo[] Search(string text, EmployeeStatus status);

        UserInfo[] Search(string text, EmployeeStatus status, Guid groupId);


        GroupInfo[] GetUserGroups(Guid userID);

        GroupInfo[] GetUserGroups(Guid userID, IncludeType includeType);

        GroupInfo[] GetUserGroups(Guid userID, Guid categoryID);

        UserInfo[] GetUsersByGroup(Guid groupID);

        bool IsUserInGroup(Guid userID, Guid groupID);

        void AddUserIntoGroup(Guid userID, Guid groupID);

        void RemoveUserFromGroup(Guid userID, Guid groupID);

        #endregion


        #region Company

        UserInfo GetCompanyCEO();

        void SetCompanyCEO(Guid userId);

        GroupInfo[] GetDepartments();

        Guid GetDepartmentManager(Guid deparmentID);

        void SetDepartmentManager(Guid deparmentID, Guid userID);

        #endregion
    }
}