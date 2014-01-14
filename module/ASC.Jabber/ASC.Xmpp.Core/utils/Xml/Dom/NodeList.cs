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
using System.Collections;

#endregion

namespace ASC.Xmpp.Core.utils.Xml.Dom
{

    #region usings

    #endregion

    /// <summary>
    /// </summary>
    public class NodeList : CollectionBase
    {
        #region Members

        /// <summary>
        ///   Owner (Parent) of the ChildElement Collection
        /// </summary>
        private readonly Node m_Owner;

        #endregion

        #region Constructor

        /// <summary>
        /// </summary>
        public NodeList()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="owner"> </param>
        public NodeList(Node owner)
        {
            m_Owner = owner;
        }

        #endregion

        #region Methods

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public void Add(Node e)
        {
            // can't add a empty node, so return immediately
            // Some people tried this which caused an error
            if (e == null)
            {
                return;
            }

            if (m_Owner != null)
            {
                e.Parent = m_Owner;
                if (e.Namespace == null)
                {
                    e.Namespace = m_Owner.Namespace;
                }
            }

            e.m_Index = Count;

            List.Add(e);
        }

        // Method implementation from the CollectionBase class
        /// <summary>
        /// </summary>
        /// <param name="index"> </param>
        /// <exception cref="Exception"></exception>
        public void Remove(int index)
        {
            if (index > Count - 1 || index < 0)
            {
                // Handle the error that occurs if the valid page index is       
                // not supplied.    
                // This exception will be written to the calling function             
                throw new Exception("Index out of bounds");
            }

            List.RemoveAt(index);
            RebuildIndex(index);
        }

        /// <summary>
        /// </summary>
        /// <param name="e"> </param>
        public void Remove(Element e)
        {
            int idx = e.Index;
            List.Remove(e);
            RebuildIndex(idx);

            // 			for ( int i = 0; i< this.Count; i++)
            // 			{
            // 				if (e == (Element) this.List[i])
            // 				{
            // 					Remove(i);
            // 					return;
            // 				}
            // 			}
        }

        /// <summary>
        /// </summary>
        /// <param name="index"> </param>
        /// <returns> </returns>
        public Node Item(int index)
        {
            return (Node) List[index];
        }

        /// <summary>
        /// </summary>
        /// <returns> </returns>
        public object[] ToArray()
        {
            var ar = new object[List.Count];
            for (int i = 0; i < List.Count; i++)
            {
                ar[i] = List[i];
            }

            return ar;
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// </summary>
        internal void RebuildIndex()
        {
            RebuildIndex(0);
        }

        /// <summary>
        /// </summary>
        /// <param name="start"> </param>
        internal void RebuildIndex(int start)
        {
            for (int i = start; i < Count; i++)
            {
                // Element e = (Element) List[i];
                var node = (Node) List[i];
                node.m_Index = i;
            }
        }

        #endregion
    }
}