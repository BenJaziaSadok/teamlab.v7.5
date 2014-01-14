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
    [SQLiteFunction(Name = "lower", Arguments = 1, FuncType = FunctionType.Scalar)]
    public class LowerFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length == 0 || args[0] == null) return null;
            if (args[0] == DBNull.Value) return DBNull.Value;
            return ((string) args[0]).ToLower();
        }
    }
}