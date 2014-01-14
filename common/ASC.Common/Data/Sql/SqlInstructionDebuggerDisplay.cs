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
using System.Text;

namespace ASC.Common.Data.Sql
{
    class SqlInstructionDebuggerDisplay
    {
        private readonly ISqlInstruction i;


        public SqlInstructionDebuggerDisplay(ISqlInstruction i)
        {
            this.i = i;
        }

        public override string ToString()
        {
            if (i == null) return null;

            var parts = i.ToString().Split('?');
            var result = new StringBuilder(parts[0]);
            var counter = 0;
            foreach (var p in i.GetParameters())
            {
                counter++;
                if (p == null)
                {
                    result.Append("null");
                }
                else
                {
                    var format = "{0}";
                    if (p is DateTime || p is Guid || p is string) format = "'{0}'";
                    result.AppendFormat(format, p);
                }
                if (counter < parts.Length) result.Append(parts[counter]);
            }
            return result.ToString();
        }
    }
}
