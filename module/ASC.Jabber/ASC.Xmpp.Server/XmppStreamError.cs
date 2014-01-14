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

using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;

namespace ASC.Xmpp.Server
{
	public static class XmppStreamError
	{
		public static Error BadFormat
		{
			get { return new Error(StreamErrorCondition.BadFormat) { Prefix = Uri.PREFIX }; }
		}

		public static Error Conflict
		{
			get { return new Error(StreamErrorCondition.Conflict) { Prefix = Uri.PREFIX }; }
		}

		public static Error HostUnknown
		{
			get { return new Error(StreamErrorCondition.HostUnknown) { Prefix = Uri.PREFIX }; }
		}

		public static Error BadNamespacePrefix
		{
			get { return new Error(StreamErrorCondition.BadNamespacePrefix) { Prefix = Uri.PREFIX }; }
		}

		public static Error NotAuthorized
		{
			get { return new Error(StreamErrorCondition.NotAuthorized) { Prefix = Uri.PREFIX }; }
		}

		public static Error InvalidFrom
		{
			get { return new Error(StreamErrorCondition.InvalidFrom) { Prefix = Uri.PREFIX }; }
		}

		public static Error ImproperAddressing
		{
			get { return new Error(StreamErrorCondition.ImproperAddressing) { Prefix = Uri.PREFIX }; }
		}

		public static Error InternalServerError
		{
			get { return new Error(StreamErrorCondition.InternalServerError) { Prefix = Uri.PREFIX }; }
		}
	}
}
