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
    public class BetweenExp : Exp
    {
        private readonly string column;
        private readonly object maxValue;
        private readonly object minValue;

        public BetweenExp(string column, object minValue, object maxValue)
        {
            this.column = column;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return string.Format("{0} {1}between ? and ?", column, Not ? "not " : string.Empty);
        }

        public override object[] GetParameters()
        {
            return new[] {minValue, maxValue};
        }
    }
}