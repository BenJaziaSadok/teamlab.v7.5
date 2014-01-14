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

//-----------------------------------------------------------------------
// <copyright file="TokenManager.cs" company="Andrew Arnott">
//     Copyright (c) Andrew Arnott. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace DotNetOpenAuth.ApplicationBlock {
	using System;
	using DotNetOpenAuth.OAuth2;

	public class TokenManager : IClientAuthorizationTracker {
		public IAuthorizationState GetAuthorizationState(Uri callbackUrl, string clientState) {
			return new AuthorizationState (null){
				Callback = callbackUrl,
			};
		}
	}
}
