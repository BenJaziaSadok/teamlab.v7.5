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
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Server.Services
{
	public interface IXmppService : IConfigurable
	{
		Jid Jid
		{
			get;
			set;
		}

		string Name
		{
			get;
			set;
		}
		
		DiscoInfo DiscoInfo
		{
			get;
		}

		DiscoItem DiscoItem
		{
			get;
		}

		Vcard Vcard
		{
			get;
		}

		IXmppService ParentService
		{
			get;
			set;
		}

		void OnRegister(IServiceProvider serviceProvider);

		void OnUnregister(IServiceProvider serviceProvider);
	}
}
