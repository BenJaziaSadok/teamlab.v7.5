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
    internal enum AgregateType
    {
        count,
        min,
        max,
        avg,
        sum
    }

    internal class SelectAgregate : ISqlInstruction
    {
        private readonly AgregateType agregateType;
        private readonly string column;

        public SelectAgregate(AgregateType agregateType)
            : this(agregateType, null)
        {
        }

        public SelectAgregate(AgregateType agregateType, string column)
        {
            this.agregateType = agregateType;
            this.column = column;
        }

        #region ISqlInstruction Members

        public string ToString(ISqlDialect dialect)
        {
            return string.Format("{0}({1})", agregateType, column == null ? "*" : column);
        }

        public object[] GetParameters()
        {
            return new object[0];
        }

        #endregion

        public override string ToString()
        {
            return ToString(SqlDialect.Default);
        }
    }
}