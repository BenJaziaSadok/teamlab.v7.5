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
using ASC.Core.Tenants;

namespace ASC.Mail.Aggregator.Extension
{
    public static class DateTimeExtensions
    {
        public static string ToVerbString(this DateTime dateTime)
        {
            try
            {
                TimeSpan diff = (TenantUtil.DateTimeNow().Date - dateTime.Date);

                if (diff.Days == 0)
                {
                    return dateTime.ToShortTimeString();
                } 
                if (TenantUtil.DateTimeNow().Year == dateTime.Date.Year)
                {
                    return String.Format("{0}", dateTime.ToString("MMMM dd"));                                                    
                }
                return String.Format("{0} {1} {2}", dateTime.ToString("dd"), dateTime.ToString("MMMM"), dateTime.ToString("yyyy"));                    
            }
            catch (Exception)
            {
                return String.Format("{0} {1}", dateTime.ToShortDateString(), dateTime.ToShortTimeString());
            }
        }
    }
}