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

namespace ASC.Mail.Net.IMAP.Client
{
    #region usings

    using Server;

    #endregion

    /// <summary>
    /// IMAP ACL entry. Defined in RFC 2086.
    /// </summary>
    public class IMAP_Acl
    {
        #region Members

        private readonly string m_Name = "";
        private readonly IMAP_ACL_Flags m_Rights = IMAP_ACL_Flags.None;

        #endregion

        #region Properties

        /// <summary>
        /// Gets authentication identifier name. Normally this is user or group name.
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Gets the rights associated with this ACL entry.
        /// </summary>
        public IMAP_ACL_Flags Rights
        {
            get { return m_Rights; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Authentication identifier name. Normally this is user or group name.</param>
        /// <param name="rights">Rights associated with this ACL entry.</param>
        public IMAP_Acl(string name, IMAP_ACL_Flags rights)
        {
            m_Name = name;
            m_Rights = rights;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Parses ACL entry from IMAP ACL response string.
        /// </summary>
        /// <param name="aclResponseString">IMAP ACL response string.</param>
        /// <returns></returns>
        internal static IMAP_Acl Parse(string aclResponseString)
        {
            string[] args = TextUtils.SplitQuotedString(aclResponseString, ' ', true);
            return new IMAP_Acl(args[1], IMAP_Utils.ACL_From_String(args[2]));
        }

        #endregion
    }
}