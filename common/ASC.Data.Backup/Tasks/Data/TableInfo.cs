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

using System.Diagnostics;

namespace ASC.Data.Backup.Tasks.Data
{
    internal enum InsertMethod
    {
        None,
        Insert,
        Replace,
        Ignore
    }

    [DebuggerDisplay("Name={Name}")]
    internal class TableInfo
    {
        public string Name { get; private set; }
        public string AutoIncrementColumn { get; set; }
        public string GuidIDColumn { get; set; }
        public string TenantColumn { get; set; }
        public string[] UserIDColumns { get; set; }
        public InsertMethod InsertMethod { get; set; }

        public TableInfo(string name)
        {
            Name = name;
            AutoIncrementColumn = null;
            TenantColumn = null;
            UserIDColumns = new string[0];
            InsertMethod = InsertMethod.Insert;
        }

        public bool HasIncrementColumn()
        {
            return !string.IsNullOrEmpty(AutoIncrementColumn);
        }

        public bool HasTenantColumn()
        {
            return !string.IsNullOrEmpty(TenantColumn);
        }
    }
}
