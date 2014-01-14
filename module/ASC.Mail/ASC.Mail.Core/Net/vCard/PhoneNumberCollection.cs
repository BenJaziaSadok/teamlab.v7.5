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

namespace ASC.Mail.Net.Mime.vCard
{
    #region usings

    using System.Collections;
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// vCard phone number collection implementation.
    /// </summary>
    public class PhoneNumberCollection : IEnumerable
    {
        #region Members

        private readonly List<PhoneNumber> m_pCollection;
        private readonly vCard m_pOwner;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="owner">Owner vCard.</param>
        internal PhoneNumberCollection(vCard owner)
        {
            m_pOwner = owner;
            m_pCollection = new List<PhoneNumber>();

            foreach (Item item in owner.Items.Get("TEL"))
            {
                m_pCollection.Add(PhoneNumber.Parse(item));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets number of items in the collection.
        /// </summary>
        public int Count
        {
            get { return m_pCollection.Count; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add new phone number to the collection.
        /// </summary>
        /// <param name="type">Phone number type. Note: This value can be flagged value !</param>
        /// <param name="number">Phone number.</param>
        public void Add(PhoneNumberType_enum type, string number)
        {
            Item item = m_pOwner.Items.Add("TEL", PhoneNumber.PhoneTypeToString(type), number);
            m_pCollection.Add(new PhoneNumber(item, type, number));
        }

        /// <summary>
        /// Removes specified item from the collection.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        public void Remove(PhoneNumber item)
        {
            m_pOwner.Items.Remove(item.Item);
            m_pCollection.Remove(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            foreach (PhoneNumber number in m_pCollection)
            {
                m_pOwner.Items.Remove(number.Item);
            }
            m_pCollection.Clear();
        }

        /// <summary>
        /// Gets enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return m_pCollection.GetEnumerator();
        }

        #endregion
    }
}