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

namespace ASC.Core
{
    public interface ISubscriptionManagerClient
    {
        string[] GetRecipients(string sourceID, string actionID, string objectID);

        string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID);

        string[] GetSubscriptions(string sourceID, string actionID, string recipientID);

        bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID);

        void Subscribe(string sourceID, string actionID, string objectID, string recipientID);

        void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID);

        void UnsubscribeAll(string sourceID, string actionID, string objectID);

        void UnsubscribeAll(string sourceID, string actionID);

        void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames);
    }
}
