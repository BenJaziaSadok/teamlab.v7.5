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

namespace ASC.Xmpp.Core.protocol.extensions.pubsub
{
    /*
        <iq type='result'
            from='pubsub.shakespeare.lit'
            to='francisco@denmark.lit'
            id='affil1'>
          <pubsub xmlns='http://jabber.org/protocol/pubsub'>
            <affiliations>
              <affiliation node='node1' jid='francisco@denmark.lit' affiliation='owner'/>
              <affiliation node='node2' jid='francisco@denmark.lit' affiliation='publisher'/>
              <affiliation node='node5' jid='francisco@denmark.lit' affiliation='outcast'/>
              <affiliation node='node6' jid='francisco@denmark.lit' affiliation='owner'/>
            </affiliations>
          </pubsub>
        </iq>
    */

    public class Affiliations : Element
    {
        #region << Consrtuctors >>

        public Affiliations()
        {
            TagName = "affiliations";
            Namespace = Uri.PUBSUB;
        }

        #endregion

        public Affiliation AddAffiliation()
        {
            var aff = new Affiliation();
            AddChild(aff);
            return aff;
        }


        public Affiliation AddAffiliation(Affiliation aff)
        {
            AddChild(aff);
            return aff;
        }

        public Affiliation[] GetAffiliations()
        {
            ElementList nl = SelectElements(typeof (Affiliation));
            var items = new Affiliation[nl.Count];
            int i = 0;
            foreach (Element e in nl)
            {
                items[i] = (Affiliation) e;
                i++;
            }
            return items;
        }
    }
}