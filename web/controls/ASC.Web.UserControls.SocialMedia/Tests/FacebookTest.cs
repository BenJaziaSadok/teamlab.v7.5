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
using ASC.SocialMedia.Facebook;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.SocialMedia.Tests
{
    [TestClass]
    public class FacebookTest
    {
        [TestMethod]
        public void GetUrlOfUserImageTest()
        {
            var ai = new FacebookApiInfo { AccessToken = "186245251433148|5fecd56abddd9eb63b506530.1-100002072952328|akD66RBlkeedQmhy50T9V_XCTYs" };
            var provider = new FacebookDataProvider(ai);
            var url = provider.GetUrlOfUserImage("kamorin.roman", FacebookDataProvider.ImageSize.Original);
        }
    }
}
#endif