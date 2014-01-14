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

using System.Text;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    internal class EntityBodyChunkReader
    {
        internal char Current;
        private readonly int _size;
        private readonly byte[] _buffer;
        private readonly Encoding _encoding = Encoding.UTF8;

        internal EntityBodyChunkReader(byte[] buffer, int offset)
        {
            Index = offset - 1;
            _buffer = buffer;
            _size = buffer.Length;
            Current = '0';
        }

        internal bool MoveTo(int pos)
        {
            if (_size == 0 || (pos >= _size && pos != 0))
                return false;

            Index = pos;
            Current = _encoding.GetChars(_buffer, Index, 1)[0];
            return true;
        }

        internal bool Read()
        {
            return MoveTo(++Index);
        }

        public int Index { get; private set; }
    }
}