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
using System.Linq;
using System.Threading;
using log4net;

namespace ASC.Mail.Autoreply
{
    internal class CooldownInspector
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(CooldownInspector));

        private readonly object _syncRoot = new object();
        private readonly Dictionary<Guid, List<DateTime>> _lastUsagesByUser = new Dictionary<Guid, List<DateTime>>();

        private readonly int _allowedRequests;
        private readonly TimeSpan _duringTime;
        private readonly TimeSpan _cooldownLength;

        private Timer _clearTimer;

        public CooldownInspector(CooldownConfigurationElement config)
        {
            _allowedRequests = config.AllowedRequests;
            _duringTime = config.DuringTimeInterval;
            _cooldownLength = config.Length;
        }

        public void Start()
        {
            if (!IsDisabled() && _clearTimer == null)
            {
                _clearTimer = new Timer(x => ClearExpiredRecords(), null, _cooldownLength, _cooldownLength);
            }
        }

        public void Stop()
        {
            if (_clearTimer != null)
            {
                _clearTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _clearTimer.Dispose();
                _clearTimer = null;
                _lastUsagesByUser.Clear();
            }
        }

        public TimeSpan GetCooldownRemainigTime(Guid userId)
        {
            if (IsDisabled()) return TimeSpan.Zero;

            lock (_syncRoot)
            {
                var lastUsages = GetLastUsages(userId);
                return lastUsages.Count >= _allowedRequests ? _cooldownLength - (DateTime.UtcNow - lastUsages.Max()) : TimeSpan.Zero;
            }
        }

        public void RegisterServiceUsage(Guid userId)
        {
            if (IsDisabled()) return;

            lock (_syncRoot)
            {
                var lastUsages = GetLastUsages(userId);
                lastUsages.Add(DateTime.UtcNow);
                _lastUsagesByUser[userId] = lastUsages;
            }
        }

        private void ClearExpiredRecords()
        {
            lock (_syncRoot)
            {
                _log.Debug("start clearing expired usage records");
                try
                {
                    foreach (var userId in _lastUsagesByUser.Keys.ToList())
                    {
                        _lastUsagesByUser[userId] = GetLastUsages(userId);
                        if (_lastUsagesByUser[userId].Count == 0)
                            _lastUsagesByUser.Remove(userId);
                    }
                }
                catch (Exception error)
                {
                    _log.Error("error while clearing expired usage records", error);
                }
            }
        }

        private List<DateTime> GetLastUsages(Guid userId)
        {
            if (!_lastUsagesByUser.ContainsKey(userId))
                return new List<DateTime>();

            return _lastUsagesByUser[userId]
                .Where(timestamp => timestamp > DateTime.UtcNow - _duringTime)
                .OrderByDescending(timestamp => timestamp)
                .Take(_allowedRequests)
                .ToList();
        }

        private bool IsDisabled()
        {
            return _allowedRequests <= 0 || _duringTime <= TimeSpan.Zero || _cooldownLength <= TimeSpan.Zero;
        }
    }
}
