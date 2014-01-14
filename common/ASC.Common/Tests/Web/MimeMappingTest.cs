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
using ASC.Common.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ASC.Common.Tests.Web {

	[TestClass]
	public class MimeMappingTest {

		[TestMethod]
		public void GetExtentionTest() {
			var ext = MimeMapping.GetExtention("application/x-zip-compressed");
			Assert.AreEqual(".zip", ext);

			ext = MimeMapping.GetExtention("Application/x-zip-Compressed");
			Assert.AreEqual(".zip", ext);

			ext = MimeMapping.GetExtention("Application/ZIP");
			Assert.AreEqual(".zip", ext);
		}
	}
}
#endif