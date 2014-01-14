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

#if DEBUG
using ASC.SocialMedia.LinkedIn;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace ASC.SocialMedia.Tests
{
    class LinkedInDBTokenManager : IConsumerTokenManager
    {
        public string ConsumerKey { get; private set; }

        public string ConsumerSecret { get; private set; }


        public LinkedInDBTokenManager(string consumerKey, string consumerSecret)
        {
            ConsumerKey = consumerKey;
            ConsumerSecret = consumerSecret;
        }


        public string GetTokenSecret(string token)
        {
            return "43db680e-6c6a-4069-9155-9c16ef815586";
        }

        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            throw new NotImplementedException();
        }

        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            throw new NotImplementedException();
        }
    }

    [TestClass]
    public class LinkedInTest
    {
        [TestMethod]
        public void GetUserInfoTest()
        {
            var tokenManager = new LinkedInDBTokenManager("qnwIL9_wRC4Ew3iLl5sdEKvEDaSTgFn-RRaedF0XfXLZov0jDCq577Ta6wDLZr_8", "gJCNJ4UsvfCgPGHQRQt0CJ82GZTN6njeT1XxhyUaSsYHBAtCf58EE0P0ocBcLLqp");
            var provider = new LinkedInDataProvider(tokenManager, "8a17d3b4-5e99-4f5f-8ad3-5c9f0b28d9d1");
            var userInfo = provider.GetUserInfo("A_lDUH3Vb3");
        }
    }
}
#endif