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
using ASC.Common.Threading.Workers;

namespace ASC.Mail.Aggregator.CollectionService
{
    public class MailWorkerQueue:WorkerQueue<MailQueueItem>
    {
        private readonly Collector _collector;

        public MailWorkerQueue(int workerCount, TimeSpan waitInterval, Collector collector) : base(workerCount, waitInterval)
        {
            _collector = collector;
        }

        public MailWorkerQueue(int workerCount, TimeSpan waitInterval, int errorCount, bool stopAfterFinsih, Collector collector)
            : base(workerCount, waitInterval, errorCount, stopAfterFinsih)
        {
            _collector = collector;
        }

        protected override WorkItem<MailQueueItem> Selector()
        {
            // The following block stops generating tasks if service is stopped.
            if (StopEvent.WaitOne(0))
            {
                return null;
            }

            MailQueueItem item = _collector.GetItem();
            return item != null ? new WorkItem<MailQueueItem>(item) : null;
        }

        protected override void PostComplete(WorkItem<MailQueueItem> item)
        {
            _collector.ItemCompleted(item.Item);
        }

        protected override void Error(WorkItem<MailQueueItem> item, Exception exception)
        {
            _collector.ItemError(item.Item, exception);
        }

        protected override bool QueueEmpty(bool fallAsleep)
        {
            return false;
        }

        internal void Start()
        {
            Start(ProcessItem, false);
        }

        private void ProcessItem(MailQueueItem item)
        {
            //Console.WriteLine("Start ProcessItem({0})", item.Account.Account);
            // The following code prevents current task from starting if service is stopped.
            if (StopEvent.WaitOne(0))
            {
                return;
            }

            item.Retrieve(_collector.ItemsPerSession, StopEvent);
        }
    }
}