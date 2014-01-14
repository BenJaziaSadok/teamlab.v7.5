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

using ASC.Xmpp.Core.protocol.client;

namespace ASC.Xmpp.Core.protocol.x.muc.iq.admin
{

    #region usings

    #endregion

    /*
        Example 72. Moderator Kicks Occupant

        <iq from='fluellen@shakespeare.lit/pda'
            id='kick1'
            to='harfleur@henryv.shakespeare.lit'
            type='set'>
          <query xmlns='http://jabber.org/protocol/muc#admin'>
            <item nick='pistol' role='none'>
              <reason>Avaunt, you cullion!</reason>
            </item>
          </query>
        </iq>
    */

    /// <summary>
    /// </summary>
    public class AdminIq : IQ
    {
        #region Members

        /// <summary>
        /// </summary>
        private readonly Admin m_Admin = new Admin();

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public AdminIq()
        {
            base.Query = m_Admin;
            GenerateId();
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        public AdminIq(IqType type) : this()
        {
            Type = type;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="to"> </param>
        public AdminIq(IqType type, Jid to) : this(type)
        {
            To = to;
        }

        /// <summary>
        /// </summary>
        /// <param name="type"> </param>
        /// <param name="to"> </param>
        /// <param name="from"> </param>
        public AdminIq(IqType type, Jid to, Jid from) : this(type, to)
        {
            From = from;
        }

        #endregion

        #region Properties

        /// <summary>
        /// </summary>
        public new Admin Query
        {
            get { return m_Admin; }
        }

        #endregion
    }
}