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

using System.IO;
using ASC.Data.Backup.Tasks.Modules;

namespace ASC.Data.Backup.Tasks
{
    internal static class KeyHelper
    {
        public static string GetTableZipKey(IModuleSpecifics module, string tableName)
        {
            return string.Format("databases/{0}/{1}", module.ConnectionStringName, tableName);
        }

        public static string GetFileZipKey(BackupFileInfo file)
        {
            return Path.Combine(file.Module, file.Domain, file.Path).Replace('\\', '/');
        }

        public static string GetStorageRestoreInfoZipKey()
        {
            return "storage/restore_info";
        }
    }
}
