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
    /// RFC 2822 3.4. (Address Specification) Group address.
    /// <p/>
    /// Syntax: display-name':'[mailbox *(',' mailbox)]';'
    /// </summary>
    [Obsolete("See LumiSoft.Net.MIME or LumiSoft.Net.Mail namepaces for replacement.")]
    public class GroupAddress : Address
    {
        #region Members

        private readonly MailboxAddressCollection m_pGroupMembers;
        private string m_DisplayName = "";

        #endregion

        #region Properties

        /// <summary>
        /// Gets Group as RFC 2822(3.4. Address Specification) string. Syntax: display-name':'[mailbox *(',' mailbox)]';'
        /// </summary>
        public string GroupString
        {
            get { return TextUtils.QuoteString(DisplayName) + ":" + GroupMembers.ToMailboxListString() + ";"; }
        }

        /// <summary>
        /// Gets or sets display name.
        /// </summary>
        public string DisplayName
        {
            get { return m_DisplayName; }

            set
            {
                m_DisplayName = value;

                OnChanged();
            }
        }

        /// <summary>
        /// Gets group members collection.
        /// </summary>
        public MailboxAddressCollection GroupMembers
        {
            get { return m_pGroupMembers; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GroupAddress() : base(true)
        {
            m_pGroupMembers = new MailboxAddressCollection();
            m_pGroupMembers.Owner = this;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Parses Rfc 2822 3.4 group address from group address string. Syntax: display-name':'[mailbox *(',' mailbox)]';'
        /// </summary>
        /// <param name="group">Group address string.</param>
        /// <returns></returns>
        public static GroupAddress Parse(string group)
        {
            GroupAddress g = new GroupAddress();

            // Syntax: display-name':'[mailbox *(',' mailbox)]';'
            string[] parts = TextUtils.SplitQuotedString(group, ':');
            if (parts.Length > -1)
            {
                g.DisplayName = TextUtils.UnQuoteString(parts[0]);
            }
            if (parts.Length > 1)
            {
                g.GroupMembers.Parse(parts[1]);
            }

            return g;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// This called when group address has changed.
        /// </summary>
        internal void OnChanged()
        {
            if (Owner != null)
            {
                if (Owner is AddressList)
                {
                    ((AddressList) Owner).OnCollectionChanged();
                }
            }
        }

        #endregion
    }
}