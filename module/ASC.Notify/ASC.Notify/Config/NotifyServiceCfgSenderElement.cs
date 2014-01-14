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

using System.Collections.Generic;
using System.Configuration;

namespace ASC.Notify.Config
{
    class NotifyServiceCfgSenderElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get { return (string)base["type"]; }
        }


        public new IDictionary<string, string> Properties
        {
            get;
            private set;
        }


        public NotifyServiceCfgSenderElement()
        {
            Properties = new Dictionary<string, string>();
        }

        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            Properties[name] = value;
            return true;
        }
    }
}
