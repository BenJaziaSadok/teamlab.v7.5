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
using ASC.Notify.Recipients;


namespace ASC.Notify.Model
{
    public class TopSubscriptionProvider : ISubscriptionProvider
    {
        private readonly string[] defaultSenderMethods = new string[0];
        private readonly ISubscriptionProvider subscriptionProvider;
        private readonly IRecipientProvider recipientProvider;


        public TopSubscriptionProvider(IRecipientProvider recipientProvider, ISubscriptionProvider directSubscriptionProvider)
        {
            if (recipientProvider == null) throw new ArgumentNullException("recipientProvider");
            if (directSubscriptionProvider == null) throw new ArgumentNullException("directSubscriptionProvider");

            this.recipientProvider = recipientProvider;
            subscriptionProvider = directSubscriptionProvider;
        }

        public TopSubscriptionProvider(IRecipientProvider recipientProvider, ISubscriptionProvider directSubscriptionProvider, string[] defaultSenderMethods)
            : this(recipientProvider, directSubscriptionProvider)
        {
            this.defaultSenderMethods = defaultSenderMethods;
        }


        public virtual string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            var senders = subscriptionProvider.GetSubscriptionMethod(action, recipient);
            if (senders == null || senders.Length == 0)
            {
                var parents = WalkUp(recipient);
                foreach (var parent in parents)
                {
                    senders = subscriptionProvider.GetSubscriptionMethod(action, parent);
                    if (senders != null && senders.Length != 0) break;
                }
            }

            return senders != null && 0 < senders.Length ? senders : defaultSenderMethods;
        }

        public virtual IRecipient[] GetRecipients(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");

            var recipents = new List<IRecipient>(5);
            var directRecipients = subscriptionProvider.GetRecipients(action, objectID) ?? new IRecipient[0];
            recipents.AddRange(directRecipients);
            return recipents.ToArray();
        }

        public virtual bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            return subscriptionProvider.IsUnsubscribe(recipient, action, objectID);
        }


        public virtual void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            subscriptionProvider.Subscribe(action, objectID, recipient);
        }

        public virtual void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");

            subscriptionProvider.UnSubscribe(action, objectID, recipient);
        }

        public void UnSubscribe(INotifyAction action, string objectID)
        {
            if (action == null) throw new ArgumentNullException("action");

            subscriptionProvider.UnSubscribe(action, objectID);
        }

        public void UnSubscribe(INotifyAction action)
        {
            if (action == null) throw new ArgumentNullException("action");

            subscriptionProvider.UnSubscribe(action);
        }

        public virtual void UnSubscribe(INotifyAction action, IRecipient recipient)
        {
            var objects = GetSubscriptions(action, recipient);
            foreach (string objectID in objects)
            {
                subscriptionProvider.UnSubscribe(action, objectID, recipient);
            }
        }

        public virtual void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
        {
            if (action == null) throw new ArgumentNullException("action");
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (senderNames == null) throw new ArgumentNullException("senderNames");

            subscriptionProvider.UpdateSubscriptionMethod(action, recipient, senderNames);
        }

        public virtual string[] GetSubscriptions(INotifyAction action, IRecipient recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            if (action == null) throw new ArgumentNullException("action");

            var objects = new List<string>();
            var direct = subscriptionProvider.GetSubscriptions(action, recipient) ?? new string[0];
            MergeObjects(objects, direct);
            var parents = WalkUp(recipient);
            foreach (var parent in parents)
            {
                direct = subscriptionProvider.GetSubscriptions(action, parent) ?? new string[0];
                if (recipient is IDirectRecipient)
                {
                    foreach (var groupsubscr in direct)
                    {
                        if (!objects.Contains(groupsubscr) && !subscriptionProvider.IsUnsubscribe(recipient as IDirectRecipient, action, groupsubscr))
                        {
                            objects.Add(groupsubscr);
                        }
                    }
                }
                else
                {
                    MergeObjects(objects, direct);
                }
            }
            return objects.ToArray();
        }


        private List<IRecipient> WalkUp(IRecipient recipient)
        {
            var parents = new List<IRecipient>();
            var groups = recipientProvider.GetGroups(recipient) ?? new IRecipientsGroup[0];
            foreach (var group in groups)
            {
                parents.Add(group);
                parents.AddRange(WalkUp(group));
            }
            return parents;
        }

        private void MergeActions(List<INotifyAction> result, IEnumerable<INotifyAction> additions)
        {
            foreach (var addition in additions)
            {
                if (!result.Contains(addition)) result.Add(addition);
            }
        }

        private void MergeObjects(List<string> result, IEnumerable<string> additions)
        {
            foreach (var addition in additions)
            {
                if (!result.Contains(addition)) result.Add(addition);
            }
        }
    }
}