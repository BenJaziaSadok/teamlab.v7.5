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

namespace ASC.Thrdparty.Configuration
{
    public class ConsumerConfigurationSection : ConfigurationSection
    {
        public const string SectionName = "consumers";

        [ConfigurationProperty("keys")]
        public KeyElementCollection Keys
        {
            get { return (KeyElementCollection)base["keys"]; }
            set { base["keys"] = value; }
        }

        [ConfigurationProperty("connectionstring")]
        public string ConnectionString
        {
            get { return (string)base["connectionstring"]; }
            set { base["connectionstring"] = value; }
        }

        public static ConsumerConfigurationSection GetSection()
        {
            return (ConsumerConfigurationSection)ConfigurationManager.GetSection(SectionName);
        }
    }

    public class KeyElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new KeyElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((KeyElement)element).Name;
        }

        public KeyElement GetKey(string name)
        {
            return BaseGet(name) as KeyElement;
        }

        public string GetKeyValue(string name)
        {
            var keyElement = GetKey(name);
            return keyElement != null ? keyElement.Value : string.Empty;
        }
    }

    public class KeyElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("value", DefaultValue = "")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        [ConfigurationProperty("consumer")]
        public string ConsumerName
        {
            get { return (string)base["consumer"]; }
            set { base["consumer"] = value; }
        }

        [ConfigurationProperty("type", DefaultValue = KeyType.Default)]
        public KeyType Type
        {
            get { return (KeyType)base["type"]; }
            set { base["type"] = value; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public enum KeyType
        {
            Default,
            Key,
            Secret
        }
    }
}