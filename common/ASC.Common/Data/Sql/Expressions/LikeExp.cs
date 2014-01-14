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

#endregion

namespace ASC.Common.Data.Sql.Expressions
{
    [Flags]
    public enum SqlLike
    {
        StartWith = 1,
        EndWith = 2,
        AnyWhere = StartWith | EndWith,
    }

    public class LikeExp : Exp
    {
        private readonly string column;
        private readonly string str;

        public LikeExp(string column, string str, SqlLike like)
        {
            this.column = column;
            if (str != null)
            {
                if ((like & SqlLike.StartWith) == SqlLike.StartWith) str += "%";
                if ((like & SqlLike.EndWith) == SqlLike.EndWith) str = "%" + str;
            }
            this.str = str;
        }

        public override string ToString(ISqlDialect dialect)
        {
            return str != null
                       ? string.Format("{0} {1}like ?", column, Not ? "not " : string.Empty)
                       : string.Format("{0} is {1}null", column, Not ? "not " : string.Empty);
        }

        public override object[] GetParameters()
        {
            return str == null ? new object[0] : new object[] {str};
        }
    }
}