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

namespace ASC.Specific.AuthorizationApi
{
    [DataContract(Name = "token", Namespace = "")]
    public class AuthenticationTokenData
    {
        [DataMember(Order = 1)]
        public string Token { get; set; }

        [DataMember(Order = 2)]
        public ApiDateTime Expires { get; set; }

        [DataMember(Order = 3)]
        public bool Sms { get; set; }

        [DataMember(Order = 4)]
        public string PhoneNoise { get; set; }

        public static AuthenticationTokenData GetSample()
        {
            return new AuthenticationTokenData
                {
                    Expires = new ApiDateTime(DateTime.UtcNow.AddYears(1)),
                    Token = "sdjhfskjdhkqy739459234",
                    Sms = false,
                    PhoneNoise = string.Empty
                };
        }
    }
}