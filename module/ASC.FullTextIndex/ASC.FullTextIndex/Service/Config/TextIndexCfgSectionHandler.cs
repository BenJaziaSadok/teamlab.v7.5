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

namespace ASC.FullTextIndex.Service.Config
{
	class TextIndexCfgSectionHandler : ConfigurationSection
	{
        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get { return (string)this["connectionStringName"]; }
        }

        [ConfigurationProperty("changedCron", IsRequired = true)]
		public string ChangedCron
		{
			get { return (string)base["changedCron"]; }
		}

		[ConfigurationProperty("removedCron", IsRequired = true)]
		public string RemovedCron
		{
			get { return (string)base["removedCron"]; }
		}

        [ConfigurationProperty("indexPath", DefaultValue = "")]
		public string IndexPath
		{
			get { return (string)base["indexPath"]; }
		}

        [ConfigurationProperty("userActivityDays", DefaultValue = 1)]
        public int UserActivityDays
		{
            get { return (int)base["userActivityDays"]; }
		}

		[ConfigurationProperty("modules")]
		public TextIndexCfgModuleCollection Modules
		{
			get { return (TextIndexCfgModuleCollection)base["modules"]; }
		}
	}
}
