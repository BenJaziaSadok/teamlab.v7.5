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

namespace ASC.Common.Data.SQLite
{
    [SQLiteFunction(Name = "substring", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class SubstringFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length != 3 || args[0] == null) return null;
            if (args[0] == DBNull.Value) return DBNull.Value;
            var str = Convert.ToString(args[0]);
            var pos = Convert.ToInt32(args[1]);
            var length = Convert.ToInt32(args[2]);
            if (pos < 0 || pos > str.Length - 1 || length < 1 || pos + length > str.Length)
                return str;
            return str.Substring(pos, length);
        }
    }
}
