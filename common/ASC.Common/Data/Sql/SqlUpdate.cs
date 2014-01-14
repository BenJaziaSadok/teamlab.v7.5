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
using System.Diagnostics;
using System.Text;
using ASC.Common.Data.Sql.Expressions;

namespace ASC.Common.Data.Sql
{
    [DebuggerTypeProxy(typeof(SqlDebugView))]
    public class SqlUpdate : ISqlInstruction
    {
        private readonly List<string> expressions = new List<string>();
        private readonly Dictionary<string, object> sets = new Dictionary<string, object>();
        private readonly SqlIdentifier table;
        private Exp where = Exp.Empty;

        public SqlUpdate(string table)
        {
            this.table = (SqlIdentifier)table;
        }

        #region ISqlInstruction Members

        public string ToString(ISqlDialect dialect)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("update {0} set ", table.ToString(dialect));
            foreach (var set in sets)
            {
                sql.AppendFormat("{0} = {1}, ", set.Key, set.Value is ISqlInstruction ? ((ISqlInstruction)set.Value).ToString(dialect) : "?");
            }
            expressions.ForEach(expression => sql.AppendFormat("{0}, ", expression));
            sql.Remove(sql.Length - 2, 2);
            if (where != Exp.Empty) sql.AppendFormat(" where {0}", where.ToString(dialect));
            return sql.ToString();
        }

        public object[] GetParameters()
        {
            var parameters = new List<object>();
            foreach (object parameter in sets.Values)
            {
                if (parameter is ISqlInstruction)
                {
                    parameters.AddRange(((ISqlInstruction)parameter).GetParameters());
                }
                else
                {
                    parameters.Add(parameter);
                }
            }
            if (where != Exp.Empty) parameters.AddRange(where.GetParameters());
            return parameters.ToArray();
        }

        #endregion

        public SqlUpdate Set(string expression)
        {
            expressions.Add(expression);
            return this;
        }

        public SqlUpdate Set(string column, object value)
        {
            sets[column] = value is SqlQuery ? new SubExp((SqlQuery)value) : value;
            return this;
        }

        public SqlUpdate Where(Exp where)
        {
            this.where = this.where & where;
            return this;
        }

        public SqlUpdate Where(string column, object value)
        {
            return Where(Exp.Eq(column, value));
        }

        public override string ToString()
        {
            return ToString(SqlDialect.Default);
        }
    }
}