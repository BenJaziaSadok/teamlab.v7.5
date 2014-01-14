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

using System.Configuration;

namespace ASC.Xmpp.Common.Configuration
{
    public class JabberConfigurationSection : ConfigurationSection
	{
		[ConfigurationProperty(Schema.LISTENERS, IsDefaultCollection = false)]
		public JabberConfigurationCollection Listeners
		{
			get { return (JabberConfigurationCollection)base[Schema.LISTENERS]; }
		}

		[ConfigurationProperty(Schema.STORAGES, IsDefaultCollection = false)]
		public JabberConfigurationCollection Storages
		{
			get { return (JabberConfigurationCollection)base[Schema.STORAGES]; }
		}

		[ConfigurationProperty(Schema.SERVICES, IsDefaultCollection = false)]
		public ServiceConfigurationCollection Services
		{
			get { return (ServiceConfigurationCollection)base[Schema.SERVICES]; }
		}
	}
}