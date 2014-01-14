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
using System.Data.SQLite;
using System.Text;
using SqliteFunction = System.Data.SQLite.SQLiteFunction;
using SqliteFunctionAttribute = System.Data.SQLite.SQLiteFunctionAttribute;

namespace ASC.Common.Data.SQLite
{
    [SqliteFunction(Name = "concat", Arguments = -1, FuncType = FunctionType.Scalar)]
    public class ConcatFunction : SqliteFunction
    {
        public override object Invoke(object[] args)
        {
            var result = new StringBuilder();
            foreach (object arg in args)
            {
                result.Append(arg);
            }
            return result.Length != 0 ? result.ToString() : null;
        }
    }

    [SqliteFunction(Name = "concat_ws", Arguments = -1, FuncType = FunctionType.Scalar)]
    public class ConcatWSFunction : SqliteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length < 2 || args[0] == null || DBNull.Value.Equals(args[0])) return null;
            var result = new StringBuilder();
            for (int i = 1; i < args.Length; i++)
            {
                result.AppendFormat("{0}{1}", args[i], i == args.Length - 1 ? string.Empty : args[0]);
            }
            return result.Length != 0 ? result.ToString() : null;
        }
    }
}