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
using System.Collections.Generic;
using System.Linq;

namespace ASC.Data.Backup.Exceptions
{
    internal static class ThrowHelper
    {
        public static DbBackupException CantDetectTenant(string tableName)
        {
            return new DbBackupException(string.Format("Can't detect tenant column for table {0}.", tableName));
        }

        public static DbBackupException CantOrderTables(IEnumerable<string> conflictingTables)
        {
            return new DbBackupException(string.Format("Can't order tables [\"{0}\"].", string.Join("\", \"", conflictingTables.ToArray())));
        }

        public static DbBackupException CantOrderModules(IEnumerable<Type> conflictingTypes)
        {
            return new DbBackupException(string.Format("Can't order modules [\"{0}\"].", string.Join("\", \"", conflictingTypes.Select(x => x.Name).ToArray())));
        }

        public static DbBackupException CantRestoreTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't restore table {0}.", tableName), reason);
        }

        public static DbBackupException CantBackupTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't backup table {0}.", tableName), reason);
        }

        public static DbBackupException CantDeleteTable(string tableName, Exception reason)
        {
            return new DbBackupException(string.Format("Can't delete table {0}.", tableName), reason);
        }
    }
}
