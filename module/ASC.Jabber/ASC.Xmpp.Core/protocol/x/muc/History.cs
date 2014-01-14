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
using ASC.Xmpp.Core.utils;
using ASC.Xmpp.Core.utils.Xml.Dom;

namespace ASC.Xmpp.Core.protocol.x.muc
{

    #region usings

    #endregion

    /*
        Example 29. User Requests Limit on Number of Messages in History

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history maxstanzas='20'/>
          </x>
        </presence>
              

        Example 30. User Requests History in Last 3 Minutes

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history seconds='180'/>
          </x>
        </presence>
              

        Example 31. User Requests All History Since the Beginning of the Unix Era

        <presence
            from='hag66@shakespeare.lit/pda'
            to='darkcave@macbeth.shakespeare.lit/thirdwitch'>
          <x xmlns='http://jabber.org/protocol/muc'>
            <history since='1970-01-01T00:00Z'/>
          </x>
        </presence>
    */

    /// <summary>
    ///   This is used to get the history of a muc room
    /// </summary>
    public class History : Element
    {
        #region Constructor

        /// <summary>
        ///   Empty default constructor
        /// </summary>
        public History()
        {
            TagName = "history";
            Namespace = Uri.MUC;
        }

        /// <summary>
        ///   get the history starting from a given date when available
        /// </summary>
        /// <param name="date"> </param>
        public History(DateTime date) : this()
        {
            Since = date;
        }

        /// <summary>
        ///   Specify the maximum nunber of messages to retrieve from the history
        /// </summary>
        /// <param name="max"> </param>
        public History(int max) : this()
        {
            MaxStanzas = max;
        }

        #endregion

        #region Properties

        /// <summary>
        ///   Limit the total number of characters in the history to "X" (where the character count is the characters of the complete XML stanzas, not only their XML character data).
        /// </summary>
        public int MaxCharacters
        {
            get { return GetAttributeInt("maxchars"); }

            set { SetAttribute("maxchars", value); }
        }

        /// <summary>
        ///   Request maximum stanzas of history when available
        /// </summary>
        public int MaxStanzas
        {
            get { return GetAttributeInt("maxstanzas"); }

            set { SetAttribute("maxstanzas", value); }
        }

        /// <summary>
        ///   request the last xxx seconds of history when available
        /// </summary>
        public int Seconds
        {
            get { return GetAttributeInt("seconds"); }

            set { SetAttribute("seconds", value); }
        }

        /// <summary>
        ///   Request history from a given date when available
        /// </summary>
        public DateTime Since
        {
            get { return Time.ISO_8601Date(GetAttribute("since")); }

            set { SetAttribute("since", Time.ISO_8601Date(value)); }
        }

        #endregion
    }
}