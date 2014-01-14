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
using System.Threading;
using ASC.Data.Storage;

public static class Extensions
{
    private const int BufferSize = 2048;//NOTE: set to 2048 to fit in minimum tcp window

    public static Stream GetBuffered(this Stream srcStream)
    {
        if (srcStream == null) throw new ArgumentNullException("srcStream");
        if (!srcStream.CanSeek || srcStream.CanTimeout)
        {
            //Buffer it
            var memStream = TempStream.Create();
            srcStream.StreamCopyTo(memStream);
            memStream.Position = 0;
            return memStream;
        }
        return srcStream;
    }

    public static byte[] GetCorrectBuffer(this Stream stream)
    {
        Stream memoryStream = stream;
        if (memoryStream == null) throw new ArgumentNullException("stream");
        memoryStream = memoryStream.GetBuffered();
        var buffer = new byte[memoryStream.Length];
        memoryStream.Position = 0;
        memoryStream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public static void StreamCopyTo(this Stream srcStream, Stream dstStream)
    {
        if (srcStream == null) throw new ArgumentNullException("srcStream");
        if (dstStream == null) throw new ArgumentNullException("dstStream");

        var buffer = new byte[BufferSize];
        int readed;
        while ((readed = srcStream.Read(buffer, 0, BufferSize)) > 0)
        {
            dstStream.Write(buffer, 0, readed);
        }
    }

    public static Stream IronReadStream(this IDataStore store, string domain, string path, int tryCount)
    {
        var ms = TempStream.Create();
        IronReadToStream(store, domain, path, tryCount, ms);
        ms.Seek(0, SeekOrigin.Begin);
        return ms;
    }

    public static void IronReadToStream(this IDataStore store, string domain, string path, int tryCount, Stream readTo)
    {
        if (tryCount < 1) throw new ArgumentOutOfRangeException("tryCount", "Must be greater or equal 1.");
        if (!readTo.CanWrite) throw new ArgumentException("stream cannot be written", "readTo");

        var tryCurrent = 0;
        var offset = 0;

        while (tryCurrent < tryCount)
        {
            try
            {
                tryCurrent++;
                using (var stream = store.GetReadStream(domain, path, offset))
                {
                    var buffer = new byte[BufferSize];
                    var readed = 0;
                    while ((readed = stream.Read(buffer, 0, BufferSize)) > 0)
                    {
                        readTo.Write(buffer, 0, readed);
                        offset += readed;
                    }
                }
                break;
            }
            catch (Exception ex)
            {
                if (tryCurrent >= tryCount)
                {
                    throw new IOException("Can not read stream. Tries count: " + tryCurrent + ".", ex);
                }
                Thread.Sleep(tryCount * 50);
            }
        }
    }
}