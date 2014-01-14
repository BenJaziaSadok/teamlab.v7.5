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
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Server.Handler
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class XmppHandlerAttribute : Attribute
	{
		public Type XmppElementType
		{
			get;
			private set;
		}

		public XmppHandlerAttribute(Type xmppElementType) {
			if (xmppElementType == null) throw new ArgumentNullException("xmppElementType");

			if (!typeof(Element).IsAssignableFrom(xmppElementType)) throw new ArgumentException("xmppElementType not assigned from Element.");
			XmppElementType = xmppElementType;
		}
	}
}
