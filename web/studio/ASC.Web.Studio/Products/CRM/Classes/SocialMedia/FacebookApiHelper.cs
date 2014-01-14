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
using ASC.SocialMedia.Facebook;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class FacebookApiHelper
    {
        public static FacebookApiInfo GetFacebookApiInfoForCurrentUser()
        {
            FacebookApiInfo apiInfo = new FacebookApiInfo();

            SetDefaultTokens(apiInfo);

            if (String.IsNullOrEmpty(apiInfo.AccessToken))
                return null;
            else
                return apiInfo;
        }

        private static void SetDefaultTokens(FacebookApiInfo apiInfo)
        {
            apiInfo.AccessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyFacebookDefaultAccessToken);
        }
    }
}
