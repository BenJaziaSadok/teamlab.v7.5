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
using System.Linq;
using System.Security;
using ASC.Api.Attributes;
using ASC.Api.Collections;
using ASC.Api.Exceptions;
using ASC.Api.Utils;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Studio.Core.Users;
using ASC.Web.Studio.Utility;
using SecurityContext = ASC.Core.SecurityContext;

namespace ASC.Api.Employee
{
    ///<summary>
    ///Groups access
    ///</summary>
    public class GroupsApi : Interfaces.IApiEntryPoint
    {
        ///<summary>
        ///</summary>
        public string Name
        {
            get { return "group"; }
        }

        ///<summary>
        ///Returns the general information about all groups, such as group ID and group manager
        ///</summary>
        ///<short>
        ///All groups
        ///</short>
        ///<returns>List of groups</returns>
        /// <remarks>
        /// This method returns partial group info
        /// </remarks>
        [Read("")]
        public IEnumerable<GroupWrapperSummary> GetAll()
        {
            return CoreContext.UserManager.GetDepartments().Select(x => new GroupWrapperSummary(x)).ToSmartList();
        }

        ///<summary>
        ///Returns the detailed information about the selected group: group name, category, description, manager, users and parent group if any
        ///</summary>
        ///<short>
        ///Specific group
        ///</short>
        ///<param name="groupid">Group ID</param>
        ///<returns>Group</returns>
        /// <remarks>
        /// That method returns full group info
        /// </remarks>
        [Read("{groupid}")]
        public GroupWrapperFull GetById(Guid groupid)
        {
            return new GroupWrapperFull(GetGroupInfo(groupid), true);
        }

        /// <summary>
        /// Adds a new group with the group manager, name and users specified
        /// </summary>
        /// <short>
        /// Add new group
        /// </short>
        /// <param name="groupManager">Group manager</param>
        /// <param name="groupName">Group name</param>
        /// <param name="members">List of group users</param>
        /// <returns>Newly created group</returns>
        [Create("")]
        public GroupWrapperFull AddGroup(Guid groupManager, string groupName, IEnumerable<Guid> members)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);

            var group = CoreContext.GroupManager.SaveGroupInfo(new GroupInfo { Name = groupName });

            TransferUserToDepartment(groupManager, @group, true);
            if (members != null)
            {
                foreach (var member in members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }
            return GetById(group.ID);
        }

