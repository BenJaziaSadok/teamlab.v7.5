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

#region using

using System;
using System.Text;
using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.authorization.Plain
{

    #region usings

    #endregion

    /// <summary>
    ///   Summary description for PlainMechanism.
    /// </summary>
    public class PlainMechanism : Mechanism
    {
        #region Members

        //private XmppClientConnection m_XmppClient = null;

        #endregion

        #region Constructor

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <summary>
        /// </summary>
        /// <param name="con"> </param>
        public override void Init()
        {
            // m_XmppClient = con;

            // <auth mechanism="PLAIN" xmlns="urn:ietf:params:xml:ns:xmpp-sasl">$Message</auth>
            //m_XmppClient.Send(new Auth(MechanismType.PLAIN, Message()));
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public override void Parse(Node e)
        {
            // not needed here in PLAIN mechanism
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        private string Message()
        {
            // NULL Username NULL Password
            var sb = new StringBuilder();

            // sb.Append( (char) 0 );
            // sb.Append(this.m_XmppClient.MyJID.Bare);
            sb.Append((char) 0);
            sb.Append(Username);
            sb.Append((char) 0);
            sb.Append(Password);

            byte[] msg = Encoding.UTF8.GetBytes(sb.ToString());
            return Convert.ToBase64String(msg, 0, msg.Length);
        }

        #endregion
    }
}