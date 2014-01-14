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
using System.Runtime.Serialization;

namespace ASC.Mail.Aggregator
{
    [Serializable]
    [DataContract(Namespace = "", Name="Account")]
    public class MailAccount
    {
        [DataMember(IsRequired = true)]
        public string Address
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public bool Enabled
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public bool QuotaError { get; set; }

        [DataMember(IsRequired = true)]
        public bool AuthError { get; set; }

        [DataMember(IsRequired = true)]
        public string Name
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public int Id
        {
            get;
            set;
        }

        [DataMember(IsRequired = true)]
        public bool OAuthConnection { get; set; }

        public override string ToString()
        {
            return Name + " <"+Address+">";
        }

        public MailAccount(string address, string name, bool enabled, bool quota_error, bool auth_error, int id, bool oauth_connection)
        {
            Address = address;
            Name = name;
            Enabled = enabled;
            QuotaError = quota_error;
            AuthError = auth_error;
            Id = id;
            OAuthConnection = oauth_connection;
        }
    }
}
