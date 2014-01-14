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
using System.Xml;

namespace ASC.FullTextIndex.Service.Config
{
	class TextIndexCfgModuleElement : ConfigurationElement
	{
		[ConfigurationProperty("name", IsKey = true, IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty("connectionStringName")]
		public string ConnectionStringName
		{
			get { return (string)this["connectionStringName"]; }
			set { this["connectionStringName"] = value; }
		}

		public string Select
		{
			get;
			private set;
		}

		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			reader.MoveToAttribute("name");
			Name = reader.Value;

			ConnectionStringName = reader.MoveToAttribute("connectionStringName") ? reader.Value : null;

			while (reader.Read())
			{
				if (reader.NodeType == XmlNodeType.EndElement) break;
				if (reader.NodeType == XmlNodeType.CDATA) Select = reader.Value;
			}
		}
	}
}
