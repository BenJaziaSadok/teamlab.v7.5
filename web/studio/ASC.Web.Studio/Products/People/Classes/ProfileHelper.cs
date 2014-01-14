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
using System.Web;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.People.Core;

namespace ASC.Web.People.Classes
{
    public sealed class ProfileHelper
    {
        private Guid _userID;
        private UserInfo _ownInfo;
        private UserInfo _userInfo;

        public MyUserProfile userProfile { get; set; }
        public UserInfo userInfo
        {
            get
            {
                return _userInfo;
            }
        }

        public bool canEdit
        {
            get
            {
                return _ownInfo != null && (_ownInfo.ID == _userInfo.ID || _ownInfo.IsAdmin() || _ownInfo.IsOwner());
            }
        }

        public bool isMe
        {
            get
            {
                return _ownInfo != null && (_ownInfo.ID == _userInfo.ID);
            }
        }

        public ProfileHelper() : this(SecurityContext.CurrentAccount.ID.ToString()) { }

        public ProfileHelper(string id)
        {
            if (SecurityContext.IsAuthenticated)
            {
                id = String.IsNullOrEmpty(id) ? SecurityContext.CurrentAccount.ID.ToString() : id;
                _ownInfo = CoreContext.UserManager.GetUsers(SecurityContext.CurrentAccount.ID);
            }

            if (!String.IsNullOrEmpty(id))
            {
                _userInfo = CoreContext.UserManager.GetUserByUserName(id);
            }

            if (_userInfo == null || _userInfo == Constants.LostUser)
            {
                if (!String.IsNullOrEmpty(id))
                {
                    try
                    {
                        _userID = new Guid(id);
                    }
                    catch
                    {
                        _userID = SecurityContext.CurrentAccount.ID;
                    }
                }

                if (!CoreContext.UserManager.UserExists(_userID))
                {
                    _userID = SecurityContext.CurrentAccount.ID;
                }
                _userInfo = CoreContext.UserManager.GetUsers(_userID);
            }
            else
            {
                _userID = _userInfo.ID;
            }

            userProfile = new MyUserProfile(_userID);
        }

        public string GetProfileLink()
        {
            return VirtualPathUtility.ToAbsolute(PeopleProduct.ProductPath + "profile.aspx") +"?user=" + userProfile.UserName;
        }

        public string GetActivityLink()
        {
            return VirtualPathUtility.ToAbsolute(PeopleProduct.ProductPath + "activity.aspx") + "?user=" + userProfile.UserName;
        }

        public string GetSubscriptionsLink()
        {
            return VirtualPathUtility.ToAbsolute(PeopleProduct.ProductPath + "subscriptions.aspx") + "?user=" + userProfile.UserName;
        }

        public string GetCustomizationLink()
        {
            return VirtualPathUtility.ToAbsolute(PeopleProduct.ProductPath + "customization.aspx") + "?user=" + userProfile.UserName;
        }
    }
}
