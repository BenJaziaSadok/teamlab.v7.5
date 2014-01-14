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

namespace ASC.Mail.Net.Mime.vCard
{
    /// <summary>
    /// vCal delivery address type. Note this values may be flagged !
    /// </summary>
    public enum DeliveryAddressType_enum
    {
        /// <summary>
        /// Delivery address type not specified.
        /// </summary>
        NotSpecified = 0,

        /// <summary>
        /// Preferred delivery address.
        /// </summary>
        Preferred = 1,

        /// <summary>
        /// Domestic delivery address.
        /// </summary>
        Domestic = 2,

        /// <summary>
        /// International delivery address.
        /// </summary>
        Ineternational = 4,

        /// <summary>
        /// Postal delivery address.
        /// </summary>
        Postal = 8,

        /// <summary>
        /// Parcel delivery address.
        /// </summary>
        Parcel = 16,

        /// <summary>
        /// Delivery address for a residence.
        /// </summary>
        Home = 32,

        /// <summary>
        /// Address for a place of work.
        /// </summary>
        Work = 64,
    }
}