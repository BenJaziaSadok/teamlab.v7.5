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

#region usings

using System;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    public interface ISubscriptionProvider
    {
        string[] GetSubscriptions(INotifyAction action, IRecipient recipient);

        string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient);

        IRecipient[] GetRecipients(INotifyAction action, string objectID);


        bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID);

        void Subscribe(INotifyAction action, string objectID, IRecipient recipient);

        void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient);

        void UnSubscribe(INotifyAction action, string objectID);

        void UnSubscribe(INotifyAction action);

        void UnSubscribe(INotifyAction action, IRecipient recipient);

        void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames);
    }

    public static class SubscriptionProviderHelper
    {
        public static bool IsSubscribed(this ISubscriptionProvider provider, INotifyAction action, IRecipient recipient,
                                        string objectID)
        {
            return Array.Exists(
                provider.GetSubscriptions(action, recipient),
                id => id == objectID || (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(objectID))
                );
        }
    }
}