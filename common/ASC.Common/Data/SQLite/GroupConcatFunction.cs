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
    [SQLiteFunction(Name = "group_concat", Arguments = 2, FuncType = FunctionType.Aggregate)]
    public class GroupConcatFunction : SQLiteFunction
    {
        public override object Final(object contextData)
        {
            return contextData != null ? contextData.ToString() : null;
        }

        public override void Step(object[] args, int stepNumber, ref object contextData)
        {
            if (contextData == null)
            {
                contextData = new GroupConcater(1 < args.Length ? args[1] : ',');
            }
            ((GroupConcater) contextData).Step(args[0]);
        }

        private class GroupConcater
        {
            private readonly string separator;
            private string text;

            public GroupConcater(object separator)
            {
                this.separator = IsDBNull(separator) ? "," : separator.ToString();
            }

            public void Step(object arg)
            {
                if (!IsDBNull(arg))
                {
                    text += string.Format("{0}{1}", arg, separator);
                }
            }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(text)) return null;
                string result = text.Remove(text.Length - separator.Length, separator.Length);
                return !string.IsNullOrEmpty(result) ? result : null;
            }

            private bool IsDBNull(object arg)
            {
                return arg == null || DBNull.Value.Equals(arg);
            }
        }
    }

    [SQLiteFunction(Name = "group_concat", Arguments = 1, FuncType = FunctionType.Aggregate)]
    public class GroupConcatFunction2 : GroupConcatFunction
    {
    }
}