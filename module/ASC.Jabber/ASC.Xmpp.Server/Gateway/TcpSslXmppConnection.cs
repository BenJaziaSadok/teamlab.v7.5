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
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace ASC.Xmpp.Server.Gateway
{
	class TcpSslXmppConnection : TcpXmppConnection
	{
        public TcpSslXmppConnection(Socket socket, long maxPacket, X509Certificate cert)
			: base(socket, maxPacket)
		{
			sendStream = recieveStream = new SslStream(recieveStream, false);
			((SslStream)recieveStream).AuthenticateAsServer(cert, false, SslProtocols.Ssl3, false);
		}


	}
}