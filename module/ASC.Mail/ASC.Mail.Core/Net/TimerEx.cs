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

    using System.Timers;

    #endregion

    /// <summary>
    /// Simple timer implementation.
    /// </summary>
    public class TimerEx : Timer
    {
        #region Constructor

        /// <summary>
        /// Default contructor.
        /// </summary>
        public TimerEx() {}

        /// <summary>
        /// Default contructor.
        /// </summary>
        /// <param name="interval">The time in milliseconds between events.</param>
        public TimerEx(double interval) : base(interval) {}

        /// <summary>
        /// Default contructor.
        /// </summary>
        /// <param name="interval">The time in milliseconds between events.</param>
        /// <param name="autoReset">Specifies if timer is auto reseted.</param>
        public TimerEx(double interval, bool autoReset) : base(interval)
        {
            AutoReset = autoReset;
        }

        #endregion

        // TODO: We need to do this class CF compatible.
    }
}