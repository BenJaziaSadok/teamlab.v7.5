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

using System.Diagnostics;
using System.Runtime.Serialization;

namespace ASC.Files.Core
{
    [DataContract(Name = "AuthData", Namespace = "")]
    [DebuggerDisplay("{Login} {Password} {Token}")]
    public class AuthData
    {
        [DataMember(Name = "login")]
        public string Login { get; set; }

        [DataMember(Name = "password")]
        public string Password { get; set; }

        [DataMember(Name = "token")]
        public string Token { get; set; }

        public AuthData(string token)
        {
            Token = token;
        }

        public AuthData(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public AuthData(string login, string password, string token)
        {
            Login = login;
            Password = password;
            Token = token;
        }
    }
}