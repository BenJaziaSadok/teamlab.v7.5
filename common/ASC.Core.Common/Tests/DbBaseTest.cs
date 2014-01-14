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

#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using log4net;
    using log4net.Config;
    using log4net.Appender;
    using log4net.Layout;

    public class DbBaseTest<TDbService>
    {
        protected TDbService Service
        {
            get;
            private set;
        }

        protected int Tenant
        {
            get;
            private set;
        }


        protected DbBaseTest()
        {
            Service = (TDbService)Activator.CreateInstance(typeof(TDbService), ConfigurationManager.ConnectionStrings["core"]);
            Tenant = 1024;

            var pattern = "%message (%property{duration} ms)     %property{sql}    %property{sqlParams}%newline";
            BasicConfigurator.Configure(new DebugAppender { Layout = new PatternLayout(pattern) });
        }
    }
}
#endif
