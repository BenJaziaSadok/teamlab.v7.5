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

namespace ASC.Common.Data.Sql.Expressions
{
    public class SubExp : Exp
    {
        private readonly string alias;
        private readonly SqlQuery subQuery;

        public SubExp(SqlQuery subQuery)
            : this(subQuery, null)
        {
        }

        public SubExp(SqlQuery subQuery, string alias)
        {
            this.subQuery = subQuery;
            this.alias = alias;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("({0}){1}", subQuery.ToString(dialect),
                                 string.IsNullOrEmpty(alias) ? string.Empty : " as " + alias);
        }

        public override object[] GetParameters()
        {
            return subQuery.GetParameters();
        }
    }
}