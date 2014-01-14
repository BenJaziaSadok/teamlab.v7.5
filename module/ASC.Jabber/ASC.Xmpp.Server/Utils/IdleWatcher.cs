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
using log4net;

namespace ASC.Xmpp.Server.Utils
{
    static class IdleWatcher
    {
        private static readonly object syncRoot;

        private static readonly IDictionary<object, IdleItem> items;

        private static readonly TimeSpan timerPeriod;

        private static readonly Timer timer;

        private static bool timerStoped;

        private static readonly ILog log = LogManager.GetLogger(typeof(IdleWatcher));


        static IdleWatcher()
        {
            syncRoot = new object();
            items = new SynchronizedDictionary<object, IdleItem>();
            timerPeriod = TimeSpan.FromSeconds(1.984363f);
            timer = new Timer(TimerCallback, null, Timeout.Infinite, 0);
            timerStoped = true;
        }

        public static void StartWatch(object idleObject, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler)
        {
            StartWatch(idleObject, timeout, handler, null);
        }

        public static void StartWatch(object idleObject, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler, object data)
        {
            if (idleObject == null) throw new ArgumentNullException("idleObject");
            if (handler == null) throw new ArgumentNullException("handler");
            if (timeout == TimeSpan.Zero) throw new ArgumentOutOfRangeException("timeout");

            lock (syncRoot)
            {
                if (items.ContainsKey(idleObject))
                {
                    log.WarnFormat("An item with the same key ({0}) has already been added.", idleObject);
                }
                items[idleObject] = new IdleItem(idleObject, timeout, handler, data);
                if (timerStoped)
                {
                    timer.Change(timerPeriod, timerPeriod);
                    timerStoped = false;
                    log.DebugFormat("Timer started.");
                }
            }
            log.DebugFormat("Start watch idle object: {0}, timeout: {1}", idleObject, timeout);
        }

        public static void UpdateTimeout(object idleObject)
        {
            UpdateTimeout(idleObject, TimeSpan.Zero);
        }

        public static void UpdateTimeout(object idleObject, TimeSpan timeout)
        {
            if (idleObject == null) throw new ArgumentNullException("idleObject");
            lock (syncRoot)
            {
                if (items.ContainsKey(idleObject)) items[idleObject].UpdateTimeout(timeout);
            }
            log.DebugFormat("Update timeout idle object: {0}, timeout: {1}", idleObject, timeout);
        }

        public static bool StopWatch(object idleObject)
        {
            bool result = false;
            if (idleObject != null)
            {
                lock (syncRoot)
                {
                    try
                    {
                        result = items.Remove(idleObject);
                        StopTimerIfNeeded();
                    }
                    catch (Exception ex)
                    {
                        log.DebugFormat("Error stop watch idle object: {0}, ex: {1}", idleObject, ex);
                    }
                }
            }
            if (result) log.DebugFormat("Stop watch idle object: {0}", idleObject);
            else log.DebugFormat("Stop watch idle object: {0} - idle object not found.", idleObject);

            return result;
        }

        private static void TimerCallback(object state)
        {
            try
            {
                lock (syncRoot)
                {
                    foreach (var item in new Dictionary<object, IdleItem>(items))
                    {
                        if (!item.Value.IsExpired()) continue;

                        log.DebugFormat("Find idle object: {0}, invoke handler.", item.Key);
                        item.Value.InvokeHandler();
                        items.Remove(item.Key);
                    }
                    StopTimerIfNeeded();
                }
            }
            catch (Exception err)
            {
                log.Error(err);
            }
        }

        private static void StopTimerIfNeeded()
        {
            if (timerStoped || items.Count != 0) return;

            timer.Change(Timeout.Infinite, 0);
            timerStoped = true;
            log.DebugFormat("Timer stopped.");
        }


        private class IdleItem
        {
            private DateTime created;

            private EventHandler<TimeoutEventArgs> handler;

            private TimeSpan timeout;

            private object obj;

            private object data;

            public IdleItem(object obj, TimeSpan timeout, EventHandler<TimeoutEventArgs> handler, object data)
            {
                this.obj = obj;
                this.data = data;
                this.handler = handler;
                UpdateTimeout(timeout);
            }

            public void UpdateTimeout(TimeSpan timeout)
            {
                created = DateTime.UtcNow;
                if (timeout != TimeSpan.Zero) this.timeout = timeout.Duration();
            }

            public bool IsExpired()
            {
                return timeout < (DateTime.UtcNow - created);
            }

            public void InvokeHandler()
            {
                try
                {
                    handler.BeginInvoke(this, new TimeoutEventArgs(obj, data), null, null);
                }
                catch { }
            }
        }
    }

    class TimeoutEventArgs : EventArgs
    {
        public object IdleObject
        {
            get;
            private set;
        }

        public object Data
        {
            get;
            private set;
        }

        public TimeoutEventArgs(object obj, object data)
        {
            IdleObject = obj;
            Data = data;
        }
    }
}