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


namespace ASC.Web.Core.Subscriptions
{
    public class SubscriptionObject
    {
        public string ID { get; set; }

        public string URL { get; set; }

        public string Name { get; set; }
        
        public SubscriptionType SubscriptionType { get; set; }

        public SubscriptionGroup SubscriptionGroup { get; set; }
    }
}
