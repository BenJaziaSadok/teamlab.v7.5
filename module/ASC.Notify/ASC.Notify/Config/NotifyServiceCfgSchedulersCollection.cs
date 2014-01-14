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

namespace ASC.Notify.Config
{
    public class NotifyServiceCfgSchedulersCollection : ConfigurationElementCollection
    {
        [ConfigurationProperty("serverRoot", DefaultValue = "http://*/")]
        public string ServerRoot
        {
            get { return (string)base["serverRoot"]; }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new NotifyServiceCfgSchedulerElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NotifyServiceCfgSchedulerElement)element).Name;
        }
    }
}
