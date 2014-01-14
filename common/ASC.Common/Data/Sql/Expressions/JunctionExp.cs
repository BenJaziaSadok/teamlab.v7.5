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

using System.Collections.Generic;

#endregion

namespace ASC.Common.Data.Sql.Expressions
{
    public class JunctionExp : Exp
    {
        private readonly bool and;
        private readonly Exp exp1;
        private readonly Exp exp2;

        public JunctionExp(Exp exp1, Exp exp2, bool and)
        {
            this.exp1 = exp1;
            this.exp2 = exp2;
            this.and = and;
        }

        public override string ToString(ISqlDialect dialect)
        {
            string format = exp1 is JunctionExp && ((JunctionExp) exp1).and != and ? "({0})" : "{0}";
            format += " {1} ";
            format += exp2 is JunctionExp && ((JunctionExp) exp2).and != and ? "({2})" : "{2}";
            return Not
                       ? string.Format(format, (!exp1).ToString(dialect), and ? "or" : "and",
                                       (!exp2).ToString(dialect))
                       : string.Format(format, exp1.ToString(dialect), and ? "and" : "or", exp2.ToString(dialect));
        }

        public override object[] GetParameters()
        {
            var parameters = new List<object>();
            parameters.AddRange(exp1.GetParameters());
            parameters.AddRange(exp2.GetParameters());
            return parameters.ToArray();
        }
    }
}