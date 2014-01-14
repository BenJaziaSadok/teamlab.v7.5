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

using System.Configuration;

namespace ASC.Mail.Aggregator.Client.Configuration
{
    public class MailQueueConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_CHECK_INTERVAL, IsRequired = false, IsKey = false, DefaultValue = 1)]
        public int CheckInterval
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_CHECK_INTERVAL]; }
            set { this[Schema.CONFIGURATION_PROPERTY_CHECK_INTERVAL] = value; }
        }

        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_CONCURENTTHREADS, IsRequired = false, IsKey = false, DefaultValue = 5)]
        public int Threads
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_CONCURENTTHREADS]; }
            set { this[Schema.CONFIGURATION_PROPERTY_CONCURENTTHREADS] = value; }
        }

        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_NEWMESSAGESPERSESSION, IsRequired = false, IsKey = false, DefaultValue = 200)]
        public int MaxNewMessages
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_NEWMESSAGESPERSESSION]; }
            set { this[Schema.CONFIGURATION_PROPERTY_NEWMESSAGESPERSESSION] = value; }
        }

        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_ACTIVITY_TIMEOUT, IsRequired = false, IsKey = false, DefaultValue = 90)]
        public int ActivityTimeout
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_ACTIVITY_TIMEOUT]; }
            set { this[Schema.CONFIGURATION_PROPERTY_ACTIVITY_TIMEOUT] = value; }
        }

        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_OVERDUEACCOUNTDELAY, IsRequired = false, IsKey = false, DefaultValue = 600)]
        public int OverdueAccountDelay
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_OVERDUEACCOUNTDELAY]; }
            set { this[Schema.CONFIGURATION_PROPERTY_OVERDUEACCOUNTDELAY] = value; }
        }

        [ConfigurationProperty(Schema.CONFIGURATION_PROPERTY_LONGDEADACCOUNTDELAY, IsRequired = false, IsKey = false, DefaultValue = 3600)]
        public int LongDeadAccountDelay
        {
            get { return (int)this[Schema.CONFIGURATION_PROPERTY_LONGDEADACCOUNTDELAY]; }
            set { this[Schema.CONFIGURATION_PROPERTY_LONGDEADACCOUNTDELAY] = value; }
        }
    }
}