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

#region usings

using System;
using System.Threading;
using System.Web;
using System.Web.Caching;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;

#endregion

namespace ASC.Api.Utils
{
    public class HttpContextLifetimeManager : LifetimeManager, IDisposable
    {
        private readonly Type _type;

        public HttpContextLifetimeManager(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveValue();
        }

        #endregion

        public override object GetValue()
        {
            return HttpContext.Current.Items[_type.AssemblyQualifiedName];
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Items.Remove(_type.AssemblyQualifiedName);
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Items[_type.AssemblyQualifiedName]
                = newValue;
        }
    }

    public class HttpContextLifetimeManager2 : LifetimeManager, IDisposable
    {
        private readonly HttpContextBase _context;
        private readonly string _type;

        public HttpContextLifetimeManager2() : this(new HttpContextWrapper(HttpContext.Current))
        {
        }

        public HttpContextLifetimeManager2(HttpContextBase context)
        {
            _context = context;
            _type = Guid.NewGuid().ToString();
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveValue();
        }

        #endregion

        public override object GetValue()
        {
            return _context.Items[_type];
        }

        public override void RemoveValue()
        {
            _context.Items.Remove(_type);
        }

        public override void SetValue(object newValue)
        {
            _context.Items[_type] = newValue;
        }
    }

    public class HttpSessionLifetimeManager : LifetimeManager, IDisposable
    {
        private readonly Type _type;

        public HttpSessionLifetimeManager(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveValue();
        }

        #endregion

        public override object GetValue()
        {
            return HttpContext.Current.Session[_type.AssemblyQualifiedName];
        }

        public override void RemoveValue()
        {
            HttpContext.Current.Session.Remove(_type.AssemblyQualifiedName);
        }

        public override void SetValue(object newValue)
        {
            HttpContext.Current.Session[_type.AssemblyQualifiedName]
                = newValue;
        }
    }

    public class HttpCacheLifetimeManager : LifetimeManager, IDisposable
    {
        private readonly TimeSpan _slidingExpiration;
        private readonly Type _type;

        public HttpCacheLifetimeManager(Type type, TimeSpan slidingExpiration)
        {
            if (type == null) throw new ArgumentNullException("type");
            _type = type;
            _slidingExpiration = slidingExpiration;
        }

        #region IDisposable Members

        public void Dispose()
        {
            RemoveValue();
        }

        #endregion

        public override object GetValue()
        {
            return HttpRuntime.Cache[_type.AssemblyQualifiedName];
        }

        public override void RemoveValue()
        {
            HttpRuntime.Cache.Remove(_type.AssemblyQualifiedName);
        }

        public override void SetValue(object newValue)
        {
            HttpRuntime.Cache.Insert(_type.AssemblyQualifiedName, newValue, null, Cache.NoAbsoluteExpiration,
                                     _slidingExpiration, CacheItemPriority.NotRemovable, null);
        }
    }

    public class NewInstanceLifetimeManager : LifetimeManager, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public override object GetValue()
        {
            return null;
        }

        public override void SetValue(object newValue)
        {
        }

        public override void RemoveValue()
        {
        }
    }

    class SingletonLifetimeManager2 : ContainerControlledLifetimeManager
    {
        protected override void SynchronizedSetValue(object newValue)
        {
            //base.GetValue();
            base.SynchronizedSetValue(newValue);
        }
    }

    public class SingletonLifetimeManager : LifetimeManager, IDisposable, IRequiresRecovery
    {
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        private object _value;

        protected virtual object SynchronizedGetValue()
        {
            return _value;
        }

        protected virtual void SynchronizedSetValue(object newValue)
        {
            _value = newValue;
        }

        public override object GetValue()
        {
            try
            {
                _lock.EnterReadLock();
                return SynchronizedGetValue();
            }
            finally 
            {
                _lock.ExitReadLock();
            }
        }

        public override void SetValue(object newValue)
        {
            try
            {
                _lock.EnterWriteLock();
                SynchronizedSetValue(newValue);
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public override void RemoveValue()
        {
            Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (_value == null)
                return;
            var disposable = _value as IDisposable;
            if (disposable != null)
                (disposable).Dispose();
            _value = null;
        }

        public void Recover()
        {
        }
    }
}