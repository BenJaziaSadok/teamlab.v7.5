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

using ASC.Core.Users;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Feed.Aggregator
{
    public class UserWrapper
    {
        public UserInfo UserInfo { get; private set; }

        public string DisplayName
        {
            get { return DisplayUserSettings.GetFullUserName(UserInfo); }
        }

        public string ProfileUrl
        {
            get
            {
                var profileUrl = CommonLinkUtility.GetUserProfile(UserInfo.ID.ToString(), false);
                return CommonLinkUtility.ToAbsolute(profileUrl);
            }
        }

        public UserWrapper(UserInfo userInfo)
        {
            UserInfo = userInfo;
        }
    }
}