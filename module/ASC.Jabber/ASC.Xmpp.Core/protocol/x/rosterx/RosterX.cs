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

using ASC.Xmpp.Core.utils.Xml.Dom;

#endregion

namespace ASC.Xmpp.Core.protocol.x.rosterx
{

    #region usings

    #endregion

    /// <summary>
    ///   Roster Item Exchange (JEP-0144)
    /// </summary>
    public class RosterX : Element
    {
        #region Constructor

        /// <summary>
        ///   Initializes a new instance of the <see cref="RosterX" /> class.
        /// </summary>
        public RosterX()
        {
            TagName = "x";
            Namespace = Uri.X_ROSTERX;
        }

        #endregion

        #region Methods

        /// <summary>
        ///   Gets the roster.
        /// </summary>
        /// <returns> </returns>
        public RosterItem[] GetRoster()
        {
            ElementList nl = SelectElements(typeof (RosterItem));
            int i = 0;
            var result = new RosterItem[nl.Count];
            foreach (RosterItem ri in nl)
            {
                result[i] = ri;
                i++;
            }

            return result;
        }

        /// <summary>
        ///   Adds a roster item.
        /// </summary>
        /// <param name="r"> The r. </param>
        public void AddRosterItem(RosterItem r)
        {
            ChildNodes.Add(r);
        }

        #endregion

        /*
		<message from='horatio@denmark.lit' to='hamlet@denmark.lit'>
		<body>Some visitors, m'lord!</body>
		<x xmlns='http://jabber.org/protocol/rosterx'> 
			<item action='add'
				jid='rosencrantz@denmark.lit'
				name='Rosencrantz'>
				<group>Visitors</group>
			</item>
			<item action='add'
				jid='guildenstern@denmark.lit'
				name='Guildenstern'>
				<group>Visitors</group>
			</item>
		</x>
		</message>
		*/
    }
}