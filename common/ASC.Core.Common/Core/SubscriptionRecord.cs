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
    [Serializable]
    public class SubscriptionRecord
    {
        public int Tenant
        {
            get;
            set;
        }

        public string SourceId
        {
            get;
            set;
        }

        public string ActionId
        {
            get;
            set;
        }

        public string RecipientId
        {
            get;
            set;
        }

        public string ObjectId
        {
            get;
            set;
        }

        public bool Subscribed
        {
            get;
            set;
        }
    }
}
