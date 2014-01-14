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

using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /*
     
        <iq from='crone1@shakespeare.lit/desktop'
            id='begone'
            to='heath@macbeth.shakespeare.lit'
            type='set'>
          <query xmlns='http://jabber.org/protocol/muc#owner'>
            <destroy jid='darkcave@macbeth.shakespeare.lit'>
              <reason>Macbeth doth come.</reason>
            </destroy>
          </query>
        </iq>
     
     */

    /// <summary>
    /// </summary>
    public class Destroy : Element
    {
        #region Constructor

        /// <summary>
        /// </summary>
        public Destroy()
        {
            TagName = "destroy";
            Namespace = Uri.MUC_OWNER;
        }

        /// <summary>
        /// </summary>
        /// <param name="reason"> </param>
        public Destroy(string reason) : this()
        {
            Reason = reason;
        }

        /// <summary>
        /// </summary>
        /// <param name="altVenue"> </param>
        public Destroy(Jid altVenue) : this()
        {
            AlternateVenue = altVenue;
        }

        /// <summary>
        /// </summary>
        /// <param name="reason"> </param>
        /// <param name="altVenue"> </param>
        public Destroy(string reason, Jid altVenue) : this()
        {
            Reason = reason;
            AlternateVenue = altVenue;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Pptional attribute for a alternate venue
        /// </summary>
        public Jid AlternateVenue
        {
            get
            {
                if (HasAttribute("jid"))
                {
                    return new Jid(GetAttribute("jid"));
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (value != null)
                {
                    SetAttribute("jid", value.ToString());
                }
            }
        }

        /// <summary>
        /// </summary>
        public string Password
        {
            get { return GetTag("password"); }

            set { SetTag("password", value); }
        }

        /// <summary>
        /// </summary>
        public string Reason
        {
            get { return GetTag("reason"); }

            set { SetTag("reason", value); }
        }

        #endregion
    }
}