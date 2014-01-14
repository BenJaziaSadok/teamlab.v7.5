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
using ASC.Geolocation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Common.Tests.Geolocation
{
    [TestClass]
    public class GeolocationTest
    {
        [TestMethod]
        public void GetIPGeolocationTest()
        {
            var helper = new GeolocationHelper("db");
            var info = helper.GetIPGeolocation("62.213.10.13");
            Assert.AreEqual("Nizhny Novgorod", info.City);
            Assert.AreEqual("062.213.011.127", info.IPEnd);
            Assert.AreEqual("062.213.008.240", info.IPStart);
            Assert.AreEqual("RU", info.Key);
            Assert.AreEqual("Europe/Moscow", info.TimezoneName);
            Assert.AreEqual(4d, info.TimezoneOffset);

            info = helper.GetIPGeolocation("");
            Assert.AreEqual(IPGeolocationInfo.Default.City, info.City);
            Assert.AreEqual(IPGeolocationInfo.Default.IPEnd, info.IPEnd);
            Assert.AreEqual(IPGeolocationInfo.Default.IPStart, info.IPStart);
            Assert.AreEqual(IPGeolocationInfo.Default.Key, info.Key);
            Assert.AreEqual(IPGeolocationInfo.Default.TimezoneName, info.TimezoneName);
            Assert.AreEqual(IPGeolocationInfo.Default.TimezoneOffset, info.TimezoneOffset);
        }
    }
}
#endif