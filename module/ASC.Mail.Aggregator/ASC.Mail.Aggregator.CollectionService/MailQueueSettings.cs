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
using System.Configuration;
using ASC.Mail.Aggregator.Client.Configuration;

namespace ASC.Mail.Aggregator.Client
{
    public class MailQueueSettings
    {
        public int MaxMessagesPerSession { get; set; }
        public int ConcurrentThreadCount { get; set; }
        public TimeSpan CheckInterval { get; set; }
        public TimeSpan ActivityTimeout { get; set; }
        public TimeSpan OverdueAccountDelay { get; set; }
        public TimeSpan LongDeadAccountDelay { get; set; }

        public static readonly MailQueueSettings Default = new MailQueueSettings
                                                               {
                                                                   MaxMessagesPerSession = 200,
                                                                   CheckInterval = TimeSpan.FromSeconds(1),
                                                                   ConcurrentThreadCount = 5,
                                                                   ActivityTimeout = TimeSpan.FromSeconds(90),
                                                                   OverdueAccountDelay = TimeSpan.FromSeconds(600),
                                                                   LongDeadAccountDelay = TimeSpan.FromSeconds(3600)
                                                               };

        public static MailQueueSettings FromConfig
        {
            get
            {
                var configured = MailQueueSettings.Default;
                try
                {
                    var section = (MailQueueConfigurationSection)ConfigurationManager.GetSection(Schema.SECTION_NAME);
                    configured.CheckInterval = TimeSpan.FromSeconds(section.QueueConfiguration.CheckInterval);
                    configured.ConcurrentThreadCount = section.QueueConfiguration.Threads;
                    configured.MaxMessagesPerSession = section.QueueConfiguration.MaxNewMessages;
                    configured.ActivityTimeout = TimeSpan.FromSeconds(section.QueueConfiguration.ActivityTimeout);
                    configured.OverdueAccountDelay = TimeSpan.FromSeconds(section.QueueConfiguration.OverdueAccountDelay);
                    configured.LongDeadAccountDelay = TimeSpan.FromSeconds(section.QueueConfiguration.LongDeadAccountDelay);
                }
                catch (Exception)
                {
                    
                }
                return configured;
            }
        }
    }
}