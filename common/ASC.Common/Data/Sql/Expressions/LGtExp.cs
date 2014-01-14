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
    public class LGExp : Exp
    {
        private readonly SqlIdentifier column;
        private readonly bool equal;
        private readonly object value;

        public LGExp(string column, object value, bool equal)
        {
            this.column = (SqlIdentifier) column;
            this.value = value;
            this.equal = equal;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return Not
                       ? string.Format("{0} >{1} ?", column.ToString(dialect), !equal ? "=" : string.Empty)
                       : string.Format("{0} <{1} ?", column.ToString(dialect), equal ? "=" : string.Empty);
        }

        public override object[] GetParameters()
        {
            return new[] {value};
        }
    }
}