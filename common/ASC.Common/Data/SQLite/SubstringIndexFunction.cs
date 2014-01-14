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
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace ASC.Common.Data.SQLite
{
    [SQLiteFunction(Name = "substring_index", Arguments = 3, FuncType = FunctionType.Scalar)]
    public class SubstringIndexFunction : SQLiteFunction
    {
        public override object Invoke(object[] args)
        {
            if (args.Length != 3 || args[0] == null)
            {
                return null;
            }
            if (args[0] == DBNull.Value)
            {
                return DBNull.Value;
            }

            var str = Convert.ToString(args[0]);
            var delimiter = Convert.ToString(args[1]);
            var count = Convert.ToInt32(args[2]);

            var result = (IEnumerable<string>)str.Split(new[] { delimiter }, StringSplitOptions.None);
            if (count < 0)
            {
                result = result.Reverse();
            }
            result = result.Take(0 <= count ? count : -count);
            return string.Join(delimiter, result.ToArray());
        }
    }
}
