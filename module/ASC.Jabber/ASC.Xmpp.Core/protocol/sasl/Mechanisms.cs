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

//	<mechanisms xmlns='urn:ietf:params:xml:ns:xmpp-sasl'>
//		<mechanism>DIGEST-MD5</mechanism>
//		<mechanism>PLAIN</mechanism>
//	</mechanisms>

namespace ASC.Xmpp.Core.protocol.sasl
{
    /// <summary>
    ///   Summary description for Mechanisms.
    /// </summary>
    public class Mechanisms : Element
    {
        public Mechanisms()
        {
            TagName = "mechanisms";
            Namespace = Uri.SASL;
        }

        public Mechanism[] GetMechanisms()
        {
            ElementList elements = SelectElements("mechanism");

            var items = new Mechanism[elements.Count];
            int i = 0;
            foreach (Element e in elements)
            {
                items[i] = (Mechanism) e;
                i++;
            }
            return items;
        }

        public bool SupportsMechanism(MechanismType type)
        {
            foreach (Mechanism m in GetMechanisms())
            {
                if (m.MechanismType == type)
                    return true;
            }
            return false;
        }
    }
}