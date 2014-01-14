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

using System.ServiceModel;
using ASC.Common.Module;
using ASC.Xmpp.Server;
using ASC.Xmpp.Server.Configuration;

namespace ASC.Xmpp.Host
{
    public class XmppServerLauncher : IServiceController
    {
        private ServiceHost host;
        private XmppServer xmppServer;


        public void Start()
        {
            xmppServer = new XmppServer();
            JabberConfiguration.Configure(xmppServer);
            xmppServer.StartListen();

            var jabberService = new JabberService(xmppServer);

            host = new ServiceHost(jabberService);
            host.Open();
        }

        public void Stop()
        {
            if (xmppServer != null)
            {
                xmppServer.StopListen();
                xmppServer.Dispose();
                xmppServer = null;
            }
            if (host != null)
            {
                host.Close();
                host = null;
            }
        }
    }
}
