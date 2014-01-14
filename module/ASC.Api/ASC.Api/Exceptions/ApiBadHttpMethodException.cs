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
using System.Web;

namespace ASC.Api.Exceptions
{
    
    [Serializable]
    public class ApiBadHttpMethodException : HttpException
    {
        public ApiBadHttpMethodException(string currentMethod)
            : base(403, string.Format("{0} Not allowed", currentMethod))
        {
        }

        public ApiBadHttpMethodException()
        {
        }


        public ApiBadHttpMethodException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected ApiBadHttpMethodException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}