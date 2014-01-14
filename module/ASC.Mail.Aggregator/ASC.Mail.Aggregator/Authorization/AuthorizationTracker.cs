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

namespace ASC.Mail.Aggregator.Authorization
{
    public class AuthorizationTracker : IClientAuthorizationTracker
    {
        private List<string> _scope;

        public AuthorizationTracker(List<string> scope)
        {
            _scope = scope;
        }

        public IAuthorizationState GetAuthorizationState(
          Uri callbackUrl, string clientState)
        {
            return new AuthorizationState(_scope)
            {
                Callback = callbackUrl
            };
        }
    }
}
