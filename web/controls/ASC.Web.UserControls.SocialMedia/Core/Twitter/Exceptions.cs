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

namespace ASC.SocialMedia.Twitter
{
    public class TwitterException : SocialMediaException
    {
        public TwitterException(string message)
            : base(message)
        {
        }
    }

    public class ConnectionFailureException : TwitterException
    {
        public ConnectionFailureException(string message)
            : base(message)
        {
        }
    }

    public class RateLimitException : TwitterException
    {
        public RateLimitException(string message)
            : base(message)
        {
        }
    }

    public class ResourceNotFoundException : TwitterException
    {
        public ResourceNotFoundException(string message)
            : base(message)
        {
        }
    }

    public class UnauthorizedException : TwitterException
    {
        public UnauthorizedException(string message)
            : base(message)
        {
        }
    }
}
