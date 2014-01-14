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

namespace ASC.Xmpp.Core.protocol.iq.roster
{
    /// <summary>
    ///   Zusammenfassung f�r Roster.
    /// </summary>
    public class Roster : Element
    {
        // Request Roster:
        // <iq id='someid' to='myjabber.net' type='get'>
        //		<query xmlns='jabber:iq:roster'/>
        // </iq>
        public Roster()
        {
            TagName = "query";
            Namespace = Uri.IQ_ROSTER;
        }

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

        public void AddRosterItem(RosterItem r)
        {
            ChildNodes.Add(r);
        }
    }
}