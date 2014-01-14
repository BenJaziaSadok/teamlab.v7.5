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
        ///    Returns list of all trusted addresses for image displaying.
        /// </summary>
        /// <returns>Addresses list. Email adresses represented as string name@domain.</returns>
        /// <short>Get trusted addresses</short> 
        /// <category>Images</category>
        [Read(@"display_images/addresses")]
        public IEnumerable<string> GetDisplayImagesAddresses()
        {
            return mailBoxManager.GetDisplayImagesAddresses(TenantId, Username);
        }

        ///  <summary>
        ///     Add the address to trusted addresses.
        ///  </summary>
        /// <param name="address">Address for adding. </param>
        /// <returns>Added address</returns>
        ///  <short>Add trusted address</short> 
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        ///  <category>Images</category>
        [Create(@"display_images/address")]
        public string AddDisplayImagesAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Invalid address. Address can't be empty.", "address");

            mailBoxManager.AddDisplayImagesAddress(TenantId, Username, address);
            return address;
        }

        ///  <summary>
        ///     Remove the address from trusted addresses.
        ///  </summary>
        /// <param name="address">Address for removing</param>
        /// <returns>Removed address</returns>
        ///  <short>Remove from trusted addresses</short> 
        /// <exception cref="ArgumentException">Exception happens when in parameters is invalid. Text description contains parameter name and text description.</exception>
        ///  <category>Images</category>
        [Delete(@"display_images/address")]
        public string RemovevDisplayImagesAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentException("Invalid address. Address can't be empty.", "address");

            mailBoxManager.RemovevDisplayImagesAddress(TenantId, Username, address);
            return address;
        }

    }
}
