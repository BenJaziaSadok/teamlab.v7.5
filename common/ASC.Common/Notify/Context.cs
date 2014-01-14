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
using ASC.Notify.Channels;
using ASC.Notify.Engine;
using ASC.Notify.Model;
using ASC.Notify.Sinks;

namespace ASC.Notify
{
    public sealed class Context : INotifyRegistry
    {
        public const string SYS_RECIPIENT_ID = "_#" + _SYS_RECIPIENT_ID + "#_";
        internal const string _SYS_RECIPIENT_ID = "SYS_RECIPIENT_ID";
        internal const string _SYS_RECIPIENT_NAME = "SYS_RECIPIENT_NAME";
        internal const string _SYS_RECIPIENT_ADDRESS = "SYS_RECIPIENT_ADDRESS";

        private readonly Dictionary<string, ISenderChannel> channels = new Dictionary<string, ISenderChannel>(2);


        public NotifyEngine NotifyEngine
        {
            get;
            private set;
        }

        public INotifyRegistry NotifyService
        {
            get { return this; }
        }

        public DispatchEngine DispatchEngine
        {
            get;
            private set;
        }


        public event Action<Context, INotifyClient> NotifyClientRegistration;


        public Context()
        {
            NotifyEngine = new NotifyEngine(this);
            DispatchEngine = new DispatchEngine(this);
        }


        void INotifyRegistry.RegisterSender(string senderName, ISink senderSink)
        {
            lock (channels)
            {
                channels[senderName] = new SenderChannel(this, senderName, null, senderSink);
            }
        }

        void INotifyRegistry.UnregisterSender(string senderName)
        {
            lock (channels)
            {
                channels.Remove(senderName);
            }
        }

        ISenderChannel INotifyRegistry.GetSender(string senderName)
        {
            lock (channels)
            {
                ISenderChannel channel;
                channels.TryGetValue(senderName, out channel);
                return channel;
            }
        }

        INotifyClient INotifyRegistry.RegisterClient(INotifySource source)
        {
            //ValidateNotifySource(source);
            var client = new NotifyClientImpl(this, source);
            if (NotifyClientRegistration != null)
            {
                NotifyClientRegistration(this, client);
            }
            return client;
        }


        private void ValidateNotifySource(INotifySource source)
        {
            foreach (var a in source.GetActionProvider().GetActions())
            {
                var senderNames = Enumerable.Empty<string>();
                lock (channels)
                {
                    senderNames = channels.Values.Select(s => s.SenderName);
                }
                foreach (var s in senderNames)
                {
                    try
                    {
                        var pattern = source.GetPatternProvider().GetPattern(a, s);
                        if (pattern == null)
                        {
                            throw new NotifyException(string.Format("In notify source {0} pattern not found for action {1} and sender {2}", source.ID, a.ID, s));
                        }
                    }
                    catch (Exception error)
                    {
                        log4net.LogManager.GetLogger("ASC.Notify").ErrorFormat("Source: {0}, action: {1}, sender: {2}, error: {3}", source.ID, a.ID, s, error);
                    }
                }
            }
        }
    }
}
