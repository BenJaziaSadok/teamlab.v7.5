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
using ASC.Xmpp.Server.Authorization;
using ASC.Xmpp.Server.Gateway;
using ASC.Xmpp.Server.Session;
using ASC.Xmpp.Server.Storage;
using ASC.Xmpp.Server.Users;

namespace ASC.Xmpp.Server.Handler
{
    public class XmppHandlerContext
    {
        public IServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public IXmppSender Sender
        {
            get { return (IXmppSender)ServiceProvider.GetService(typeof(IXmppSender)); }
        }

        public UserManager UserManager
        {
            get { return (UserManager)ServiceProvider.GetService(typeof(UserManager)); }
        }

        public XmppSessionManager SessionManager
        {
            get { return (XmppSessionManager)ServiceProvider.GetService(typeof(XmppSessionManager)); }
        }

        public StorageManager StorageManager
        {
            get { return (StorageManager)ServiceProvider.GetService(typeof(StorageManager)); }
        }

        public AuthManager AuthManager
        {
            get { return (AuthManager)ServiceProvider.GetService(typeof(AuthManager)); }
        }

        public XmppHandlerContext(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            ServiceProvider = serviceProvider;
        }
    }
}
