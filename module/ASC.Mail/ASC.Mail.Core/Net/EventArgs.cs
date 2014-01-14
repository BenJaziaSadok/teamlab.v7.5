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

namespace ASC.Mail.Net
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// This class universal event arguments for transporting single value.
    /// </summary>
    /// <typeparam name="T">Event data.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        #region Members

        private readonly T m_pValue;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="value">Event data.</param>
        public EventArgs(T value)
        {
            m_pValue = value;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets event data.
        /// </summary>
        public T Value
        {
            get { return m_pValue; }
        }

        #endregion
    }
}