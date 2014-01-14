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

#region usings

using System.Diagnostics;
using System.Text;
using ASC.Common.Data.Sql.Expressions;

#endregion

namespace ASC.Common.Data.Sql
{
    [DebuggerTypeProxy(typeof(SqlDebugView))]
    public class SqlDelete : ISqlInstruction
    {
        private readonly string table;
        private Exp where = Exp.Empty;

        public SqlDelete(string table)
        {
            this.table = table;
        }

        #region ISqlInstruction Members

        public string ToString(ISqlDialect dialect)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("delete from {0}", table);
            if (where != Exp.Empty) sql.AppendFormat(" where {0}", where.ToString(dialect));
            return sql.ToString();
        }

        public object[] GetParameters()
        {
            return where != Exp.Empty ? where.GetParameters() : new object[0];
        }

        #endregion

        public SqlDelete Where(Exp where)
        {
            this.where = this.where & where;
            return this;
        }

        public SqlDelete Where(string column, object value)
        {
            return Where(Exp.Eq(column, value));
        }

        public override string ToString()
        {
            return ToString(SqlDialect.Default);
        }
    }
}