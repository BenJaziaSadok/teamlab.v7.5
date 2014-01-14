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
using ASC.Collections;
using ASC.Xmpp.Server.Storage.Interface;

namespace ASC.Xmpp.Server.Storage
{
    public class StorageManager : IDisposable
    {
        private IDictionary<string, object> storages = new SynchronizedDictionary<string, object>();

        public IOfflineStore OfflineStorage
        {
            get { return GetStorage<IOfflineStore>("offline"); }
        }

        public IRosterStore RosterStorage
        {
            get { return GetStorage<IRosterStore>("roster"); }
        }

        public IVCardStore VCardStorage
        {
            get { return GetStorage<IVCardStore>("vcard"); }
        }

        public IPrivateStore PrivateStorage
        {
            get { return GetStorage<IPrivateStore>("private"); }
        }

        public IMucStore MucStorage
        {
            get { return GetStorage<IMucStore>("muc"); }
        }

        public IUserStore UserStorage
        {
            get { return GetStorage<IUserStore>("users"); }
        }

        public object this[string storageName]
        {
            get { return storages.ContainsKey(storageName) ? storages[storageName] : null; }
            set { storages[storageName] = value; }
        }

        public T GetStorage<T>(string storageName)
        {
            return (T)this[storageName];
        }

        public void SetStorage(string storageName, object storage)
        {
            this[storageName] = storage;
        }

        public void Dispose()
        {
            foreach (var s in storages.Values)
            {
                var disposable = s as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}