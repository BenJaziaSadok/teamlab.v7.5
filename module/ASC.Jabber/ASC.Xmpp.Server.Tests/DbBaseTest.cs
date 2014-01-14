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

using System.Collections.Generic;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
    [TestFixture]
    public class DbBaseTest
    {
        public IDictionary<string, string> GetConfiguration()
        {
            var props = new Dictionary<string, string>();
            props["connectionString"] = "Data Source=|DataDirectory|\\test.db3;Version=3";
            //props["connectionStringName"] = "mysql";
            return props;
        }
    }
}
