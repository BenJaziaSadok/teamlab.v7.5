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
using System.Configuration;
using System.Reflection;
using ASC.Core.Notify.Senders;

namespace ASC.Notify.Config
{
    static class NotifyServiceCfg
    {
        public static ConnectionStringSettings ConnectionString
        {
            get;
            private set;
        }

        public static IDictionary<string, INotifySender> Senders
        {
            get;
            private set;
        }

        public static int MaxThreads
        {
            get;
            private set;
        }

        public static int BufferSize
        {
            get;
            private set;
        }

        public static int MaxAttempts
        {
            get;
            private set;
        }

        public static TimeSpan AttemptsInterval
        {
            get;
            private set;
        }

        public static IDictionary<string, MethodInfo> Schedulers
        {
            get;
            private set;
        }

        public static string ServerRoot
        {
            get;
            private set;
        }

        public static bool DeleteSendedMessages
        {
            get;
            private set;
        }

        static NotifyServiceCfg()
        {
            var section = ConfigurationManager.GetSection("notify") as NotifyServiceCfgSectionHandler;
            if (section == null)
            {
                throw new ConfigurationErrorsException("Section notify not found.");
            }

            ConnectionString = ConfigurationManager.ConnectionStrings[section.ConnectionStringName];
            Senders = new Dictionary<string, INotifySender>();
            foreach (NotifyServiceCfgSenderElement element in section.Senders)
            {
                var sender = (INotifySender)Activator.CreateInstance(Type.GetType(element.Type, true));
                sender.Init(element.Properties);
                Senders.Add(element.Name, sender);
            }
            MaxThreads = section.Process.MaxThreads == 0 ? Environment.ProcessorCount : section.Process.MaxThreads;
            BufferSize = section.Process.BufferSize;
            MaxAttempts = section.Process.MaxAttempts;
            AttemptsInterval = section.Process.AttemptsInterval;

            Schedulers = new Dictionary<string, MethodInfo>();
            foreach (NotifyServiceCfgSchedulerElement element in section.Schedulers)
            {
                var typeName = element.Register.Substring(0, element.Register.IndexOf(','));
                var assemblyName = element.Register.Substring(element.Register.IndexOf(','));
                var type = Type.GetType(typeName.Substring(0, typeName.LastIndexOf('.')) + assemblyName, true);
                Schedulers[element.Name] = type.GetMethod(typeName.Substring(typeName.LastIndexOf('.') + 1), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            }
            ServerRoot = section.Schedulers.ServerRoot;
            DeleteSendedMessages = section.DeleteSendedMessages;
        }
    }
}
