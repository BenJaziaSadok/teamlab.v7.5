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
    public class MailQueueConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty(Schema.CONFIGURATION_ELEMENT_NAME, IsRequired = true)]
        public MailQueueConfigurationElement QueueConfiguration
        {
            get { return (MailQueueConfigurationElement)base[Schema.CONFIGURATION_ELEMENT_NAME]; }
        }
    }
}