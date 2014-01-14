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
    public class ModuleConfigurationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ModuleConfigurationElement();
        }

        internal ModuleConfigurationElement GetModuleElement(string name)
        {
            return (ModuleConfigurationElement) BaseGet(name);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ModuleConfigurationElement) element).Name;
        }

        public void Add(ModuleConfigurationElement element)
        {
            BaseAdd(element);
        }

        internal ModuleConfigurationElement GetModuleElement(int index)
        {
            return (ModuleConfigurationElement) BaseGet(index);
        }
    }
}