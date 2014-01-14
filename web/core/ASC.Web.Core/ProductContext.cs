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

using ASC.Web.Core.Subscriptions;

namespace ASC.Web.Core
{
    public class ProductContext : WebItemContext
    {
        public string MasterPageFile { get; set; }

        public string ProductHTMLOverview { get; set; }

        private IProductSubscriptionManager _sunscriptionManager;

        public new IProductSubscriptionManager SubscriptionManager
        {
            get { return _sunscriptionManager; }
            set
            {
                _sunscriptionManager = value;
                base.SubscriptionManager = value;
            }
        }
    }
}