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
using System.Globalization;
using System.Threading;
using ASC.Common.Module;
using ASC.Notify;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    public abstract class NotifySource : INotifySource
    {
        private readonly object syncRoot = new object();
        private bool initialized;

        private readonly IDictionary<CultureInfo, IActionProvider> actions = new Dictionary<CultureInfo, IActionProvider>();

        private readonly IDictionary<CultureInfo, IPatternProvider> patterns = new Dictionary<CultureInfo, IPatternProvider>();


        protected ISubscriptionProvider SubscriprionProvider;

        protected IRecipientProvider RecipientsProvider;


        protected IActionProvider ActionProvider
        {
            get { return GetActionProvider(); }
        }

        protected IPatternProvider PatternProvider
        {
            get { return GetPatternProvider(); }
        }


        public string ID
        {
            get;
            private set;
        }


        public NotifySource(string id)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentNullException("id");

            ID = id;
        }

        public NotifySource(Guid id)
            : this(id.ToString())
        {
        }

        public IActionProvider GetActionProvider()
        {
            lock (actions)
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                if (!actions.ContainsKey(culture))
                {
                    actions[culture] = CreateActionProvider();
                }
                return actions[culture];
            }
        }

        public IPatternProvider GetPatternProvider()
        {
            lock (patterns)
            {
                var culture = Thread.CurrentThread.CurrentCulture;
                if (Thread.CurrentThread.CurrentUICulture != culture)
                {
                    Thread.CurrentThread.CurrentUICulture = culture;
                }
                if (!patterns.ContainsKey(culture))
                {
                    patterns[culture] = CreatePatternsProvider();
                }
                return patterns[culture];
            }
        }

        public IRecipientProvider GetRecipientsProvider()
        {
            LazyInitializeProviders();
            return RecipientsProvider;
        }

        public ISubscriptionProvider GetSubscriptionProvider()
        {
            LazyInitializeProviders();
            return SubscriprionProvider;
        }

        protected void LazyInitializeProviders()
        {
            if (!initialized)
            {
                lock (syncRoot)
                {
                    if (!initialized)
                    {
                        RecipientsProvider = CreateRecipientsProvider();
                        if (RecipientsProvider == null)
                        {
                            throw new NotifyException(String.Format("Provider {0} not instanced.", "IRecipientsProvider"));
                        }

                        SubscriprionProvider = CreateSubscriptionProvider();
                        if (SubscriprionProvider == null)
                        {
                            throw new NotifyException(String.Format("Provider {0} not instanced.", "ISubscriprionProvider"));
                        }

                        initialized = true;
                    }
                }
            }
        }


        protected abstract IPatternProvider CreatePatternsProvider();

        protected abstract IActionProvider CreateActionProvider();


        protected virtual ISubscriptionProvider CreateSubscriptionProvider()
        {
            var subscriptionProvider = new DirectSubscriptionProvider(ID, CoreContext.SubscriptionManager, RecipientsProvider);
            return new TopSubscriptionProvider(RecipientsProvider, subscriptionProvider, WorkContext.DefaultClientSenders);
        }

        protected virtual IRecipientProvider CreateRecipientsProvider()
        {
            return new RecipientProviderImpl();
        }
    }
}