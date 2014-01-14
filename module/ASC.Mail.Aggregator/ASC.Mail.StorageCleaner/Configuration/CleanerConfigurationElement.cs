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
    public class CleanerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.property_max_threads, IsRequired = false, IsKey = false, DefaultValue = 1)]
        public int MaxThreads
        {
            get { return (int)this[Schema.property_max_threads]; }
            set { this[Schema.property_max_threads] = value; }
        }

        [ConfigurationProperty(Schema.property_tasks_chunk_size, IsRequired = false, IsKey = false, DefaultValue = 10)]
        public int TasksChunkSize
        {
            get { return (int)this[Schema.property_tasks_chunk_size]; }
            set { this[Schema.property_tasks_chunk_size] = value; }
        }

        [ConfigurationProperty(Schema.property_tasks_gen_chunks_count, IsRequired = false, IsKey = false, DefaultValue = 10)]
        public int TasksGenChunkCount
        {
            get { return (int)this[Schema.property_tasks_gen_chunks_count]; }
            set { this[Schema.property_tasks_gen_chunks_count] = value; }
        }

        [ConfigurationProperty(Schema.property_db_lock_name, IsRequired = false, IsKey = false, DefaultValue = "storage_cleaner")]
        public string DbLockName
        {
            get { return (string)this[Schema.property_db_lock_name]; }
            set { this[Schema.property_db_lock_name] = value; }
        }

        [ConfigurationProperty(Schema.property_db_lock_timeout, IsRequired = false, IsKey = false, DefaultValue = 5)]
        public int DbLockTimeot
        {
            get { return (int)this[Schema.property_db_lock_timeout]; }
            set { this[Schema.property_db_lock_timeout] = value; }
        }

        [ConfigurationProperty(Schema.property_watchdog_timeout, IsRequired = false, IsKey = false, DefaultValue = 600)]
        public int WatchdogTimeout
        {
            get { return (int)this[Schema.property_watchdog_timeout]; }
            set { this[Schema.property_watchdog_timeout] = value; }
        }
    }
}