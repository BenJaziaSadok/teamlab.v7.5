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
using System.Linq;
using System.Text;

namespace ASC.Common.Data.Sql.Expressions
{
    public class InExp : Exp
    {
        private readonly string column;
        private readonly SqlQuery subQuery;
        private readonly object[] values;

        public InExp(string column, object[] values)
        {
            this.column = column;
            this.values = values;
        }

        public InExp(string column, SqlQuery subQuery)
        {
            this.column = column;
            this.subQuery = subQuery;
        }

        public override string ToString(ISqlDialect dialect)
        {
            if (values != null && values.Count() < 2) 
            {
                var exp = values.Count() == 0 ? Exp.False : Exp.Eq(column, values.ElementAt(0));
                return (Not ? !exp : exp).ToString(dialect);
            }

            var sql = new StringBuilder(column);
            if (Not) sql.Append(" not");
            sql.Append(" in (");

            if (values != null)
            {
                sql.Append(string.Join(",", Enumerable.Repeat("?", values.Count()).ToArray()));
            }
            if (subQuery != null)
            {
                sql.Append(subQuery.ToString(dialect));
            }
            return sql.Append(")").ToString();
        }

        public override object[] GetParameters()
        {
            if (values != null) return values;
            if (subQuery != null) return subQuery.GetParameters();
            return new object[0];
        }
    }
}