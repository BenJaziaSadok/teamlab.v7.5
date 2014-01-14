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
using ASC.Xmpp.Core.protocol.client;
using ASC.Xmpp.Core.protocol.iq.register;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Core.utils.Xml.Dom;
using ASC.Xmpp.Server.Handler;
using ASC.Xmpp.Server.Streams;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Services.Jabber
{
	[XmppHandler(typeof(Register))]
	class RegisterHandler : XmppStanzaHandler
	{
		public override IQ HandleIQ(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			if (iq.Type == IqType.get) return GetRegister(stream, iq, context);
			else if (iq.Type == IqType.set) return SetRegister(stream, iq, context);
			return null;
		}

		private IQ GetRegister(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var register = (Register)iq.Query;
			register.Username = string.Empty;
			register.Password = string.Empty;
			iq.Type = IqType.result;

			if (iq.From.HasUser && context.UserManager.IsUserExists(iq.From))
			{
				register.Username = iq.From.User;
				register.AddChild(new Element("registered"));
				iq.SwitchDirection();
				iq.From = null;
			}
			else
			{
				iq.From = iq.To = null;
			}
			return iq;
		}

		private IQ SetRegister(XmppStream stream, IQ iq, XmppHandlerContext context)
		{
			var register = (Register)iq.Query;
			iq.Type = IqType.result;

			if (register.RemoveAccount)
			{
				if (!stream.Authenticated || !iq.From.HasUser) context.Sender.SendToAndClose(stream, XmppStreamError.NotAuthorized);

				context.UserManager.RemoveUser(iq.From);
				foreach (var s in context.SessionManager.GetBareJidSessions(iq.From))
				{
					if (s.Stream.Id == stream.Id) continue;
					context.Sender.SendToAndClose(s.Stream, XmppStreamError.Conflict);
				}
				//TODO: remove roster subscriptions
				register.RemoveAllChildNodes();
				iq.SwitchDirection();
				return iq;
			}

			if (string.IsNullOrEmpty(register.Username) ||
				string.IsNullOrEmpty(register.Password) ||
				Stringprep.NamePrep(register.Username) != register.Username)
			{
				var error = XmppStanzaError.ToNotAcceptable(iq);
				if (string.IsNullOrEmpty(register.Username)) error.Error.Message = "Empty required field Username.";
				else if (string.IsNullOrEmpty(register.Password)) error.Error.Message = "Empty required field Password.";
				else if (Stringprep.NamePrep(register.Username) != register.Username) error.Error.Message = "Invalid character.";
				return error;
			}

			var userJid = new Jid(register.Username, stream.Domain, null);
			if (context.UserManager.IsUserExists(userJid))
			{
				return XmppStanzaError.ToConflict(iq);
			}

			var user = new User(userJid, register.Password);
			context.UserManager.SaveUser(user);

			register.RemoveAllChildNodes();
			if (stream.Authenticated) iq.SwitchDirection();
			else iq.To = null;
			iq.From = null;
			return iq;
		}
	}
}