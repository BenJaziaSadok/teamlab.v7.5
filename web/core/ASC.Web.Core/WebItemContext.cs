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

namespace ASC.Web.Core
{
    public class WebItemContext
    {
        public SpaceUsageStatManager SpaceUsageStatManager { get; set; }

        public Func<string> GetCreateContentPageAbsoluteUrl { get; set; }

        public ISubscriptionManager SubscriptionManager { get; set; }

        public Func<List<string>> UserOpportunities { get; set; }

        public Func<List<string>> AdminOpportunities { get; set; }

        public string SmallIconFileName { get; set; }

        public string DisabledIconFileName { get; set; }
        
        public string IconFileName { get; set; }

        public string LargeIconFileName { get; set; }

        public int DefaultSortOrder { get; set; }

        public bool HasComplexHierarchyOfAccessRights { get; set; }

        public bool CanNotBeDisabled { get; set; }

        public WebItemContext()
        {
            GetCreateContentPageAbsoluteUrl = () => string.Empty;
        }
    }
}
