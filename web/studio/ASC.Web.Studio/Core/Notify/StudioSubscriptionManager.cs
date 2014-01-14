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
using ASC.Web.Core.Subscriptions;
using ASC.Notify.Model;

namespace ASC.Web.Studio.Core.Notify
{
    internal class StudioSubscriptionManager : ISubscriptionManager
    {
        private static StudioSubscriptionManager _instance = new StudioSubscriptionManager();

        public static StudioSubscriptionManager Instance
        {
            get { return _instance; }
        }

        private StudioSubscriptionManager()
        { }

        #region ISubscriptionManager Members

        public List<SubscriptionObject> GetSubscriptionObjects(Guid subItem)
        {
            return new List<SubscriptionObject>();
        }

        public List<SubscriptionType> GetSubscriptionTypes()
        {
            var types = new List<SubscriptionType>();
            types.Add(new SubscriptionType()
            {
                ID = new Guid("{148B5E30-C81A-4ff8-B749-C46BAE340093}"),
                Name = Resources.Resource.WhatsNewSubscriptionName,
                NotifyAction = Constants.ActionSendWhatsNew,
                Single = true
            });

            var astype = new SubscriptionType()
            {
                ID = new Guid("{A4FFC01F-BDB5-450e-88C4-03FED17D67C5}"),
                Name = Resources.Resource.AdministratorNotifySenderTypeName,
                NotifyAction = Constants.ActionSendWhatsNew,
                Single = false
            };
            
            types.Add(astype);

            return types;
        }

        public ISubscriptionProvider SubscriptionProvider
        {
            get { return StudioNotifyService.Instance.source.GetSubscriptionProvider(); }
        }

        #endregion
    }
}
