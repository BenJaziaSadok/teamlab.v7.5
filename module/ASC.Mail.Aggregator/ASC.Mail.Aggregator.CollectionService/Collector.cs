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
using System.Web;
using System.Web.Caching;
using ASC.Mail.Aggregator.Client;
using NLog;
using System.Globalization;

namespace ASC.Mail.Aggregator.CollectionService
{
    public class Collector
    {
        private readonly MailBoxManager _manager;
        private readonly MailItemManager _itemManger;
        private readonly MailQueueSettings _settings;
        private readonly MailWorkerQueue _queue;
        private readonly Logger _log;
        private bool _noTasks;

        public Collector(MailBoxManager manager, MailQueueSettings settings)
        {
            _log = LogManager.GetLogger("Collector");
            _manager = manager;
            _settings = settings;
            _itemManger = new MailItemManager(_manager);
            _queue = new MailWorkerQueue(settings.ConcurrentThreadCount, settings.CheckInterval, this);

            _log.Info("MailWorkerQueue: ConcurrentThreadCount = {0} and CheckInterval = {1}", 
                settings.ConcurrentThreadCount, settings.CheckInterval);

            
        }

        public void SaveMessage(string user_id, int tenant_id, string email, string uidl)
        {
            var n_attempts = 0;
            do
            {
                try
                {
                    if (string.IsNullOrEmpty(user_id))
                        throw new Exception("user_id is empty");

                    if (tenant_id < 0)
                        throw new Exception("tenant_id must have non-negative value");

                    if (string.IsNullOrEmpty(email))
                        throw new Exception("email is empty");

                    if (string.IsNullOrEmpty(uidl))
                        throw new Exception("uidl is empty");

                    _log.Debug("user_id:\t'{0}'", user_id);
                    _log.Debug("tenant_id:\t'{0}'", tenant_id);
                    _log.Debug("email:\t\t'{0}'", email);
                    _log.Debug("uidl:\t\t'{0}'", uidl);
                    _log.Debug("Search for mailbox");
                    var mailbox = _manager.GetMailBox(tenant_id, user_id, new System.Net.Mail.MailAddress(email));
                    if (mailbox != null)
                    {
                        var s_message_path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format("message_{0}.eml", uidl));
                        _log.Debug("download path:\t'{0}'", s_message_path);
                        var loader = new MailQueueItem(mailbox);
                        loader.DownloadMessage(uidl, s_message_path);
                    }
                    else
                        _log.Info("Mailbox was not found!");

                    return;

                }
                catch (Exception ex)
                {
                    _log.Error("SaveMessage() Exception:\r\n{0}\r\n", ex.ToString());
                    _log.Debug("Attemp {0}/3", ++n_attempts);
                }

            } while (n_attempts < 4);
        }

        public int ItemsPerSession { 
            get { 
                return _settings.MaxMessagesPerSession; 
            } 
        }

        public void Start()
        {
            try
            {
                _log.Info("Collector.Start() -> _queue.IsStarted = {0}", _queue.IsStarted);
                if (!_queue.IsStarted)
                {
                    _log.Debug("Starting collector\r\n");
                    AggregatorLogger.Instance.Start();
                    _queue.Start();
                }
            }
            catch (Exception e)
            {
                _log.Fatal("Collector.Start() Exception: {0}", e.ToString());
                throw;
            }
        }

        public void Stop()
        {
            try
            {
                _log.Info("Collector.Stop() -> _queue.IsStarted = {0}", _queue.IsStarted);
                if (_queue.IsStarted)
                {
                    _log.Debug("Stopping collector");
                    _queue.Stop();
                    AggregatorLogger.Instance.Stop();
                }
            }
            catch (Exception e)
            {
                _log.Fatal("Collector.Stop() Exception: {0}", e.ToString());
                throw;
            }
        }

        private static readonly object SyncObject = new object();
        private static bool _isGetMailboxRunning = false;

