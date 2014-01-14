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
using ASC.Xmpp.Common.Configuration;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.utils.Idn;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Services;
using log4net.Config;

namespace ASC.Xmpp.Server.Configuration
{
	public static class JabberConfiguration
	{
		public static void Configure(XmppServer server)
		{
			Configure(server, null);
		}

		public static void Configure(XmppServer server, string configFile)
		{
			XmlConfigurator.Configure();

			var jabberSection = GetSection(configFile);

			ConfigureListeners(jabberSection, server);
			ConfigureStorages(jabberSection, server);
			ConfigureServices(jabberSection, server);
		}

		private static void ConfigureServices(JabberConfigurationSection jabberSection, XmppServer server)
		{
			foreach (ServiceConfigurationElement se in jabberSection.Services)
			{
				var service = (IXmppService)Activator.CreateInstance(Type.GetType(se.TypeName, true));
				service.Jid = new Jid(Stringprep.NamePrep(se.Jid));
				service.Name = se.Name;
				if (!string.IsNullOrEmpty(se.Parent))
				{
					service.ParentService = server.GetXmppService(new Jid(Stringprep.NamePrep(se.Parent)));
				}
				service.Configure(se.GetProperties());
				server.RegisterXmppService(service);
			}
		}

		private static void ConfigureStorages(JabberConfigurationSection jabberSection, XmppServer server)
		{
			foreach (JabberConfigurationElement se in jabberSection.Storages)
			{
				var storage = Activator.CreateInstance(Type.GetType(se.TypeName, true));
				if (storage is IConfigurable) ((IConfigurable)storage).Configure(se.GetProperties());
				server.StorageManager.SetStorage(se.Name, storage);
			}
		}

		private static void ConfigureListeners(JabberConfigurationSection jabberSection, XmppServer server)
		{
			foreach (JabberConfigurationElement le in jabberSection.Listeners)
			{
				var listener = (IXmppListener)Activator.CreateInstance(Type.GetType(le.TypeName, true));
				listener.Name = le.Name;
				listener.Configure(le.GetProperties());
				server.AddXmppListener(listener);
			}
		}


		private static JabberConfigurationSection GetSection(string configFile)
		{
			if (string.IsNullOrEmpty(configFile))
			{
				return (JabberConfigurationSection)ConfigurationManager.GetSection(Schema.SECTION_NAME);
			}

			var cfg = ConfigurationManager.OpenMappedExeConfiguration(
				new ExeConfigurationFileMap() { ExeConfigFilename = configFile },
				ConfigurationUserLevel.None
			);
			return (JabberConfigurationSection)cfg.GetSection(Schema.SECTION_NAME);
		}
	}
}