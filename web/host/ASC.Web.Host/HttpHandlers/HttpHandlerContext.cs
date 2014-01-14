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

using System.Security.Principal;
using ASC.Web.Host.HttpRequestProcessor;

namespace ASC.Web.Host.HttpHandlers
{
	class HttpHandlerContext
	{
		public Server Server
		{
			get;
			private set;
		}

		public HttpRequestProcessor.Host Host
		{
			get;
			private set;
		}

		public Connection Connection
		{
			get;
			private set;
		}

		public IIdentity Identity
		{
			get;
			private set;
		}

		public HttpHandlerContext(Server server, HttpRequestProcessor.Host host, Connection connection, IIdentity identity)
		{
			Server = server;
			Host = host;
			Connection = connection;
			Identity = identity;
		}
	}
}