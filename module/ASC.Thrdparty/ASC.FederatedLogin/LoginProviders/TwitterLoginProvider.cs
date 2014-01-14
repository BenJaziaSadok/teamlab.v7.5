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
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Xml.Linq;
using System.Xml.XPath;
using ASC.FederatedLogin.Helpers;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty;
using ASC.Thrdparty.Configuration;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.OAuth;
using ASC.Thrdparty.TokenManagers;
using TweetSharp;

namespace ASC.FederatedLogin.LoginProviders
{
    class TwitterLoginProvider : ILoginProvider
    {
        public LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params)
        {
            var twitterService = new TwitterService(KeyStorage.Get("twitterKey"), KeyStorage.Get("twitterSecret"));

            if (String.IsNullOrEmpty(context.Request["oauth_token"]) ||
                String.IsNullOrEmpty(context.Request["oauth_verifier"]))
            {
                var requestToken = twitterService.GetRequestToken(context.Request.Url.AbsoluteUri);

                var uri = twitterService.GetAuthorizationUri(requestToken);

                context.Response.Redirect(uri.ToString(), true);
            }
            else
            {
                var requestToken = new OAuthRequestToken { Token = context.Request["oauth_token"] };
                var accessToken = twitterService.GetAccessToken(requestToken, context.Request["oauth_verifier"]);
                twitterService.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

                var user = twitterService.VerifyCredentials(new VerifyCredentialsOptions());

                return ProfileFromTwitter(user);
            }

            return new LoginProfile();

        }

        internal static LoginProfile ProfileFromTwitter(TwitterUser twitterUser)
        {
            return new LoginProfile()
                {
                    Name = twitterUser.Name,
                    DisplayName = twitterUser.ScreenName,
                    Avatar = twitterUser.ProfileImageUrl,
                    TimeZone = twitterUser.TimeZone,
                    Locale = twitterUser.Location,
                    Id = twitterUser.Id.ToString(CultureInfo.InvariantCulture),
                    Link = twitterUser.Url,
                    Provider = ProviderConstants.Twitter
                };
        }

        internal static LoginProfile ProfileFromTwitter(XDocument info)
        {
            XPathNavigator nav = info.CreateNavigator();
            var profile = new LoginProfile
                              {
                                  Name = nav.SelectNodeValue("//screen_name"),
                                  DisplayName = nav.SelectNodeValue("//name"),
                                  Avatar = nav.SelectNodeValue("//profile_image_url"),
                                  TimeZone = nav.SelectNodeValue("//time_zone"),
                                  Locale = nav.SelectNodeValue("//lang"),
                                  Id = nav.SelectNodeValue("//id"),
                                  Link = nav.SelectNodeValue("//url"),
                                  Provider = ProviderConstants.Twitter
                              };
            return profile;
        }
    }
}