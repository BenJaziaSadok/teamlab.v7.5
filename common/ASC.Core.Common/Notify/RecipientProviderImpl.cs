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
using ASC.Core.Users;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    public class RecipientProviderImpl : IRecipientProvider
    {
        public virtual IRecipient GetRecipient(string id)
        {
            var recID = Guid.Empty;
            if (TryParseGuid(id, out recID))
            {
                var user = CoreContext.UserManager.GetUsers(recID);
                if (user.ID != Constants.LostUser.ID) return new DirectRecipient(user.ID.ToString(), user.ToString());

                var group = CoreContext.GroupManager.GetGroupInfo(recID);
                if (group.ID != Constants.LostGroupInfo.ID) return new RecipientsGroup(group.ID.ToString(), group.Name);
            }
            return null;
        }

        public virtual IRecipient[] GetGroupEntries(IRecipientsGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            var result = new List<IRecipient>();
            var groupID = Guid.Empty;
            if (TryParseGuid(group.ID, out groupID))
            {
                var coreGroup = CoreContext.GroupManager.GetGroupInfo(groupID);
                if (coreGroup.ID != Constants.LostGroupInfo.ID)
                {
                    var users = CoreContext.UserManager.GetUsersByGroup(coreGroup.ID);
                    Array.ForEach(users, u => result.Add(new DirectRecipient(u.ID.ToString(), u.ToString())));
                }
            }
            return result.ToArray();
        }

        public virtual IRecipientsGroup[] GetGroups(IRecipient recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");

            var result = new List<IRecipientsGroup>();
            var recID = Guid.Empty;
            if (TryParseGuid(recipient.ID, out recID))
            {
                if (recipient is IRecipientsGroup)
                {
                    var group = CoreContext.GroupManager.GetGroupInfo(recID);
                    while (group != null && group.Parent != null)
                    {
                        result.Add(new RecipientsGroup(group.Parent.ID.ToString(), group.Parent.Name));
                        group = group.Parent;
                        break;
                    }
                }
                else if (recipient is IDirectRecipient)
                {
                    foreach (var group in CoreContext.UserManager.GetUserGroups(recID, IncludeType.Distinct))
                    {
                        result.Add(new RecipientsGroup(group.ID.ToString(), group.Name));
                    }
                }
            }
            return result.ToArray();
        }

        public virtual string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");

            Guid userID;
            if (TryParseGuid(recipient.ID, out userID))
            {
                var user = CoreContext.UserManager.GetUsers(userID);
                if (user.ID != Constants.LostUser.ID)
                {
                    if (senderName == ASC.Core.Configuration.Constants.NotifyEMailSenderSysName) return new[] {user.Email};
                    if (senderName == ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName) return new[] {user.UserName};
                    if (senderName == ASC.Core.Configuration.Constants.NotifyPushSenderSysName) return new[] {user.UserName};
                }
            }
            return new string[0];
        }


        public IRecipient GetRecipient(string id, string objectID)
        {
            return GetRecipient(id);
        }

        public IRecipient[] GetGroupEntries(IRecipientsGroup group, string objectID)
        {
            return GetGroupEntries(group);
        }

        public IRecipientsGroup[] GetGroups(IRecipient recipient, string objectID)
        {
            return GetGroups(recipient);
        }

        public string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName, string objectID)
        {
            return GetRecipientAddresses(recipient, senderName);
        }

        /// <summary>
        /// Check if user with this email is activated
        /// </summary>
        /// <param name="recipient"></param>
        /// <returns></returns>
        public IDirectRecipient FilterRecipientAddresses(IDirectRecipient recipient)
        {
            //Check activation
            if (recipient.CheckActivation)
            {
                //It's direct email
                if (recipient.Addresses != null && recipient.Addresses.Any())
                {
                    //Filtering only missing users and users who activated already
                    var filteredAddresses = from address in recipient.Addresses
                                            let user = CoreContext.UserManager.GetUserByEmail(address)
                                            where user.ID == Constants.LostUser.ID || (user.IsActive && (user.Status & EmployeeStatus.Default) == user.Status)
                                            select address;

                    return new DirectRecipient(recipient.ID, recipient.Name, filteredAddresses.ToArray(), false);
                }
            }
            return recipient;
        }


        private bool TryParseGuid(string id, out Guid guid)
        {
            guid = Guid.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    guid = new Guid(id);
                    return true;
                }
                catch (FormatException) { }
                catch (OverflowException) { }
            }
            return false;
        }
    }
}
