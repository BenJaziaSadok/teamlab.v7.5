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
using System.Text;

namespace ASC.Mail.Aggregator
{
    public class AggregatorLogger
    {
        private static readonly  Object _syncObject = new object();
        private static volatile AggregatorLogger _instance;
        private int _currentAggregatorId = -1;
        private MailBoxManager _mgr;
        private string _aggregator_ip = "";

        private AggregatorLogger(){}

        public static AggregatorLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new AggregatorLogger();
                        }
                    }
                }
                return _instance;
            }
        }

        public bool IsInitialized
        {
            get { return _currentAggregatorId > 0; }
        }

        public void Initialize(MailBoxManager mgr, string aggregator_ip)
        {
            _mgr = mgr;
            _aggregator_ip = aggregator_ip;
        }

        public void Start()
        {
            if (!IsInitialized)
            {
                _currentAggregatorId = _mgr.RegisterAggregator(_aggregator_ip);
            }
        }

        public void Stop()
        {
            if (IsInitialized)
            {
                _mgr.UnregisterAggregator(_currentAggregatorId);
                _currentAggregatorId = -1;
            }
        }

        public long MailBoxProccessingStarts(int mailbox_id, int thread_id)
        {
            if (IsInitialized)
            {
                return _mgr.RegisterMailBoxProccessing(mailbox_id, thread_id, _currentAggregatorId);
            }
            else
            {
                throw new Exception("Call Start() method before logging anything");
            }
        }

        public void MailBoxProccessingEnds(long record_id, int? proccessed_message_count)
        {
            if (IsInitialized)
            {
                _mgr.RegisterFinishMailBoxProccessing(record_id, proccessed_message_count);
            }
        }
    }
}
