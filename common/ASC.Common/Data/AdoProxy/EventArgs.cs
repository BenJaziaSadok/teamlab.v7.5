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
using System.Data;
using System.Text;

namespace ASC.Common.Data.AdoProxy
{
    class ExecutedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; private set; }

        public string SqlMethod { get; private set; }

        public string Sql { get; private set; }

        public string SqlParameters { get; private set; }


        public ExecutedEventArgs(string method, TimeSpan duration)
            : this(method, duration, null)
        {

        }

        public ExecutedEventArgs(string method, TimeSpan duration, IDbCommand command)
        {
            SqlMethod = method;
            Duration = duration;
            if (command != null)
            {
                Sql = command.CommandText;
                
                if (0 < command.Parameters.Count)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (IDbDataParameter p in command.Parameters)
                    {
                        if (!string.IsNullOrEmpty(p.ParameterName)) stringBuilder.AppendFormat("{0}=", p.ParameterName);
                        stringBuilder.AppendFormat("{0}, ", p.Value == null || DBNull.Value.Equals(p.Value) ? "NULL" : p.Value.ToString());
                    }
                    SqlParameters = stringBuilder.ToString(0, stringBuilder.Length - 2);
                }
            }
        }
    }
}