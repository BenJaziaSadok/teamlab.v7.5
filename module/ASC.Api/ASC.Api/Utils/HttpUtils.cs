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
using System.Web;

namespace ASC.Api.Utils
{
    public static class HttpUtils
    {
        private const int BufferReadLength = 2048;

        public static void WriteStreamToResponce(this HttpResponseBase response, Stream stream)
        {
            //set unbuffered output
            response.Buffer = false;
            response.BufferOutput = false;
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            var buffer = new byte[BufferReadLength];
            int readed;
            while ((readed = stream.Read(buffer, 0, BufferReadLength)) > 0)
            {
                var subbufer = new byte[readed];
                Array.Copy(buffer, subbufer, readed);
                response.BinaryWrite(subbufer);
            }
        }
    }
}