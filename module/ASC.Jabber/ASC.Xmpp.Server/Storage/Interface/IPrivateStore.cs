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
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Storage.Interface
{

	public interface IPrivateStore
	{
		Element GetPrivate(Jid jid, Element element);

        void SetPrivate(Jid jid, Element element);
	}
}