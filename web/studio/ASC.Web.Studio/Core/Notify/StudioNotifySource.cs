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

using ASC.Core.Notify;
using ASC.Notify.Model;
using ASC.Notify.Patterns;
using ASC.Notify.Recipients;

namespace ASC.Web.Studio.Core.Notify
{
    class StudioNotifySource : NotifySource
    {
        public StudioNotifySource()
            : base("asc.web.studio")
        {
        }


        protected override IActionProvider CreateActionProvider()
        {
            return new ConstActionProvider(
                    Constants.ActionPasswordChanged,

                    Constants.ActionYouAddedAfterInvite,
                    Constants.ActionYouAddedLikeGuest,

                    Constants.ActionYourProfileUpdated,

                    Constants.ActionSelfProfileUpdated,
                    Constants.ActionSendPassword,
                    Constants.ActionInviteUsers,
                    Constants.ActionJoinUsers,
                    Constants.ActionSendWhatsNew,
                    Constants.ActionUserHasJoin,
                    Constants.ActionBackupCreated,
                    Constants.ActionPortalDeactivate,
                    Constants.ActionPortalDelete,
                    Constants.ActionDnsChange,
                    Constants.ActionConfirmOwnerChange,
                    Constants.ActionActivateUsers,
                    Constants.ActionActivateGuests,
                    Constants.ActionEmailChange,
                    Constants.ActionPasswordChange,
                    Constants.ActionActivateEmail,
                    Constants.ActionProfileDelete,
                    Constants.ActionPhoneChange,
                    Constants.ActionMigrationPortalStart,
                    Constants.ActionMigrationPortalSuccess,
                    Constants.ActionMigrationPortalError,
                    Constants.ActionMigrationPortalServerFailure,

                    Constants.ActionUserMessageToAdmin,
                    Constants.ActionCongratulations
                );
        }

        protected override IPatternProvider CreatePatternsProvider()
        {
            return new XmlPatternProvider2(WebPatternResource.webstudio_patterns);
        }

        protected override ISubscriptionProvider CreateSubscriptionProvider()
        {
            return new AdminNotifySubscriptionProvider(base.CreateSubscriptionProvider());
        }


        private class AdminNotifySubscriptionProvider : ISubscriptionProvider
        {
            private readonly ISubscriptionProvider provider;


            public AdminNotifySubscriptionProvider(ISubscriptionProvider provider)
            {
                this.provider = provider;
            }


            public string[] GetSubscriptions(INotifyAction action, IRecipient recipient)
            {
                return provider.GetSubscriptions(GetAdminAction(action), recipient);
            }

            public void Subscribe(INotifyAction action, string objectID, IRecipient recipient)
            {
                provider.Subscribe(GetAdminAction(action), objectID, recipient);
            }

            public void UnSubscribe(INotifyAction action, IRecipient recipient)
            {
                provider.UnSubscribe(GetAdminAction(action), recipient);
            }

            public void UnSubscribe(INotifyAction action)
            {
                provider.UnSubscribe(GetAdminAction(action));
            }

            public void UnSubscribe(INotifyAction action, string objectID)
            {
                provider.UnSubscribe(GetAdminAction(action), objectID);
            }

            public void UnSubscribe(INotifyAction action, string objectID, IRecipient recipient)
            {
                provider.UnSubscribe(GetAdminAction(action), objectID, recipient);
            }

            public void UpdateSubscriptionMethod(INotifyAction action, IRecipient recipient, params string[] senderNames)
            {
                provider.UpdateSubscriptionMethod(GetAdminAction(action), recipient, senderNames);
            }

            public IRecipient[] GetRecipients(INotifyAction action, string objectID)
            {
                return provider.GetRecipients(GetAdminAction(action), objectID);
            }

            public string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient)
            {
                return provider.GetSubscriptionMethod(GetAdminAction(action), recipient);
            }

            public bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID)
            {
                return provider.IsUnsubscribe(recipient, action, objectID);
            }

            private INotifyAction GetAdminAction(INotifyAction action)
            {
                if (Constants.ActionSelfProfileUpdated.ID == action.ID ||
                    Constants.ActionUserHasJoin.ID == action.ID ||
                    Constants.ActionUserMessageToAdmin.ID == action.ID)
                {
                    return Constants.ActionAdminNotify;
                }
                else
                {
                    return action;
                }
            }
        }
    }
}
