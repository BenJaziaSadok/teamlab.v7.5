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

using System.Diagnostics;
using ASC.Xmpp.Common;
using NUnit.Framework;

namespace ASC.Xmpp.Server.Tests
{
	[TestFixture]
	public class JabberServiceClientTest
	{
		private JabberServiceClient jabberClient;

		[TestFixtureSetUp]
		public void SetUp()
		{
			jabberClient = new JabberServiceClient();
		}

		[Test]
		public void GetNewMessagesCountTest()
		{
			Assert.AreEqual(0, jabberClient.GetNewMessagesCount("nikolay.ivanov",0));
		}

		[Test]
		public void GetAuthTokenTest()
		{
			var token = jabberClient.GetAuthToken(0);
			Debug.WriteLine(string.Format("Success get authentication token from jabber service. Token is {0}", token));
		}
	}
}
