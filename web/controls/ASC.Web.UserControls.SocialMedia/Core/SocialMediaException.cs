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
using System.Runtime.Serialization;

namespace ASC.SocialMedia
{
    public class SocialMediaException : Exception
    {
        public SocialMediaException()
            : base()
        {
        }

        public SocialMediaException(string message)
            : base(message)
        {
        }

        public SocialMediaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected SocialMediaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}