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

namespace LumiSoft.Net.AUTH
{
	/// <summary>
	/// Authentication type.
	/// </summary>
	public enum AuthType
	{
		/// <summary>
		/// Clear text username/password authentication.
		/// </summary>
		PLAIN = 0,

		/// <summary>
		/// APOP.This is used by POP3 only. RFC 1939 7. APOP.
		/// </summary>
		APOP  = 1,
	
		/// <summary>
		/// CRAM-MD5 authentication. RFC 2195 AUTH CRAM-MD5.
		/// </summary>
		CRAM_MD5 = 3,

		/// <summary>
		/// DIGEST-MD5 authentication. RFC 2831 AUTH DIGEST-MD5.
		/// </summary>
		DIGEST_MD5 = 4,
	}
}
