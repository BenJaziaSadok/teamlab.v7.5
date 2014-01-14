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

namespace ASC.Xmpp.Core.protocol.iq.blocklist
{
    public class Blocklist : BlockBase
    {
        public Blocklist()
        {
            TagName = "blocklist";
        }
    }

    public class Block : BlockBase
    {
        public Block()
        {
            TagName = "block";
        }
    }

    public class Unblock : BlockBase
    {
        public Unblock()
        {
            TagName = "unblock";
        }
    }

    public class BlockBase : Element
    {
        public BlockBase()
        {
            Namespace = Uri.IQ_BLOCKLIST;
        }


        public BlockItem[] GetItems()
        {
            ElementList nl = SelectElements(typeof (BlockItem));
            int i = 0;
            var result = new BlockItem[nl.Count];
            foreach (BlockItem ri in nl)
            {
                result[i] = ri;
                i++;
            }
            return result;
        }

        public void AddBlockItem(BlockItem r)
        {
            ChildNodes.Add(r);
        }
    }
}