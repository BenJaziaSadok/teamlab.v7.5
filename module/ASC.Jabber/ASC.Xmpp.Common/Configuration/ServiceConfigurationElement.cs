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
using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
    public class ServiceConfigurationElement : JabberConfigurationElement
	{
		[ConfigurationProperty(Schema.JID, IsRequired = true)]
		public string Jid
		{
			get { return (string)this[Schema.JID]; }
			set { this[Schema.JID] = value; }
		}

		[ConfigurationProperty(Schema.PARENT)]
		public string Parent
		{
			get { return (string)this[Schema.PARENT]; }
			set { this[Schema.PARENT] = value; }
		}

		
		public ServiceConfigurationElement()
		{
			
		}

		public ServiceConfigurationElement(string jid, string name, Type type)
			: base(name, type)
		{
			Jid = jid;
		}

		public ServiceConfigurationElement(string jid, string name, Type type, string parentJid)
			: this(jid, name, type)
		{
			Parent = parentJid;
		}

    }
}