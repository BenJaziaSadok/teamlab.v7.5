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
using System.Globalization;

namespace ASC.Data.Storage
{
    public class TennantPath
    {
        public static string CreatePath(string tenant)
        {
            if (tenant == null) throw new ArgumentNullException("tenant");
            //Try parse first
            long tennantId;
            if (long.TryParse(tenant, NumberStyles.Integer, CultureInfo.InvariantCulture, out tennantId))
            {
                return tennantId == 0
                           ? tennantId.ToString(CultureInfo.InvariantCulture)
                           : tennantId.ToString("00/00/00", CultureInfo.InvariantCulture);
                //Make path
            }
            return tenant;
        }
    }
}