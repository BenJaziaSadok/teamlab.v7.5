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

using System.Data;

namespace ASC.Data.Backup.Extensions
{
    public static class DataExtensions
    {
        public static IDbCommand WithTimeout(this IDbCommand command, int timeout)
        {
            if (command != null)
            {
                command.CommandTimeout = timeout;
            }
            return command;
        }

        public static IDbConnection Fix(this IDbConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Open)
            {
                connection.Close();
                connection.Open();
            }
            return connection;
        }
    }
}
