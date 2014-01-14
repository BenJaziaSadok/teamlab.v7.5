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

using System.Collections.Generic;
using System.Linq;

namespace ASC.Data.Backup.Tasks.Data
{
    internal class DataRowInfo
    {
        private readonly Dictionary<string, object> _items = new Dictionary<string, object>(); 

        public string TableName { get; set; }

        public string[] ColumnNames
        {
            get { return _items.Keys.ToArray(); }
        }

        public object[] Values
        {
            get { return _items.Values.ToArray(); }
        }

        public object this[int index]
        {
            get { return _items.ElementAt(index).Value; }
            set { InsertItem(_items.ElementAt(index).Key, value); }
        }

        public object this[string columnName]
        {
            get { return _items[columnName]; }
            set { InsertItem(columnName, value); }
        }

        public void InsertItem(string columnName, object item)
        {
            _items[columnName] = item;
        }

        public DataRowInfo(string tableName)
        {
            TableName = tableName;
        }
    }
}
