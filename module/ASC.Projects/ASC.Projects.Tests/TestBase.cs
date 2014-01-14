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
using System.Configuration;
using ASC.Common.Data;
using log4net;
using log4net.Config;
using NUnit.Framework;

namespace ASC.Projects.Tests
{
    [TestFixture]
    public class TestBase : IDisposable
    {
        protected static string DbId = "test";

        protected DbManager Db
        {
            get;
            private set;
        }

        protected ILog Log
        {
            get;
            private set;
        }

        protected TestBase()
        {
            if (!DbRegistry.IsDatabaseRegistered(DbId))
            {
                DbRegistry.RegisterDatabase(DbId, ConfigurationManager.ConnectionStrings[DbId]);
            }
            Db = new DbManager(DbId);

            XmlConfigurator.Configure();
            Log = LogManager.GetLogger("ASC.Projects.Tests");
        }

        [TestFixtureTearDown]
        public void Dispose()
        {
            Db.Dispose();
        }
    }
}
