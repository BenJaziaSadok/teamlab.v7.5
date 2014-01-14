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
using System.Threading;
using ASC.Collections;
using ASC.Xmpp.Core;
using ASC.Xmpp.Core.protocol;
using log4net;

namespace ASC.Xmpp.Server.Services
{
    public class XmppServiceManager
    {
        private IServiceProvider serviceProvider;

        private IDictionary<Jid, IXmppService> services = new SynchronizedDictionary<Jid, IXmppService>();

        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private static ILog log = LogManager.GetLogger(typeof(XmppServiceManager));


        public XmppServiceManager(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null) throw new ArgumentNullException("serviceProvider");

            this.serviceProvider = serviceProvider;
        }

        public void RegisterService(IXmppService service)
        {
            if (service == null) throw new ArgumentNullException("service");
            try
            {
                locker.EnterWriteLock();
                services.Add(service.Jid, service);
            }
            finally
            {
                locker.ExitWriteLock();
            }

            log.DebugFormat("Register XMPP service '{0}' on '{1}'", service.Name, service.Jid);

            try
            {
                service.OnRegister(serviceProvider);
            }
            catch (Exception error)
            {
                log.ErrorFormat("Error on register service '{0}' and it has will unloaded. {1}", service.Name, error);
                UnregisterService(service.Jid);                
                throw;
            }
        }

        public void UnregisterService(Jid address)
        {
            if (address == null) throw new ArgumentNullException("address");
            IXmppService service = null;
            try
            {
                locker.EnterWriteLock();
                if (services.ContainsKey(address))
                {
                    service = services[address];
                    services.Remove(address);
                }
            }
            finally
            {
                locker.ExitWriteLock();
            }

            if (service != null)
            {
                log.DebugFormat("Unregister XMPP service '{0}' on '{1}'", service.Name, service.Jid);
                service.OnUnregister(serviceProvider);
            }
        }

        public ICollection<IXmppService> GetChildServices(IXmppService parentService)
        {
            return GetChildServices(parentService != null ? parentService.Jid : null);
        }

        public ICollection<IXmppService> GetChildServices(Jid parentAddress)
        {
            try
            {
                locker.EnterReadLock();

                var list = new List<IXmppService>();
                foreach (var s in services.Values)
                {
                    var parentJid = s.ParentService != null ? s.ParentService.Jid : null;
                    if (parentAddress == parentJid) list.Add(s);
                }
                return list;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public IXmppService GetService(Jid address)
        {
            if (address == null) return null;
            try
            {
                locker.EnterReadLock();
                return services.ContainsKey(address) ? services[address] : null;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
    }
}