        public MailQueueItem GetItem()
        {
            MailQueueItem result = null;
            try
            {
                while(true)
                {
                    if(!_noTasks) _log.Debug("Getting new Item...");

                    MailBox mbox = null;
                    var locked_in_this_thread = false;
                    lock (SyncObject)
                    {
                        if (!_isGetMailboxRunning)
                        {
                            _isGetMailboxRunning = true;
                            locked_in_this_thread = true;
                        }
                    }
                    try
                    {
                        if (locked_in_this_thread && _isGetMailboxRunning)
                            mbox = _manager.GetMailboxForProcessing(_settings.ActivityTimeout);
                    }
                    finally
                    {
                        if (locked_in_this_thread && _isGetMailboxRunning)
                            _isGetMailboxRunning = false;
                    }


                    if (mbox == null)
                    {
                        if (!_noTasks) _log.Debug("Nothing to do.");
                        _noTasks = true;
                        break;
                    }

                    var absence = false;
                    var type = HttpRuntime.Cache.Get(mbox.TenantId.ToString(CultureInfo.InvariantCulture));
                    if (type == null)
                    {
                        absence = true;
                        try
                        {
                            type = _manager.GetTariffType(mbox.TenantId);
                        }
                        catch (Exception e)
                        {
                            _log.Error("Collector.GetItem() -> GetTariffType Exception: {0}", e.ToString());
                            type = MailBoxManager.TariffType.Active;
                        }
                    }
                    else if (mbox.Active && (MailBoxManager.TariffType) type != MailBoxManager.TariffType.Active)
                    {
                        HttpRuntime.Cache.Remove(mbox.TenantId.ToString(CultureInfo.InvariantCulture));
                        absence = true;
                        type = MailBoxManager.TariffType.Active;
                    }

                    TimeSpan delay;
                    switch ((MailBoxManager.TariffType) type)
                    {
                        case MailBoxManager.TariffType.LongDead:
                            delay = _settings.LongDeadAccountDelay;
                            break;
                        case MailBoxManager.TariffType.Overdue:
                            delay = _settings.OverdueAccountDelay;
                            break;
                        default:
                            delay = TimeSpan.FromDays(1);
                            break;
                    }
                    if (absence)
                        HttpRuntime.Cache.Insert(mbox.TenantId.ToString(CultureInfo.InvariantCulture), type, null,
                                                 DateTime.UtcNow.Add(delay), Cache.NoSlidingExpiration);

                    _noTasks = false;
                    _log.Info("MailboxId: {0} is being processed. EMail: '{1}'  User: '{2}' TenanntId: {3} ",
                              mbox.MailBoxId, 
                              mbox.EMail.Address, 
                              mbox.UserId, 
                              mbox.TenantId);

                    if ((MailBoxManager.TariffType) type == MailBoxManager.TariffType.LongDead ||
                        (MailBoxManager.TariffType) type == MailBoxManager.TariffType.Overdue)
                    {
                        _log.Info("Tenant {0} is not paid. Stop processing MailboxId: {1} EMail: '{2}'.", 
                            mbox.TenantId, 
                            mbox.MailBoxId, 
                            mbox.EMail.Address);

                        _manager.SetNextLoginDelayedFor(mbox, delay);
                    }
                    else
                    {
                        _log.Debug("CreateItemForAccount()...");
                        result = MailItemQueueFactory.CreateItemForAccount(mbox, _itemManger);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                _log.Error("Collector.GetItem() Exception: {0}", e.ToString());
            }

            return result;
        }

        public void ItemCompleted(MailQueueItem item)
        {
            try
            {
                _log.Info("MailboxProcessingCompleted(MailBoxId: {0} Email: '{1}')\r\n", 
                    item.Account.MailBoxId, 
                    item.Account.EMail.Address);

                _manager.MailboxProcessingCompleted(item.Account);
            }
            catch (Exception e)
            {
                _log.Fatal("Collector.ItemCompleted(MailBoxId: {0} Email: '{1}') Exception: {2}\r\n", 
                    item.Account.MailBoxId, 
                    item.Account.EMail.Address, 
                    e.ToString());
            }
        }

        public void ItemError(MailQueueItem item, Exception exception)
        {
            try
            {
                _log.Info("MailboxProcessingError(MailBoxId: {0} Email: '{1}' Tenant: {2} User: '{3}' Exception:\r\n{4})\r\n",
                    item.Account.MailBoxId, 
                    item.Account.EMail.Address, 
                    item.Account.TenantId, 
                    item.Account.UserId, 
                    exception.ToString());

                _manager.MailboxProcessingError(item.Account, exception);
            }
            catch (Exception e)
            {
                _log.Fatal("Collector.ItemError(MailBoxId: {0} Email: '{1}' Tenant: {2} User: '{3}' ExceptionMessage: {4}) Exception:\r\n{5}\r\n",
                    item.Account.MailBoxId, 
                    item.Account.EMail.Address, 
                    item.Account.TenantId, 
                    item.Account.UserId, 
                    exception.ToString(), 
                    e.ToString());
            }
        }
    }
}