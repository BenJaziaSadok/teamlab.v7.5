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

using System.Runtime.Serialization;
using ASC.Files.Core;

namespace ASC.Web.Files.Services.WCFService
{
    [DataContract(Name = "third_party", Namespace = "")]
    public class ThirdPartyParams
    {
        [DataMember(Name = "auth_data", EmitDefaultValue = false)]
        public AuthData AuthData { get; set; }

        [DataMember(Name = "corporate")]
        public bool Corporate { get; set; }

        [DataMember(Name = "customer_title")]
        public string CustomerTitle { get; set; }

        [DataMember(Name = "provider_id")]
        public string ProviderId { get; set; }

        [DataMember(Name = "provider_key")]
        public string ProviderKey { get; set; }
    }
}