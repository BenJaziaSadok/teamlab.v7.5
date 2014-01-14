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
    public class StorageConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty(Schema.MODULES, IsDefaultCollection = false)]
        public ModuleConfigurationCollection Modules
        {
            get { return (ModuleConfigurationCollection) base[Schema.MODULES]; }
        }

        [ConfigurationProperty(Schema.HANDLERS, IsDefaultCollection = false)]
        public HandlersConfigurationCollection Handlers
        {
            get { return (HandlersConfigurationCollection) base[Schema.HANDLERS]; }
        }

        [ConfigurationProperty(Schema.APPENDERS, IsDefaultCollection = false)]
        public AppenderConfigurationCollection Appenders
        {
            get { return (AppenderConfigurationCollection) base[Schema.APPENDERS]; }
        }

        [ConfigurationProperty(Schema.FIXERS, IsDefaultCollection = false)]
        public FixerConfigurationCollection Fixers
        {
            get { return (FixerConfigurationCollection) base[Schema.FIXERS]; }
        }
    }
}