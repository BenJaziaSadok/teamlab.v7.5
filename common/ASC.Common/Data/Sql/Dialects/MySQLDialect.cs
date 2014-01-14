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

namespace ASC.Common.Data.Sql.Dialects
{
    class MySQLDialect : ISqlDialect
    {
        public string IdentityQuery
        {
            get { return "last_insert_id()"; }
        }

        public string Autoincrement
        {
            get { return "auto_increment"; }
        }

        public virtual string InsertIgnore
        {
            get { return "insert ignore"; }
        }
        
        public bool SupportMultiTableUpdate
        {
            get { return true; }
        }

        public bool SeparateCreateIndex
        {
            get { return false; }
        }

        public string DbTypeToString(DbType type, int size, int precision)
        {
            switch (type)
            {
                case DbType.Guid:
                    return "char(38)";

                case DbType.AnsiString:
                case DbType.String:
                    if (size <= 8192) return string.Format("VARCHAR({0})", size);
                    else if (8192 < size && size <= UInt16.MaxValue) return "TEXT";
                    else if (UInt16.MaxValue < size && size <= ((int)Math.Pow(2, 24) - 1)) return "MEDIUMTEXT";
                    else return "LONGTEXT";

                case DbType.AnsiStringFixedLength:
                case DbType.StringFixedLength:
                    return string.Format("CHAR({0})", size);

                case DbType.Xml:
                    return "MEDIUMTEXT";

                case DbType.Binary:
                case DbType.Object:
                    if (size <= 8192) return string.Format("BINARY({0})", size);
                    else if (8192 < size && size <= UInt16.MaxValue) return "BLOB";
                    else if (UInt16.MaxValue < size && size <= ((int)Math.Pow(2, 24) - 1)) return "MEDIUMBLOB";
                    else return "LONGBLOB";

                case DbType.Boolean:
                case DbType.Byte:
                    return "TINYINY";
                case DbType.SByte:
                    return "TINYINY UNSIGNED";

                case DbType.Int16:
                    return "SMALLINT";
                case DbType.UInt16:
                    return "SMALLINT UNSIGNED";

                case DbType.Int32:
                    return "INT";
                case DbType.UInt32:
                    return "INT UNSIGNED";

                case DbType.Int64:
                    return "BIGINT";
                case DbType.UInt64:
                    return "BIGINT UNSIGNED";

                case DbType.Date:
                    return "DATE";
                case DbType.DateTime:
                case DbType.DateTime2:
                    return "DATETIME";
                case DbType.Time:
                    return "TIME";

                case DbType.Decimal:
                    return string.Format("DECIMAL({0},{1})", size, precision);
                case DbType.Double:
                    return "DOUBLE";
                case DbType.Single:
                    return "FLOAT";

                default:
                    throw new ArgumentOutOfRangeException(type.ToString());
            }
        }

        public IsolationLevel GetSupportedIsolationLevel(IsolationLevel il)
        {
            return il;
        }
    }
}