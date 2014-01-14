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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty;
using ASC.Thrdparty.Configuration;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Facebook;
using FacebookClient = DotNetOpenAuth.ApplicationBlock.FacebookClient;

namespace ASC.FederatedLogin.LoginProviders
{
    class FacebookLoginProvider : ILoginProvider
    {
        public LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params)
        {
            var builder = new UriBuilder(context.Request.GetUrlRewriter()) {Query = "p=" + context.Request["p"]};
            var oauth = new FacebookOAuthClient
            {
                AppId = KeyStorage.Get("facebookAppID"),
                AppSecret = KeyStorage.Get("facebookAppSecret"),
                RedirectUri = builder.Uri
            };
            FacebookOAuthResult result;
            if (FacebookOAuthResult.TryParse(context.Request.GetUrlRewriter(), out result))
            {
                if (result.IsSuccess)
                {
                    var accessToken = (Facebook.JsonObject)oauth.ExchangeCodeForAccessToken(result.Code);
                    var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString((string)accessToken["access_token"]));
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var graph = FacebookGraph.Deserialize(responseStream);
                            var profile = ProfileFromFacebook(graph);
                            return profile;
                        }
                    }
                }
                return LoginProfile.FromError(new Exception(result.ErrorReason));
            }
            //Maybe we didn't query
            var extendedPermissions = new[] { "email", "user_about_me" };
            var parameters = new Dictionary<string, object>
                                 {
                                     { "display", "popup" }
                                 };

            if (extendedPermissions.Length > 0)
            {
                var scope = new StringBuilder();
                scope.Append(string.Join(",", extendedPermissions));
                parameters["scope"] = scope.ToString();
            }
            var loginUrl = oauth.GetLoginUrl(parameters);
            context.Response.Redirect(loginUrl.ToString());
            return LoginProfile.FromError(new Exception("Failed to login with facebook"));
        }

        internal static LoginProfile ProfileFromFacebook(FacebookGraph graph)
        {
            var profile = new LoginProfile
                              {
                                  BirthDay = graph.Birthday,
                                  Link = graph.Link.ToString(),
                                  FirstName = graph.FirstName,
                                  LastName = graph.LastName,
                                  Gender = graph.Gender,
                                  DisplayName = graph.FirstName + graph.LastName,
                                  EMail = graph.Email,
                                  Id = graph.Id.ToString(),
                                  TimeZone = graph.Timezone,
                                  Locale = graph.Locale,
                                  Provider = ProviderConstants.Facebook,
                                  Avatar = string.Format("http://graph.facebook.com/{0}/picture?type=large", graph.Id)
                              };

            return profile;
        }
    }
}