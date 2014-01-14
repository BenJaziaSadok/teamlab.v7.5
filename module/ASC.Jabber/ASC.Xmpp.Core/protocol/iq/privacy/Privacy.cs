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

namespace ASC.Xmpp.Core.protocol.iq.privacy
{
    public class Privacy : Element
    {
        public Privacy()
        {
            TagName = "query";
            Namespace = Uri.IQ_PRIVACY;
        }

        /// <summary>
        ///   The active list
        /// </summary>
        public Active Active
        {
            get { return SelectSingleElement(typeof (Active)) as Active; }
            set
            {
                if (HasTag(typeof (Active)))
                    RemoveTag(typeof (Active));

                if (value != null)
                    AddChild(value);
            }
        }

        /// <summary>
        ///   The default list
        /// </summary>
        public Default Default
        {
            get { return SelectSingleElement(typeof (Default)) as Default; }
            set
            {
                if (HasTag(typeof (Default)))
                    RemoveTag(typeof (Default));

                AddChild(value);
            }
        }

        /// <summary>
        ///   Add a provacy list
        /// </summary>
        /// <param name="list"> </param>
        public void AddList(List list)
        {
            AddChild(list);
        }

        /// <summary>
        ///   Get all Lists
        /// </summary>
        /// <returns> Array of all privacy lists </returns>
        public List[] GetList()
        {
            ElementList el = SelectElements(typeof (List));
            int i = 0;
            var result = new List[el.Count];
            foreach (List list in el)
            {
                result[i] = list;
                i++;
            }
            return result;
        }
    }
}