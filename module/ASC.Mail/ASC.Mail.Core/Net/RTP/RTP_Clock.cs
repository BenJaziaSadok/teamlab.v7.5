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

namespace ASC.Mail.Net.RTP
{
    #region usings

    using System;

    #endregion

    /// <summary>
    /// Implements RTP media clock.
    /// </summary>
    public class RTP_Clock
    {
        #region Members

        private readonly int m_BaseValue;
        private readonly DateTime m_CreateTime;
        private readonly int m_Rate = 1;

        #endregion

        #region Properties

        /// <summary>
        /// Gets clock base value from where clock started.
        /// </summary>
        public int BaseValue
        {
            get { return m_BaseValue; }
        }

        /// <summary>
        /// Gets current clock rate in Hz.
        /// </summary>
        public int Rate
        {
            get { return m_Rate; }
        }

        /// <summary>
        /// Gets current RTP timestamp.
        /// </summary>
        public uint RtpTimestamp
        {
            get
            {
                /*
                    m_Rate  -> 1000ms
                    elapsed -> x
                */

                long elapsed = (long) ((DateTime.Now - m_CreateTime)).TotalMilliseconds;

                return (uint) (m_BaseValue + ((m_Rate*elapsed)/1000));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="baseValue">Clock base value from where clock starts.</param>
        /// <param name="rate">Clock rate in Hz.</param>
        public RTP_Clock(int baseValue, int rate)
        {
            if (rate < 1)
            {
                throw new ArgumentException("Argument 'rate' value must be between 1 and 100 000.", "rate");
            }

            m_BaseValue = baseValue;
            m_Rate = rate;
            m_CreateTime = DateTime.Now;
        }

        #endregion
    }
}