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
using System.Drawing;
using System.Text;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Core.Users
{
    public static class UserInfoExtension
    {
        public static string DisplayUserName(this UserInfo userInfo)
        {
            return DisplayUserName(userInfo, true);
        }

        public static string DisplayUserName(this UserInfo userInfo, bool withHtmlEncode)
        {
            return DisplayUserSettings.GetFullUserName(userInfo, withHtmlEncode);
        }

        public static List<UserInfo> SortByUserName(this IEnumerable<UserInfo> userInfoCollection)
        {
            if (userInfoCollection == null) return new List<UserInfo>();

            var users = new List<UserInfo>(userInfoCollection);
            users.Sort(UserInfoComparer.Default);
            return users;
        }

        public static Size GetPhotoSize(this UserInfo userInfo)
        {
            return UserPhotoManager.GetPhotoSize(Guid.Empty, userInfo.ID);
        }

        public static string GetPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetPhotoAbsoluteWebPath(Guid.Empty, userInfo.ID);
        }
     
        public static string GetBigPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetBigPhotoURL(userInfo.ID);
        }

        public static string GetMediumPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetMediumPhotoURL(userInfo.ID);
        }

        public static string GetSmallPhotoURL(this UserInfo userInfo)
        {
            return UserPhotoManager.GetSmallPhotoURL(userInfo.ID);
        }

        public static string RenderProfileLinkBase(this UserInfo userInfo, Guid productID)
        {
            var sb = new StringBuilder();

            //check for removed users
            if (userInfo == null || !CoreContext.UserManager.UserExists(userInfo.ID))
            {
                sb.Append("<span class='userLink text-medium-describe' style='white-space:nowrap;'>profile removed</span>");
            }
            else
            {
                var popupID = Guid.NewGuid();
                sb.AppendFormat("<span class=\"userLink\" style='white-space:nowrap;' id='{0}' data-uid='{1}'>", popupID, userInfo.ID);
                sb.AppendFormat("<a class='linkDescribe' href=\"{0}\">{1}</a>", userInfo.GetUserProfilePageURLGeneral(), userInfo.DisplayUserName());
                sb.Append("</span>");

                sb.AppendFormat("<script language='javascript'> StudioUserProfileInfo.RegistryElement('{0}','\"{1}\",\"{2}\"'); </script>", popupID, userInfo.ID, productID);
            }
            return sb.ToString();
        }

        /// <summary>
        /// return absolute profile link
        /// </summary>
        /// <param name="userInfo"></param>        
        /// <returns></returns>
        private static string GetUserProfilePageURLGeneral(this UserInfo userInfo)
        {
            return CommonLinkUtility.GetUserProfile(userInfo.ID);
        }
    }
}