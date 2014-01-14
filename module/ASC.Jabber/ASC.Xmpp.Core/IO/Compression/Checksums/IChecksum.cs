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

namespace ASC.Xmpp.Core.IO.Compression.Checksums
{
    /// <summary>
    ///   Interface to compute a data checksum used by checked input/output streams. A data checksum can be updated by one byte or with a byte array. After each update the value of the current checksum can be returned by calling <code>getValue</code> . The complete checksum object can also be reset so it can be used again with new data.
    /// </summary>
    public interface IChecksum
    {
        #region Properties

        /// <summary>
        ///   Returns the data checksum computed so far.
        /// </summary>
        long Value { get; }

        #endregion

        #region Methods

        /// <summary>
        ///   Resets the data checksum as if no update was ever called.
        /// </summary>
        void Reset();

        /// <summary>
        ///   Adds one byte to the data checksum.
        /// </summary>
        /// <param name="bval"> the data value to add. The high byte of the int is ignored. </param>
        void Update(int bval);

        /// <summary>
        ///   Updates the data checksum with the bytes taken from the array.
        /// </summary>
        /// <param name="buffer"> buffer an array of bytes </param>
        void Update(byte[] buffer);

        /// <summary>
        ///   Adds the byte array to the data checksum.
        /// </summary>
        /// <param name="buf"> the buffer which contains the data </param>
        /// <param name="off"> the offset in the buffer where the data starts </param>
        /// <param name="len"> the length of the data </param>
        void Update(byte[] buf, int off, int len);

        #endregion
    }
}