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
using System.Data;

namespace ASC.Common.Data.Sql
{
    public class SQLiteDialect : ISqlDialect
    {
        public string IdentityQuery
        {
            get { return "last_insert_rowid()"; }
        }

        public string Autoincrement
        {
            get { return "autoincrement"; }
        }

        public virtual string InsertIgnore
        {
            get { return "insert or ignore"; }
        }

        public bool SupportMultiTableUpdate
        {
            get { return false; }
        }

        public bool SeparateCreateIndex
        {
            get { return true; }
        }


        public string DbTypeToString(DbType type, int size, int precision)
        {
            switch (type)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.Xml:
                case DbType.Guid:
                    return "TEXT";

                case DbType.Binary:
                case DbType.Object:
                    return "BLOB";

                case DbType.Boolean:
                case DbType.Currency:
                case DbType.Decimal:
                case DbType.VarNumeric:
                    return "NUMERIC";

                case DbType.Date:
                case DbType.DateTime:
                case DbType.DateTime2:
                case DbType.DateTimeOffset:
                case DbType.Time:
                    return "DATETIME";

                case DbType.Byte:
                case DbType.Int16:
                case DbType.Int32:
                case DbType.Int64:
                case DbType.SByte:
                case DbType.UInt16:
                case DbType.UInt32:
                case DbType.UInt64:
                    return "INTEGER";

                case DbType.Double:
                case DbType.Single:
                    return "REAL";
            }
            throw new ArgumentOutOfRangeException(type.ToString());
        }

        public IsolationLevel GetSupportedIsolationLevel(IsolationLevel il)
        {
            if (il <= IsolationLevel.ReadCommitted) return IsolationLevel.ReadCommitted;
            return IsolationLevel.Serializable;
        }
    }
}