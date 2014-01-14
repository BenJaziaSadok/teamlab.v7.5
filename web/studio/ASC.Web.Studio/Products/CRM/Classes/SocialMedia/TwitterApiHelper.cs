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

using ASC.SocialMedia.Twitter;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class TwitterApiHelper
    {
        public static TwitterApiInfo GetTwitterApiInfoForCurrentUser()
        {
            TwitterApiInfo apiInfo = new TwitterApiInfo
            {
                ConsumerKey = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterConsumerKey),
                ConsumerSecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterConsumerSecretKey)
            };

            SetDefaultTokens(apiInfo);

            return apiInfo;
        }

        private static void SetDefaultTokens(TwitterApiInfo apiInfo)
        {
            apiInfo.AccessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterDefaultAccessToken);
            apiInfo.AccessTokenSecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyTwitterDefaultAccessTokenSecret);
        }
    }
}
