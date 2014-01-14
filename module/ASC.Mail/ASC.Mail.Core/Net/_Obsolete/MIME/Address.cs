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

namespace ASC.Mail.Net.Mime
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Rfc 2822 3.4 Address class. This class is base class for MailboxAddress and GroupAddress.
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public abstract class Address
    {
        #region Members

        private readonly bool m_GroupAddress;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="groupAddress">Spcified is address is group or mailbox address.</param>
        public Address(bool groupAddress)
        {
            m_GroupAddress = groupAddress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets if address is group address or mailbox address.
        /// </summary>
        public bool IsGroupAddress
        {
            get { return m_GroupAddress; }
        }

        /// <summary>
        /// Gets or sets owner of this address.
        /// </summary>
        internal object Owner { get; set; }

        #endregion
    }
}