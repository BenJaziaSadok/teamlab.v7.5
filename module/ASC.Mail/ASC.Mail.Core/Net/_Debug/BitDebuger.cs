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
    using System.Text;

    #endregion

    /// <summary>
    /// This class provides bit debugging methods.
    /// </summary>
    internal class BitDebuger
    {
        #region Methods

        /// <summary>
        /// Converts byte array to bit(1 byte = 8 bit) representation.
        /// </summary>
        /// <param name="buffer">Data buffer.</param>
        /// <param name="count">Numer of bytes to convert.</param>
        /// <param name="bytesPerLine">Number of bytes per line.</param>
        /// <returns>Returns byte array as bit(1 byte = 8 bit) representation.</returns>
        public static string ToBit(byte[] buffer, int count, int bytesPerLine)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            StringBuilder retVal = new StringBuilder();

            int offset = 0;
            int bytesInCurrentLine = 1;
            while (offset < count)
            {
                byte currentByte = buffer[offset];
                char[] bits = new char[8];
                for (int i = 7; i >= 0; i--)
                {
                    bits[i] = ((currentByte >> (7 - i)) & 0x1).ToString()[0];
                }
                retVal.Append(bits);

                if (bytesInCurrentLine == bytesPerLine)
                {
                    retVal.AppendLine();
                    bytesInCurrentLine = 0;
                }
                else
                {
                    retVal.Append(" ");
                }
                bytesInCurrentLine++;
                offset++;
            }

            return retVal.ToString();
        }

        #endregion
    }
}