        /// <summary>
        /// Updates an existing group changing the group manager, name and/or users
        /// </summary>
        /// <short>
        /// Update existing group
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <param name="groupManager">Group manager</param>
        /// <param name="groupName">Group name</param>
        /// <param name="members">List of group users</param>
        /// <returns>Newly created group</returns>
        [Update("{groupid}")]
        public GroupWrapperFull UpdateGroup(Guid groupid, Guid groupManager, string groupName, IEnumerable<Guid> members)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);
            var group = CoreContext.GroupManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull("group not found");
            if (group.ID == Core.Users.Constants.LostGroupInfo.ID)
                throw new ItemNotFoundException("group not found");

            group.Name = groupName ?? group.Name;
            CoreContext.GroupManager.SaveGroupInfo(group);

            TransferUserToDepartment(groupManager, @group, true);
            if (members != null)
            {
                foreach (var member in members)
                {
                    TransferUserToDepartment(member, group, false);
                }
            }
            return GetById(groupid);
        }

        /// <summary>
        /// Deletes the selected group from the list of groups on the portal
        /// </summary>
        /// <short>
        /// Delete group
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <returns></returns>
        [Delete("{groupid}")]
        public GroupWrapperFull DeleteGroup(Guid groupid)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);
            var @group = GetGroupInfo(groupid);
            var groupWrapperFull = new GroupWrapperFull(group, false);

            CoreContext.GroupManager.DeleteGroup(groupid);

            return groupWrapperFull;
        }

        private static GroupInfo GetGroupInfo(Guid groupid)
        {
            var group =
                CoreContext.GroupManager.GetGroups().SingleOrDefault(x => x.ID == groupid).NotFoundIfNull(
                    "group not found");
            if (group.ID == Core.Users.Constants.LostGroupInfo.ID)
                throw new ItemNotFoundException("group not found");
            return @group;
        }

        /// <summary>
        /// Move all the users from the selected group to another one specified
        /// </summary>
        /// <short>
        /// Move group users
        /// </short>
        /// <param name="groupid">ID of group to move from</param>
        /// <param name="newgroupid">ID of group to move to</param>
        /// <returns>Group info which users were moved</returns>
        [Update("{groupid}/members/{newgroupid}")]
        public GroupWrapperFull TransferMembersTo(Guid groupid, Guid newgroupid)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);
            var oldgroup = GetGroupInfo(groupid);

            var newgroup = GetGroupInfo(newgroupid);

            var users = CoreContext.UserManager.GetUsersByGroup(oldgroup.ID);
            foreach (var userInfo in users)
            {
                TransferUserToDepartment(userInfo.ID, newgroup, false);
            }
            return GetById(newgroupid);
        }

        /// <summary>
        /// Manages the group users deleting the current users and setting new ones instead
        /// </summary>
        /// <short>
        /// Set group users
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <param name="members">User list</param>
        /// <returns></returns>
        [Create("{groupid}/members")]
        public GroupWrapperFull SetMembersTo(Guid groupid, IEnumerable<Guid> members)
        {
            RemoveMembersFrom(groupid, CoreContext.UserManager.GetUsersByGroup(groupid).Select(x => x.ID));
            AddMembersTo(groupid, members);
            return GetById(groupid);
        }

        /// <summary>
        /// Add new group users keeping the current users and adding new ones
        /// </summary>
        /// <short>
        /// Add group users
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <param name="members">User list</param>
        /// <returns></returns>
        [Update("{groupid}/members")]
        public GroupWrapperFull AddMembersTo(Guid groupid, IEnumerable<Guid> members)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in members)
            {
                TransferUserToDepartment(userId, group, false);
            }
            return GetById(group.ID);
        }

        /// <summary>
        /// Sets the user with the ID sent as a manager to the selected group
        /// </summary>
        /// <short>
        /// Set group manager
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <param name="userid">User ID to become manager</param>
        /// <returns></returns>
        /// <exception cref="ItemNotFoundException"></exception>
        [Update("{groupid}/manager")]
        public GroupWrapperFull SetManager(Guid groupid, Guid userid)
        {
            var group = GetGroupInfo(groupid);
            if (CoreContext.UserManager.UserExists(userid))
                CoreContext.UserManager.SetDepartmentManager(group.ID, userid);
            else
            {
                throw new ItemNotFoundException("user not found");
            }
            return GetById(groupid);
        }

        /// <summary>
        /// Removes the specified group users with all the rest current group users retained
        /// </summary>
        /// <short>
        /// Remove group users
        /// </short>
        /// <param name="groupid">Group ID</param>
        /// <param name="members">User list</param>
        /// <returns></returns>
        [Delete("{groupid}/members")]
        public GroupWrapperFull RemoveMembersFrom(Guid groupid, IEnumerable<Guid> members)
        {
            SecurityContext.DemandPermissions(Core.Users.Constants.Action_EditGroups, Core.Users.Constants.Action_AddRemoveUser);
            var group = GetGroupInfo(groupid);

            foreach (var userId in members)
            {
                RemoveUserFromDepartment(userId, group);
            }
            return GetById(group.ID);
        }

        private static void RemoveUserFromDepartment(Guid userId, GroupInfo @group)
        {
            if (CoreContext.UserManager.UserExists(userId))
            {
                var user = CoreContext.UserManager.GetUsers(userId);
                CoreContext.UserManager.RemoveUserFromGroup(user.ID, group.ID);
                CoreContext.UserManager.SaveUserInfo(user);
            }
        }

        private static void TransferUserToDepartment(Guid userId, GroupInfo group, bool setAsManager)
        {
            if (CoreContext.UserManager.UserExists(userId) || userId == Guid.Empty)
            {
                if (setAsManager)
                {
                    CoreContext.UserManager.SetDepartmentManager(group.ID, userId);
                }
                CoreContext.UserManager.AddUserIntoGroup(userId, group.ID);
            }
        }
    }
}