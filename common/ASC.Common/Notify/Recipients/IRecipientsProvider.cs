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

namespace ASC.Notify.Recipients
{
    public interface IRecipientProvider
    {
        IRecipient GetRecipient(string id);

        IRecipient[] GetGroupEntries(IRecipientsGroup group);

        IRecipientsGroup[] GetGroups(IRecipient recipient);

        string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName);

        IRecipient[] GetGroupEntries(IRecipientsGroup group, string objectID);

        string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName, string objectID);
        IDirectRecipient FilterRecipientAddresses(IDirectRecipient recipient);
    }
}