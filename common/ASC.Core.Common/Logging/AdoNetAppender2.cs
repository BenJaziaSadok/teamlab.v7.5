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
using ASC.Common.Data;
using log4net.Appender;

namespace ASC.Core.Common.Logging
{
    public class AdoNetAppender2 : AdoNetAppender
    {
        public string ConnectionStringName
        {
            get;
            set;
        }


        protected override Type ResolveConnectionType()
        {
            if (!string.IsNullOrEmpty(ConnectionStringName) && string.IsNullOrEmpty(ConnectionString))
            {
                try
                {
                    ConnectionString = DbRegistry.GetConnectionString(ConnectionStringName).ConnectionString;
                    using (var connection = DbRegistry.CreateDbConnection(ConnectionStringName))
                    {
                        ConnectionType = connection.GetType().AssemblyQualifiedName;
                    }
                }
                catch (Exception exception)
                {
                    ErrorHandler.Error("Failed to load resolve connection [" + ConnectionStringName + "]", exception);
                    throw;
                }
            }
            return base.ResolveConnectionType();
        }
    }
}
