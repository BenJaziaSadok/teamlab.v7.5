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
    /// <summary>
    /// Socket log entry.
    /// </summary>
    public class SocketLogEntry
    {
        #region Members

        private readonly long m_Size;
        private readonly string m_Text = "";
        private readonly SocketLogEntryType m_Type = SocketLogEntryType.FreeText;

        #endregion

        #region Properties

        /// <summary>
        /// Gets log text.
        /// </summary>
        public string Text
        {
            get { return m_Text; }
        }

        /// <summary>
        /// Gets size of data readed or sent.
        /// </summary>
        public long Size
        {
            get { return m_Size; }
        }

        /// <summary>
        /// Gets log entry type.
        /// </summary>
        public SocketLogEntryType Type
        {
            get { return m_Type; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="text">Log text.</param>
        /// <param name="size">Data size.</param>
        /// <param name="type">Log entry type</param>
        public SocketLogEntry(string text, long size, SocketLogEntryType type)
        {
            m_Text = text;
            m_Type = type;
            m_Size = size;
        }

        #endregion
    }
}