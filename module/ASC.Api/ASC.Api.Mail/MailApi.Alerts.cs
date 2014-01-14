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
using System.Linq;
using System.Text;
using ASC.Api.Attributes;
using ASC.Mail.Aggregator;

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        ///    Returns the list of alerts for the authenticated user
        /// </summary>
        /// <returns>Alerts list</returns>
        /// <short>Get alerts list</short> 
        /// <category>Alerts</category>
        [Read(@"alert")]
        public IList<MailBoxManager.MailAlert> GetAlerts()
        {
            return mailBoxManager.GetMailAlerts(TenantId, Username);
        }

        /// <summary>
        ///    Deletes the alert with the ID specified in the request
        /// </summary>
        /// <param name="id">Alert ID</param>
        /// <returns>Deleted alert id. Same as request parameter.</returns>
        /// <short>Delete alert by ID</short> 
        /// <category>Alerts</category>
        [Delete(@"alert/{id}")]
        public Int64 DeleteAlert(Int64 id)
        {
            if(id < 0)
                throw new ArgumentException("Invalid alert id. Id must be positive integer.", "id");

            mailBoxManager.DeleteAlert(TenantId, Username, id);
            return id;
        }
    }
}
