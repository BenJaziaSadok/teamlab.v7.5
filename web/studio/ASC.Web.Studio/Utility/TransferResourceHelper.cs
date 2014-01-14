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
namespace ASC.Web.Studio.Utility
{
    public static class TransferResourceHelper
    {
        public static string GetRegionDescription(string region)
        {
            if (string.Equals("eu", region, StringComparison.OrdinalIgnoreCase))
                return Resources.Resource.EuServerRegion;

            if (string.Equals("us", region, StringComparison.OrdinalIgnoreCase))
                return Resources.Resource.UsServerRegion;

            return string.Empty;
        }
    }
}
