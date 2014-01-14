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
using System.Configuration;
using System.Web;
using System.Web.Caching;
using ASC.Thrdparty.Configuration;
using ASC.Thrdparty.TokenManagers;
using DotNetOpenAuth.OAuth.ChannelElements;

namespace ASC.Thrdparty
{
    public class TokenManagerHolder
    {
        public static IAssociatedTokenManager Get(string providerKey)
        {
            String consumerKey;
            String consumerSecret;

            switch (providerKey)
            {
                case  ProviderConstants.Twitter:
                    consumerKey = "twitterKey";
                    consumerSecret = "twitterSecret";
                    break;
                case ProviderConstants.Facebook:
                    consumerKey = "facebookAppID";
                    consumerSecret = "facebookAppSecret";
                    break;
                case ProviderConstants.LinkedIn:
                    consumerKey = "linkedInKey";
                    consumerSecret = "linkedInSecret";
                    break;
                default:
                    throw new NotSupportedException();
            }

            return Get(providerKey, consumerKey, consumerSecret);

        }

        public static IAssociatedTokenManager Get(string providerKey, string consumerKey, string consumerSecret)
        {
            var tokenManager = (IAssociatedTokenManager)HttpRuntime.Cache.Get(providerKey);
            if (tokenManager == null)
            {
                if (!string.IsNullOrEmpty(consumerKey))
                {
                    tokenManager = GetTokenManager(KeyStorage.Get(consumerKey), KeyStorage.Get(consumerSecret));
                    HttpRuntime.Cache.Add(providerKey, tokenManager, null, Cache.NoAbsoluteExpiration,
                                          Cache.NoSlidingExpiration, CacheItemPriority.NotRemovable, null);
                }
            }
            return tokenManager;
        }

        private static IAssociatedTokenManager GetTokenManager(string consumerKey, string consumerSecret)
        {
            IAssociatedTokenManager tokenManager = null;
            var section = ConsumerConfigurationSection.GetSection();
            if (section!=null && !string.IsNullOrEmpty(section.ConnectionString))
            {
                tokenManager = new DbTokenManager(KeyStorage.Get(consumerKey), KeyStorage.Get(consumerSecret),
                                                  "auth_tokens",
                                                  ConfigurationManager.ConnectionStrings[section.ConnectionString]);
            }
            else
            {
                //For testing return the inmemorytokenmanager
                tokenManager = new InMemoryTokenManager(consumerKey, consumerSecret);    
            }
            
            return tokenManager;
        }
    }
}