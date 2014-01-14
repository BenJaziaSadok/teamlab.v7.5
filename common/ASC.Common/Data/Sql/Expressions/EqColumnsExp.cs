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
    public class EqColumnsExp : Exp
    {
        private readonly ISqlInstruction column1;
        private readonly ISqlInstruction column2;

        public EqColumnsExp(string column1, string column2)
        {
            this.column1 = (SqlIdentifier) column1;
            this.column2 = (SqlIdentifier) column2;
        }

        public EqColumnsExp(string column1, SqlQuery query)
        {
            this.column1 = (SqlIdentifier) column1;
            column2 = new SubExp(query);
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("{0} {1} {2}",
                                 column1.ToString(dialect),
                                 Not ? "<>" : "=",
                                 column2.ToString(dialect));
        }

        public override object[] GetParameters()
        {
            return column2.GetParameters();
        }
    }
}