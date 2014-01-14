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

using System.Data;
using System.Text;

namespace ASC.Common.Data.Sql
{
    public class SqlDialect : ISqlDialect
    {
        public static readonly ISqlDialect Default = new SqlDialect();


        public virtual string IdentityQuery
        {
            get { return "@@identity"; }
        }

        public virtual string Autoincrement
        {
            get { return "AUTOINCREMENT"; }
        }

        public virtual string InsertIgnore
        {
            get { return "insert ignore"; }
        }


        public virtual bool SupportMultiTableUpdate
        {
            get { return true; }
        }

        public virtual bool SeparateCreateIndex
        {
            get { return true; }
        }


        public virtual string DbTypeToString(DbType type, int size, int precision)
        {
            var s = new StringBuilder(type.ToString().ToLower());
            if (0 < size)
            {
                s.AppendFormat(0 < precision ? "({0}, {1})" : "({0})", size, precision);
            }
            return s.ToString();
        }

        public virtual IsolationLevel GetSupportedIsolationLevel(IsolationLevel il)
        {
            return il;
        }
    }
}