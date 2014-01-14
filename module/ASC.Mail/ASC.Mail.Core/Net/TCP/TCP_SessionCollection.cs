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

using System.Collections;

namespace ASC.Mail.Net.TCP
{
    #region usings

    using System;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This class implements TCP session collection.
    /// </summary>
    public class TCP_SessionCollection<T>:IEnumerable<T> where T : TCP_Session
    {
        #region Members

        private readonly Dictionary<string, T> m_pItems;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        internal TCP_SessionCollection()
        {
            m_pItems = new Dictionary<string, T>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return m_pItems.Count; }
        }

        /// <summary>
        /// Gets TCP session with the specified ID.
        /// </summary>
        /// <param name="id">Session ID.</param>
        /// <returns>Returns TCP session with the specified ID.</returns>
        public T this[string id]
        {
            get { return m_pItems[id]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Copies all TCP server session to new array. This method is thread-safe.
        /// </summary>
        /// <returns>Returns TCP sessions array.</returns>
        public T[] ToArray()
        {
            lock (m_pItems)
            {
                T[] retVal = new T[m_pItems.Count];
                m_pItems.Values.CopyTo(retVal, 0);

                return retVal;
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Adds specified TCP session to the colletion.
        /// </summary>
        /// <param name="session">TCP server session to add.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null.</exception>
        internal void Add(T session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            lock (m_pItems)
            {
                m_pItems.Add(session.ID, session);
            }
        }

        /// <summary>
        /// Removes specified TCP server session from the collection.
        /// </summary>
        /// <param name="session">TCP server session to remove.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>session</b> is null.</exception>
        internal void Remove(T session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            lock (m_pItems)
            {
                m_pItems.Remove(session.ID);
            }
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        internal void Clear()
        {
            lock (m_pItems)
            {
                m_pItems.Clear();
            }
        }

        #endregion

        public IEnumerator<T> GetEnumerator()
        {
            return m_pItems.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}