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

namespace ASC.Mail.StorageCleaner.Configuration
{
    internal static class Schema
    {
        public const string connection_string = "storage_cleaner";
        public const string section_name = "storage_cleaner";
        public const string configuration_element_name = "cleaner";
        public const string property_max_threads = "max_threads";
        public const string property_tasks_chunk_size = "tasks_chunck_size";
        public const string property_tasks_gen_chunks_count = "tasks_gen_chunks_count";
        public const string property_db_lock_name = "db_lock_name";
        public const string property_db_lock_timeout = "db_lock_timeout";
        public const string property_watchdog_timeout = "watchdog_timeout";
    }
}