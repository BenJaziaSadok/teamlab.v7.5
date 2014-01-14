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

namespace ASC.Api.Mail
{
    public partial class MailApi
    {
        /// <summary>
        ///    Generates random GUID. This Guid needed for set stream id in message. StreamId used by data storage key.
        /// </summary>
        /// <returns>Guid represented as string</returns>
        /// <short>Generate random GUID</short> 
        /// <category>GUID</category>
        [Read(@"random_guid")]
        public string GenRandomGuid()
        {
            return mailBoxManager.CreateNewStreamId();
        }
    }
}
