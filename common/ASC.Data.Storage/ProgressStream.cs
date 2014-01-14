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
using System.IO;

namespace ASC.Data.Storage
{
    public class ProgressStream : Stream
    {
        private readonly Stream stream;
        private long length = long.MaxValue;

        public ProgressStream(Stream stream)
        {
            if (stream == null) throw new ArgumentNullException("stream");

            this.stream = stream;
            try
            {
                length = stream.Length;
            }
            catch (Exception) { }
        }

        public override bool CanRead
        {
            get { return stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return stream.CanWrite; }
        }

        public override long Length
        {
            get { return stream.Length; }
        }

        public override long Position
        {
            get { return stream.Position; }
            set { stream.Position = value; }
        }

        public event Action<ProgressStream, int> OnReadProgress;

        public void InvokeOnReadProgress(int progress)
        {
            var handler = OnReadProgress;
            if (handler != null)
            {
                handler(this, progress);
            }
        }

        public override void Flush()
        {
            stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
            length = value;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var readed = stream.Read(buffer, offset, count);
            OnReadProgress(this, (int)(stream.Position / (double)length * 100));
            return readed;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}