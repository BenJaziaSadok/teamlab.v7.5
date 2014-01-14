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
    class SqlDebugView
    {
        private readonly ISqlInstruction instruction;


        public string Sql
        {
            get { return GetSqlWithParameters(); }
        }

        public object[] Parameters
        {
            get { return instruction.GetParameters(); }
        }


        public SqlDebugView(ISqlInstruction instruction)
        {
            this.instruction = instruction;
        }

        private string GetSqlWithParameters()
        {
            var sql = instruction.ToString();
            var parameters = instruction.GetParameters();
            var sb = new StringBuilder();
            var i = 0;
            foreach (var part in sql.Split('?'))
            {
                sb.Append(part);
                if (i < parameters.Length)
                {
                    var p = parameters[i];
                    if (p == null)
                    {
                        sb.Append("null");
                    }
                    else if (p is string || p is char || p is DateTime || p is Guid)
                    {
                        sb.AppendFormat("'{0}'", p);
                    }
                    else
                    {
                        sb.Append(p);
                    }
                }
                i++;
            }
            return sb.ToString();
        }
    }
}