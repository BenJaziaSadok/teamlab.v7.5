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
    using System.Collections.Generic;

    #endregion

    /// <summary>
    /// This is base class for <b>RTP_Participant_Local</b> and <b>RTP_Participant_Remote</b> class.
    /// </summary>
    public abstract class RTP_Participant
    {
        #region Events

        /// <summary>
        /// Is raised when participant disjoins(timeout or BYE all sources) the RTP multimedia session.
        /// </summary>
        public event EventHandler Removed = null;

        /// <summary>
        /// Is raised when participant gets new RTP source.
        /// </summary>
        public event EventHandler<RTP_SourceEventArgs> SourceAdded = null;

        /// <summary>
        /// Is raised when RTP source removed from(Timeout or BYE) participant.
        /// </summary>
        public event EventHandler<RTP_SourceEventArgs> SourceRemoved = null;

        #endregion

        #region Members

        private readonly string m_CNAME = "";
        private List<RTP_Source> m_pSources;

        #endregion

        #region Properties

        /// <summary>
        /// Gets canonical name of participant.
        /// </summary>
        public string CNAME
        {
            get { return m_CNAME; }
        }

        /// <summary>
        /// Gets the sources what participant owns(sends).
        /// </summary>
        public RTP_Source[] Sources
        {
            get { return m_pSources.ToArray(); }
        }

        /// <summary>
        /// Gets or sets user data.
        /// </summary>
        public object Tag { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="cname">Canonical name of participant.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>cname</b> is null reference.</exception>
        public RTP_Participant(string cname)
        {
            if (cname == null)
            {
                throw new ArgumentNullException("cname");
            }
            if (cname == string.Empty)
            {
                throw new ArgumentException("Argument 'cname' value must be specified.");
            }

            m_CNAME = cname;

            m_pSources = new List<RTP_Source>();
        }

        #endregion

        #region Utility methods

        /// <summary>
        /// Raises <b>Removed</b> event.
        /// </summary>
        private void OnRemoved()
        {
            if (Removed != null)
            {
                Removed(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises <b>SourceAdded</b> event.
        /// </summary>
        /// <param name="source">RTP source.</param>
        private void OnSourceAdded(RTP_Source source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (SourceAdded != null)
            {
                SourceAdded(this, new RTP_SourceEventArgs(source));
            }
        }

        /// <summary>
        /// Raises <b>SourceRemoved</b> event.
        /// </summary>
        /// <param name="source">RTP source.</param>
        private void OnSourceRemoved(RTP_Source source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (SourceRemoved != null)
            {
                SourceRemoved(this, new RTP_SourceEventArgs(source));
            }
        }

        #endregion

        #region Internal methods

        /// <summary>
        /// Cleans up any resources being used.
        /// </summary>
        internal void Dispose()
        {
            m_pSources = null;

            Removed = null;
            SourceAdded = null;
            SourceRemoved = null;
        }

        /// <summary>
        /// Adds specified source to participant if participant doesn't contain the specified source.
        /// </summary>
        /// <param name="source">RTP source.</param>
        /// <exception cref="ArgumentNullException">Is raised when <b>source</b> is null reference.</exception>
        internal void EnsureSource(RTP_Source source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (!m_pSources.Contains(source))
            {
                m_pSources.Add(source);

                OnSourceAdded(source);

                source.Disposing += delegate
                                        {
                                            if (m_pSources.Remove(source))
                                            {
                                                OnSourceRemoved(source);

                                                // If last source removed, the participant is dead, so dispose participant.
                                                if (m_pSources.Count == 0)
                                                {
                                                    OnRemoved();
                                                    Dispose();
                                                }
                                            }
                                        };
            }
        }

        #endregion
    }
}