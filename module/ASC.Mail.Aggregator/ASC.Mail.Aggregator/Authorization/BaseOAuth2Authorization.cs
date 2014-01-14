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
using System.Linq;
using System.Text;
using NLog;
using DotNetOpenAuth.OAuth2;

namespace ASC.Mail.Aggregator.Authorization
{
    public class BaseOAuth2Authorization
    {
        protected readonly Logger _log;

        public string ClientID { get; protected set; }
        public string ClientSecret { get; protected set; }
        public string RedirectUrl { get; protected set; }
        public AuthorizationServerDescription ServerDescription { get; protected set; }
        public List<string> Scope { get; protected set; }

        public BaseOAuth2Authorization()
        {
            _log = LogManager.GetLogger("Collector");
        }

        private IAuthorizationState PrepareAuthorizationState(string refreshToken)
        {
            return new AuthorizationState(Scope)
            {
                RefreshToken = refreshToken,
                Callback = new Uri(RedirectUrl),
            };

        }

        public IAuthorizationState RequestAccessToken(string refreshToken)
        {
            {
                WebServerClient consumer = new WebServerClient(ServerDescription, ClientID, ClientSecret)
                {
                    AuthorizationTracker = new AuthorizationTracker(Scope)
                };

                IAuthorizationState grantedAccess = PrepareAuthorizationState(refreshToken);

                if (grantedAccess != null)
                {
                    try
                    {
                        consumer.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter(ClientSecret);
                        consumer.RefreshAuthorization(grantedAccess, null);

                        return grantedAccess;
                    }
                    catch (Exception ex)
                    {
                        _log.Error("RefreshAuthorization() Exception:\r\n{0}\r\n", ex.ToString());
                    }
                }

                return null;
            }
        }
    }
}
