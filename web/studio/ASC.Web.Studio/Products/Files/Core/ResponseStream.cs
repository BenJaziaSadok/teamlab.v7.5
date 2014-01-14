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

using System.IO;
using System.Net;

namespace ASC.Web.Files.Core
{
    public class ResponseStream : Stream
    {
        private readonly WebResponse _webResponse;
        private readonly Stream _stream;
        private readonly long _length;

        public ResponseStream(Stream stream, long length)
        {
            _stream = stream;
            _length = length;
        }

        public ResponseStream(WebResponse response)
        {
            _stream = response.GetResponseStream();
            _length = response.ContentLength;
            _webResponse = response;
        }

        public override bool CanRead
        {
            get { return _stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _stream.CanWrite; }
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Length
        {
            get { return _length; }
        }

        public override long Position
        {
            get { return _stream.Position; }
            set { _stream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
                _webResponse.Dispose();
            }
            base.Dispose(disposing);
        }

        public override void Close()
        {
            _stream.Close();
            _webResponse.Close();
        }
    }
}