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

namespace ASC.Notify.Config
{
    class NotifyServiceCfgSectionHandler : ConfigurationSection
    {
        [ConfigurationProperty("connectionStringName", IsRequired = true)]
        public string ConnectionStringName
        {
            get { return (string)base["connectionStringName"]; }
        }

        [ConfigurationProperty("deleteSendedMessages", DefaultValue = false)]
        public bool DeleteSendedMessages
        {
            get { return (bool)base["deleteSendedMessages"]; }
        }

        [ConfigurationProperty("process")]
        public NotifyServiceCfgProcessElement Process
        {
            get { return (NotifyServiceCfgProcessElement)base["process"]; }
        }

        [ConfigurationProperty("senders")]
        [ConfigurationCollection(typeof(NotifyServiceCfgSendersCollection), AddItemName = "sender")]
        public NotifyServiceCfgSendersCollection Senders
        {
            get { return (NotifyServiceCfgSendersCollection)base["senders"]; }
        }

        [ConfigurationProperty("schedulers")]
        [ConfigurationCollection(typeof(NotifyServiceCfgSchedulersCollection), AddItemName = "scheduler")]
        public NotifyServiceCfgSchedulersCollection Schedulers
        {
            get { return (NotifyServiceCfgSchedulersCollection)base["schedulers"]; }
        }
    }
}
