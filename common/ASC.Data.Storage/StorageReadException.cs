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
using System.Runtime.Serialization;

namespace ASC.Data.Storage
{
    [Serializable]
    public class StorageReadException : IOException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //
        public long Readed { get; set; }


        public StorageReadException(long readed)
        {
            Readed = readed;
        }

        public StorageReadException(long readed, string message)
            : base(message)
        {
            Readed = readed;
        }

        public StorageReadException(long readed, string message, Exception inner)
            : base(message, inner)
        {
            Readed = readed;
        }


        public StorageReadException()
        {
        }

        public StorageReadException(string message) : base(message)
        {
        }

        public StorageReadException(string message, Exception inner) : base(message, inner)
        {
        }

        protected StorageReadException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}