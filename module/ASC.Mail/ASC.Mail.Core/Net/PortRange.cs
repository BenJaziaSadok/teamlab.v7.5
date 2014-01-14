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
    /// This class holds UDP or TCP port range.
    /// </summary>
    public class PortRange
    {
        #region Members

        private readonly int m_End = 1100;
        private readonly int m_Start = 1000;

        #endregion

        #region Properties

        /// <summary>
        /// Gets start port.
        /// </summary>
        public int Start
        {
            get { return m_Start; }
        }

        /// <summary>
        /// Gets end port.
        /// </summary>
        public int End
        {
            get { return m_End; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="start">Start port.</param>
        /// <param name="end">End port.</param>
        /// <exception cref="ArgumentOutOfRangeException">Is raised when any of the aruments value is out of range.</exception>
        public PortRange(int start, int end)
        {
            if (start < 1 || start > 0xFFFF)
            {
                throw new ArgumentOutOfRangeException("Argument 'start' value must be > 0 and << 65 535.");
            }
            if (end < 1 || end > 0xFFFF)
            {
                throw new ArgumentOutOfRangeException("Argument 'end' value must be > 0 and << 65 535.");
            }
            if (start > end)
            {
                throw new ArgumentOutOfRangeException(
                    "Argumnet 'start' value must be >= argument 'end' value.");
            }

            m_Start = start;
            m_End = end;
        }

        #endregion
    }
}