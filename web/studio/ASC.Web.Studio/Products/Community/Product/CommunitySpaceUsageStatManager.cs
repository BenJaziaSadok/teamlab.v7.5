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

using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASC.Web.Core;
using ASC.Web.Studio.UserControls.Statistics;

namespace ASC.Web.Community.Product
{
    public class CommunitySpaceUsageStatManager : SpaceUsageStatManager
    {
        public override List<UsageSpaceStatItem> GetStatData()
        {
            return WebItemManager.Instance.GetSubItems(CommunityProduct.ID, ItemAvailableState.All)
                .Select(webItem => new UsageSpaceStatItem
                    {
                        Name = webItem.Name,
                        ImgUrl = webItem.GetIconAbsoluteURL(),
                        SpaceUsage = TenantStatisticsProvider.GetUsedSize(webItem.ID),
                        Url = VirtualPathUtility.ToAbsolute(webItem.StartURL)
                    })
                .Where(statItem => statItem.SpaceUsage > 0)
                .ToList();
        }
    }
}
