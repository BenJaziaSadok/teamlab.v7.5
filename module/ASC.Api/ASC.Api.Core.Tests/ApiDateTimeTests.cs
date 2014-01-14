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
using System;
using ASC.Specific;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Api.Core.Tests
{
    [TestClass]
    public class ApiDateTimeTests
    {
        [TestMethod]
        public void TestParsing()
        {
            const string parseTime = "2012-01-11T07:01:00.0000001Z";
            var apiDateTime1 = ApiDateTime.Parse(parseTime);
            var dateTime = (DateTime) apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind,utcTime.Kind);
            Assert.AreEqual(dateTime,utcTime);
            Assert.AreEqual(apiDateTime1.ToString(),parseTime);
        }

        [TestMethod]
        public void TestNull()
        {
            var apiDateTime = (ApiDateTime) null;
            Assert.IsNull(apiDateTime);
        }

        [TestMethod]
        public void TestLocal2()
        {

            var apiDateTime = new ApiDateTime(DateTime.Now,TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
            var stringv =apiDateTime.ToString();
        }


        [TestMethod]
        public void TestParsing2()
        {
            const string parseTime = "2012-01-31T20:00:00.0000000Z";
            var apiDateTime1 = ApiDateTime.Parse(parseTime);
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
            Assert.AreEqual(apiDateTime1.ToString(), parseTime);
        }

        [TestMethod]
        public void TestUtc()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now,TimeZoneInfo.Utc);
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }

        [TestMethod]
        public void TestLocal()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time"));
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }

        [TestMethod]
        public void Test00()
        {
            var apiDateTime1 = new ApiDateTime(DateTime.Now);
            var stringrep = apiDateTime1.ToString();
            var dateTime = (DateTime)apiDateTime1;
            var utcTime = apiDateTime1.UtcTime;
            Assert.AreEqual(dateTime.Kind, utcTime.Kind);
            Assert.AreEqual(dateTime, utcTime);
        }
    }
}
#endif