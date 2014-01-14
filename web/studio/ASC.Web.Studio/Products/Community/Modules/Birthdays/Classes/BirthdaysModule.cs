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
using ASC.Web.Community.Birthdays;
using ASC.Web.Community.Birthdays.Resources;
using ASC.Web.Community.Product;
using ASC.Web.Core;
using ASC.Web.Core.ModuleManagement;

[assembly: Product(typeof(BirthdaysModule))]

namespace ASC.Web.Community.Birthdays
{
    public class BirthdaysModule : Module
    {
        public static readonly Guid ModuleId = WebItemManager.BirthdaysProductID;

        private static readonly object locker = new object();
        private static bool registered;

        public override Guid ID
        {
            get { return ModuleId; }
        }

        public override Guid ProjectId
        {
            get { return CommunityProduct.ID; }
        }

        public override string Name
        {
            get { return BirthdaysResource.BirthdaysModuleTitle; }
        }

        public override string Description
        {
            get { return BirthdaysResource.BirthdaysModuleDescription; }
        }

        public override string StartURL
        {
            get { return "~/products/community/modules/birthdays/"; }
        }

        public BirthdaysModule()
        {
            Context = new ModuleContext
            {
                DefaultSortOrder = 6,
                SmallIconFileName = string.Empty,
                IconFileName = string.Empty,
                SubscriptionManager = new BirthdaysSubscriptionManager()
            };
        }

        public static void RegisterSendMethod()
        {
            lock (locker)
            {
                if (!registered)
                {
                    registered = true;
                    BirthdaysNotifyClient.Instance.RegisterSendMethod();
                }
            }
        }
    }
}
