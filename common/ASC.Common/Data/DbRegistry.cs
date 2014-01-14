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
using System.Configuration;
using System.Data;
using System.Data.Common;
using ASC.Common.Data.Sql;
using ASC.Common.Data.Sql.Dialects;

namespace ASC.Common.Data
{
    public static class DbRegistry
    {
        private static readonly object syncRoot = new object();
        private static readonly IDictionary<string, DbProviderFactory> providers = new Dictionary<string, DbProviderFactory>();
        private static readonly IDictionary<string, string> connnectionStrings = new Dictionary<string, string>();
        private static readonly IDictionary<string, ISqlDialect> dialects = new Dictionary<string, ISqlDialect>();
        private static volatile bool configured = false;

        static DbRegistry()
        {
            dialects["MySql.Data.MySqlClient.MySqlClientFactory"] = new MySQLDialect();
            dialects["Devart.Data.MySql.MySqlProviderFactory"] = new MySQLDialect();
            dialects["System.Data.SQLite.SQLiteFactory"] = new SQLiteDialect();
        }

        public static void RegisterDatabase(string databaseId, DbProviderFactory providerFactory, string connectionString)
        {
            if (string.IsNullOrEmpty(databaseId)) throw new ArgumentNullException("databaseId");
            if (providerFactory == null) throw new ArgumentNullException("providerFactory");

            if (!providers.ContainsKey(databaseId))
            {
                lock (syncRoot)
                {
                    if (!providers.ContainsKey(databaseId))
                    {
                        providers.Add(databaseId, providerFactory);
                        if (!string.IsNullOrEmpty(connectionString))
                        {
                            connnectionStrings.Add(databaseId, connectionString);
                        }
                    }
                }
            }
        }

        public static void RegisterDatabase(string databaseId, string providerInvariantName, string connectionString)
        {
            RegisterDatabase(databaseId, DbProviderFactories.GetFactory(providerInvariantName), connectionString);
        }

        public static void RegisterDatabase(string databaseId, ConnectionStringSettings connectionString)
        {
            RegisterDatabase(databaseId, connectionString.ProviderName, connectionString.ConnectionString);
        }

        public static void RegisterDatabase(string databaseId, DbProviderFactory providerFactory)
        {
            RegisterDatabase(databaseId, providerFactory, null);
        }

        public static void RegisterDatabase(string databaseId, string providerInvariantName)
        {
            RegisterDatabase(databaseId, providerInvariantName, null);
        }

        public static bool IsDatabaseRegistered(string databaseId)
        {
            lock (syncRoot)
            {
                return providers.ContainsKey(databaseId);
            }
        }

        public static IDbConnection CreateDbConnection(string databaseId)
        {
            Configure();
            var connection = providers[databaseId].CreateConnection();
            if (connnectionStrings.ContainsKey(databaseId))
            {
                connection.ConnectionString = connnectionStrings[databaseId];
            }
            return connection;
        }

        public static DbProviderFactory GetDbProviderFactory(string databaseId)
        {
            Configure();
            return providers.ContainsKey(databaseId) ? providers[databaseId] : null;
        }

        public static ConnectionStringSettings GetConnectionString(string databaseId)
        {
            Configure();
            return connnectionStrings.ContainsKey(databaseId) ? new ConnectionStringSettings(databaseId, connnectionStrings[databaseId], providers[databaseId].GetType().Name) : null;
        }

        public static ISqlDialect GetSqlDialect(string databaseId)
        {
            var provider = GetDbProviderFactory(databaseId);
            if (provider != null && dialects.ContainsKey(provider.GetType().FullName))
            {
                return dialects[provider.GetType().FullName];
            }
            return SqlDialect.Default;
        }


        public static void Configure()
        {
            if (!configured)
            {
                lock (syncRoot)
                {
                    if (!configured)
                    {
                        var factories = DbProviderFactories.GetFactoryClasses();
                        AppDomain.CurrentDomain.SetData("DataDirectory", AppDomain.CurrentDomain.BaseDirectory); //SQLite
                        foreach (ConnectionStringSettings cs in ConfigurationManager.ConnectionStrings)
                        {
                            var factory = factories.Rows.Find(cs.ProviderName);
                            if (factory == null)
                            {
                                throw new ConfigurationErrorsException("Db factory " + cs.ProviderName + " not found.");
                            }
                            RegisterDatabase(cs.Name, cs);
                        }
                        configured = true;
                    }
                }
            }
        }
    }
}