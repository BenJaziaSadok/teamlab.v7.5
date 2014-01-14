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
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Studio.Core.Users
{
    public sealed class ProfileHelper
    {
        private readonly Guid userID;
        private readonly UserInfo ownInfo;
        public MyUserProfile userProfile { get; set; }
        public UserInfo UserInfo { get; private set; }

        public bool CanEdit
        {
            get
            {
                return ownInfo != null && (ownInfo.ID == UserInfo.ID || ownInfo.IsAdmin() || ownInfo.IsOwner());
            }
        }

        public bool isMe
        {
            get
            {
                return UserInfo.IsMe();
            }
        }
        public bool isVisitor
        {
            get
            {
                return UserInfo.IsVisitor();
            }
        }      

        public ProfileHelper() : this(SecurityContext.CurrentAccount.ID.ToString()) { }

        public ProfileHelper(string id)
        {
            if (SecurityContext.IsAuthenticated)
            {
                id = String.IsNullOrEmpty(id) ? SecurityContext.CurrentAccount.ID.ToString() : id;
                ownInfo = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            }

            if (!String.IsNullOrEmpty(id))
            {
                UserInfo = CoreContext.UserManager.GetUserByUserName(id);
            }

            if (UserInfo == null || UserInfo.Equals(Constants.LostUser))
            {
                if (!String.IsNullOrEmpty(id))
                {
                    try
                    {
                        userID = new Guid(id);
                    }
                    catch
                    {
                        userID = SecurityContext.CurrentAccount.ID;
                    }
                }

                if (!CoreContext.UserManager.UserExists(userID))
                {
                    userID = SecurityContext.CurrentAccount.ID;
                }
                UserInfo = CoreContext.UserManager.GetUsers(userID);
            }
            else
            {
                userID = UserInfo.ID;
            }

            userProfile = new MyUserProfile(userID);
        }
    }
}
