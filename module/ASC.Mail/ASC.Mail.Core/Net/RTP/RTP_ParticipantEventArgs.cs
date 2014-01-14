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
    /// This claass provides data for <b>RTP_MultimediaSession.NewParticipant</b> event.
    /// </summary>
    public class RTP_ParticipantEventArgs : EventArgs
    {
        #region Members

        private readonly RTP_Participant_Remote m_pParticipant;

        #endregion

        #region Properties

        /// <summary>
        /// Gets participant.
        /// </summary>
        public RTP_Participant_Remote Participant
        {
            get { return m_pParticipant; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="participant">RTP participant.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>participant</b> is null reference.</exception>
        public RTP_ParticipantEventArgs(RTP_Participant_Remote participant)
        {
            if (participant == null)
            {
                throw new ArgumentNullException("participant");
            }

            m_pParticipant = participant;
        }

        #endregion
    }
}