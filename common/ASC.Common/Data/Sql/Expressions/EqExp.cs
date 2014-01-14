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
    public class EqExp : Exp
    {
        private readonly string column;
        private readonly object value;

        public EqExp(string column, object value)
        {
            this.column = column;
            this.value = value;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("{0} {1}",
                                 column,
                                 value != null ? (Not ? "<> ?" : "= ?") : (Not ? "is not null" : "is null"));
        }

        public override object[] GetParameters()
        {
            return value == null ? new object[0] : new[] {value};
        }
    }
}