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

namespace ASC.Data.Storage.Configuration
{
    public class AppenderConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.NAME, IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string) this[Schema.NAME]; }
            set { this[Schema.NAME] = value; }
        }

        [ConfigurationProperty(Schema.APPEND, IsRequired = true, IsKey = false)]
        public string Append
        {
            get { return (string) this[Schema.APPEND]; }
            set { this[Schema.APPEND] = value; }
        }

        [ConfigurationProperty(Schema.APPENDSECURE, IsRequired = false, IsKey = false)]
        public string AppendSecure
        {
            get { return (string) this[Schema.APPENDSECURE]; }
            set { this[Schema.APPENDSECURE] = value; }
        }

        [ConfigurationProperty(Schema.ACCEPT_ENCODING, IsRequired = false, IsKey = false)]
        public string AcceptEncoding
        {
            get { return (string) this[Schema.ACCEPT_ENCODING]; }
            set { this[Schema.ACCEPT_ENCODING] = value; }
        }

        [ConfigurationProperty(Schema.EXTs, IsRequired = false, IsKey = false)]
        public string Extensions
        {
            get { return (string)this[Schema.EXTs]; }
            set { this[Schema.EXTs] = value; }
        }
    }
}