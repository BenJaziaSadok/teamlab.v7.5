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
using ASC.SocialMedia.LinkedIn;
using ASC.Thrdparty.Configuration;
using ASC.Web.CRM.SocialMedia;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Web.CRM.Classes.SocialMedia
{
    public static class LinkedInApiHelper
    {
        public static LinkedInDataProvider GetLinkedInDataProviderForCurrentUser()
        {
            IConsumerTokenManager tokenManager = null;
            string accessToken = null;

            string consumerKey = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInConsumerKey);
            string consumerKeySecret = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInConsumerSecretKey);

            accessToken = KeyStorage.Get(SocialMediaConstants.ConfigKeyLinkedInDefaultAccessToken);
            if (String.IsNullOrEmpty(accessToken)) return null;
            tokenManager = new LinkedInDefaultAccountTokenManager(consumerKey, consumerKeySecret);
            return new LinkedInDataProvider(tokenManager, accessToken);
        }
    }
}
