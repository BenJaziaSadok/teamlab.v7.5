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

using System;
using System.Text;

#endregion

namespace ASC.Common.Data.Sql.Expressions
{
    public class ExistsExp : Exp
    {
        private readonly SqlQuery query;


        public ExistsExp(SqlQuery query)
        {
            this.query = query;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("{0}exists({1})", Not ? "not " : string.Empty, query.ToString(dialect));
        }

        public override object[] GetParameters()
        {
            if (query != null) return query.GetParameters();
            return new object[0];
        }
    }
}