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
using System.Net;
using System.Web;
using ASC.FederatedLogin.Profile;
using ASC.Thrdparty;
using DotNetOpenAuth.ApplicationBlock;
using DotNetOpenAuth.ApplicationBlock.Facebook;
using DotNetOpenAuth.OAuth2;

namespace ASC.FederatedLogin.LoginProviders
{
    public class ProviderManager
    {
        private static readonly Dictionary<string, ILoginProvider> Providers = new Dictionary<string, ILoginProvider>()
                                                                           {
                                                                               {
                                                                                   ProviderConstants.Facebook,
                                                                                   new FacebookLoginProvider()
                                                                                   },
                                                                               {
                                                                                   ProviderConstants.OpenId,
                                                                                   new OpenIdLoginProvider()
                                                                                   },
                                                                               {
                                                                                   ProviderConstants.Twitter,
                                                                                   new TwitterLoginProvider()
                                                                                }
                                                                                ,
                                                                                {
                                                                                   ProviderConstants.LinkedIn,
                                                                                   new LinkedInLoginProvider()
                                                                                },
                                                                           };



        public static LoginProfile Process(string providerType, HttpContext context, IDictionary<string,string> @params)
        {
            return Providers[providerType].ProcessAuthoriztion(context,@params);
        }
    }
}