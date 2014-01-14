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

namespace ASC.Core.Configuration
{
    [Serializable]
    public class SmtpSettings
    {
        public string Host;

        public int? Port;

        public string SenderAddress;

        public string SenderDisplayName;

        public string CredentialsDomain;

        public string CredentialsUserName;

        public string CredentialsUserPassword;

        public bool EnableSSL;

        public override bool Equals(object obj)
        {
            var settings = obj as SmtpSettings;
            if (settings == null) return false;
            return
                Host == settings.Host &&
                SenderAddress == settings.SenderAddress &&
                SenderDisplayName == settings.SenderDisplayName &&
                CredentialsDomain == settings.CredentialsDomain &&
                CredentialsUserName == settings.CredentialsUserName &&
                CredentialsUserPassword == settings.CredentialsUserPassword &&
                EnableSSL == settings.EnableSSL &&
                Port == settings.Port;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
