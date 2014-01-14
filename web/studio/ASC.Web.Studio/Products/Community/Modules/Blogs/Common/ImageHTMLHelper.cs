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
using ASC.Blogs.Core;
using ASC.Web.Core.Users;

namespace ASC.Web.Community.Blogs
{
    public class ImageHTMLHelper
    {
        public static string GetHTMLUserAvatar(Guid userID)
        {
            string imgPath = UserPhotoManager.GetBigPhotoURL(userID);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" alt='' src=\"" + imgPath + "\"/>";

            return string.Empty;
        }

        public static string GetHTMLSmallUserAvatar(Guid userID)
        {
            string imgPath = UserPhotoManager.GetSmallPhotoURL(userID);
            if (imgPath != null)
                return "<img class=\"userMiniPhoto\" alt='' src=\"" + imgPath + "\"/>";

            return string.Empty;
        }

        public static string GetLinkUserAvatar(Guid userID)
        {
            string imgPath = UserPhotoManager.GetBigPhotoURL(userID);
            if (imgPath != null)
                return String.Format("<a href=\"{0}\"><img class=\"userPhoto\" src=\"{1}\"/></a>",
                        Constants.UserPostsPageUrl+userID.ToString(),
                        imgPath);

            return string.Empty;
        }
    }
}
