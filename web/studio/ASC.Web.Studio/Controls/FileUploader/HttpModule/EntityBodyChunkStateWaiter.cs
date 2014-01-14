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

using System;
using System.Text;

namespace ASC.Web.Studio.Controls.FileUploader.HttpModule
{
    internal class EntityBodyChunkStateWaiter
    {
        private readonly bool _collectInput;
        private StringBuilder _input;
        private EntityBodyChunkReader _reader;

        private readonly int _guardSize;
        private readonly char[] _guard;

        internal event EventHandler<EventArgs> MeetGuard;

        internal EntityBodyChunkStateWaiter(string waitFor, bool collectInput)
        {
            _collectInput = collectInput;

            if (collectInput) _input = new StringBuilder();

            _guardSize = waitFor.Length;
            _guard = waitFor.ToCharArray();

            CharFound = 0;
        }

        internal string Value
        {
            get { return !_collectInput ? string.Empty : _input.ToString(); }
        }

        public int Index
        {
            get { return _reader.Index; }
        }

        public int CharFound { get; private set; }

        internal void Reset()
        {
            _input = new StringBuilder();
            CharFound = 0;
        }

        internal void Wait(byte[] buffer, int offset)
        {
            _reader = new EntityBodyChunkReader(buffer, offset);
            Wait();
        }

        internal void Wait(EntityBodyChunkStateWaiter waiter)
        {
            _reader = waiter._reader;
            Wait();
        }

        internal void Wait()
        {
            while (_reader.Read())
            {
                var c = _reader.Current;

                if (_collectInput)
                    _input.Append(c);

                if (c != _guard[CharFound])
                {
                    CharFound = 0;
                }
                else
                {
                    CharFound++;
                    if (CharFound == _guard.Length)
                    {
                        if (MeetGuard != null)
                        {
                            if (_collectInput && _input.Length >= _guardSize)
                            {
                                _input.Remove(_input.Length - _guardSize, _guardSize);
                            }
                            MeetGuard(this, new EventArgs());
                        }
                        break;
                    }
                }
            }
        }
    }
}