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

namespace ASC.Web.Host.Common
{
    class ByteParser
    {
        private readonly byte[] _bytes;
        private int _pos;

        internal ByteParser(byte[] bytes)
        {
            _bytes = bytes;
            _pos = 0;
        }

        internal ByteString ReadLine()
        {
            ByteString str = null;
            for (int i = _pos; i < _bytes.Length; i++)
            {
                if (_bytes[i] == 10)
                {
                    int length = i - _pos;
                    if ((length > 0) && (_bytes[i - 1] == 13))
                    {
                        length--;
                    }
                    str = new ByteString(_bytes, _pos, length);
                    _pos = i + 1;
                    return str;
                }
            }
            if (_pos < _bytes.Length)
            {
                str = new ByteString(_bytes, _pos, _bytes.Length - _pos);
            }
            _pos = _bytes.Length;
            return str;
        }

        internal int CurrentOffset
        {
            get { return _pos; }
        }
    }
}