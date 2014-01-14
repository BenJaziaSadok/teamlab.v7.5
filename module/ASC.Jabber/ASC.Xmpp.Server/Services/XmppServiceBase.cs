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
using System.Collections.Generic;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using ASC.Xmpp.Core.protocol.iq.disco;
using ASC.Xmpp.Core.protocol.iq.vcard;
using ASC.Xmpp.Server.Handler;

namespace ASC.Xmpp.Server.Services
{
    public class XmppServiceBase : IXmppService
    {
        protected IList<IXmppHandler> Handlers
        {
            get;
            private set;
        }

        protected bool Registered
        {
            get;
            private set;
        }

        public Jid Jid
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public IXmppService ParentService
        {
            get;
            set;
        }

        public DiscoInfo DiscoInfo
        {
            get;
            private set;
        }

        public virtual DiscoItem DiscoItem
        {
            get { return new DiscoItem() { Name = Name, Jid = Jid }; }
        }

        public virtual Vcard Vcard
        {
            get { return new Vcard() { Fullname = Name }; }
        }

        public XmppServiceBase()
        {
            Handlers = new List<IXmppHandler>();
            Registered = false;
            DiscoInfo = new DiscoInfo();
        }

        public XmppServiceBase(IXmppService parent)
            : this()
        {
            ParentService = parent;
        }

        public virtual void Configure(IDictionary<string, string> properties)
        {

        }

        public void OnRegister(IServiceProvider serviceProvider)
        {
            var handlerManager = (XmppHandlerManager)serviceProvider.GetService(typeof(XmppHandlerManager));
            var serviceManager = (XmppServiceManager)serviceProvider.GetService(typeof(XmppServiceManager));

            foreach (var h in Handlers) handlerManager.AddXmppHandler(Jid, h);
            Registered = true;
            OnRegisterCore(handlerManager, serviceManager, serviceProvider);

            DiscoveryFearures(Handlers);
        }

        public void OnUnregister(IServiceProvider serviceProvider)
        {
            var handlerManager = (XmppHandlerManager)serviceProvider.GetService(typeof(XmppHandlerManager));
            var serviceManager = (XmppServiceManager)serviceProvider.GetService(typeof(XmppServiceManager));

            foreach (var h in Handlers) handlerManager.RemoveXmppHandler(h);
            Registered = false;
            OnUnregisterCore(handlerManager, serviceManager, serviceProvider);
        }

        protected virtual void OnRegisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {

        }

        protected virtual void OnUnregisterCore(XmppHandlerManager handlerManager, XmppServiceManager serviceManager, IServiceProvider serviceProvider)
        {

        }


        private void DiscoveryFearures(IList<IXmppHandler> handlers)
        {
            foreach (var handler in handlers)
            {
                foreach (XmppHandlerAttribute attribute in handler.GetType().GetCustomAttributes(typeof(XmppHandlerAttribute), true))
                {
                    var nameSpace = ElementFactory.GetElementNamespace(attribute.XmppElementType);
                    if (!string.IsNullOrEmpty(nameSpace) && !DiscoInfo.HasFeature(nameSpace))
                    {
                        DiscoInfo.AddFeature(new DiscoFeature(nameSpace));
                    }
                }
            }
        }
    }
}