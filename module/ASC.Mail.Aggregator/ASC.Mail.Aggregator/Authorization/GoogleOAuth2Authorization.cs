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
using DotNetOpenAuth.OAuth2;
using System.Configuration;
using System.Web.Configuration;

namespace ASC.Mail.Aggregator.Authorization
{
    public class GoogleOAuth2Authorization : BaseOAuth2Authorization
    {
        public GoogleOAuth2Authorization() : base()
        {
            Func<string, string> getConfigVal = (value) => {
                return (ConfigurationManager.AppSettings.Get(value) ?? WebConfigurationManager.AppSettings.Get(value)); 
            };

            try
            {
                ClientID = getConfigVal("mail.googleClientID");
                ClientSecret = getConfigVal("mail.googleClientSecret");

                if (String.IsNullOrEmpty(ClientID)) throw new ArgumentNullException("ClientID");
                if (String.IsNullOrEmpty(ClientSecret)) throw new ArgumentNullException("ClientSecret");
            }
            catch (Exception ex)
            {
                _log.Error("GoogleOAuth2Authorization() Exception:\r\n{0}\r\n", ex.ToString());
            }

            RedirectUrl = "urn:ietf:wg:oauth:2.0:oob";
 
            ServerDescription = new AuthorizationServerDescription
            {
                AuthorizationEndpoint = new Uri("https://accounts.google.com/o/oauth2/auth?access_type=offline"),
                TokenEndpoint = new Uri("https://accounts.google.com/o/oauth2/token"),
                ProtocolVersion = DotNetOpenAuth.OAuth2.ProtocolVersion.V20,
            };

            Scope = new List<string>
            {
                "https://mail.google.com/"
            };
        }
    }
}
