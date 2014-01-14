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
using ASC.Common.Data.Sql;

namespace ASC.Mail.Aggregator
{
    public partial class MailBoxManager
    {
        // ReSharper disable InconsistentNaming
        private const string MAIL_AGGREGATORS = "mail_aggregators";
        private const string MAIL_LOG = "mail_log";
        // ReSharper restore InconsistentNaming

        private struct MailAggregatorFields
        {
            public const string id = "id";
            public const string ip = "ip";
            public const string start_work_time = "start_work";
            public const string finish_work_time = "end_work";
        }

        private struct MailLogFields
        {
            public const string id = "id";
            public const string id_aggregator = "id_aggregator";
            public const string id_thread = "id_thread";
            public const string id_mailbox = "id_mailbox";
            public const string proccessing_start_time = "processing_start_time";
            public const string proccessing_end_time = "processing_end_time";
            public const string proccessed_mails_count = "processed_mails_count";
        }

        public int RegisterAggregator(string aggregator_ip)
        {
            using (var db = GetDb())
            {
                return db.ExecuteScalar<int>(new SqlInsert(MAIL_AGGREGATORS)
                                   .InColumnValue(MailAggregatorFields.id, 0)
                                   .InColumnValue(MailAggregatorFields.ip, aggregator_ip)
                                   .InColumnValue(MailAggregatorFields.start_work_time, DateTime.Now)
                                   .Identity(0, 0, true)
                                   );
            }
        }

        public void UnregisterAggregator(int aggregator_id)
        {
            using (var db = GetDb())
            {
                db.ExecuteList(new SqlUpdate(MAIL_AGGREGATORS)
                                   .Set(MailAggregatorFields.finish_work_time, DateTime.Now)
                                   .Where(MailAggregatorFields.id, aggregator_id));
            }
        }

        public long RegisterMailBoxProccessing(int mailbox_id, int thread_id, int aggregator_id)
        {
            using (var db = GetDb())
            {
                return db.ExecuteScalar<long>(new SqlInsert(MAIL_LOG)
                                   .InColumnValue(MailLogFields.id, 0)
                                   .InColumnValue(MailLogFields.id_aggregator, aggregator_id)
                                   .InColumnValue(MailLogFields.id_mailbox, mailbox_id)
                                   .InColumnValue(MailLogFields.id_thread, thread_id)
                                   .InColumnValue(MailLogFields.proccessing_start_time, DateTime.Now)
                                   .Identity(0, 0, true)
                                   );
            }
        }

        public void RegisterFinishMailBoxProccessing(long record_id, int? proccessed_message_count)
        {
            using (var db = GetDb())
            {
                db.ExecuteList(new SqlUpdate(MAIL_LOG)
                                   .Set(MailLogFields.proccessing_end_time, DateTime.Now)
                                   .Set(MailLogFields.proccessed_mails_count, proccessed_message_count)
                                   .Where(MailLogFields.id, record_id));
            }
        }
    }
}
