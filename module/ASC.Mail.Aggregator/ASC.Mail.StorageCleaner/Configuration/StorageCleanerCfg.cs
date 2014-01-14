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

namespace ASC.Mail.StorageCleaner.Configuration
{
    internal static class StorageCleanerCfg
    {
        public static ConnectionStringSettings ConnectionString { get; private set; }

        public static int MaxThreads { get; private set; }

        public static int TasksChunkSize { get; private set; }

        public static int TasksGenChunkCount { get; private set; }

        public static string DbLockName { get; private set; }

        public static int DbLockTimeot { get; private set; }

        public static int WatchdogTimeout { get; private set; }

        static StorageCleanerCfg()
        {
            var section = ConfigurationManager.GetSection(Schema.section_name) as CleanerConfigurationSection;
            if (section == null)
            {
                throw new ConfigurationErrorsException("Section " + Schema.section_name + " not found.");
            }

            ConnectionString = ConfigurationManager.ConnectionStrings[Schema.connection_string];

            MaxThreads = section.CleanerConfigurationElement.MaxThreads;
            TasksChunkSize = section.CleanerConfigurationElement.TasksChunkSize;
            DbLockName = section.CleanerConfigurationElement.DbLockName;
            DbLockTimeot = section.CleanerConfigurationElement.DbLockTimeot;
            TasksGenChunkCount = section.CleanerConfigurationElement.TasksGenChunkCount;
            WatchdogTimeout = section.CleanerConfigurationElement.WatchdogTimeout;
        }
    }
}
