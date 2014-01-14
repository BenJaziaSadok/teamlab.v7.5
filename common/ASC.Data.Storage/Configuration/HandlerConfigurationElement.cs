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
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace ASC.Data.Storage.Configuration
{
    public class HandlerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this[Schema.NAME]; }
            set { this[Schema.NAME] = value; }
        }

        [ConfigurationProperty(Schema.TYPE, IsRequired = true)]
        public string TypeName
        {
            get { return (string) this[Schema.TYPE]; }
            set { this[Schema.TYPE] = value; }
        }

        [ConfigurationProperty(Schema.PROPERTIES, IsDefaultCollection = false)]
        public NameValueConfigurationCollection HandlerProperties
        {
            get { return (NameValueConfigurationCollection) this[Schema.PROPERTIES]; }
            set { this[Schema.PROPERTIES] = value; }
        }

        public Type Type
        {
            get { return Type.GetType(TypeName, true); }
        }

        public IDictionary<string, string> GetProperties()
        {
            var properties = new Dictionary<string, string>();
            foreach (NameValueConfigurationElement nameValuePair in HandlerProperties)
            {
                properties.Add(nameValuePair.Name, nameValuePair.Value);
            }
            return properties;
        }

        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            if (elementName == Schema.PROPERTY)
            {
                HandlerProperties.Add(new NameValueConfigurationElement(reader.GetAttribute(0), reader.GetAttribute(1)));
                return true;
            }
            return base.OnDeserializeUnrecognizedElement(elementName, reader);
        }
    }
}